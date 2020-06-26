var dataTable;
var count;
$(document).ready(function () {
    loadDataTable();
    count = 0;
    realTime();
});

function realTime() {
    setInterval(function () {
        $.ajax({
            type: "GET",
            url: "http://localhost:5005/VariableValues/ApiIndex",
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
            "url": "VariableValues/ApiIndex",
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
                        <a class="text-danger" onclick=Delete('/VariableValues/Delete?id='+${data},true)>
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

//function Delete(url) {
//    swal({
//        title: "Are you sure?",
//        text: "Once deleted, you will not be able to recover",
//        icon: "warning",
//        buttons: true,
//        dangerMode: true
//    }).then((willDelete) => {
//        if (willDelete) {
//            $.ajax({
//                type: "DELETE",
//                url: url,
//                success: function (data) {
//                    if (data.success) {
//                        toastr.success(data.message);
//                        dataTable.ajax.reload();
//                    }
//                    else {
//                        toastr.error(data.message);
//                    }
//                }
//            });
//        }
//    });
//}