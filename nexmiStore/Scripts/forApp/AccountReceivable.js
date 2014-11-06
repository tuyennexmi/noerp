var counter = 0;

function fnPopupProductInStockDialog(id) {
    var windowId = fnRandomString();
    OpenPopup(windowId, "Cập nhật số lượng sản phẩm trong kho", appPath + "Stocks/ProductInStockForm", { id: id }, true, true, true, true, 600, 500);
    var done = function () {
        fnSaveOrUpdateProductInStock();
    }
    var cancel = function () {
        ClosePopup(windowId);
        if (counter > 0 && $('.ui-dialog:visible').length == 0) {
            fnLoadContent(undefined, "", "", "");
        }
    }
    var reset = function () {
        fnResetFormProductInStock();
    };
    var dialogOpts = {
        buttons: {
            "Hoàn tất": done,
            "Đóng": cancel,
            "Nhập lại": reset
        }
    };
    $("#" + windowId).dialog(dialogOpts);
}

function fnSaveOrUpdateProductInStock() {
    if (NMValidator("formProductInStock")) {
        var id = $("#txtId").val();
        var name = $("#txtName").val();
        var unit = $("#slProductUnits").val();
        var VAT = $("#txtVAT").val().replace(/,/g, "");
        var description = $("#txtDescription").val();
        var productCategory = $("#slProductCategories1").val();
        var manufactureId = $("#txtManufacturerId").val();
        showProcessing();
        $.post(appPath + "Directories/SaveOrUpdateProduct", { id: id, name: name, unit: unit, description: description,
            productCategory: productCategory, manufactureId: manufactureId, VAT: VAT
        },
        function (data) {
            if (data == "") {
                showSuccess();
                counter++;
                fnResetFormProductInStock();
            }
            else {
                alert(data);
            }
            showButtons();
        });
    }
}

function fnDeleteProductInStock(id) {
    var answer = confirm("Bạn muốn xóa sản phẩm này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Stocks/DeleteProductInStock", { id: id },
        function(data) {
            CloseProcessing();
            if (data == "") {
                fnLoadContent();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnResetFormProductInStock() {
    $("#txtId").val("");
    $("#txtName").val("");
    $("#txtDescription").val("");
    $("#txtVAT").val("");
    $("#txtId").focus();
}

function fnLoadContent() {
    var strError = "";
    var monthYear = "";
    var fromDate = "";
    var toDate = "";
    switch ($('input[name=cbType]:checked').val()) {
        case "1":
            break;
        case "2":
            //monthYear = $("#slMonths").val() + "/" + $("#slYears").val();
            monthYear = $("#txtMonthYear").val();
            if (monthYear == "") {
                strError = "Chưa chọn ngày!";
            }
            break;
        case "3":
            fromDate = $("#txtFromDate").val();
            toDate = $("#txtToDate").val();
            if (fromDate == "" || toDate == "") {
                strError = "Chưa chọn ngày!";
            }
            break;
    }
    if (strError == "") {
        LoadContent("", "Sales/AccountReceivable?monthYear=" + monthYear + "&fromDate=" + fromDate + "&toDate=" + toDate);
    } else {
        alert(strError);
    }
}