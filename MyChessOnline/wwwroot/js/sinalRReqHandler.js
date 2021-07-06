var connection = new signalR.HubconnectionBuilder().withUrl('/Home/Index').build();

hubConnection.on("Receive", function (data) {

    let elem = document.createElement("p");
    elem.appendChild(document.createTextNode(data));
    let firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);

});

connection.start().catch(error => {
    error.message
});

function sendMessageToHub(message) {
    connection.invoke('send')
}