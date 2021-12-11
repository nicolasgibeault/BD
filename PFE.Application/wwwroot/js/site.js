// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var clientID;
var client;

function startConnect() {
    // Generate a random client ID
    clientID = "clientID_" + parseInt(Math.random() * 100);

    // Fetch the hostname/IP address and port number from the form
    var host = 'wss://70.52.17.228:707/';
    var port = 707;

    client = new Paho.MQTT.Client(host, clientID);

    client.onConnectionLost = onConnectionLost;
    client.onMessageArrived = onMessageArrived;

    client.connect({
        onSuccess: onConnect
    });
}

// Called when the client connects
function onConnect() {
    // Fetch the MQTT topic from the form
    var topic = 'Dev.To';

    // Print output for the user in the messages div
    document.getElementById("messages").innerHTML += '<span>Subscribing to: ' + topic + '</span><br/>';

    // Subscribe to the requested topic
    client.subscribe(topic);
}

// Called when the client loses its connection
function onConnectionLost(responseObject) {
    document.getElementById("messages").innerHTML += '<span>ERROR: Connection lost</span><br/>';
    if (responseObject.errorCode !== 0) {
        document.getElementById("messages").innerHTML += '<span>ERROR: ' + + responseObject.errorMessage + '</span><br/>';
    }
}

// Called when a message arrives
function onMessageArrived(message) {
    console.log("onMessageArrived: " + message.payloadString);
    document.getElementById("messages").innerHTML += '<span>Topic: ' + message.destinationName + '  | ' + message.payloadString + '</span><br/>';
}

// Called when the disconnection button is pressed
function startDisconnect() {
    client.disconnect();
    document.getElementById("messages").innerHTML += '<span>Disconnected</span><br/>';
}



function DataPoolingFromServer() {

    $.ajax({
        url: "/Home/GetInformation",
        type: "GET",
        success: function (data) {
            $('#testarea').html(data);
        },
        error: function () {
            $("#testarea").html("ERROR");
        }
    });
}

var intervalVar;
startConnect();
$('#ConnectButton').click(function () {
    $.ajax({
        url: "/Home/Connect",
        type: "GET",
        datatype: "json",
        success: function (data) {
            $('#DisconnectButton').removeClass('disable');
            $('#ConnectButton').addClass('disable');
            intervalVar = setInterval(function () { DataPoolingFromServer(); }, 1000);
            alert("Connected");      
        }
    });
});

$('#DisconnectButton').click(function () {
    $.ajax({
        url: "/Home/Disconnect",
        type: "GET",
        datatype: "json",
        success: function (data) {
            $('#DisconnectButton').addClass('disable');
            $('#ConnectButton').removeClass('disable');
            $('#testarea').html("");
            clearInterval(intervalVar);
            alert("Disonnected");          
        }
    });
});



