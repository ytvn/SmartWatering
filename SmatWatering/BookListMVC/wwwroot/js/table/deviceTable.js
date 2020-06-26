var dataTable;
var count;
$(document).ready(function () {
    loadDataTable();
    //count = 0;
    //realTime();
});

function realTime() {
    setInterval(function () {
        $.ajax({
            type: "GET",
            url: "http://localhost:5005/Devices/ApiIndex",
            success: function (data) {
                console.log(count);
                if (data.quantity > count) {
                    //loadDataTable();
                    dataTable.ajax.reload()
                    console.log("log");
                }
                count = data.quantity
            }
        });
    }, 3000);
}


function loadDataTable() {
   
    dataTable = $('#devices-table').DataTable({
        //"ordering": false,
        "ajax": {
            "url": "Devices/ApiIndex",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "chipId", "width": "30%" },
            { "data": "description", "width": "30%" },
            {
                "data": "variableValueId",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/VariableValues/Edit?id=${data}" class="text-danger"><i class="fas fa-pen"></i></a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer; width:70px;'
                            onclick=Delete('/VariableValues/Delete?id='+${data},true)>
                            Delete
                        </a>
                        </div>`;//<i class="fas fa-trash"></i>
                },
                "width": "30%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
   
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