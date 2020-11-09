$(document).ready(function () {
    getVariable();
});


function getVariable() {
    var val;
    $.ajax({
        type: "GET",
        url: "/triggers/getinfo",
        async: false,
        success: function (data) {
            val = data;
        }
    });
    var list = document.getElementById("Variable");
    while (list.firstChild) {
        list.removeChild(list.lastChild);
    }
    val.forEach(e => {
        $("#Variable").append("<option value=" + e.variableId + ">" + e.variableName + "</option>");
    });

}

function stop() {
    $.ajax({
        type: "GET",
        url: "/triggers/stop",
        async: false,
        success: function (data) {
            
        }
    });
} 