var counter = 0;
var counter1 = 0;
var idTemp = "";

function fnShowCustomerForm(id) {
    var groupId = $('#CustomerGroupId').val();
    LoadContent('', 'Sales/CustomerForm?id=' + id + '&groupId=' + groupId);
}

function fnPopupCustomerDialog(id, groupId, parentElement) {
    openWindow('Thêm khách hàng mới', 'Sales/CustomerForm', { groupId: groupId, parentElement: parentElement }, '90%', '');
}

function fnDeleteCustomer(id, groupId) {
    if (confirm("Bạn muốn xóa khách hàng này?")) {
        //OpenProcessing();
        $.post(appPath + "Sales/DeleteCustomer", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined)
                    fnRefreshCustomers();
                else if (nextId == "")
                    fnLoadCustomers();
                else
                    fnLoadCustomerDetail(nextId, groupId);
            }
            else {
                alert(data);
            }
        });
    }
}

function fnLoadCustomers(page) {
    var viewType = $('#ViewType').val();
    var groupId = $('#CustomerGroupId').val();
    var keyword = $("#txtKeywordCustomerList").val();
    var area = '';
    try {
        area = $('#cbbAreas').jqxComboBox('getSelectedItem').value;
    } catch (err) { }

    LoadContentDynamic("CustomerGrid", "Sales/CustomerList", {
        groupId: groupId,
        viewType: viewType,
        keyword: keyword,
        area: area,
        pageNum: page
    });
}

function fnChangeCustomersView(viewType) {
    var groupId = $('#CustomerGroupId').val();
    var keyword = $("#txtKeywordCustomerList").val();
    var area = '';
    try {
        area = $('#cbbAreas').jqxComboBox('getSelectedItem').value;
    } catch (err) { }

    LoadContentDynamic("CustomerGrid", "Sales/CustomerList", {
        groupId: groupId,
        viewType: viewType,
        keyword: keyword,
        area: area
    });
}


function fnLoadCustomerDetail(id, groupId) {
    if (id == "") {
        var rowindex = $("#CustomerGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#CustomerGrid").jqxGrid('getrowdata', rowindex);
        if (dataRecord != null) {
            id = dataRecord.CustomerId;
        }
        else {
            alert('Bạn đã lướt hết danh sách!');
        }
    }
    if (id != "") {
        LoadContent("", "Sales/CustomerDetail?id=" + id + "&groupId=" + groupId);
    }
}

function fnReloadCustomers() {
    var area = $('#cbbAreas').jqxComboBox('getSelectedItem').value;
    var keyword = $("#txtKeywordCustomerList").val();
    var groupId = $("#txtGroupId").val();
    fnLoadCustomerList('', keyword, area, groupId);
}

function fnPopupCustomerDetail(id, mode) {
    //LoadContent("", "Sales/CustomerDetail?id=" + id + "&groupId=" + groupId);
    openWindow('Thông tin khách hàng', 'Sales/CustomerDetail', { id: id, mode: mode }, '90%', '');
}
