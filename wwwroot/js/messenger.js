"use strict";

const messageCooldownInSeconds = 5;
const btnDefaultTextContent = "&#10148;";

const userInputElement = document.getElementById("userInput");
const saveUserButton = document.getElementById("saveUserButton");
const messageInputElement = document.getElementById('messageInput');
const sendButtonElement = document.getElementById('sendMessageButton');
const charCountElement = document.getElementById('messageCharCount');

function updateMessageCooldownState() {
    const timeout = parseInt(sessionStorage.getItem("messageCooldownDelay"), 10);
    if (timeout > 0) {
        sendButtonElement.disabled = true;
        sendButtonElement.innerHTML = `(${timeout.toString()}) ${btnDefaultTextContent}`;
        sessionStorage.setItem("sendTimeout", (timeout - 1).toString());
    }
    else {
        const timeoutInterval = parseInt(sessionStorage.getItem("messageCooldownInterval"), 10);
        sendButtonElement.innerHTML = btnDefaultTextContent;
        sendButtonElement.disabled = false;
        clearInterval(timeoutInterval);
    }
}

function updateCharCount() {
    const charCount = messageInputElement.value.length;
    const maxChars = messageInputElement.getAttribute("maxlength");
    charCountElement.textContent = `${charCount}/${maxChars}`;
}

function setUser(user) {
    userInputElement.value = user;
    userInputElement.style.display = "none";
    saveUserButton.style.display = "none";
    document.getElementById("connectingMessage").style.display = "block";
    sessionStorage.setItem("user", user);
    setTimeout(() => {
        connection.start().catch((err) => {
            return console.error(err.toString());
        })
    }, 1000);
}

function loadForm() {
    if (sessionStorage.getItem("user") != null) {
        setUser(sessionStorage.getItem("user"));
    }
    else {
        document.getElementById("usernameForm").style.display = "block";

        const connection = new signalR.HubConnectionBuilder().withUrl("/liveChatHub").build();
        connection.on("RejectedConnection", (err) => {
            document.getElementById("connectingMessage").style.display = "none";
            document.getElementById("connectionFailureMessage").style.display = "block";
        });
        connection.on("AcceptedConnection", () => {
            sendButtonElement.disabled = false;
            document.getElementById("connectingMessage").style.display = "none";
            document.getElementById("connectionSuccessMessage").style.display = "block";
            document.getElementById("connectionSuccessMessage").textContent = `Povezani ste kot ${sessionStorage.getItem("user")}`;
            document.getElementById("chatForm").style.display = "block";
        });

        userInputElement.addEventListener("keyup", function (event) {
            saveUserButton.disabled = event.target.value.trim().length === 0;
        });
        saveUserButton.addEventListener('click', () => setUser(userInputElement.value));

        messageInputElement.addEventListener("input", updateCharCount);
        sendButtonElement.addEventListener("click", function (event) {
            const user = userInputElement.value;
            const message = messageInputElement.value;
            if (user === "" || message === "")
                return;

            sendButtonElement.disabled = true;
            connection.invoke("SendMessage", user, message)
                .then(() => {
                    messageInputElement.value = "";
                    updateCharCount();
                    sessionStorage.setItem("messageCooldownDelay", messageCooldownInSeconds.toString());
                    updateMessageCooldownState();
                    sessionStorage.setItem("messageCooldownInterval", setInterval(updateMessageCooldownState, 1000).toString());
                })
                .catch(function (err) {
                    sendButtonElement.disabled = false;
                    return console.error(err.toString());
                });
            event.preventDefault();
        });
    }
}

loadForm();