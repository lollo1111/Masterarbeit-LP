FROM python:3.9

WORKDIR /app

COPY bridge.py .

RUN pip install paho-mqtt
RUN pip install confluent_kafka

CMD ["python","-u","bridge.py"]