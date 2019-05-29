from services_handler import ServicesHandler

def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+ str(rc))
    # Subscribing in on_connect() means that if we lose the connection and reconnect then subscriptions will be renewed.
    client.subscribe("pi-lamp")

def on_message(client, userdata, message):
    print("Received message '" + str(message.payload) + "' on topic '"
        + message.topic + "' with QoS " + str(message.qos))
    if message.topic == "pi-lamp":
        payload = str(message.payload)
        if "'on'" in payload:
            service.gpio_status = True
        elif "'off'" in payload:
            service.gpio_status = False
    
service = ServicesHandler("raspberry-pi", "tcp", "broker ip address", 1884, 60, on_connect, on_message)
service.loop()


