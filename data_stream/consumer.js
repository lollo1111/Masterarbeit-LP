const { Kafka, logLevel } = require('kafkajs');
const { InfluxDB } = require('@influxdata/influxdb-client');
const dotenv = require('dotenv');
dotenv.config({ path: './.env' });

const token = process.env.INFLUXDB_TOKEN;
const org = process.env.INFLUXDB_ORG;
const bucket = process.env.INFLUXDB_BUCKET;
const url = 'http://influxdb:8086';

const client = new InfluxDB({ url, token });

const { Point } = require('@influxdata/influxdb-client');
const writeApi = client.getWriteApi(org, bucket);
writeApi.useDefaultTags({ host: 'host1' });

const kafka = new Kafka({
  logLevel: logLevel.INFO,
  brokers: ["broker:29092"],
  clientId: 'example-consumer',
});

const topic = 'Test';
// const topic = 'test-topic';
const consumer = kafka.consumer({ groupId: 'test-group' });

const run = async () => {
  await consumer.connect();
  await consumer.subscribe({ topic, fromBeginning: true });
  await consumer.run({
    eachMessage: async ({ topic, partition, message }) => {
      const prefix = `${topic}[${partition} | ${message.offset}] / ${message.timestamp}`;
      console.log(`- ${prefix} ${message.key}#${message.value}`);
      // const point = new Point("weatherstation")
      //   .tag("name", "Shiesh")
      //   .intField("val", 1)
      //   .timestamp(new Date(Date.now()));
      // writeApi
      //   .writePoint(point)
      // writeApi
      //   .flush()
      //   .then(() => {
      //     console.log('FINISHED')
      //   })
      //   .catch(e => {
      //     console.error(e)
      //     console.log('Finished ERROR')
      //   })
    }
  });
};

run().catch(e => console.error(`[example/consumer] ${e.message}`, e));