let hourFormat = [];
function getDate(date) {
    var d = date.split("-").map(Number);
    //console.log(d);
    let payload = [];
    for (let i = 0; i < 24; i++) {
        month = parseInt(d[1]);
        day = parseInt(d[2]);
        //console.log(month + day);
        tmp = {
            "time_in_sec": i * 3660,
            "Month": month,
            "Day_of_month": day        }
        
        var hour = `${d[0]}-${d[1]}-${d[2]} ${i}:00:00`;
        //console.log(hour);
        hourFormat.push(hour);
        payload.push(tmp);
    }
    payloadJson = JSON.stringify(payload);
    return payloadJson;
}

function predictTemperature(date) {
    var data = getDate(date);

    var config = {
        method: 'post',
        url: 'http://localhost:5000/gboost',
        headers: {
            'Content-Type': 'application/json'
        },
        data: data
    };
    return new Promise(function (resolve, reject) {
        axios(config)
            .then(function (response) {
                var tmp = response.data["prediction"];
                resolve(tmp);

            })
            .catch(function (error) {
                console.log(error);
                reject(error);  
            });
    });
    
    
}

function predictDay() {
    var inputVal = document.getElementById("basicDate").value;
    //console.log(inputVal);
    visualizeChart(inputVal);
}

async function visualizeChart(date) {
    var result = await predictTemperature(date);
    //console.log(result);
    var trace1 = {
        type: "scatter",
        //fill: 'toself',
        mode: 'lines+markers',
        //fill: 'tonexty',
        name: 'Temperature',
        //x: ["2020-10-3 0:00:00", "2020-10-3 1:00:00", "2020-10-3 2:00:00", "2020-10-3 3:00:00", "2020-10-3 4:00:00", "2020-10-3 5:00:00", "2020-10-3 6:00:00", "2020-10-3 7:00:00", "2020-10-3 8:00:00", "2020-10-3 9:00:00", "2020-10-3 10:00:00", "2020-10-3 11:00:00", "2020-10-3 12:00:00", "2020-10-3 13:00:00", "2020-10-3 14:00:00", "2020-10-3 15:00:00", "2020-10-3 16:00:00", "2020-10-3 17:00:00", "2020-10-3 18:00:00", "2020-10-3 19:00:00", "2020-10-3 20:00:00", "2020-10-3 21:00:00", "2020-10-3 22:00:00", "2020-10-3 23:00:00"],
        //y: [52.50651364659498, 52.220512291682866, 51.803436640472725, 51.52272579200673, 50.894780295351055, 50.37440046095183, 50.328793725530886, 53.40630807826795, 58.231587761732534, 59.21347726787657, 60.425497929436155, 61.460338757394844, 61.26656527365754, 60.3040132501506, 59.530133017273776, 58.73028369351343, 59.09043579155105, 57.764529821784855, 56.32853959688089, 55.593997243173504, 54.23550173689696, 53.33574732644626, 52.39923664728884, 51.499628368814854],
        x: hourFormat,
        y: result,
        //line: { color: '#7F7F7F' }
        line: { color: '#fbb35d' }


    }

    var data = [trace1];
    var layout = {
        title: 'Temperature Prediction',
        showlegend: true
    };
    var config = {
        //displayModeBar: false,
        scrollZoom: false

    }

    Plotly.newPlot('chart', data, layout, config);
    hourFormat = [];
}



window.onload = function () {
    flatpickr("#basicDate", {});
    var d = new Date();
    date = d.getFullYear() + "-" + d.getMonth() + "-" + d.getDate();
    //console.log(date);
    visualizeChart(date);
}

