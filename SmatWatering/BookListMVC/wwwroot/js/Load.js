
function Load() {
    var chipId = document.getElementById("ChipId").value;
    var val;
    $.ajax({
        type: "GET",
        url: "/DevicePins/GetInfo?_chipId=" + chipId +"&type=0",
        async: false,
        success: function (data) {
            val = data;
        }
    });
    var list = document.getElementById("PIN");
    while (list.firstChild) {
        list.removeChild(list.lastChild);
    }
    console.log("vo r");
    val.forEach(e => {
        $("#PIN").append("<option value=" + e.pin + ">" + e.value + "</option>");
    });

}