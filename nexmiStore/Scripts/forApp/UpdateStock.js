var counter = 0;

//$(document).ready(function () {
//    $("#btFillQuantity, #cbType1, #cbType2").click(function () {
//        var rows = $("#tbUpdateStock").find("tbody>tr");
//        if ($('input[name=cbType]:checked').val() == "1") {
//            for (var x = 0; x < rows.length; x++) {
//                $("#txtUpdate" + x).val($("#End" + x).html());
//            }
//        } else {
//            for (var x = 0; x < rows.length; x++) {
//                $("#txtUpdate" + x).val($("#Quantity" + x).html());
//            }
//        }
//    });
//});

//function fnPopupProductInStockDialog(id) {
//    var windowId = fnRandomString();
//    OpenPopup(windowId, "Cập nhật số lượng sản phẩm trong kho", appPath + "Stocks/ProductInStockForm", { id: id }, true, true, true, true, 600, 500);
//    var done = function () {
//        fnSaveOrUpdateProductInStock();
//    }
//    var cancel = function () {
//        ClosePopup(windowId);
//        if (counter > 0) {
//            fnLoadContent();
//        }
//    }
//    var reset = function () {
//        fnResetFormProductInStock();
//    };
//    var dialogOpts = {
//        buttons: {
//            "Hoàn tất": done,
//            "Đóng": cancel,
//            "Nhập lại": reset
//        }
//    };
//    $("#" + windowId).dialog(dialogOpts);
//}

//function fnSaveOrUpdateProductInStock() {
//    if (NMValidator("formProductInStock")) {
//        var id = $("#txtId").val();
//        var name = $("#txtName").val();
//        var unit = $("#slProductUnits").val();
//        var VAT = $("#txtVAT").val().replace(/,/g, "");
//        var description = $("#txtDescription").val();
//        var productCategory = $("#slProductCategories1").val();
//        var manufactureId = $("#txtManufacturerId").val();
//        showProcessing();
//        $.post(appPath + "Directories/SaveOrUpdateProduct", { id: id, name: name, unit: unit, description: description,
//            productCategory: productCategory, manufactureId: manufactureId, VAT: VAT
//        },
//        function (data) {
//            if (data == "") {
//                showSuccess();
//                counter++;
//                fnResetFormProductInStock();
//            }
//            else {
//                alert(data);
//            }
//            showButtons();
//        });
//    }
//}

//function fnDeleteProductInStock(id) {
//    var answer = confirm("Bạn muốn xóa sản phẩm này?");
//    if (answer) {
//        OpenProcessing();
//        $.post(appPath + "Stocks/DeleteProductInStock", { id: id },
//        function(data) {
//            CloseProcessing();
//            if (data == "") {
//                fnLoadContent();
//            }
//            else {
//                alert(data);
//            }
//        });
//    }
//}

//function fnResetFormProductInStock() {
//    $("#txtId").val("");
//    $("#txtName").val("");
//    $("#txtDescription").val("");
//    $("#txtVAT").val("");
//    $("#txtId").focus();
//}

function fnLoadContent(stockId) {
    if (stockId == undefined) {
        stockId = $("#slStocks").val();
    }
    var categoryId = '';
    try {
        categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
    } catch (err) { }
    LoadContent('', 'Stocks/UpdateStock?stockId=' + stockId + '&categoryId=' + categoryId);
}

function fnUpdateStock() {
    var rows = $("#tbUpdateStock").find("tbody>tr");
    var quantity, stockId, productId;
    for (var x = 0; x < rows.length; x++) {
        stockId = $("#StockId" + x).val();
        productId = $("#ProductId" + x).val();
        quantity = $("#txtUpdate" + x).val();
        badQuantity = $("#txtBad" + x).val();
        $.post(appPath + "Stocks/UpdateProductInStock", { stockId: stockId, productId: productId, quantity: quantity, badQuantity: badQuantity }, function (data) {
            if (data != "") {
                alert(data);
            }
        });
    }
}