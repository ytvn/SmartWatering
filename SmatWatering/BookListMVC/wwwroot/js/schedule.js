$(document).ready(function () {
    myFunction();
    getDevice();
});
function myFunction() {
    var x = document.getElementById("intervalType").value;
    var type = x.substring(10); 
    document.getElementById("Interval").innerHTML = "Interval(" + type + ")";
}
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
        $("#Device").append("<option value=" + e.chipId + ">" + e.name + "</option>");
    });
    getPin(val[0].chipId);

}
function getPin(chipId) {
    var val;
    $.ajax({
        type: "GET",
        url: "http://localhost:5005/DevicePins/GetInfo?_chipId=" + chipId + "&type=1",
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
