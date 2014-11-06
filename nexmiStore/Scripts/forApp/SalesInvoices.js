var counter = 0;
var idTemp = "";

function fnDeleteSI(id) {
    if (confirm("Bạn muốn xóa hóa đơn này?")) {
        OpenProcessing();
        $.post(appPath + "Sales/DeleteSalesInvoice", { id: id }, function (data) {
            CloseProcessing();
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined)
                    fnRefreshSIList();
                else if (nextId == "")
                    fnLoadSIs();
                else
                    fnLoadSIDetail(nextId);
            }
            else {
                alert(data);
            }
        });
    }
}

function fnLoadSIs() {
    var status = $("#slStatus").val();
    LoadContent("", "Sales/SalesInvoices?status=" + status);
}

function fnLoadSIDetail(invoiceId) {
    if (invoiceId == "") {
        var rowindex = $("#SIGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#SIGrid").jqxGrid('getrowdata', rowindex);
        invoiceId = dataRecord.InvoiceId;
    }
    LoadContent("", "Sales/SalesInvoice?invoiceId=" + invoiceId);
}

function fnCreateInvoice(invoiceId, orderId, mode) {
    LoadContent("", "Sales/SalesInvoiceForm?invoiceId=" + invoiceId + "&orderId=" + orderId + "&mode=" + mode);
}

function fnSetInvoiceStatus(id, reference) {
    if (confirm('Xác nhận?')) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Sales/ConfirmInvoice',
            data: {
                id: id, reference: reference
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data != '')
                    alert(data);
                else {
                    fnSuccess();
                    $(window).hashchange();
                }
            },
            error: function (data) {
                fnError();
            },
            complete: function () {
                fnComplete();
            }
        });
    }
}

function fnReceipt(invoiceId) {
    //openWindow("Phiếu thu", "UserControl/ReceiptForm", { invoiceId: invoiceId }, 650, 550);
    LoadContent('', 'UserControl/ReceiptForm?id=' + id + '&invoiceId=' + invoiceId);
}

function fnPrintSalesInvoice(id) {
    $.get(appPath + "Sales/PrintInvoice", { id: id }, function (data) {
        $.post(appPath + "UserControl/GetDataUrl", { html: Base64.encode(data), mode: 'ORD' }, function (data2) {
            window.open(appPath + "UserControl/PrintPage");
        });
    });
}

function fnRefund(invoiceId, typeId) {
    openWindow("Phiếu chi", "UserControl/RefundForm", { invoiceId: invoiceId, typeId: typeId }, 650, 550);
}

function fnExport2Excel(mode) {
    $.download(appPath + 'Common/SaleInvoices2Excel', 
            'keyword=' + $("#txtKeywordSI").val() + 
            '&status=' + $("#slStatus").val() + 
            '&mode=' + mode + 
            '&from=' + $('#dtFrom').val() +
            '&to=' + $('#dtTo').val());
}