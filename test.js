const { Kafka, logLevel } = require('kafkajs');
const { InfluxDB } = require('@influxdata/influxdb-client');

const token = 'AXEejzCSVeLZoOWTFnmrFeN1Ikxpx9iD2u4DSQDJCsKkPdxC0syBCLYAIibaHzgMz3UtS7k2aqzthXBn_UHpkQ==';
const org = 'Uni Wien';
const bucket = 'FactoryIO';

const client = new InfluxDB({ url: 'http://localhost:8086', token: token });

const { Point } = require('@influxdata/influxdb-client');
const writeApi = client.getWriteApi(org, bucket);
writeApi.useDefaultTags({ host: 'host1' });

const kafka = new Kafka({
  logLevel: logLevel.INFO,
  brokers: ["localhost:9092"],
  clientId: 'example-consumer',
});

const topic = 'test-topic';
const consumer = kafka.consumer({ groupId: 'test-group' });

const run = async () => {
  await consumer.connect();
  await consumer.subscribe({ topic, fromBeginning: true });
  await consumer.run({
    eachMessage: async ({ topic, partition, message }) => {
      const prefix = `${topic}[${partition} | ${message.offset}] / ${message.timestamp}`;
      console.log(`- ${prefix} ${message.key}#${message.value}`);
      const msg = JSON.parse(message.value);
      const point = new Point("weatherstation")
        .tag("name", msg.name)
        .tag("type", msg.type)
        .tag("address", msg.address)
        .intField("bool", msg.value ? 1 : 0)
        .timestamp(new Date(Date.parse(msg.timestamp)));
      writeApi
        .writePoint(point)
      writeApi
        .flush()
        .then(() => {
          console.log('FINISHED')
        })
        .catch(e => {
          console.error(e)
          console.log('Finished ERROR')
        })
      // try {
      //   writeApi.writePoint(point);
      // } catch (err) {
      //   console.log(err);
      // }
    }
  });
};

run().catch(e => console.error(`[example/consumer] ${e.message}`, e));