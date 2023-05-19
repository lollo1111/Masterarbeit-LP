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
    topics: ["device-machineA", "device-default", "device-sliding", "device-machineB", "device-circular", "device-angular", "device-classic", "device-modern", "device-startsensor", "device-endsensor", "device-tank", "device-quality", "device-product"],
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
        point = new Point("sensor")
          .tag("name", "startsensor")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'endsensor') {
        point = new Point("sensor")
          .tag("name", "endsensor")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'tank') {
        point = new Point("machine")
          .tag("name", "tank")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'product') {
        point = new Point("product")
          .tag("name", payload.val === "A" ? "schreibtisch" : "schrank")
          .intField("val", 1)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === "quality") {
        point = new Point("quality")
          .booleanField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === "machineA") {
        point = new Point("machines")
          .tag("name", "Maschine A")
          .booleanField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === "machineB") {
        point = new Point("machines")
          .tag("name", "Maschine B")
          .booleanField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'classic') {
        point = new Point("tableStyle")
          .tag("name", "classic")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'modern') {
        point = new Point("tableStyle")
          .tag("name", "modern")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'circular') {
        point = new Point("mirrorShape")
          .tag("name", "circular")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'angular') {
        point = new Point("mirrorShape")
          .tag("name", "angular")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'default') {
        point = new Point("doorType")
          .tag("name", "default")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else if (deviceid === 'sliding') {
        point = new Point("doorType")
          .tag("name", "sliding")
          .intField("val", payload.val)
          .timestamp(new Date(payload.timestamp));
      } else {
        console.log("Received message from unknown device")
      }
      writeApi
        .writePoint(point);
      try {
        await writeApi.flush()
      } catch (e) {
        console.error()
      }
    }
  });
};

run().catch(e => console.error(`[factory/consumer] ${e.message}`, e));