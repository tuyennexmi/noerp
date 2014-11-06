var counter = 0;
var idTemp = '';
var isValid = true;

function fnPopupSalesOrderDialog(id, mode, type) {
    LoadContent('', 'Sales/SalesOrderForm?id=' + id + '&mode=' + mode + '&type=' + type);
}

function fnDeleteSalesOrder(id) {
    var answer = confirm('Bạn muốn xóa đơn hàng này?');
    if (answer) {
        OpenProcessing();
        $.post(appPath + 'Sales/DeleteSalesOrder', { id: id }, function (data) {
            CloseProcessing();
            if (data == '') {
                var nextId = $('#btNext').val();
                if (nextId == undefined)
                    fnRefreshSalesOrderList();
                else if (nextId == '')
                    fnLoadSalesOrders();
                else
                    fnLoadSalesOrderDetail(nextId);
            }
            else {
                alert(data);
            }
        });
    }
}

function fnSendQuotation(id) {
    $.get(appPath + 'Sales/PrintQuotation', { id: id }, function (data) {
        openWindow('Gởi báo giá', 'UserControl/SendEmailForm', { id: id, type: 'Quotation', html: Base64.encode(data) }, '', '');
    });
}

function fnConfirmQuotation(id, status, typeId) {
    if (confirm('Xác nhận đơn hàng?')) {
        fnSetOrderStatus(id, status, typeId, 'Đơn đặt hàng đã được xác nhận');
    }
}

function fnCancelQuotation(id) {
    openWindow('Vui lòng cho biết lý do hủy báo giá này?', 'Sales/CancelSOForm', { id: id }, 600, 300);
}

function fnCancelSalesOrder(id) {
    openWindow('Vui lòng cho biết lý do hủy đơn đặt hàng này?', 'Sales/CancelSOForm', { id: id }, 600, 300);
}

function fnApprovalQuotation(id) {
    if (confirm('Bạn muốn duyệt báo giá này?')) {
        //fnSetOrderStatus(id, status, typeId, 'Báo giá đã được duyệt!');
        $.post(appPath + 'Sales/ApprovalQuotation', { id: id }, function (data) {
            if (data != '') {
                alert(data);
            } else {
                $(window).hashchange();
            }
        });
    }
}

function fnReactiveSalesOrder(id, status, typeId, description) {
    if (confirm('Phục hồi đơn hàng này?'))
        fnSetOrderStatus(id, status, typeId, description);
}

function fnSetOrderStatus(id, status, typeId, description) {
    if (description == undefined) description = '';
    $.post(appPath + 'Common/SetStatus', { objectName: 'SalesOrders', id: id, status: status, typeId: typeId, description: description }, function (data) {
        if (data != '') {
            alert(data);
        } else {
            $(window).hashchange();
        }
    });
}

function fnPrintOrder(id, mode, lang) {
    //$.ajaxSetup({ async: false });
    $.get(appPath + 'Sales/PrintQuotation', { id: id, mode: mode, lang: lang }, function (data) {
        $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: mode }, function (data2) {
            window.open(appPath + 'UserControl/PrintPage');
        });
    });
    //$.ajaxSetup({ async: true });
}

function fnLoadSalesOrders(SOType) {
    if (SOType == undefined)
        SOType = '';
    LoadContent('', 'Sales/SalesOrders?SOType=' + SOType);
}

function fnLoadSalesOrderDetail(id) {
    if (id == '') {
        var rowindex = $('#SalesOrderGrid').jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $('#SalesOrderGrid').jqxGrid('getrowdata', rowindex);
        id = dataRecord.OrderId;
    }
    LoadContent('', 'Sales/SalesOrderForm?id=' + id + '&viewName=SalesOrderDetail');
}

//function fnSetInfo(productId, productName, quantity, price, discount, vat, amount, unitName, ordinalNumber) {
//    $('#txtProductId').val(productId);
//    $('#txtProductName').val(productName);
//    $('#txtQuantity').val(quantity);
//    $('#txtPrice').val(price);
//    $('#txtDiscount').val(discount);
//    $('#txtVATRate').val(vat);
//    $('#txtAmount').val(amount);
//    $('#lbUnitName').text(unitName);
//    $('#txtOrdinalNumber').val(ordinalNumber);
//}

function fnLoadExports(orderId) {
    LoadContent('', 'UserControl/ExportList?orderId=' + orderId);
}

