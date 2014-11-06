function fnShowPaymentForm(id, invoiceId, typeId, mode) {
    
    if (mode == 'popup')
        openWindow('Phiếu chi', 'Accounting/PaymentForm', { invoiceId: invoiceId, typeId: typeId }, 650, 550);
    else
        LoadContent('', 'Accounting/PaymentForm?id=' + id + '&invoiceId=' + invoiceId + '&typeId=' + typeId);
}

function fnLoadPayment(pageNum) {
    if (pageNum == undefined)
        pageNum = $('#pagination-Payment input').first().val().split(' /')[0];

    LoadContentDynamic('PaymentGrid', 'Accounting/PaymentGrid',
        {   pageNum: pageNum,
            status: $('#slStatus').val(),
            keyword: $('#txtKeyword').val(),
            type: $('#slPaymentTypes').val(),
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
        });
}

function fnLoadPaymentDetail(id) {
    LoadContent('', 'Accounting/PaymentForm?id=' + id + '&viewName=Payment');
}

function fnDeletePayment(id) {
    if (confirm("Bạn muốn xóa phiếu này?")) {
        OpenProcessing();
        $.post(appPath + "Accounting/DeletePayment", { id: id }, function (data) {
            CloseProcessing();
            if (data == "") {
                fnLoadPayment();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnApprovePayment(id, statusId) {
    $.ajax({
        type: 'POST',
        url: appPath + 'Accounting/ApprovePayment',
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

function fnConfirmPayment(id, statusId) {
    if (confirm('Xác nhận?')) {
        fnApprovePayment(id, statusId);
    }
}

function fnPrintPayment(id) {
    $.ajaxSetup({ async: false });

    $.get(appPath + 'Accounting/PaymentForm', { id: id, viewName: 'PaymentPrint' }, function (data) {
        $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'ORD', size: 'A5' }, function (data2) {
            window.open(appPath + 'UserControl/PrintPage');
        });
    });

    $.ajaxSetup({ async: true });
}
