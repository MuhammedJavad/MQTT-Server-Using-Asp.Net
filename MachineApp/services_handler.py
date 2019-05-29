from gpiozero import LED 
import paho.mqtt.client as mqtt


class ServicesHandler():
    def __init__(self, client_id, protocol, host, port, time_out, on_connect, on_message):
        self.gpio_switcher = False
        self.led = LED(17) # Pick Raspberry Pi 17th GPIO 
        self.client = mqtt.Client(client_id=client_id, clean_session=True, userdata=None, protocol=mqtt.MQTTv31, transport=protocol)
        self.client.on_connect = on_connect
        self.client.on_message = on_message
        self.client.connect(host, port, time_out)

    @property
    def gpio_status(self):
        return self.gpio_switcher

    @gpio_status.setter
    def gpio_status(self, value):
        self.gpio_switcher = value
        self.__switch_led()

    def __switch_led(self):
        status = False
        if self.gpio_switcher:
            try:
                self.led.on()
                status = True
            except:
                status = False
        else:
            try:
                self.led.off()
                status = False
            except:
                status = True
        self.__publish_result("on" if status else "off")

    def __publish_result(self, status):
        self.client.publish("pi-result", payload=status, qos=1, retain=False)

    def loop(self):
        # Blocking call that processes network traffic, dispatches callbacks and
        # handles reconnecting.
        # Other loop*() functions are available that give a threaded interface and a
        # manual interface.
        self.client.loop_forever()