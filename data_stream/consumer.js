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
  clientId: 'factory-consumer',
});

const consumer = kafka.consumer({ groupId: 'factory-data-stream' });

const run = async () => {
  await consumer.connect();
  await consumer.subscribe({ 
    topics: ["device-startsensor"],
    fromBeginning: true
  });
  await consumer.run({
    eachMessage: async ({ topic, partition, message }) => {
      const prefix = `${topic}[${partition} | ${message.offset}] / ${message.timestamp}`;
      console.log(`- ${prefix} ${message.key}#${message.value}`);
      const deviceid = topic.split('-')[1]
      const payload = JSON.parse(message.value.toString());
      let point;
      if (deviceid === 'startsensor') {
        console.log(`Received message from startsensor`)
        point = new Point("sensor")
          .tag("name", "startsensor")
          .intField("val", 1)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'device2') {
        console.log(`Received message from else if`)
      } else {
        console.log(`Received message from unknown device`)
      }
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
    }
  });
};

run().catch(e => console.error(`[factory/consumer] ${e.message}`, e));