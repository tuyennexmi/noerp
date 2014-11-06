<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMSalesInvoicesWSI WSI = (NEXMI.NMSalesInvoicesWSI)ViewData["WSI"];
    string id = ViewData["Id"].ToString();
    string previousId = NEXMI.NMCommon.PreviousId(id, "InvoiceId", "SalesInvoices", "");
    string nextId = NEXMI.NMCommon.NextId(id, "InvoiceId", "SalesInvoices", "");
    string current = ViewData["InvoiceStatus"].ToString();
    
%>

<script type="text/javascript">
    $(function () {
        $("#tabsSalesInvoice").jqxTabs({ theme: theme, keyboardNavigation: false });

        $('#tabsSalesInvoice').bind('selected', function (event) {
            var item = event.args.item;
            switch (item) {
                case 3:
                    LoadContentDynamic('Accounting', 'Accounting/GeneralJournals', { IssueId: '<%=id %>' });
                    break;
            }
        });

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
                <%if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SI))
                { %>
                    <button onclick="javascript:fnCreateInvoice('', '', '')" class="button color red"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% }
                    if (current == NEXMI.NMConstant.SIStatuses.Draft)
                    {
                        if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SI))
                        {
                %>
                            <button onclick="javascript:fnCreateInvoice('<%=id%>', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                        <%}
                        if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SI))
                        { %>
                            <button onclick="javascript:fnDeleteSI('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                        <% }
                    }
                    %>
                    <%if (GetPermissions.GetDuplicate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SI))
                    { %>
                        <button onclick="javascript:fnCreateInvoice('<%=id%>', '', 'Copy')" class="button"><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></button>
                    <%} %>
                    <%if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SI))
                    { %>
                        <button onclick="javascript:fnPrintSalesInvoice('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                    <%} %>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <input id="btNext" type="hidden" value="<%=nextId%>" />
                                <ul class="button-group">
	                                <li title="Trước"><button class="button" onclick="javascript:fnLoadSIDetail('<%=previousId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                    <li title="Sau"><button class="button" onclick="javascript:fnLoadSIDetail('<%=nextId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                                </ul>
                            </td>
                            <td>
                                <%Html.RenderPartial("SalesInvoiceSV"); %>
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
                if (current == NEXMI.NMConstant.SIStatuses.Draft)
                {
            %>
            <%--<button class="color red button" onclick='javascript:fnSetInvoiceStatus("<%=id%>", "<%=ViewData["Reference"]%>")'><%= NEXMI.NMCommon.GetInterface("CONFIRM", langId) %></button>--%>
                    <button class="color red button" onclick="javascript:fnShowReceiptForm('', '<%=id%>')"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></button>
                    <button class="color red button" onclick="javascript:fnSetInvoiceStatus('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("DEBIT", langId)%></button>
            <%                    
                }
                else if (current == NEXMI.NMConstant.SIStatuses.Debit)
                { 
            %>
                    <button class="color red button" onclick="javascript:fnShowReceiptForm('', '<%=id%>')"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></button>
            <%--<button class="color red button" onclick="javascript:fnReceipt('<%=id%>')">Ghi nợ</button>--%>
            <%--<button class="button" onclick="javascript:fnRefund('<%=id%>', '<%=NEXMI.NMConstant.PaymentType.Refund%>')">Hoàn tiền</button>--%>
            <%
                }
                else
                {
                    double balance = double.Parse(ViewData["BalanceAmount"].ToString());
                    if(balance > 0)
                    {
            %>
                        <button class="color red button" onclick="javascript:fnShowReceiptForm('', '<%=id%>')"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></button>
            <%      }
            %>
            <%--<button class="button" onclick="javascript:fnRefund('<%=id%>', '<%=NEXMI.NMConstant.PaymentType.Refund%>')">Hoàn tiền</button>--%>
            <%
                }
            %>
            </div>
        </div>     
        <div class="divStatusBar">
            <%
                Html.RenderAction("StatusBar", "UserControl", new { objectName = "SalesInvoices", current = current });
            %>
        </div>
    </div>
    <div class='divContentDetails'>
    <form id="formSalesInvoice" action="" style="margin: 10px;">
        <table style="width: 100%" class="frmInput">
            <tr>
                <td>
                    <label class="lbId"><%= NEXMI.NMCommon.GetInterface("INVOICE", langId)%> <label id="txtId"><%=id%></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                            <td><%=ViewData["CustomerName"]%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></td>
                            <td><%=ViewData["InvoiceDate"]%></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></td>
                            <td><%=ViewData["slUsers"]%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                            <td><%=ViewData["Reference"]%></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                            <td><%= NEXMI.NMCommon.GetName(WSI.Invoice.StockId, langId)%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REFERENCE", langId)%></td>
                            <td><%= WSI.Invoice.SourceDocument %></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div id="tabsSalesInvoice">
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
                            <table style="width: 100%" id="tbSalesInvoiceDetail" class="tbDetails">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %>(%)</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <% 
                                        List<NEXMI.SalesInvoiceDetails> Details = (List<NEXMI.SalesInvoiceDetails>)ViewData["Details"];
                                        int i = 1;
                                        foreach (NEXMI.SalesInvoiceDetails Item in Details)
                                        {
                                    %>
                                    <tr>
                                        <td><%=i++%></td>
                                        <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                        <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
                                        <td><%=Item.Quantity.ToString("N3")%></td>
                                        <td><%=Item.Price.ToString("N3")%></td>
                                        <td><%=Item.Discount.ToString("N3")%></td>
                                        <td><%=Item.Tax.ToString("N3")%></td>
                                        <td align="right"><%=Item.TotalAmount.ToString("N3")%></td>
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
                                    <td valign="top" width='50%'>
                                        <table style="width: 100%">
                                            <tr>
                                                <td><b><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>: </b></td>                                                    
                                            </tr>
                                            <tr>
                                                <td><%= WSI.Invoice.DescriptionInVietnamese %></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td valign="top">
                                        <table style="width: 100%">
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
                                                <td align="right" style="width: 120px;"><label id="lbAmount"><%=ViewData["Amount"]%></label></td>
                                            </tr>                                                    
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalDiscount"><%=ViewData["Discount"]%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalVATRate"><%=ViewData["Tax"]%></label></td>
                                            </tr>
                                            <%--<tr>
                                                <td colspan="6" align="right"><b>Phí vận chuyển: </b></td>
                                                <td align="right"><label><%=ViewData["ShipCost"]%></label></td>
                                            </tr>--%>
                                            <%--<tr>
                                                <td colspan="6" align="right"><b>Phí khác: </b></td>
                                                <td align="right"><label><%=ViewData["OtherCost"]%></label></td>
                                            </tr>--%>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalAmount"><%=ViewData["TotalAmount"]%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("PAID", langId) %>: </b></td>
                                                <td align="right"><label id="Label1"><%=ViewData["PaidAmount"]%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %>: </b></td>
                                                <td align="right"><label id="Label2"><%=ViewData["BalanceAmount"]%></label></td>
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
                        <%  List<NEXMI.Receipts> Receipts = (List<NEXMI.Receipts>)ViewData["Receipts"];
                        %>
                            <table id="tbReceipts" class="tbDetails" width="100%">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("BILL_NO", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></th>
                                        <th>Số tiền</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                <%
                                    if (Receipts.Count > 0)
                                    {   
                                        string functionReceipt = NEXMI.NMConstant.Functions.Receipts;
                                        double total = 0;
                                        foreach (NEXMI.Receipts Item in Receipts)
                                        {
                                            
                                            total += Item.ReceiptAmount;
                                %>
                                    <tr ondblclick="javascript:fnLoadReceiptDetail('<%=Item.ReceiptId%>')">
                                        <td style="width: 15px;"><%=i++%></td>
                                        <td style="width: 120px;"><div><%=Item.ReceiptId%></div></td>
                                        <td style="width: 100px;"><div><%=Item.ReceiptDate.ToString("dd-MM-yyyy")%></div></td>                                        
                                        <td><%=NEXMI.NMCommon.GetParameterName(Item.PaymentMethodId) %></td>
                                        <td style="width: 120px;" align="right"><div><%=(Item.ReceiptAmount).ToString("N3")%></div></td>
                                        <td><div title="<%=Item.DescriptionInVietnamese%>"><%=Item.DescriptionInVietnamese%></div></td>
                                        <td><div><%=NEXMI.NMCommon.GetStatusName(Item.ReceiptStatus, langId)%></div></td>
                                        <td class="actionCols">
                                            <a href="javascript:fnLoadReceiptDetail('<%=Item.ReceiptId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                <% 
                                            if (Item.ReceiptStatus == NEXMI.NMConstant.ReceiptStatuses.Draft)
                                            {
                                                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                                                {    
                                %>
                                            <a href="javascript:fnShowReceiptForm('<%=Item.ReceiptId%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
                                <% 
                                                }
                                %>
                                <% 
                                                if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                                                {
                                %>
                                            <a href="javascript:fnDeleteReceipt('<%=Item.ReceiptId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
                                    <tr><td colspan="8" align="center"><h3>Không có dữ liệu.</h3></td></tr>
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
            </div>
        </div>
    </form>
    
    <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = ViewData["SendTo"] });%>
    </div>
    </div>
</div>