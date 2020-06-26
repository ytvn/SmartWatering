$(document).ready(function () {
    realTime();
});
function realTime() {
    setInterval(function () {
        $.ajax({
            type: "GET",
            url: "http://localhost:5005/api/VariableValues/average",
            success: function (data) {
                document.getElementById("avg-moi").innerHTML = data[0].quantity + " %"
                document.getElementById("avg-tem").innerHTML = data[1].quantity + " &#8451;"
                document.getElementById("avg-hum").innerHTML = data[2].quantity + " %"
            }
        });
        
        $.ajax({
            type: "GET",
            url: "http://localhost:5005/api/VariableValues/4",
            success: function (data) {
                var tem = data[0].value;
                document.getElementById("level-water").innerHTML = tem + " %";
           
            }
        });
      
}, 3000);
}


