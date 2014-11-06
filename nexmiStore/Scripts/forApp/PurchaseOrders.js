function fnShowPOForm(id, mode, type) {
    LoadContent('', 'Purchase/POForm?id=' + id + '&mode=' + mode + '&type=' + type);
}

function fnDeletePO(id) {
    if (confirm('Bạn muốn xóa đơn hàng này?')) {
        OpenProcessing();
        $.post(appPath + 'Purchase/DeletePO', { id: id },
        function (data) {
            CloseProcessing();
            if (data == '') {
                var nextId = $('#btNext').val();
                if (nextId == undefined)
                    fnRefreshPOList();
                else if (nextId == '')
                    fnLoadPO();
                else
                    fnLoadPODetail(nextId);
            }
            else {
                alert(data);
            }
        });
    }
}

function fnSendPQuotation(id) {
    $.get(appPath + 'Purchase/PQuotationPDFContent', { id: id }, function (data) {
        openWindow('Gởi yêu cầu báo giá', 'UserControl/SendEmailForm', { id: id, type: 'PQuotation', html: Base64.encode(data) }, '', '');
    });
}

function fnConfirmPQuotation(id, status, typeId, customerId, maxDebitAmount) {
//    if (confirm('Xác nhận đơn hàng?'))
    //        fnSetPOrderStatus(id, status, typeId);
    if (confirm('Bạn muốn Xác nhận đơn hàng này?')) {
        var Approval = false;
        $.post(appPath + 'Purchase/CheckAPDebit', { customerId: customerId, maxDebitAmount: maxDebitAmount }, function (data) {
            if (data == 'Y') {  //không vượt hạn mức
                if (confirm('Khách hàng này đã vượt hạn mức nợ! Bạn có duyệt không?')) {
                    Approval = true;
                }
            }
            else {
                Approval = true;
            }
            if (Approval) {
                //alert('Duyệt!');
                $.post(appPath + 'Purchase/ConfirmPO', { id: id, status: status, typeId: typeId }, function (data) {
                    if (data != '') {
                        alert(data);
                    } else {
                        $(window).hashchange();
                    }
                });
            }
        });
    }
}

function fnCancelQuotation(id, status) {
    if (confirm('Hủy báo giá này?'))
    fnSetPOrderStatus(id, status, null);
}

function fnCancelPO(id, status) {
    openWindow('Vui lòng cho biết lý do hủy báo giá này?', 'Sales/CancelSOForm', { id: id }, 600, 300);
    if (confirm('Hủy đơn hàng này?'))
        fnSetPOrderStatus(id, status, null);
}

function fnReactivelQuotation(id, status) {
    if (confirm('Phục hồi đơn hàng này?'))
        fnSetPOrderStatus(id, status, null);
}

function fnSetPOrderStatus(id, status, typeId) {
    $.post(appPath + 'Purchase/ConfirmPO', { id: id, status: status, typeId: typeId }, function (data) {
        if (data != '') {
            alert(data);
        } else {
            $(window).hashchange();
        }
    });
}

function fnPrintPOrder(id, mode) {
    $.ajaxSetup({ async: false });
    $.get(appPath + 'Purchase/POForm', { id: id, viewName: 'POPrint' }, function (data) {
        $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: '' }, function (data2) {
            window.open(appPath + 'UserControl/PrintPage');
        });
    });
    
    $.ajaxSetup({ async: true });
}

function fnLoadPO(typeId) {
    if (typeId == undefined)
        typeId = '';
    LoadContent('', 'Purchase/PurchaseOrders?typeId=' + typeId);
}

function fnLoadPODetail(id) {
    if (id == '') {
        var rowindex = $('#POGrid').jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $('#POGrid').jqxGrid('getrowdata', rowindex);
        if (dataRecord != null)
            id = dataRecord.OrderId;
    }
    LoadContent('', 'Purchase/POForm?id=' + id + '&viewName=PODetail');
}

function fnReloadPO() {
    $('#POGrid').jqxGrid('updatebounddata');
}

function fnSetInfo(productId, productName, quantity, price, discount, vat, amount, unitName, ordinalNumber) {
    $('#txtProductId').val(productId);
    $('#txtProductName').val(productName);
    $('#txtQuantity').val(quantity);
    $('#txtPrice').val(price);
    //$('#txtDiscount').val(discount);
    $('#txtVATRate').val(vat);
    $('#txtAmount').val(amount);
    $('#lbUnitName').text(unitName);
    $('#txtOrdinalNumber').val(ordinalNumber);
}

function fnLoadImports(orderId) {
    LoadContent('', 'Stocks/ImportList?orderId=' + orderId);
}

function fnSaveImportFromPO(orderId) {
    $.ajax({
        type: 'POST',
        url: appPath + 'Purchase/SaveImportFromPO',
        data: {
            orderId: orderId
        },
        beforeSend: function () {
            fnDoing();
        },
        success: function (data) {
            if (data.error == '') {
                fnSuccess();
                LoadContent('', 'Stocks/Import?id=' + data.id);
            }
            else
                alert(data.error);
        },
        error: function () {
            fnError();
        },
        complete: function () {
            fnComplete();
        }
    });
}

function fnImportProduct(exportId, orderId) {
    LoadContent('', 'Stocks/ImportForm?orderId=' + orderId);
}

function fnCreatePInvoice(invoiceId, orderId) {
    LoadContent('', 'Purchase/PurchaseInvoiceForm?orderId=' + orderId);
}

function fnPOrders2Excel(type, mode) {
    $.download(appPath + 'Common/POrders2Excel',
            'keyword=' + $('#txtKeyword').val() +
            '&type=' + type +
            '&mode=' + mode +
            '&status=' + $("#slStatus").val() +
            '&from=' + $('#dtFrom').val() +
            '&to=' + $('#dtTo').val());
}