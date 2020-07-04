var chartColors = {
	red: 'rgb(231,74,59)',
	orange: 'rgb(255, 159, 64)',
	yellow: 'rgb(246,194,62)',
	green: 'rgb(75, 192, 192)',
	blue: 'rgb(54,185,204)',
	purple: 'rgb(153, 102, 255)',
	grey: 'rgb(201, 203, 207)'
};


function ScalingFactor(id) {
	var result;
	$.ajax({
		type: "GET",
		url: "http://localhost:5005/api/VariableValues/all",
		async: false,
		success: function (data) {
			result = data;
		}
	});
	return result;
}

function onRefresh(chart) {
	var data = ScalingFactor();
	chart.config.data.datasets[0].data.push({
		x: Date.now(),
		y: data.id2[0].value 
	});
	chart.config.data.datasets[1].data.push({
		x: Date.now(),
		y: data.id3[0].value 
	});
	chart.config.data.datasets[2].data.push({
		x: Date.now(),
		y: data.id4[0].value 
	});
	setGaugeValue(gaugeElement1, data.id1[0].value, 1);
	setGaugeValue(gaugeElement2, data.id2[0].value, 2);
	setGaugeValue(gaugeElement3, data.id3[0].value, 3);
	setGaugeValue(gaugeElement4, data.id4[0].value, 4);
	//var val = ScalingFactor(3);
	//console.log(chart.config2.data)
	//chart.config2.data.datasets[0].data.push({
	//	x: Date.now(),
	//	y: val.value
	//});

}
var color = Chart.helpers.color;
var config = {
	type: 'line',
	data: {
		datasets: [{
			label: 'Temperature',
			backgroundColor: chartColors.red,
			borderColor: chartColors.red,
			fill: false,
			lineTension: 0,
			data: []
		},
		{
			label: 'Humidity',
			backgroundColor: chartColors.blue,
			borderColor: chartColors.blue,
			fill: false,
			lineTension: 0,
			data: []
		},
		{
			label: 'Moisture',
			backgroundColor: chartColors.yellow,
			borderColor: chartColors.yellow,
			fill: false,
			lineTension: 0,
			data: []
		}]
	},
	options: {
		title: {
			display: true,
			text: 'Overview'
		},
		scales: {
			xAxes: [{
				type: 'realtime',
				realtime: {
					duration: 20000,
					refresh: 4000,
					delay: 2000,
					onRefresh: onRefresh
				}
			}],
			yAxes: [{
				type: 'linear',
				display: true,
				scaleLabel: {
					display: true,
					labelString: '°C / %'
				}
			}]
		},
		tooltips: {
			mode: 'nearest',
			intersect: false
		},
		hover: {
			mode: 'nearest',
			intersect: false
		},
		plugins: {
			datalabels: {
				backgroundColor: function (context) {
					return context.dataset.backgroundColor;
				},
				borderRadius: 4,
				clip: true,
				color: 'white',
				font: {
					weight: 'bold'
				},
				formatter: function (value) {
					return value.y;
				}
			}
		}
	}
};
window.onload = function () {

	var ctx = document.getElementById('myChart').getContext('2d');
	window.myChart = new Chart(ctx, config);


};


const gaugeElement1 = document.querySelector(".gauge1");
const gaugeElement2 = document.querySelector(".gauge2");
const gaugeElement3 = document.querySelector(".gauge3");
const gaugeElement4 = document.querySelector(".gauge4");

function setGaugeValue(gauge, value, flag) {
	if (value < 0 || value > 100) {
		return;
	}
	switch (flag) {
		case 1:
			gauge.querySelector(".gauge__fill1").style.transform = `rotate(${
				value / 200
				}turn)`;
			gauge.querySelector(".gauge__cover1").textContent = `${value}%`;
			break;
		case 2:
			gauge.querySelector(".gauge__fill2").style.transform = `rotate(${
				value / 200
				}turn)`;
			gauge.querySelector(".gauge__cover2").innerHTML = `${value}&#8451;`;
			break;
		case 3:
			gauge.querySelector(".gauge__fill3").style.transform = `rotate(${
				value / 200
				}turn)`;
			gauge.querySelector(".gauge__cover3").textContent = `${value}%`;
			break;
		case 4:
			gauge.querySelector(".gauge__fill4").style.transform = `rotate(${
				value / 200
				}turn)`;
			console.log(value);
			gauge.querySelector(".gauge__cover4").textContent = `${value}%`;
			break;
	}

}











//document.getElementById('randomizeData').addEventListener('click', function () {
//	config.data.datasets.forEach(function (dataset) {
//		dataset.data.forEach(function (dataObj) {
//			dataObj.y = randomScalingFactor();
//		});
//	});

//	window.myChart.update();
//});

//var colorNames = Object.keys(chartColors);
//document.getElementById('addDataset').addEventListener('click', function () {
//	var colorName = colorNames[config.data.datasets.length % colorNames.length];
//	var newColor = chartColors[colorName];
//	var newDataset = {
//		label: 'Dataset ' + (config.data.datasets.length + 1),
//		backgroundColor: newColor,
//		borderColor: newColor,
//		fill: false,
//		lineTension: 0,
//		data: []
//	};

//	config.data.datasets.push(newDataset);
//	window.myChart.update();
//});

//document.getElementById('removeDataset').addEventListener('click', function () {
//	config.data.datasets.pop();
//	window.myChart.update();
//});

//document.getElementById('addData').addEventListener('click', function () {
//	onRefresh(window.myChart);
//	window.myChart.update();
//});