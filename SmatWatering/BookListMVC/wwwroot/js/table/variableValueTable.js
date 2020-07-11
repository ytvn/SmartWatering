var dataTable;
var count;
$(document).ready(function () {
    loadDataTable();
    count = 0;
    //realTime();
});

function realTime() {
    setInterval(function () {
        $.ajax({
            type: "GET",
            url: "/VariableValues/ApiIndex",
            success: function (data) {
                console.log(count);
                if (data.quantity > count) {
                    //loadDataTable();
                    dataTable.ajax.reload()
                    //console.log("log");
                }
                count = data.quantity
            }
        });
    }, 3000);
}


function loadDataTable() {
   
    dataTable = $('#DT_load').DataTable({
        //"ordering": false,
        "ajax": {
            "url": "/VariableValues/ApiIndex",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "variableName", "width": "20%" },
            { "data": "value", "width": "10%" },
            { "data": "pin", "width": "10%" },
            { "data": "chipId", "width": "15%" },
            { "data": "createdDate", "width": "25%" },
            {
                "data": "variableValueId",
                "render": function (data) {
                    return `<div class="text-center">
                        <a class="text-danger" onclick=Delete('/VariableValues/Delete/'+${data},true)>
                            <i  class="fas fa-trash fa-lg"></i>
                        </a>
                        </div>`;
                },
                "width": "10%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
    dataTable.column('4:visible')
        .order('desc')
        .draw();
}

