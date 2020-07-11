﻿var model = [];
var listChart = {};
var Id;
window.onload = function () {
    Id = window.location.href.slice(-1);
    if (isNaN(parseInt(Id))) {
        Id = "";
    }
    console.log(Id);
    $.ajax({
        type: "GET",
        url: "http://localhost:5005/home/GetVariables/"+Id,
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
        //document.getElementById("bborder-" + model[i].variableId).style.borderBottom = "thick solid " + chartColors[i % 7]
        document.getElementById("text-" + model[i].variableId).style.webkitTextFillColor = chartColors[i % 7]
        
    }
}
function generateData() {
    var value;
    $.ajax({
        type: "GET",
        url: "http://localhost:5005/home/GetVariableValues/" + Id,
        async: false,
        success: function (data) {
            value = data;
        }
    });
    return value;
    //return [{ Id: 1, Value: Math.random() }, { Id: 2, Value: Math.random() }, { Id: 3, Value: Math.random() }, { Id: 4, Value: Math.random() }]
}
window.setInterval(function () {
    var values = generateData();
    for (var i = 0; i < values.length; i++) {
        listChart[values[i].id].appendData([{
            data: [{
                x: Date.now(),
                y: values[i].value
            }]
        }], true)
        //listChart[values[i].id].resetSeries();
    }


    //var values = generateData();
    //for (var i = 0; i < values.length; i++) {
    //    listChart[values[i].Id].updateSeries([{
    //        data: values[i].Value
    //    }])
    //}
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
                enabled: true,
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
            enabled: false,
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
            width: 2,
        },
        yaxis: {
            type: 'linear',
            min: 0,
            max:1,
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
            range: 10000,
            categories: [],
            tickPlacement: 'on',
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





