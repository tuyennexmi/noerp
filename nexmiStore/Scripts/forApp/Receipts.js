function fnShowReceiptForm(id, invoiceId, type, mode) {
    if (mode == 'popup')
        openWindow('Phiếu thu', 'UserControl/ReceiptForm', { id: id, invoiceId: invoiceId }, 650, 550);
    else
        LoadContent('', 'UserControl/ReceiptForm?id=' + id + '&invoiceId=' + invoiceId + '&type=' + type);
}

function fnLoadReceipt(pageNum) {
    if (pageNum == undefined)
        pageNum = $('#pagination-Receipt input').first().val().split(' /')[0];
    LoadContentDynamic('ReceiptGrid', 'Accounting/ReceiptGrid',
    {   pageNum: pageNum,
        status: $('#slStatus').val(),
        keyword: $('#txtKeyword').val(),
        type: $('#slPaymentTypes').val(),
        from: $('#dtFrom').val(),
        to: $('#dtTo').val()
    });
}

function fnLoadReceiptDetail(id) {
    LoadContent('', 'UserControl/ReceiptForm?id=' + id + '&viewName=ReceiptDetail');
}

function fnDeleteReceipt(id) {
    if (confirm("Bạn muốn xóa phiếu này?")) {
        //OpenProcessing();
        $.post(appPath + "Accounting/DeleteReceipt", { id: id }, function (data) {
            //CloseProcessing();
            if (data == "") {
                fnLoadReceipt('1');
            }
            else {
                alert(data);
            }
        });
    }
}

function fnApproveReceipt(id, statusId) {
    $.ajax({
        type: 'POST',
        url: appPath + 'UserControl/ApproveReceipt',
        data: {
            id: id, statusId: statusId 
        },
        beforeSend: function () {
            //fnDoing();
            OpenProcessing();
        },
        success: function (data) {
            if (data == '') {
                fnSuccess();
                $(window).hashchange();
            }
            else
                alert(data);
        },
        error: function () {
            fnError();
        },
        complete: function () {
            //fnComplete();
            CloseProcessing();
        }
    });
}

function fnConfirmReceipt(id, statusId) {
    if (confirm("Xác nhận?")) {
        fnApproveReceipt(id, statusId);
    }
}

function fnPrintReceipt(id, lang) {
    $.ajaxSetup({ async: false });

    $.get(appPath + 'UserControl/ReceiptForm', { id: id, viewName: 'ReceiptPrint', lang: lang }, function (data) {
        $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'ORD', size: 'A5' }, function (data2) {
            window.open(appPath + 'UserControl/PrintPage');
        });
    });

    $.ajaxSetup({ async: true });
}
