
function ShowHelp() {
    //window.open(appPath + "Sub/HDSD PM THANHCONG WEB.htm", "_blank");
}

function fnShowChangePassword() {
    openWindow("Thay đổi mật khẩu", "UserControl/ChangePassword", {}, 500, 300);
}

function fnShowChangeInfo() {
    openWindow("Thông tin cá nhân", "UserControl/ChangeInfo", {}, 500, 400);
}

function fnSelected(id) {
    $(".clModule").css({ 'background-color': '#009966', 'color': '#fff' });
    $("#" + id).css({ 'background-color': '#fff', 'color': '#009966' });
}

function fnShowCustomers(parentId, title, groupId) {
    openWindow(title, "UserControl/CustomerList", { parentId: parentId, groupId: groupId }, "80%", "");
}

function fnShowCategories(parentId, objectName) {
    openWindow("Danh sách loại sản phẩm", "UserControl/CategoryList", { parentId: parentId, objectName: objectName }, "80%", "");
}

function fnShowUsers() {
    openWindow("Danh sách người dùng", "UserControl/UserList", {}, "80%", "");
}

function fnShowStages(title, data) {
    openWindow(title, 'UserControl/StageList', data, '80%', '');
}

function fnGetPriceForSalesInvoice(productId) {
    $.getJSON(appPath + "Common/GetPriceForSalesInvoice?productId=" + productId,
    function (data) {
        $("#lbCurrentPrice").html(data);
    });
}

function fnShowAreas(parentId) {
    openWindow("Chọn khu vực", "UserControl/AreaList", { parentId: parentId }, '70%', 550);
}

function fnShowSalesInvoice() {
    openWindow("Danh sách hóa đơn bán hàng", "UserControl/SaleInvoiceList", {}, '80%', '');
}

function fnShowPurchaseInvoice() {
    openWindow("Danh sách hóa đơn mua hàng", "UserControl/PurchaseInvoiceList", {}, '80%', '');
}

function fnShowSalesOrder() {
    openWindow("Danh sách đơn đặt hàng", "UserControl/SaleInvoiceList", {}, '80%', '');
}

function fnShowPurchaseOrder() {
    openWindow("Danh sách đơn mua hàng", "UserControl/PurchaseOrderList", {}, '80%', '');
}

function fnGetQuantityInStock(productId) {
    try {
        if (productId == undefined) {
            productId = $("#txtProductId").val();
        }
        if (productId == undefined) {
            var Product = $("#cbbProducts").jqxComboBox('getSelectedItem');
            productId = Product.value;
        }
        var stockId = $("#slStocks").val();
        $.post(appPath + "Common/GetQuantityInStock", { productId: productId, stockId: stockId }, function (data) {
            try {
                $("#txtGoodQuantityInStock").val(data.Good);
            } catch (Error) { }
            try {
                $("#txtBadQuantityInStock").val(data.Bad);
            } catch (Error) { }
        });
    } catch (Error) { }
}

function fnGetProductInStockDymanic(stockId) {
    if (stockId == undefined) {
        stockId = $("#slStocks").val();
    }
    $(".PISQuantity").each(function (index) {
        var elm = $(this);
        var productId = elm.attr('title');
        $.post(appPath + "Common/GetQuantityInStock", { productId: productId, stockId: stockId }, function (data) {
            elm.html(data.Good);
        });
    });
}

function fnPrintElement(id) {
    $('#' + id).printElement({ printBodyOptions: { styleToAdd: 'color: #000; font-family: Times New Roman;'} });
}