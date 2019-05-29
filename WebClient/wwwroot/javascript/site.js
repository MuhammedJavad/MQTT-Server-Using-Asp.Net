"use strict";

function MqttClient(host, port, clientId, topic) {
    var options = {
        cleanSession: true,
        mqttVersion: 3,
        reconnect: true,
        onSuccess: function () {
            console.log("onConnect");
            client.subscribe("pi-result");
        },
        onFailure: function () {
            console.log("Connection attempt to " + host + " Failed");
        }
    };
    var client = new Paho.MQTT.Client(host, port, clientId);
    // set callback handlers
    client.onConnectionLost = function (responseObject) {
        if (responseObject.errorCode !== 0) {
            console.log("onConnectionLost:" + responseObject.errorMessage);
        }
    };
    client.onMessageArrived = function (message) {
        console.log("onMessageArrived:" + message.payloadString);
        if (message.payloadString === "off") {
            if (MqttClient.prototype.lamp.hasClass('green')) {
                MqttClient.prototype.lamp.toggleClass('green red')
                var html = '<div class="item">' +
                    '<div class="content">' +
                    '<div class="header">The Lamp was ' +
                    message.payloadString +
                    '</div>at ' +
                    Date(Date.now()) +
                    '</div></div>';
                MqttClient.prototype.logging.prepend(html);
                var children = MqttClient.prototype.logging.children();
                if (children.length >= 3)
                    children.last().remove();
            }
        } else if (message.payloadString === "on") {
            if (MqttClient.prototype.lamp.hasClass('red')) {
                MqttClient.prototype.lamp.toggleClass('red green');
                var html = '<div class="item">' +
                    '<div class="content">' +
                    '<div class="header">The Lamp was ' +
                    message.payloadString +
                    '</div>at ' +
                    Date(Date.now()) +
                    '</div></div>';
                MqttClient.prototype.logging.prepend(html);
                var children = MqttClient.prototype.logging.children();
                if (children.length >= 3)
                    children.last().remove();
            }
        }
    };
    // connect the client
    client.connect(options);

    this.SendMessage = function (act) {
        var message = new Paho.MQTT.Message(act);
        message.destinationName = topic;
        client.send(message);
    }
}
$(function () {
    var lamp = $("#lamp");
    MqttClient.prototype.lamp = lamp;
    MqttClient.prototype.logging = $("#output-log")

    var client = new MqttClient("localhost", 5000, "webClient-Js", "pi-lamp");
    
    $("#on-btn").click(function () {
        if (lamp.hasClass('red'))
            client.SendMessage("on");
    });

    $("#off-btn").click(function () {
        if (lamp.hasClass('green'))
            client.SendMessage("off");
    });
});