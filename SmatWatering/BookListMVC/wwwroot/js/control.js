$(document).ready(function () {
    getDevice();
});

function updatePin() {
    var x = document.getElementById("Device").value;
    getPin(x);
}
function getDevice() {
    var val;
    $.ajax({
        type: "GET",
        url: "http://localhost:5005/devices/getinfo",
        async: false,
        success: function (data) {
            val = data;
        }
    });

    val.forEach(e => {
        $("#Device").append("<option value=" + e.chipId + ">" + e.description + "</option>");
    });
    getPin(val[0].chipId);

}
function getPin(chipId) {
    var val;
    $.ajax({
        type: "GET",
        url: "http://localhost:5005/DevicePins/GetInfo?_chipId=" + chipId,
        async: false,
        success: function (data) {
            val = data;
        }
    });
    var list = document.getElementById("Pin");
    while (list.firstChild) {
        list.removeChild(list.lastChild);
    }
    val.forEach(e => {
        $("#Pin").append("<option value=" + e.pin + ">" + e.value + "</option>");
    });
}
function removeChild() {
    var list = document.getElementById("myList");
    list.removeChild(list.childNodes[0]);
}
