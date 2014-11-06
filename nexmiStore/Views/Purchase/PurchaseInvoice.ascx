<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/PurchaseInvoices.js" type="text/javascript"></script>
<% 
    NEXMI.NMPurchaseInvoicesWSI WSI = (NEXMI.NMPurchaseInvoicesWSI)ViewData["WSI"];
    string id = WSI.Invoice.InvoiceId;
    string previousId = NEXMI.NMCommon.PreviousId(id, "InvoiceId", "PurchaseInvoices", "");
    string nextId = NEXMI.NMCommon.NextId(id, "InvoiceId", "PurchaseInvoices", "");
    string current = WSI.Invoice.InvoiceStatus;
    string functionId = NEXMI.NMConstant.Functions.PI;
    double rpt = (WSI.Payments == null) ? 0 : WSI.Payments.Sum(i => i.PaymentAmount);
%>

<script type="text/javascript">
    $(function () {
        $(".tab").jqxTabs({ theme: theme, keyboardNavigation: false });
    });

    $('.tab').bind('selected', function (event) {
        var item = event.args.item;
        switch (item) {
            case 3:
                LoadContentDynamic('Accounting', 'Accounting/GeneralJournals', { IssueId: '<%=id %>' });
                break;
        }
    });

</script>

<div id="divSalesInvoice">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <%
                            if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                            { 
                        %>
                        <button onclick="javascript:fnCreatePI('', '', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                        <% 
                            }
                            if (current == NEXMI.NMConstant.PIStatuses.Draft)
                            {
                                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                {
                        %>
                                <button onclick="javascript:fnCreatePI('<%=id%>', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                        <%
                                }
                                if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                { 
                        %>
                                <button onclick="javascript:fnDeletePI('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                        <% 
                                }
                            }
                        %>
                        <%
                            if (GetPermissions.GetDuplicate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                            { 
                        %>
                            <button onclick="javascript:fnCreatePI('<%=id%>', '', 'Copy')" class="button"><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></button>
                        <%
                            }
                        %>
                        <%
                            if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                            { 
                        %>
                            <%--<button onclick="javascript:fnPrintPurchaseInvoice('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>--%>
                        <%
                            }
                        %>    
                    </div>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <input id="btNext" type="hidden" value="<%=nextId%>" />
                                <ul class="button-group">
	                                <li title="Trước"><button class="button" onclick="javascript:fnLoadPIDetail('<%=previousId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                    <li title="Sau"><button class="button" onclick="javascript:fnLoadPIDetail('<%=nextId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                                </ul>
                            </td>
                            <td>
                                <%Html.RenderPartial("PurchaseInvoiceSV"); %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <div class="NMButtons">
            <% 
                if (current == NEXMI.NMConstant.PIStatuses.Draft)
                {
            %>
            <%--<button class="color red button" onclick='javascript:fnSetPInvoiceStatus("<%=id%>", "<%=ViewData["Reference"]%>")'><%= NEXMI.NMCommon.GetInterface("CONFIRM", langId) %></button>--%>
                    <button class="color red button" onclick="javascript:fnShowPaymentForm('', '<%=id%>', '', '')"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></button>
                    <button class="color red button" onclick="javascript:fnSetPInvoiceStatus('<%=id%>')">Ghi nợ</button>          
            <%
                }
                else if (current == NEXMI.NMConstant.PIStatuses.Debit)
                {  
            %>
                    <button class="color red button" onclick="javascript:fnShowPaymentForm('', '<%=id%>', '', '')"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></button>            
            <%
                }
                else
                {
                    if (WSI.Invoice.TotalAmount - rpt > 0)
                    {
            %> 
                    <button class="color red button" onclick="javascript:fnShowPaymentForm('', '<%=id%>', '', '')"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></button>
                    <%--<button class="button" onclick="javascript:fnRefund('<%=id%>', '<%=NEXMI.NMConstant.PaymentType.Refund%>')">Hoàn tiền</button>--%>
            <%      }
                }
            %>
                </div>
            </div>     
            <div class="divStatusBar">
                <%
                    Html.RenderAction("StatusBar", "UserControl", new { objectName = "PurchaseInvoices", current = current });
                %>
            </div>
        </div>
        <div class='divContentDetails'>
        <form id="formPurchaseInvoice" class="form-to-validate" action="" style="margin: 10px;">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td>
                        <label class="lbId">Hóa đơn mua hàng <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></td>
                                <td><%=(WSI.Supplier == null) ? "" : WSI.Supplier.CompanyNameInVietnamese%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></td>
                                <td><%=WSI.Invoice.InvoiceDate.ToString("dd-MM-yyyy")%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BUYER", langId) %></td>
                                <td><%=NEXMI.NMCommon.GetCustomerName(WSI.Invoice.BuyerId)%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                                <td><%=WSI.Invoice.Reference%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("IMPORT_AT", langId) %></td>
                                <td><%= NEXMI.NMCommon.GetName(WSI.Invoice.StockId, langId)%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REFERENCE", langId) %></td>
                                <td><%=WSI.Invoice.SupplierReference%></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div class="tab">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("ACCOUNTING", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %> (%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <% 
                                            List<NEXMI.PurchaseInvoiceDetails> Details = (List<NEXMI.PurchaseInvoiceDetails>)Session["Details"];
                                            int count = 1;
                                            foreach (NEXMI.PurchaseInvoiceDetails Item in Details)
                                            {
                                        %>
                                        <tr>
                                            <td><%=count++ %></td>
                                            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                            <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
                                            <td><%=Item.Quantity.ToString("N3")%></td>
                                            <td><%=Item.Price.ToString("N3")%></td>
                                            <td><%=Item.Discount.ToString("N3")%></td>
                                            <td><%=Item.Tax.ToString("N3")%></td>
                                            <td align="right"><%=Item.Amount.ToString("N3")%></td>
                                            <td><%=Item.Description%></td>
                                        </tr>
                                        <% 
                                            }    
                                        %>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td valign="top" width='70%'>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td><b><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>: </b></td>                                                    
                                                </tr>
                                                <tr>
                                                    <td><%= WSI.Invoice.DescriptionInVietnamese%></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td valign="top">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td colspan="6" align="right"><b>Tiền hàng: </b></td>
                                                    <td align="right" style="width: 120px;"><label><%=WSI.Invoice.Amount.ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                    <td align="right"><label><%=WSI.Invoice.Discount.ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %>: </b></td>
                                                    <td align="right"><label><%=WSI.Invoice.Tax.ToString("N3")%></label></td>
                                                </tr>                                                
                                                <%--<tr>
                                                    <td colspan="6" align="right"><b>Phí vận chuyển: </b></td>
                                                    <td align="right"><label><%=WSI.Invoice.ShipCost.ToString("N3")%></label></td>
                                                </tr>--%>
                                                <%--<tr>
                                                    <td colspan="6" align="right"><b>Phí khác: </b></td>
                                                    <td align="right"><label><%=WSI.Invoice.OtherCost.ToString("N3")%></label></td>
                                                </tr>--%>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId)%>: </b></td>
                                                    <td align="right"><label><%= WSI.Invoice.TotalAmount.ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("PAID", langId) %>: </b></td>
                                                    <td align="right"><label><%= rpt.ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %>: </b></td>
                                                    <td align="right"><label><%= (WSI.Invoice.TotalAmount -rpt).ToString("N3")%></label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                    <%  ViewData["ModifiedDate"] = WSI.Invoice.ModifiedDate;
                        ViewData["ModifiedBy"] = WSI.Invoice.ModifiedBy;
                        ViewData["CreatedDate"] = WSI.Invoice.CreatedDate;
                        ViewData["CreatedBy"] = WSI.Invoice.CreatedBy ;
                        Html.RenderPartial("LastModified"); %>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table id="tbPayments" class="tbDetails" width="100%">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("BILL_NO", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></th>
                                            <th>Đã chi</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    <%
                                        functionId = NEXMI.NMConstant.Functions.Payments;
                                        if (WSI.Payments != null)
                                        {
                                            int i = 1;
                                            double total = 0;
                                            foreach (NEXMI.Payments Item in WSI.Payments)
                                            {
                                                total += Item.PaymentAmount;
                                    %>
                                        <tr ondblclick="javascript:fnLoadPaymentDetail('<%=Item.PaymentId%>')">
                                            <td style="width: 15px;"><%=i++%></td>
                                            <td style="width: 120px;"><div><%=Item.PaymentId%></div></td>
                                            <td style="width: 100px;"><div><%=Item.PaymentDate.ToString("dd-MM-yyyy")%></div></td>
                                            <td><%=NEXMI.NMCommon.GetParameterName(Item.PaymentMethodId)%></td>
                                            <td style="width: 120px;" align="right"><div><%=(Item.PaymentAmount).ToString("N3")%></div></td>
                                            <td><div title="<%=Item.DescriptionInVietnamese%>"><%=Item.DescriptionInVietnamese%></div></td>
                                            <td><div><%=NEXMI.NMCommon.GetStatusName(Item.PaymentStatus, langId)%></div></td>
                                            <td class="actionCols">
                                                <a href="javascript:fnLoadPaymentDetail('<%=Item.PaymentId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                    <% 
                                                if (Item.PaymentStatus == NEXMI.NMConstant.PaymentStatuses.Draft)
                                                {
                                                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                                    {    
                                    %>
                                                <a href="javascript:fnPopupReceiptDialog('<%=Item.PaymentId%>', '<%=Item.PaymentId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
                                    <% 
                                                    }
                                    %>
                                    <% 
                                                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                                    {
                                    %>
                                                <a href="javascript:fnDeletePayment('<%=Item.PaymentId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                                    <% 
                                                    }
                                                } 
                                    %>
                                            &nbsp;</td>
                                        </tr>
                                    <%
                                            }
                                    %>
                                        <tr>
                                            <td colspan='4'><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</td>
                                            <td align="right"><%=total.ToString("N3") %></td>
                                        </tr>
                                    <% 
                                        }
                                        else
                                        { 
                                    %>
                                        <tr><td colspan="9" align="center"><h3>Không có dữ liệu.</h3></td></tr>
                                    <%
                                        }
                                    %>
                                    </tbody>    
                                </table>    
                            </td>
                        </tr>
                    </table>
                </div>
                <div id='Accounting'>
                    <%--<%Html.RenderAction("GeneralJournals", "Accounting", new { IssueId = WSI.Invoice.InvoiceId }); %>--%>
                </div>
            </div>
        </form>
    
        <div class="logs">
            <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = WSI.Invoice.CreatedBy });%>
        </div>
        </div>
    </div>
</div>