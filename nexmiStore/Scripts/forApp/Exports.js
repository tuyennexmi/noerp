var counter = 0;
var idTemp = "";

function fnPopupExportDialog(id, mode) {
    LoadContent("", "Stocks/ExportForm?id=" + id + "&mode=" + mode);
}

function fnDeleteExport(id) {
    var answer = confirm("Bạn muốn xóa phiếu này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Stocks/DeleteExport", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined)
                    fnRefreshExport();
                else if (nextId == "")
                    fnLoadExports();
                else
                    fnLoadExportDetail(nextId);
            }
            else {
                alert(data);
            }
        });
    }
}

function fnLoadExports() {
    var status = ''; // $("#slStatus").val();
    if (status == undefined) {
        status = "";
    }
    LoadContent("", "Stocks/Exports?status=" + status);
}

function fnLoadExportDetail(id) {
    if (id == "") {
        var rowindex = $("#ExportGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#ExportGrid").jqxGrid('getrowdata', rowindex);
        id = dataRecord.ExportId;
    }
    LoadContent("", "Stocks/Export?id=" + id);
}

function fnGetSalesInvoiceDetail(id, customerId, customerName) {
    $("#txtInvoiceId").val(id);
    $("#txtCustomerId").val(customerId);
    $("#txtCustomerName").val(customerName);
    $.ajaxSetup({ async: false });
    $.post(appPath + "UserControl/GetSaleInvoiceDetails", { id: id }, function (data) {
        for (var x in data) {
            var productId = data[x][0];
            var productName = data[x][1];
            var quantity = data[x][2];
            var unitName = data[x][3];
            var elm = document.getElementById(productId);
            var row = "";
            row = "<tr id=\"row" + productId + "\" onclick='javascript:fnSetInfo(\"" + productId + "\",\"" + productName + "\",\"" + quantity + "\",\"" + unitName + "\",\"\")'>";
            row += "<td><input type=\"checkbox\" id=\"" + productId + "\" name=\"EDetails\" /></td>";
            row += "<td>" + productName + "</td>";
            row += "<td>" + quantity + "(" + unitName + ")</td>";
            //row += "<td>" + stockName + "</td>";
            //row += "<td><a href='javascript:fnPopupExportDetailDialog(\"" + productId + "\")'>Sửa</a></td>";
            row += "</tr>";
            if (elm == undefined) {
                $("#tbProducts tbody#tBody").append(row);
            }
            else {
                $("#tbProducts tbody#tBody tr#row" + productId).replaceWith(row);
            }
        }
    });
    try {
        ClosePopup("dialog-invoices");
    } catch (Error) { }
    $.ajaxSetup({ async: true });
}


///////////
function fnExportProduct(exportId, orderId, mode, typeId) {
    LoadContent("", "Stocks/ExportForm?exportId=" + exportId + "&orderId=" + orderId + "&mode=" + mode + '&typeId=' + typeId);
}

function fnDiscardExport(id) {
    history.back();
}

function fnDelivery(id) {
    openWindow('Xuất kho', 'Stocks/DeliverForm', { id: id }, 600, 400);
}

function fnReturn(id) {
    openWindow('Nhập hàng trả', 'Stocks/ReturnForm', { id: id }, 600, 400);
}

function fnConfirmTransfer(id, status) {
    if (confirm('Xác nhận chuyển kho?')) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Stocks/TransferProducts',
            data: {
                id: id,
                status: status
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data != '')
                    alert(data);
                else
                    $(window).hashchange();
            },
            error: function () {
                fnError();
            },
            complete: function () {
                fnComplete();
            }
        });
    }
}

function fnCancelExport(id, status) {
    if (confirm("Bạn muốn hủy phiếu này?")) {
        $.post(appPath + "Common/SetStatus", { objectName: "Exports", id: id, status: status, typeId: "" }, function (data) {
            if (data != "") {
                alert(data);
            } else {
                fnLoadExportDetail(id);
            }
        });
    }
}

