<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/PurchaseOrders.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $("#txtOrderDate, #txtDeliveryDate, #txtPaymentDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
        $('#formPO').jqxValidator({
            rules: [
                { input: '#cbbCustomers', message: 'Chọn nhà cung cấp', action: 'change', rule: function (input) {
                    if ($(input).jqxComboBox('getSelectedItem') == null)
                        return false;
                    return true;
                }
                },
                { input: '#txtOrderDate', message: 'Không được để trống.', action: 'keyup, blur', rule:
                        function (input, commit) {
                            if ($(input).val() == '')
                                return false;
                            return true;
                        }
                }
            ]
                });
        <%if((bool)ViewData["StockInput"] == false){ %>
            $('#slStocks').prop('disabled', true);
        <%} %>
        <%if((bool)ViewData["SalerInput"] == false){ %>
            $('#slUsers').prop('disabled', true);
        <%} %>
        //$('#slStocks, #slUsers').prop('disabled', true);
    });

    function fnSaveOrUpdatePO(saveMode) {
        if ($('#tbodyDetails tr').children().length > 0) {
            if ($('#formPO').jqxValidator('validate')) {
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Purchase/SaveOrUpdatePO',
                    data: {
                        id: $('#txtId').text(),
                        orderDate: $('#txtOrderDate').val(),
                        deliveryDate: $('#txtDeliveryDate').val(),
                        supplierId: $('#cbbCustomers').jqxComboBox('getSelectedItem').value,
                        reference: $('#txtReference').val(),
                        description: $('#txtDescription').val(),
                        type: $('#txtTypeId').val(),
                        status: $('#txtStatus').val(),
                        trans: $('#txtTransportation').val(),
                        stock: $('#slStocks').val(), 
                        buyer: $('#slUsers').val(), 
                        paydate: $('#txtPaymentDate').val()
                        //invoiceTypes: $('#slInvoiceTypes').val()
                        //invoiceDate: $('#txtPaymentDate').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == '') {
                            if (saveMode == '1')
                                fnResetFormPO();
                            else
                                fnLoadPODetail(data.result);
                        }
                        else {
                            alert(data.error);
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
            else {
                alert('Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!');
            }
        } else {
            alert('Không có sản phẩm nào');
        }
    }

    function fnResetFormPO() {
        $.post(appPath + 'Common/ClearSession', { sessionName: 'Details' });
        $('#cbbCustomers').jqxComboBox('clearSelection');
        $('#txtReference').val('');
        $('#txtDescription').val('');
        $('#lbTotalAmount').html('0.00');
        $('#lbTotalVATRate').html('0.00');
        $('#lbInvoiceAmount').html('0.00');
        $('#tbodyDetails').html('');
    }

    function fnShowPODetail(productId) {
        $.get(appPath + 'Purchase/POLineForm', { productId: productId }, function (data) {
            $('#formPODetail').html(data); //.show();
        });
    }

    function fnAddPODetail() {
        if ($('#formPODetail').jqxValidator('validate')) {
            var Product = $('#cbbProducts').jqxComboBox('getSelectedItem');
            var productId = Product.value;
            var productName = Product.label;
            var quantity = $('#txtQuantity').autoNumeric('get');
            var price = $('#txtPrice').autoNumeric('get');
            if (price == '') price = 0;
            var discount = $('#txtDiscount').autoNumeric('get');
            var VATRate = $('#txtVATRate').autoNumeric('get');
            if (VATRate == '') VATRate = 0;
            var description = $('#txtDetailDescription').val();
            $.ajax({
                type: 'POST',
                url: appPath + 'Purchase/AddPODetail',
                data: {
                    id: $('#txtDetailID').val(),
                    productId: productId,
                    unitId: $('#slProductUnits').val(),
                    quantity: quantity,
                    price: price,
                    discount: discount,
                    VATRate: VATRate,
                    description: description
                },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    $('#tbodyDetails').html(data);
                    $('#lbAmount').html($('#txtLineAmount').val());
                    $('#lbTotalVATRate').html($('#txtTaxAmount').val());
                    $('#lbTotalDiscount').html($('#txtDiscountAmount').val());
                    $('#lbTotalAmount').html($('#txtTotalAmount').val());
                    fnResetFormPODetail();
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

    function fnRemovePODetail(productId) {
        $.post(appPath + 'Purchase/RemovePODetail', { productId: productId }, function (data) {
            $('#' + productId).remove();
            $('#lbAmount').html(data.amount);
            $('#lbDiscountAmount').html(data.discountAmount);
            $('#lbTotalVATRate').html(data.taxAmount);
            $('#lbTotalAmount').html(data.totalAmount);
        });
    }

    function fnResetFormPODetail() {
        $('#txtDetailID').val('0');
        $('#cbbProducts').jqxComboBox('clearSelection');
        $('#txtQuantity').autoNumeric('set', 1);
        $('#txtPrice').autoNumeric('set', 0);
        $('#txtVATRate').autoNumeric('set', 0);
        $('#txtAmount').val('0');
        $('#txtDetailDescription').val('');
    }
</script>
<% 
    string id = ViewData["Id"].ToString();
    NEXMI.NMPurchaseOrdersWSI WSI = (NEXMI.NMPurchaseOrdersWSI)ViewData["WSI"];
%>
<div>
    <input type="hidden" id="txtTypeId" value="<%=ViewData["TypeId"]%>" />
    <input type="hidden" id="txtStatus" value="<%=ViewData["StatusId"]%>" />
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <button onclick="javascript:fnSaveOrUpdatePO()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button onclick="javascript:fnSaveOrUpdatePO('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                        <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                    </div>
                </td>
                <td align="right">
                    <%Html.RenderPartial("POSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
            </div>     
            <div class="divStatusBar">
                <%
                    Html.RenderAction("StatusBar", "UserControl", new { objectName = "PurchaseOrders", current = ViewData["StatusId"] });                
                %>
            </div>
        </div>
        <div class='divContentDetails'>
        <form id="formPO" action="" class="form-to-validate">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%=ViewData["Tilte"] %> <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></td>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { cgroupId = NEXMI.NMConstant.CustomerGroups.Supplier, customerId = ViewData["CustomerId"], customerName = ViewData["CustomerName"] });%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %></td>
                                <td><input type="text"  id="txtOrderDate" value="<%=((DateTime)ViewData["OrderDate"]).ToString("yyyy-MM-dd")%>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></td>
                                <td><input type="text" id="txtReference" name="txtReference" value="<%=ViewData["Reference"]%>" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE_EXPECTED", langId) %></td>
                                <td><input type="text"  id="txtDeliveryDate" value="<%=((DateTime)ViewData["DeliveryDate"]).ToString("yyyy-MM-dd")%>"  /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                                <td><input type="text" id="txtTransportation" name="txtTransportation" value="<%=WSI.Order.Transportation%>" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ViewData["StockId"] }); %></td>
                            </tr>
                            <%--<tr>
                                <td class="lbright">Loại hàng mua</td>
                                <td><%Html.RenderAction("slInvoiceTypes", "UserControl", new { elementId = "slInvoiceTypes", objectName = "PurchaseInvoices", value = WSI.Order.InvoiceTypeId });%></td>
                                <td></td><td></td>
                            </tr>--%>
                        </table>   
                    </td>
                </tr>
            </table>
            <div class="tabs">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" id="tbPODetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>(%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %>(%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails">
                                        <%Html.RenderPartial("PODetails", "Purchase");%>
                                    </tbody>
                                    <tfoot id="formPODetail" class="form-to-validate">
                                        <%Html.RenderAction("POLineForm", "Purchase");%>
                                    </tfoot>
                                </table>
                            </td>
                        </tr>
                        <%--<tr>
                            <td><a href="javascript:fnShowPODetail('')">Thêm chi tiết</a></td>
                        </tr>--%>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td><textarea id="txtDescription" rows="5" cols="0" style="width: 100%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."><%=ViewData["Description"]%></textarea></td>
                                        <td valign="top">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
                                                    <td align="right" style="width: 120px;"><label id="lbAmount"><%=ViewData["Amount"]%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                    <td align="right" style="width: 120px;"><label id="lbDiscountAmount"><%=ViewData["discountAmount"]%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalVATRate"><%=ViewData["Tax"]%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalAmount"><%=ViewData["TotalAmount"]%></label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="padding: 10px;">
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BUYER", langId) %></td>
                            <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers", userId = ViewData["Buyer"].ToString() }); %></td>
                            <%--<td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECEIVED_INVOICE", langId) %></td>
                            <td><input type="checkbox" /></td>--%>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_TIME", langId)%></td>
                            <td><input type="text"  id="txtPaymentDate" value="<%=((DateTime)ViewData["PaymentDate"]).ToString("yyyy-MM-dd")%>"  /></td>
                            <%--<td class="lbright">Chức vụ tài chính</td>
                            <td></td>--%>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
        </div>
    </div>
</div>