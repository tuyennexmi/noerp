var counter = 0;

function fnPopupPIDialog(id) {
    var windowId = fnRandomString();
    OpenPopup(windowId, "Thông tin hóa đơn", appPath + "Purchase/PurchaseInvoiceForm", { id: id }, true, true, true, true, '90%', $(document).height() - 30);
    var done = function () {
        fnSaveOrUpdatePurchaseInvoice();
    }
    var cancel = function () {
        ClosePopup(windowId);
        if (counter > 0 && $('.ui-dialog:visible').length == 0) {
            if (idTemp != "") {
                fnLoadPIDetail(idTemp);
            } else {
                fnRefreshPIList();
            }
        }
    }
    var reset = function () {
        fnResetFormPurchaseInvoice();
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

function fnCreatePI(invoiceId, orderId, mode) {
    LoadContent('', 'Purchase/PurchaseInvoiceForm?invoiceId=' + invoiceId + '&orderId=' + orderId + '&mode=' + mode);
}

function fnApprovalPurchaseInvoice(invoiceId) {
    if (confirm("Xác nhận?")) {
        $.post(appPath + "Purchase/ApprovalPurchaseInvoice", { invoiceId: invoiceId },
        function (data) {
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined) {
                    fnRefreshPIList();
                } else {
                    fnLoadPIDetail(nextId);
                }
            }
            else {
                alert(data);
            }
        });
    }
}

function fnDeletePI(id) {
    if (confirm("Bạn muốn xóa hóa đơn này?")) {
        OpenProcessing();
        $.post(appPath + "Purchase/DeletePurchaseInvoice", { id: id }, function (data) {
            CloseProcessing();
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined) {
                    fnRefreshPIList();
                } else {
                    fnLoadPIDetail(nextId);
                }
            }
            else {
                alert(data);
            }
        });
    }
}

function fnLoadPIs(pageNum) {
    if (pageNum == undefined)
        pageNum = $('#pagination-Invoice input').first().val().split(' /')[0];
    
    LoadContentDynamic("PIGrid", "Purchase/PurchaseInvoiceList", {
        pageNum: pageNum,
        status: $('#slStatus').val(),
        keyword: $('#txtKeywordPI').val(),
        //type: $('#slPaymentTypes').val(),
        from: $('#dtFrom').val(),
        to: $('#dtTo').val(),
        partnerId: document.getElementsByName('cbbCustomers')[0].value
    });
}

function fnReturnPIList() {
    LoadContent("", "Purchase/PurchaseInvoices");
}


function fnLoadPIDetail(invoiceId) {
    if (invoiceId == "") {
        var rowindex = $("#PIGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#PIGrid").jqxGrid('getrowdata', rowindex);
        invoiceId = dataRecord.InvoiceId;
    }
    LoadContent("", "Purchase/PurchaseInvoiceForm?invoiceId=" + invoiceId + "&viewName=PurchaseInvoice");
}

function fnSetInfo(productId, productName, quantity, price, discount, vat, amount, unitName, ordinalNumber) {
    $("#txtProductId").val(productId);
    $("#txtProductName").val(productName);
    $("#txtQuantity").val(quantity);
    $("#txtPrice").val(price);
    $("#txtDiscount").val(discount);
    $("#txtVATRate").val(vat);
    $("#txtAmount").val(amount);
    $("#lbUnitName").text(unitName);
    $("#txtOrdinalNumber").val(ordinalNumber);
}


function fnSetPInvoiceStatus(id, reference) {
    if (confirm('Bạn muốn Xác nhận hóa đơn này?')) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Purchase/ConfirmInvoice',
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
            error: function () {
                fnError();
            },
            complete: function () {
                fnComplete();
            }
        });
    }
}

function fnExportPI2Excel(mode) {
    $.download(appPath + 'Common/PInvoices2Excel',
            'keyword=' + $("#txtKeywordPI").val() +
            '&status=' + $("#slStatus").val() +
            '&mode=' + mode +
            '&from=' + $('#dtFrom').val() +
            '&to=' + $('#dtTo').val()
            );
}