function fnPrintExport(id) {
    //window.open(appPath + "UserControl/PrintPage?mode=Export");
    $.get(appPath + "Stocks/PrintExport", { id: id }, function (data) {
        $.post(appPath + "UserControl/GetDataUrl", { html: Base64.encode(data), mode: 'ORD' }, function (data2) {
            window.open(appPath + "UserControl/PrintPage");
        });
    });
}

function fnCreateInvoice(orderId) {
    LoadContent('', 'Sales/SalesInvoiceForm?orderId=' + orderId);
}

function fnExports2Excel(mode) {
    $.download(appPath + 'Common/Exports2Excel',
            'keyword=' + $('#txtKeywordExport').val() +
            '&status=' + $("#slStatus").val() +
            '&mode=' + mode +
            '&from=' + $('#dtFrom').val() +
            '&to=' + $('#dtTo').val()
            );
}

/////Detail
function fnShowExportProductDetail(productId) {
    $.get(appPath + 'Stocks/ExportDetailForm', { productId: productId }, function (data) {
        $("#formExportDetail").html(data);
    });
}

function fnSaveExportProductDetail() {
    if ($('#formExportDetail').jqxValidator('validate')) {
        var Product = $("#cbbProducts").jqxComboBox('getSelectedItem');
        var ordinalNumber = $('#txtOrdinalNumber').val();
        var productId = Product.value;
        var productName = Product.label;
        var quantity = $("#txtQuantity").autoNumeric('get');
        if (quantity == "") {
            quantity = 0;
        }
        var description = $('#txtDetailDescription').val();
        var inStock = $("#txtGoodQuantityInStock").val();
        var strSNs = $('#txtSNs').val();
        $.post(appPath + "Stocks/AddExportDetail", {
            ordinalNumber: ordinalNumber,
            productId: productId,
            unitId: $('#slProductUnits').val(),
            quantity: quantity, description: description, 
            strSNs: strSNs
        }, function (data) {
//            var strError = data.strError;
//            if (strError != "") {
//                alert(strError);
//            }
//            else {
//                var elm = document.getElementById(productId);
//                var row = "";
//                row = "<tr id='" + productId + "'>";
//                row += "<td>" + productName + "</td>";
//                row += "<td>" + $("#lbUnitName").text() + "</td>";
//                row += "<td>" + fnAddCommas(quantity.toString()) + "</td>";
//                row += "<td><label class='PISQuantity' id='" + productId + "'>" + inStock + "</label></td>";
//                if ($('#txtSNs').is('input')) {
//                    row += "<td>" + strSNs + "</td>";
//                }
//                row += "<td>" + description + "</td>";
//                row += "<td class='actionCols'><button type='button' class='btActions update' onclick='javascript:fnShowExportProductDetail(\"" + productId + "\")'></button> <button type='button' class='btActions delete' onclick='javascript:fnRemoveExportProductDetail(\"" + productId + "\")'></button></td>";
//                row += "</tr>";
//                if (elm == undefined) {
//                    $("#tbodyDetails").append(row);
//                }
//                else {
//                    $("#" + productId).replaceWith(row);
//                }
//            }
            $("#tbodyDetails").html(data);
            fnResetFormExportDetail();
        });
    }
}

function fnRemoveExportProductDetail(productId) {
    $.post(appPath + "Stocks/RemoveExportDetail", { productId: productId }, function (data) {
        $("#" + productId).remove();
    });
}

function fnResetFormExportDetail() {
    $('#txtOrdinalNumber').val('0');
    $('#cbbProducts').jqxComboBox('clearSelection');
    $('#txtDetailDescription').val('');
    $('#txtQuantity').autoNumeric('set', '1.00');
    $('#txtSNs').tagit('removeAll');
    $('#cbbProducts').find(".jqx-combobox-input").focus();
}

function fnReloadExport(page, type) {
    LoadContentDynamic('ExportGrid', 'Stocks/ExportList', {
        pageNum: page,
        typeId: type,
        status: $('#slStatus').val(),
        keyword: $('#txtKeywordExport').val(),
        from: $('#dtFrom').val(),
        to: $('#dtTo').val(),
        partnerId: document.getElementsByName('cbbCustomers')[0].value
    });
}