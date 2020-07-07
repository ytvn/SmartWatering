﻿var model = [];
var listChart = {};
window.onload = function () {
    
    $.ajax({
        type: "GET",
        url: "Home/GetVariables",
        async: false,
        success: function (data) {
            model = data;
        }
    });
    for (var i = 0; i < model.length; i++) {

        listChart[model[i].variableId] = new ApexCharts(document.querySelector("#chart-" + model[i].variableId), newSeries(model[i].variableName, chartColors[i%7]))
        listChart[model[i].variableId].render();
    }
}
function generateData() {

    return [{ Id: 1, Value: Math.random() }, { Id: 2, Value: Math.random() }, { Id: 3, Value: Math.random() }, { Id: 4, Value: Math.random() }]
}
window.setInterval(function () {
    var values = generateData();
    for (var i = 0; i < values.length; i++){
    listChart[values[i].Id].appendData([{
        data: [{
            x: Date.now(),
            y: values[i].Value
            }]
    }])
}

}, 1000);
var xlabels = [];
var ylabels = [];
var chartColors = [
    ['rgb(231,74,59)'],
    ['rgb(255, 159, 64)'],
    ['rgb(246,194,62)'],
    ['rgb(75, 192, 192)'],
    ['rgb(54,185,204)'],
    ['rgb(153, 102, 255)'],
    ['rgb(201, 203, 207)']
];
function newSeries(name,color) {
    return {
        chart: {
            type: 'line',
            height: 350
        },
        dataLabels: {
            enabled: false,
        },
        series: [{
            name: name,
            data: []
        }],
        colors: color,
        yaxis: {
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
            categories: [],
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
                }
            }
        }
    };
}





