﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/SalesOrders.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $("#txtOrderDate, #txtDeliveryDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $('#txtDescription, #txtPaymentTerm').jqte();
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
        $("#formSalesOrder").jqxValidator({
            rules: [
                { input: '#cbbCustomers', message: 'Chọn khách hàng', action: 'change', rule: function (input) {
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
    });

    function fnLoadAllProductsForm(){
        $('#formSODetail').empty();
        LoadContentDynamic('formSODetail', 'Sales/AllProductsForm','');
    }
    
</script>
<% 
    NEXMI.NMSalesOrdersWSI WSI = (NEXMI.NMSalesOrdersWSI)ViewData["WSI"];
    if (WSI.Customer == null) WSI.Customer = new NEXMI.Customers();
    string id = WSI.Order.OrderId;
%>
<div id="divSalesOrder">
    <input type="hidden" id="txtTypeId" value="<%=WSI.Order.OrderTypeId%>" />
    <input type="hidden" id="txtStatus" value="<%=WSI.Order.OrderStatus%>" />
    <input type="hidden" id="txtGroup" value="<%=WSI.Order.OrderGroup%>" />
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
                        <button onclick="javascript:fnSaveOrUpdateSalesOrder()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button onclick="javascript:fnSaveOrUpdateSalesOrder('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                        <button onclick="javascript:$('#formSalesOrder').jqxValidator('hide'); history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                    </div>
                </td>
                <td align="right">
                    <%Html.RenderPartial("SalesOrderSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <button onclick="javascript:fnLoadAllProductsForm()" class="button">Toàn bộ sản phẩm</button>
            </div>     
            <div class="divStatusBar">
                <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "SalesOrders", current = WSI.Order.OrderStatus });%>
            </div>
        </div>
        <div class='divContentDetails'>
        <form id="formSalesOrder" class="form-to-validate" action="" style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%=ViewData["Tilte"] %> <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = WSI.Customer.CustomerId, customerName = WSI.Customer.CompanyNameInVietnamese });%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALE_AT", langId) %></td>
                                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = WSI.Order.StockId }); %></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %></td>
                                <td><input type="text" id="txtOrderDate" value="<%=WSI.Order.OrderDate.ToString("yyyy-MM-dd")%>" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></td>
                                <td><input type="text" id="txtReference" name="txtReference" value="<%=WSI.Order.Reference%>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY_DATE", langId) %></td>
                                <td><input type="text" id="txtDeliveryDate" name="txtDeliveryDate" value="<%=WSI.Order.DeliveryDate.Value.ToString("yyyy-MM-dd")%>" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                                <td><input type="text" id="txtTransportation" name="txtTransportation" value="<%=WSI.Order.Transportation %>" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div class="tabs">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("PAYMENT_TERMS", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" id="tbSalesOrderDetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %>(%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId)%></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails">
                                        <%Html.RenderPartial("SalesOrderDetails", "Sales");%>
                                    </tbody>
                                    <tfoot id="formSODetail" class="form-to-validate">
                                        <%Html.RenderAction("SalesOrderDetailForm", "Sales");%>
                                    </tfoot>
                                </table>
                            </td>
                        </tr>
                        <%--<tr>
                            <td><a href="javascript:fnShowSalesOrderDetail('')">Thêm chi tiết</a></td>
                        </tr>--%>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td>Ghi chú: <br />
                                            <textarea id="txtDescription" rows="10" cols="6" style="width: 100%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."><%=WSI.Order.Description%></textarea>
                                        </td>
                                        <td valign="top">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
                                                    <td align="right" style="width: 120px;"><label id="lbAmount"><%=WSI.Details.Sum(i=>i.Amount).ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalDiscount"><%=WSI.Details.Sum(i=>i.DiscountAmount).ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalVATRate"><%=WSI.Details.Sum(i=>i.TaxAmount).ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalAmount"><%=WSI.Details.Sum(i=>i.TotalAmount).ToString("N3")%></label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="padding:10px">
                    <table class="frmInput">
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></td>
                        <td><%Html.RenderAction("slPaymentMethods", "UserControl", new { paymentMethod = ViewData["PaymentMethodId"] });%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_TERMS", langId) %></td>
                        <td>
                            <textarea id="txtPaymentTerm" rows="10" cols="0" style="width: 80%" placeholder="thêm dấu chấm phẩy để xuống dòng trong hợp đồng..."><%=WSI.Order.PaymentTerm%></textarea>
                        </td>
                    </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></td>
                            <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers", userId = WSI.Order.SalesPersonId }); %></td>
                            <td class="lbright">Incoterm</td>
                            <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slIncoterms", objectName = "Incoterms", current = WSI.Order.Incoterm });%></td>                            
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY_METHOD", langId) %></td>
                            <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slShippingPolicy", objectName = "ShippingPolicy", current = WSI.Order.ShippingPolicy });%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %></td>
                            <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slCreateInvoice", objectName = "CreateInvoice", current = WSI.Order.CreateInvoice });%></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ADVANCE", langId) %></td>
                            <td><input type="text" id="txtAdvances" value="<%=WSI.Order.Advances%>" /></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REPAIR_DATE", langId) %></td>
                            <td><input type="text" id="txtRepairDate" value="<%=WSI.Order.RepairDate%>" /></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAID", langId) %></td>
                            <td><input type="checkbox" disabled="disabled" /></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERED", langId) %></td>
                            <td><input type="checkbox" disabled="disabled" /></td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
        </div>
    </div>
</div>