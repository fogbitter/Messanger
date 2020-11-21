//Сущности
class Message {
    Id;
    User;
    Message;
    Dialog;
    Date;
}
class User {
    Id;
    Name;
    Email = '';
    Password = '';
}
class Dialog {
    Id;
    Name;
}
class DialogUserLink {
    Id;
    DialogId;
    UserId;
}
class CreateDialogDto {
    Dialog;
    Users;
}

//глобальные переменные
var currentUser;
var dialogs;
var activeDialog;

//адреса методов сервера
var mainUri = 'https://localhost:44337/api/messanger';
var loginUserUri = mainUri + '/loginuser'
var signupUserUri = mainUri + '/signupuser'
var getMessagesUri = mainUri + '/getmessages'
var getDialogsUri = mainUri + '/getdialogs'
var createDialogUri = mainUri + '/createdialog'

//устанавливаем связь с сервером через SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44337/messangersocket", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

//функция подключения к SignalR
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log("SignalR Connection error.");
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(start);
//событие на получении сообщения для всех пользователей
connection.on("ReceiveOne", (message) => { this.receivedMessage(message); });
//событие на получении сообщения для всех пользователей
connection.on("ReceiveDialogMessage", (message) => { this.receivedMessage(message); });

// Start the connection.
start();

//Функция для выова сервисов 
async function webapicall(uri, obj, callBackFunc) {
    //alert(JSON.stringify(obj));
    //вызов сервиса
    const response = await fetch((uri), {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(obj)
    }).catch();
    //если ответ нормальный
    if (response.ok === true) {
        const objResponse = await response.json();
        //обработка ответа сервиса
        callBackFunc(objResponse);
        console.log('success');
    }
    //если ответ не нормальный
    if (response.ok === false) {
        console.log('not found');
    }
}

//Авторизация
function login() {
    var email = $('.loginEmailInput').val();
    var password = $('.loginPasswordInput').val();
    if (email === '') {
        alert('Поле Email обязательное.');
        return;
    }
    if (password === '') {
        alert('Поле Password обязательное.');
        return;
    }
    //вызов метода сервиса
    webapicall(loginUserUri,
        { Email: email, Password: password },
        loginCallBackFunc);
}

function loginCallBackFunc(user) {
    this.currentUser = user;
    $('.authorisationBack').hide();
    $('#mainWindow').removeClass('d-none')
    webapicall(getDialogsUri,
        user,
        getDialogsCallBackFunc);
}

function signup() {
    var name = $('.signupNameInput').val();
    var email = $('.signupEmailInput').val();
    var password = $('.signupPasswordInput').val();
    if (name === '') {
        alert('Поле Name обязательное.');
        return;
    }
    if (email === '') {
        alert('Поле Email обязательное.');
        return;
    }
    if (password === '') {
        alert('Поле Password обязательное.');
        return;
    }
    webapicall(signupUserUri,
        { Name: name, Email: email, Password: password },
        signupCallBackFunc);
}

function signupCallBackFunc(user) {
    toLogin();
}

function getDialogsCallBackFunc(list) {
    var dialogWindow = $('.dialogBoxWindow');
    dialogWindow.empty();
    $('.chatWindow').empty();
    $('.chatWindow')[0].innerHTML = '<h2>Выберите диалог</h2>';
    list.forEach(dialog => connection.invoke("EnterGroup", dialog).catch(err => console.error(err)));
    list.forEach(dialog => drawDialog(dialogWindow, dialog));
}

function getMessagesCallBackFunc(list) {
    $('.chatWindow').empty();
    list.forEach(msg => drawMessage(msg.user, msg));
}

//Отправка сообщения
function send() {
    var message = new Message();
    message.User = this.currentUser;
    message.dialog = this.activeDialog;
    message.Message = $('.messageInput').val();
    //вызов метода на сервере
    connection.invoke("SendMessage", message).catch(err => console.error(err));
}

function receivedMessage(message) {
    console.log(message.user.name + " : " + message.message);
    drawMessage(message.user, message);
}

function drawMessage(user, message) {
    var current = currentUser.id === user.id;
    var appendHtml = getMessageHtml(user.name, message, current);
    var window = $(".chatWindow");
    window.append(appendHtml);
}

function drawDialog(dialogWindow, dialog) {
    dialogWindow.append('<li class="nav-item dialogBox" dialogId="' + dialog.id + '" dialogName="' + dialog.name + '" onClick="dialogBoxClick(this)"><a class="nav-link">' + dialog.name +'</a></li>');
}

function getMessageHtml(user, message, current) {
    var date = Date.parse(message.date);
    options = {hour: 'numeric', minute: 'numeric'};
    var formattedDate = Intl.DateTimeFormat('en-AU', options).format(date);
    if (current === false)
    {
        return '<div class="left" >' + user + '</div><div class="message darker"><p>' + message.message + '</p><span class="time-left">' + formattedDate + '</span></div >';
    }
    else if (current === true)
    {
        return '<div class="right" >' + user + '</div><div class="message"><p>' + message.message + '</p><span class="time-right">' + formattedDate + '</span></div >';
    }
    return '';
}

function toSignup() {
    $('.loginForm').hide();
    $('.signupForm').show();
}

function toLogin() {
    $('.loginForm').show();
    $('.signupForm').hide();
}

var newDialogUsers = [];
function onDialogAdd() {
    alert('ondialogadd');
    $('.dialogsBack').show();
    $('#mainWindow').addClass('d-none')
    newDialogUsers = [];
}
function addDialogUserAdd() {
    var userName = $('.addDialogUserInput').val();
    var exists;
    newDialogUsers.forEach(x => {
        if (x.Name === userName) {
            exists = true;
        }
    })
    if (exists) return;
    var newUser = new User();
    newUser.Name = userName
    newDialogUsers.push(newUser);
    $('.addDialogUsers').append('<div>' + userName + '</div>');
}
function addDialogSubmit() {
    var dialogName = $('.addDialogNameInput').val();
    var newDialog = new Dialog();
    newDialog.Name = dialogName;
    var createDialogDto = new CreateDialogDto();
    createDialogDto.Dialog = newDialog;
    createDialogDto.Users = newDialogUsers;
    webapicall(createDialogUri,
        createDialogDto,
        createDialogCallBackFunc);
    $('.dialogsBack').hide();
    $('#mainWindow').removeClass('d-none')
}
function createDialogCallBackFunc(dialog) {
    var dialogWindow = $('.dialogBoxWindow');
    drawDialog(dialogWindow, dialog);
}

var activeDialog
function dialogBoxClick(dialogBox) {
    var dialog = new Dialog();
    dialog.Id = dialogBox.attributes.dialogId.nodeValue;
    dialog.Name = dialogBox.attributes.dialogName.nodeValue;
    $('.messangerHeader').text(dialog.Name);
    webapicall(getMessagesUri,
        dialog,
        getMessagesCallBackFunc);
    activeDialog = dialog;
    $('.active').removeClass('active');
    dialogBox.firstChild.classList.add('active');
}
