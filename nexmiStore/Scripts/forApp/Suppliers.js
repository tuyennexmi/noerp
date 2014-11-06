var counter = 0;
var counter1 = 0;

function fnReloadCustomers() {
    $("#SupplierGrid").jqxGrid('updatebounddata');
}

function fnRefreshCustomers() {
    $("#txtKeywordSupplier").val("");
    $("#SupplierGrid").jqxGrid('removesort');
    fnReloadCustomers();
}

function fnShowCustomerForm(id, groupId) {
    LoadContent("", "Sales/CustomerForm?id=" + id + "&groupId=" + groupId);
}

function fnPopupCustomerDialog(id, groupId) {
    openWindow('Thêm nhà cung cấp mới', 'Sales/CustomerForm', { groupId: groupId }, '80%', '');
}

function fnDeleteCustomer(id) {
    var answer = confirm("Bạn muốn xóa nhà cung cấp này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Purchase/DeleteSupplier", { id: id },
        function(data) {
            CloseProcessing();
            if (data == "") {
                fnRefreshCustomers();
            }
            else {
                alert(data);
            }
        });
    }
}


//function fnLoadCustomers(viewType) {
//    switch (viewType) {
//        case "list":
//            LoadContent("", "Purchase/Suppliers");
//            break;
//        case "detail":
//            break;
//        default:
//            LoadContent("", "Purchase/Suppliers");
//            break;
//    }
//}

//function fnLoadCustomerDetail(id) {
//    if (id == "") {
//        var rowindex = $("#SupplierGrid").jqxGrid('getselectedrowindex');
//        if (rowindex == -1) {
//            rowindex = 0;
//        }
//        var dataRecord = $("#SupplierGrid").jqxGrid('getrowdata', rowindex);
//        id = dataRecord.CustomerId;
//    }
//    LoadContent("", "Purchase/SupplierDetail?id=" + id);
//}