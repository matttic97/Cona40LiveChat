"use strict";

var maxMessageQueueSize = 20;

const screen = document.getElementById("screen");
const connection = new signalR.HubConnectionBuilder().withUrl("/liveChatHub").build();

connection.on("ReceiveMessage", (message)=> {
    addMessage(message.user, message.text)
});
connection.on("InitMessages", function (maxMessageQueueSizeOverride, messages) {
    maxMessageQueueSize = maxMessageQueueSizeOverride;
    messages.forEach((message) => addMessage(message.user, message.text));
});
connection.start().then(function () {
    connection.invoke("JoinProjection")
        .then(() => console.log("Connected!"))
        .catch(err => console.error(err.toString()));
}).catch(function (err) {
    return console.error(err.toString());
});

function checkSessionExpiry() {
    
}

function addMessage(user, text) {
    const wrapper = document.createElement("div");
    wrapper.className = "message";

    const username = document.createElement("div");
    username.className = "message-user";
    username.textContent = user;

    const bubble = document.createElement("div");
    bubble.className = "message-bubble";
    bubble.textContent = text;

    wrapper.appendChild(username);
    wrapper.appendChild(bubble);
    screen.appendChild(wrapper);
    
    while (screen.scrollHeight > screen.clientHeight) {
        if (screen.firstChild) {
            screen.removeChild(screen.firstChild);
        } else {
            break;
        }
    }
    
    screen.scrollTop = screen.scrollHeight;
}