function fnCreateExportFromSO(orderId) {
    $.ajax({
        type: 'POST',
        url: appPath + 'Sales/CreateExportFromSO',
        data: {
            orderId: orderId
        },
        beforeSend: function () {
            fnDoing();
        },
        success: function (data) {
            if (data.error == '') {
                fnSuccess();
                LoadContent('', 'Stocks/Export?id=' + data.id);
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

function fnCreateExportFromSOForm(orderId) {
    LoadContent('', 'Stocks/ExportForm?orderId=' + orderId);
}

function fnCreateInvoiceMode(invoiceId, orderId) {
    openWindow('Tạo hóa đơn bán hàng', 'Sales/ChooseInvoiceOrder', { orderId: orderId }, 600, 300);
}

function fnShowInvoicesOfOrder(orderId) {
    LoadContent('', 'Sales/SalesInvoices?orderId=' + orderId);
}

function fnOrders2Excel(type, mode) {
    $.download(appPath + 'Common/Orders2Excel',
            'keyword=' + $('#txtKeywordSalesOrder').val() +
            '&type=' + type +
            '&mode=' + mode +
            '&status=' + $("#slStatus").val() +
            '&from=' + $('#dtFrom').val() +
            '&to=' + $('#dtTo').val()
            );
}


function fnSaveOrUpdateSalesOrder(saveMode) {
    if ($('#tbodyDetails tr').children().length > 0) {
        if ($('#formSalesOrder').jqxValidator('validate')) {
            $.ajax({
                type: 'POST',
                url: appPath + 'Sales/SaveOrUpdateSalesOrder',
                data: {
                    id: $('#txtId').text(),
                    orderDate: $('#txtOrderDate').val(),
                    deliveryDate: $('#txtDeliveryDate').val(),
                    customerId: $('#cbbCustomers').jqxComboBox('getSelectedItem').value,
                    reference: $('#txtReference').val(),
                    description: Base64.encode($('#txtDescription').val()),
                    salePerson: $('#slUsers').val(),
                    incoterm: $('#slIncoterms').val(),
                    shippingPolicy: $('#slShippingPolicy').val(),
                    createInvoice: $('#slCreateInvoice').val(),
                    paymentTerm: Base64.encode($('#txtPaymentTerm').val()),
                    advances: $('#txtAdvances').val(),
                    repair: $('#txtRepairDate').val(),
                    type: $('#txtTypeId').val(),
                    status: $('#txtStatus').val(),
                    saleAt: $('#slStocks').val(),
                    paymentMethod: $('#slPaymentMethods').val(),
                    transportation: $('#txtTransportation').val(),
                    group: $('#txtGroup').val(),
                    deposit: $('#txtDeposit').val(),
                    maintainDate: $('#txtMaintainDate').val(),
                    setupFee: $('#txtSetupFee').val()
                    //invoiceType: $('#slInvoiceTypes').val()
                },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    if (data.error == '') {
                        fnSuccess();
                        if (saveMode == '1')
                            fnResetFormSalesOrder();
                        else {
                            if (data.group == 'GSO01')
                                fnLoadSalesOrderDetail(data.result);
                            else
                                fnLoadLeaseOrderDetail(data.result);
                        }
                    }
                    else alert(data.error);
                },
                error: function () {
                    fnError();
                },
                complete: function () {
                    fnComplete();
                }
            });
        }
        else {
            alert('Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!');
        }
    } else {
        alert('Bạn chưa lưu sản phẩm nào. Vui lòng chọn một sản phẩm rồi bấm nút lưu ở cuối dòng để ghi nhớ sản phẩm đó trước khi lưu phiếu.');
    }
}

function fnResetFormSalesOrder() {
    $.post(appPath + 'Common/ClearSession', { sessionName: 'Details' });
    $('#txtId').text('');
    $('#cbbCustomers').jqxComboBox('clearSelection');
    $('#txtInvoiceDate').val('');
    $('#txtReceiptAmount').val('');
    //$('#txtShipCost').val('');
    //$('#txtOtherCost').val('');
    $('#txtDescription').val('');
    $('#lbTotalDiscount').html('');
    $('#lbTotalVATRate').html('');
    $('#lbTotalAmount').html('');
    $('#lbInvoiceAmount').html('');
    $('#lbInvoiceAmount2').html('');
    $('#tbodyDetails').html('');
}

function fnShowSalesOrderDetail(productId) {
    $.get(appPath + 'Sales/SalesOrderDetailForm', { productId: productId }, function (data) {
        $('#formSODetail').html(data); //.show();
    });
}

function fnSaveSalesOrderDetail() {
    if ($('#formSODetail').jqxValidator('validate')) {
        var Product = $('#cbbProducts').jqxComboBox('getSelectedItem');
        var productId = Product.value;
        var productName = Product.label;
        var quantity = $('#txtQuantity').autoNumeric('get');
        var price = $('#txtPrice').autoNumeric('get');
        if (price == '') {
            price = 0;
        }
        var discount = $('#txtDiscount').autoNumeric('get');
        if (discount == '') {
            discount = 0;
        }
        var VATRate = $('#txtVATRate').autoNumeric('get');
        if (VATRate == '') {
            VATRate = 0;
        }
        var description = $('#txtDescriptionDetail').val();
        if (description != '')
            description = Base64.encode($('#txtDescriptionDetail').val());

        $.post(appPath + 'Sales/AddSalesOrderDetail', {
            id: $('#txtDetailID').val(), productId: productId,
            unitId: $('#slProductUnits').val(),
            description: description, quantity: quantity, 
            price: price, discount: discount, VATRate: VATRate
        }, function (data) {
            $('#tbodyDetails').html(data);
            $('#lbAmount').html($('#txtLineAmount').val());
            $('#lbTotalVATRate').html($('#txtTaxAmount').val());
            $('#lbTotalDiscount').html($('#txtDiscountAmount').val());
            $('#lbTotalAmount').html($('#txtTotalAmount').val());
            fnResetSalesOrderDetail();
        });
    }
}

function fnRemoveSalesOrderDetail(productId) {
    $.post(appPath + 'Sales/RemoveSalesOrderDetail', { productId: productId }, function (data) {
        $('#' + productId).remove();
        $('#lbAmount').html(data.amount);
        $('#lbTotalVATRate').html(data.taxAmount);
        $('#lbTotalDiscount').html(data.discountAmount);
        $('#lbTotalAmount').html(data.totalAmount);
    });
}

function fnResetSalesOrderDetail() {
    $('#txtDetailID').val('0');
    $('#cbbProducts').jqxComboBox('clearSelection');
    $('#txtDescriptionDetail').val('');
    $('#txtQuantity').autoNumeric('set', '1.00');
    $('#txtPrice').autoNumeric('set', '0.00');
    $('#txtDiscount').autoNumeric('set', '0.00');
    $('#txtVATRate').autoNumeric('set', '0.00');
    $('#txtAmount').val('0.00');
    $('#cbbProducts').find(".jqx-combobox-input").focus();
}


// cho thue
function fnLoadLeaseOrderDetail(id) {
    LoadContent('', 'Sales/LeaseOrderForm?id=' + id + '&viewName=LeaseOrderDetail');
}

function fnCreateLeaseOrderDialog(id, mode, type) {
    LoadContent('', 'Sales/LeaseOrderForm?id=' + id + '&mode=' + mode + '&type=' + type);
}

function fnReloadLOList(pageNum) {
    if (pageNum == undefined)
        pageNum = $('#pagination-LOrder input').first().val().split(' /')[0];
    LoadContentDynamic('OrderList', 'Sales/LeaseOrderList', {
        pageNum: pageNum,
        status: $('#slStatus').val(),
        keyword: $('#txtKeywordSalesOrder').val(),
        typeId: $('#txtTypeId').val(),
        from: $('#dtFrom').val(),
        to: $('#dtTo').val(),
        partnerId: document.getElementsByName('cbbCustomers')[0].value
    });
}


function fnRefreshLeaseOrderList() {
    //$('#txtKeywordSalesOrder').val('');
    fnReloadLOList();
}

function fnPrintLOList() {
    var from = $('#dtFrom').val();
    var to = $('#dtTo').val();
    var keyword = $('#txtKeywordSalesOrder').val();
    var typeId = $('#txtTypeId').val();
    var status = $('#slStatus').val();
    $.ajaxSetup({ async: false });

    $.get(appPath + 'Sales/LeaseOrderList', {
        mode: 'Print',
        status: status,
        from: from,
        to: to,
        keyword: keyword,
        typeId: typeId,
        partnerId: document.getElementsByName('cbbCustomers')[0].value
    }, function (data) {
        $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: '', orient: 'LAND' }, function (data2) {
            window.open(appPath + 'UserControl/PrintPage');
        });
    });

    $.ajaxSetup({ async: true });
}

function fnChangeMaintainDate(orderId) {
    openWindow('Thay đổi ngày vệ sinh', 'Sales/ChangeMaintainDateDialog', { orderId: orderId }, 600, 300);
}