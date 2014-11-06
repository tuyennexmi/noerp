var counter = 0;
var idTemp = "";

function fnPopupImportDialog(id, mode) {
    LoadContent("", "Stocks/ImportForm?id=" + id + "&mode=" + mode);
}

function fnDeleteImport(id) {
    var answer = confirm("Bạn muốn xóa phiếu này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Stocks/DeleteImport", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined)
                    fnRefreshImport();
                else if (nextId == "")
                    fnLoadImports();
                else
                    fnLoadImportDetail(nextId);
            }
            else {
                alert(data);
            }
        });
    }
}

function fnConfirmImport(id, status) {
    if (confirm('Xác nhận nhập kho?')) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Stocks/ConfirmImport',
            data: {
                id: id
            },
            success: function (data) {
                if (data.error == "") {
                    $(window).hashchange();
                }
                else {
                    alert(data.error);
                }
            }
        });
    }
}

function fnCancelImport(id, status) {
    if (confirm("Bạn muốn hủy phiếu này?")) {
        fnSetImportStatus(id, status, '');
    }
}

function fnSetImportStatus(id, status, typeId) {
    $.post(appPath + "Common/SetStatus", { objectName: "Imports", id: id, status: status, typeId: typeId }, function (data) {
        if (data != "") {
            alert(data);
        } else {
            $(window).hashchange();
        }
    });
}

function fnPrintImport(id) {
    $.get(appPath + "Stocks/PrintImport", { id: id }, function (data) {
        $.post(appPath + "UserControl/GetDataUrl", { html: Base64.encode(data), mode: 'ORD' }, function (data2) {
            window.open(appPath + "UserControl/PrintPage");
        });
    });
}

function fnLoadImports() {
    var status = ''; 
    if (status == undefined) {
        status = '';
    }
    LoadContent('', 'Stocks/Imports?status=' + status);
}

function fnLoadImportDetail(id) {
    if (id == "") {
        var rowindex = $("#ImportGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#ImportGrid").jqxGrid('getrowdata', rowindex);
        id = dataRecord.ImportId;
    }
    LoadContent("", "Stocks/Import?id=" + id);
}

function fnGetPurchaseOrderDetails(orderId, windowId) {
    $("#txtInvoiceId").val(id);
    $("#txtSupplierId").val(supplierId);
    $("#txtSupplierName").val(supplierName);
    $.ajaxSetup({ async: false });
    $.post(appPath + "UserControl/GetPurchaseOrderDetails", { id: orderId }, function (data) {
        for (var x in data) {
            var productId = data[x][0];
            var productName = data[x][1];
            var goodQuantity = data[x][2];
            var badQuantity = data[x][3];
            var unitName = data[x][4];
            var elm = document.getElementById(productId);
            var row = "";
            row = "<tr id=\"row" + productId + "\" onclick='javascript:fnSetInfo(\"" + productId + "\",\"" + productName + "\",\"" + goodQuantity + "\",\"" + badQuantity + "\",\"" + unitName + "\",\"\")'>";
            row += "<td><input type=\"checkbox\" id=\"" + productId + "\" name=\"IDetails\" /></td>";
            row += "<td>" + productName + "</td>";
            row += "<td>" + goodQuantity + "(" + unitName + ")</td>";
            row += "<td>" + badQuantity + "(" + unitName + ")</td>";
            //row += "<td><a href='javascript:fnPopupImportDetailDialog(\"" + productId + "\")'>Sửa</a></td>";
            row += "</tr>";
            if (elm == undefined) {
                $("#tbodyDetails").append(row);
            }
            else {
                $("#row" + productId).replaceWith(row);
            }
        }
    });
    try {
        closeWindow(windowId);
    } catch (Error) { }
    $.ajaxSetup({ async: true });
}

function fnImports2Excel(mode) {
    $.download(appPath + 'Common/Imports2Excel',
            'keyword=' + $('#txtKeywordImport').val() +
            '&status=' + $("#slStatus").val() +
            '&mode=' + mode +
            '&from=' + $('#dtFrom').val() +
            '&to=' + $('#dtTo').val()
            );
}

function fnReceive(id) {
    openWindow('Nhập sản phẩm vào kho', 'Stocks/ReceiveForm', { id: id }, 600, 400);
}

function fnImportReturn(id) {
    openWindow('Xuất hàng trả', 'Stocks/ImportReturnForm', { id: id }, 600, 400);
}

/////////////Detail

function fnShowImportDetail(productId) {
    $.get(appPath + 'Stocks/ImportDetailForm', { productId: productId }, function (data) {
        $("#formImportDetail").html(data);
    });
}

function fnSaveImportDetail() {
    if ($('#formImportDetail').jqxValidator('validate')) {
        var Product = $("#cbbProducts").jqxComboBox('getSelectedItem');
        var goodQuantity = $("#txtGoodQuantity").autoNumeric('get');
        if (goodQuantity == "") goodQuantity = 0;
        var badQuantity = $("#txtBadQuantity").autoNumeric('get');
        if (badQuantity == "") badQuantity = 0;
        var ordinalNumber = $("#txtOrdinalNumber").val();
        var unitName = $("#lbUnitName").text();
        var productId = Product.value;
        var productName = Product.label;
        var description = $('#txtDetailDescription').val();
        var strSNs = $('#txtSNs').val();
        $.post(appPath + "Stocks/AddImportDetail", {
            ordinalNumber: ordinalNumber,
            productId: productId, unitId: $('#slProductUnits').val(),
            goodQuantity: goodQuantity, badQuantity: badQuantity, description: description, strSNs: strSNs
        }, function (data) {
            $("#tbodyDetails").html(data);
            fnResetFormImportDetail();
        });
    }
}

function fnRemoveImportDetail(productId) {
    $.post(appPath + "Stocks/RemoveImportDetail", { productId: productId }, function (data) {
        $("#" + productId).remove();
    });
}

function fnResetFormImportDetail() {
    $('#txtOrdinalNumber').val('0');
    $('#cbbProducts').jqxComboBox('clearSelection');
    $('#txtDetailDescription').val('');
    $('#txtGoodQuantity').autoNumeric('set', '1.00');
    $('#txtBadQuantity').autoNumeric('set', '0.00');
    $('#txtSNs').tagit('removeAll');
    $('#cbbProducts').find(".jqx-combobox-input").focus();
}


function fnReloadImport(page, type) {
    LoadContentDynamic('ImportGrid', 'Stocks/ImportList', {
        pageNum: page,
        typeId: type,
        status: $('#slStatus').val(),
        keyword: $('#txtKeywordImport').val(),
        from: $('#dtFrom').val(),
        to: $('#dtTo').val(),
        partnerId: document.getElementsByName('cbbCustomers')[0].value
    });
}