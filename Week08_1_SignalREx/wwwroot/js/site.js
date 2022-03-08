// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.on('ReceiveMessage', displayMessage);
connection.start();

//Send messages to server
var msgForm = document.forms.msgForm;

msgForm.addEventListener('submit', function (e) {
    e.preventDefault();
    var userMessage = document.getElementById('userMessage');
    var text = userMessage.value;
    userName = document.getElementById('username').value;
    sendMessage(userName, text);
})

function sendMessage(userName, message) {
    if (message && message.length) {
        connection.invoke("SendAllMessages", userName, message);
    }
}

function displayMessage(name, time, message) {
    var friendlyTime = moment(time).format('H:mm:ss');

    var userLi = document.createElement('li');
    userLi.className = 'userLi list-group-item';
    userLi.textContent = friendlyTime + ", " + name + " say:";

    var messageLi = document.createElement('li');
    messageLi.className = 'messageLi list-group-item pl-5';
    messageLi.textContent = message;

    var chatHistoryUl = document.getElementById('chatHistory');
    chatHistoryUl.appendChild(userLi);
    chatHistoryUl.appendChild(messageLi);

    $('#chatHistory').animate({scrollTop: $('#chatHistory').prop('scrollHeight')},50);
}



