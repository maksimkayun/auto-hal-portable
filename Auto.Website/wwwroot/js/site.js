$(document).ready(connectToSignalR);
function displayNotification(user, json) {
    console.log(json);
    var $target = $('div#signalr-notifications');
    var data = JSON.parse(json);
    var message = `OWNER <a href="/api/owners/${data.Email}">${data.Email}</a> CHANGED HIS VEHICLE ! OLD:${data.OldVehicle}, NEW:${data.LastName}`;
    var $div = $(`<div>${message}</div>`);
    $target.prepend($div);
    window.setTimeout(function () { $div.fadeOut(2000, function () { $div.remove(); }); }, 8000);
}

function connectToSignalR() {
    console.log("Connecting to SignalR...");
    window.notificationDivs = new Array();
    var conn = new signalR.HubConnectionBuilder().withUrl("/hub").build();
    conn.on("DisplayNotification", displayNotification);
    conn.start().then(function () {
        console.log("SignalR has started.");
    }).catch(function (err) {
        console.log(err);
    });
}