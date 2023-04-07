import paho.mqtt.client as mqtt
from confluent_kafka import Producer
import socket

# host.docker.internal
#broker:29092
conf = {'bootstrap.servers': "broker:29092",
        'client.id': socket.gethostname()}

producer = Producer(conf)

def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))
    client.subscribe("Test")

def on_message(client, userdata, msg):
    print(msg.topic+" "+str(msg.payload))
    orsch = str(msg.payload.decode("utf-8", "ignore"))
    producer.produce('Test', key="DiezeWert", value="OK")
    producer.flush()

client = mqtt.Client('MQTTBridge')
client.on_connect = on_connect
client.on_message = on_message

client.connect("host.docker.internal", 1883, 60)
client.loop_forever()