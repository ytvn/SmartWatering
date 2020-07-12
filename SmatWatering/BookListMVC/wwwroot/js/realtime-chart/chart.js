var model = [];
var listChart = {};
var Id;
var domain = "http://localhost:5005";

function getDeviceId() {
    Id = window.location.href.split('/');
    Id = Id[Id.length - 1];
    if (isNaN(parseInt(Id))) {
        Id = "";
    }
}
window.onload = function () {
    getDeviceId();
    $.ajax({
        type: "GET",
        url: domain + "/home/GetVariables/" + Id,
        async: false,
        success: function (data) {
            model = data;
        }
    });

    for (var i = 0; i < model.length; i++) {

        listChart[model[i].variableId] = new ApexCharts(document.querySelector("#chart-" + model[i].variableId), newSeries(model[i].variableName, chartColors[i % 7]))
        listChart[model[i].variableId].render();
        //document.getElementById("border-" + model[i].variableId).style.border = "thick solid " + chartColors[i % 7]
        //document.getElementById("border-" + model[i].variableId).style.borderTop = "thick solid " + chartColors[i % 7]
        document.getElementById("border-" + model[i].variableId).style.borderLeft = "thick solid " + chartColors[i % 7]
        document.getElementById("text-" + model[i].variableId).style.webkitTextFillColor = chartColors[i % 7]
        average(1, Id, model[i].variableId);
    }
}
function GetVariableValues() {
    var value;
    $.ajax({
        type: "GET",
        url: domain + "/home/GetVariableValues/" + Id,
        async: false,
        success: function (data) {
            value = data;
        }
    });
    return value;
    //Ex [{ Id: 1, Value: Math.random() }, { Id: 2, Value: Math.random() }, { Id: 3, Value: Math.random() }, { Id: 4, Value: Math.random() }]
}

window.setInterval(function () {
    var values = GetVariableValues();
    for (var i = 0; i < values.length; i++) {
        listChart[values[i].id].appendData([{
            data: [{
                x: Date.now(),
                y: values[i].value
            }]
        }], true)
        //listChart[values[i].id].resetSeries();
    }
}, 1000);

var xlabels = [];
var ylabels = [];
var data = [];
var chartColors = [
    ['rgb(231,74,59)'],
    ['rgb(255, 159, 64)'],
    ['rgb(246,194,62)'],
    ['rgb(75, 192, 192)'],
    ['rgb(54,185,204)'],
    ['rgb(153, 102, 255)'],
    ['rgb(201, 203, 207)']
];
function newSeries(name, color) {
    return {
        chart: {
            id: 'realtime',
            type: 'line',
            height: 350,
            width: "100%",
            animations: {
                enabled: false,
                easing: 'linear',
                dynamicAnimation: {
                    speed: 1000
                }
            },
            toolbar: {
                show: true,
                offsetX: 0,
                offsetY: 0,
                tools: {
                    download: true,
                    selection: true,
                    zoom: true,
                    zoomin: true,
                    zoomout: true,
                    pan: false,
                    reset: true | '<img src="/static/icons/reset.png" width="20">',
                    customIcons: []
                },
                autoSelected: 'zoom'
            }
        },
        dataLabels: {
            enabled: true,
        },
        markers: {
            size: 1
        },
        series: [{
            name: name,
            data: []
        }],
        colors: color,
        stroke: {
            curve: 'smooth',
            width: 2
        },
        yaxis: {
            type: 'linear',
            min: 0,
            max: 100,
            labels: {
                formatter: function (val) {
                    return (val).toFixed(2);
                },
            },
            title: {
                text: name
            },
        },
        xaxis: {
            type: 'datetime',
            range: 4000,
            categories: [],
            //tickPlacement: 'on',
            labels: {
                /**
                * Allows users to apply a custom formatter function to xaxis labels.
                *
                * @param { String } value - The default value generated
                * @param { Number } timestamp - In a datetime series, this is the raw timestamp 
                * @param { index } index of the tick / currently executing iteration in xaxis labels array
                */
                formatter: function (value, timestamp, index) {
                    return moment(new Date(timestamp)).format('HH:mm:ss')
                },

            },
        },

    };
}

function average(type, chipId, variableId) {
    var dwm = ["Day", "Week", "Month"];

    var name = "";
    model.forEach(e => {
        if (e.variableId == variableId)
            name = e.variableName;
    });
    document.getElementById("text-" + variableId).innerHTML = "Average " + name + " of " + dwm[type - 1];
    GetAverage(type, chipId, variableId);
}
var interval=[];
function GetAverage(type, deviceId, variableId) {
    var value;
    clearInterval(interval[variableId.toString()]);
    interval[variableId.toString()] = setInterval(function () {
        $.ajax({
            type: "GET",
            url: domain + "/home/average/?type=" + type + "&deviceId=" + deviceId + "&variableId=" + variableId,
            async: false,
            success: function (data) {
                value = data.average;
            }
        });
        console.log(value);
        if (value != -1)
            document.getElementById("avg-" + variableId).innerHTML = value;
    }, 3000);

    //Ex {"variableId":22,"average":41.91}
}

