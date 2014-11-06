<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/SalesOrders.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
    });

    function fnChangeOrderStatus(id, status, typeId, customerId, maxDebitAmount) {
        if (confirm('<%= NEXMI.NMCommon.GetInterface("CONFIRM_ORDER", langId) %> ?')) {
            var Approval = false;
            $.post(appPath + 'Sales/CheckDebit', { customerId: customerId, maxDebitAmount: maxDebitAmount }, function (data) {
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
                    $.post(appPath + 'Sales/ChangeStatus', { id: id, status: status, typeId: typeId }, function (data) {
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

</script>
<% 
    NEXMI.NMSalesOrdersWSI WSI = (NEXMI.NMSalesOrdersWSI)ViewData["WSI"];
    if (WSI.Customer == null) WSI.Customer = new NEXMI.Customers();
    string id = WSI.Order.OrderId;
    string current = WSI.Order.OrderStatus;
    string createInvoice = WSI.Order.CreateInvoice;
    string previousId = NEXMI.NMCommon.PreviousId(id, "OrderId", "SalesOrders", " where OrderTypeId = '" + WSI.Order.OrderTypeId + "'");
    string nextId = NEXMI.NMCommon.NextId(id, "OrderId", "SalesOrders", " where OrderTypeId = '" + WSI.Order.OrderTypeId + "'");
    string func = ViewData["FunctionId"].ToString();
%>
<div class="divActions">
    <table>
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
        <%  if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
            { 
        %>
                <button onclick='javascript:fnCreateLeaseOrderDialog("", "", "<%=WSI.Order.OrderTypeId%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
        <%  
            }
            if (current == NEXMI.NMConstant.LOStatuses.Draft || current == NEXMI.NMConstant.LOStatuses.Order || current == NEXMI.NMConstant.LOStatuses.Canceled)
            {
                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                {
        %>
                    <button onclick="javascript:fnCreateLeaseOrderDialog('<%=id%>', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
        <%
                }
                if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                { 
        %>
                    <button onclick="javascript:fnDeleteSalesOrder('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
        <%
                }
            }
            if (GetPermissions.GetDuplicate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
            {
        %>
                <button onclick="javascript:fnPopupSalesOrderDialog('<%=id%>', 'Copy', '<%=WSI.Order.OrderTypeId%>')" class="button"><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></button>
        <%
            }
            if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
            { 
        %>      
                <button onclick="javascript:fnPrintOrder('<%=id%>', 'CONTRACT','<%=Session["Lang"].ToString() %>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT_CONTRACT", langId)%></button>
        <%
            }
        %>
            </td>
            <td align="right">
                <table>
                    <tr>
                        <td>
                            <input id="btNext" type="hidden" value="<%=nextId%>" />
                            <ul class="button-group">
	                            <li title="Trước"><button class="button" onclick="javascript:fnLoadSalesOrderDetail('<%=previousId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                <li title="Sau"><button class="button" onclick="javascript:fnLoadSalesOrderDetail('<%=nextId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                            </ul>
                        </td>
                        <td>
                            <%Html.RenderPartial("SalesOrderSV"); %>    
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
<% 
    string temp = "";
    if (createInvoice == NEXMI.NMConstant.CreateInvoice.OnDemand)
        temp = NEXMI.NMConstant.LOStatuses.Maintain;
    else
        temp = NEXMI.NMConstant.LOStatuses.Order;
    if (current == NEXMI.NMConstant.LOStatuses.Draft)
    {
        if (NEXMI.NMCommon.GetSetting("APPROVAL_SALE_QUOTE"))
        {
%>
            <button class="color red button" onclick="javascript:fnApprovalQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("APPROVE", langId) %></button>
<%      }
        else
        {
%>
            <button class="color red button" onclick="javascript:fnSendQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("SEND_QUOTATION", langId) %></button>
            <button class="button" onclick="javascript:fnChangeOrderStatus('<%=id%>', '<%=NEXMI.NMConstant.LOStatuses.Order%>', '<%=NEXMI.NMConstant.SOType.SalesOrder%>', '<%=WSI.Customer.CustomerId %>', '<%=WSI.Customer.MaxDebitAmount %>')"><%= NEXMI.NMCommon.GetInterface("CONFIRM_ORDER", langId) %></button>
<%      }
%>
        <button class="button" onclick="javascript:fnCancelQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
<%
    }
    else if ( current == NEXMI.NMConstant.LOStatuses.Approval)
    {
%>
        <button class="color red button" onclick="javascript:fnSendQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("SEND_QUOTATION", langId) %></button>
        <button class="button" onclick="javascript:fnChangeOrderStatus('<%=id%>', '<%=NEXMI.NMConstant.LOStatuses.Order%>', '<%=NEXMI.NMConstant.SOType.SalesOrder%>', '<%=WSI.Customer.CustomerId %>', '<%=WSI.Customer.MaxDebitAmount %>')"><%= NEXMI.NMCommon.GetInterface("CONFIRM_ORDER", langId) %></button>
        <button class="button" onclick="javascript:fnCancelQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
<%        
    }
    else if (current == NEXMI.NMConstant.LOStatuses.Sent)
    { 
%>
        <button class="button" onclick="javascript:fnSendQuotation('<%=id%>')">Gửi báo giá lại</button>
        <button class="color red button" onclick="javascript:fnConfirmQuotation('<%=id%>', '<%=NEXMI.NMConstant.LOStatuses.Order%>', '<%=NEXMI.NMConstant.SOType.SalesOrder%>')"><%= NEXMI.NMCommon.GetInterface("CONFIRM_ORDER", langId) %></button>
        <button class="button" onclick="javascript:fnCancelQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
<%
    }
    else if (current == NEXMI.NMConstant.LOStatuses.Canceled)
    {
        string desTemp = "", statusTemp = "";
        if (WSI.Order.OrderTypeId == NEXMI.NMConstant.SOType.Quotation)
        {
            desTemp = NEXMI.NMCommon.GetInterface("STATUS", langId) + ": " + NEXMI.NMCommon.GetInterface("QUOTATION_TITLE", langId);
            statusTemp = NEXMI.NMConstant.LOStatuses.Draft;
        }
        else
        {
            desTemp = NEXMI.NMCommon.GetInterface("STATUS", langId) +": " + NEXMI.NMCommon.GetInterface("SO_TITLE", langId);
            statusTemp = NEXMI.NMConstant.LOStatuses.Order;
        }
%>
        <button class="color red button" onclick="javascript:fnReactiveSalesOrder('<%=id%>', '<%=statusTemp%>', '<%=WSI.Order.OrderTypeId%>', '<%=desTemp%>')">Phục hồi đơn hàng</button>
<%
    }
    else if (current == NEXMI.NMConstant.LOStatuses.Order || current == NEXMI.NMConstant.LOStatuses.Maintain || current == NEXMI.NMConstant.LOStatuses.Setup)
    {
        if (WSI.Exports.Count > 0)
        {
%>
            <button class="color red button" onclick="javascript:fnCreateExportFromSO('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("DELIVERY", langId) %></button>
<%      }
        else
        {
%>
            <button class="color red button" onclick="javascript:fnCreateExportFromSO('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CREATE_EXPORT", langId) %></button>
<%      }
        if (createInvoice == NEXMI.NMConstant.CreateInvoice.OnDemand)
        {
            
%>
            <%--<button class="color red button" onclick="javascript:fnCreateInvoiceMode('', '<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %></button>--%>
            <button class="color red button" onclick="javascript:fnCreateInvoice('', '<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %></button>
<%          
        }
    }
    else if(current == NEXMI.NMConstant.LOStatuses.Order){
%>
        <button class="color red button" onclick="javascript:fnCreateExportFromSO('<%=id%>')">Phiếu giao hàng</button>
        <%--<button class="color red button" onclick="javascript:fnShowInvoicesOfOrder('<%=id%>')">Xem hóa đơn</button>--%>
        <button class="color red button" onclick="javascript:fnCreateInvoice('', '<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %></button>
        <button class="button" onclick="javascript:fnCancelSalesOrder('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %> đơn hàng</button>
<%
    //}
    //else if (current == NEXMI.NMConstant.LOStatuses.Done)
    //{
        //if (createInvoice == NEXMI.NMConstant.CreateInvoice.OnDemand)
        //{
            //if (NEXMI.NMCommon.GetNumberOfSI(ViewData["Reference"].ToString()) > 0)
            //{ 
%>
                <%--<button class="color red button" onclick="javascript:()">Xem các hóa đơn</button>--%>
<%
            //}
%>
            <%--<button class="color red button" onclick="javascript:fnCreateInvoice('', '<%=id%>')">Tạo hóa đơn</button>--%>
<%
        //}
    }
%>
    </div>
    <div class="divStatusBar">  
        <%
            Html.RenderAction("StatusBar", "UserControl", new { objectName = "LeaseOrders", current = current });
        %>
    </div>                    
</div>
<div class='divContentDetails'>
    <form id="formSalesOrder" action="" style="margin: 10px;">
        <table style="width: 100%" class="frmInput">
            <tr>
                <td>
                    <label class="lbId"><%=ViewData["Tilte"].ToString()%> <label id="txtId"><%=id%></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                            <td style="width: 30%;"><b>[<%= WSI.Customer.Code%>] <%= WSI.Customer.CompanyNameInVietnamese%></b></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALE_AT", langId) %></td>
                            <td><%=NEXMI.NMCommon.GetName(WSI.Order.StockId, langId)%></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CONTRACT_DATE", langId) %></td>
                            <td><%=WSI.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DEPOSIT", langId) %></td>
                            <td><%=WSI.Order.Deposit %></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MAINTAIN_DATE", langId) %></td>
                            <td><%=WSI.Order.MaintainDate.Value.ToString("dd-MM-yyyy")%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SETUP_FEE", langId) %></td>
                            <td><%=WSI.Order.SetupFee %></td>
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
        <%if (WSI.Order.OrderTypeId == NEXMI.NMConstant.SOType.SalesOrder)
            { %>
                <li><%= NEXMI.NMCommon.GetInterface("INVOICE", langId) %></li>
                <li><%= NEXMI.NMCommon.GetInterface("EXPORT", langId) %></li>
        <%} %>
            </ul>
            <div style="padding:10px;">
                <table class="frmInput">
                    <tr>
                        <td>
                            <table style="width: 100%" id="tbSalesOrderDetail" class="tbDetails">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("MACHINE_TYPE", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %>(%)</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <% 
                                        int i = 1;
                                        List<NEXMI.SalesOrderDetails> Details = (List<NEXMI.SalesOrderDetails>)Session["Details"];
                                        foreach (NEXMI.SalesOrderDetails Item in Details)
                                        {
                                    %>
                                    <tr>
                                        <td><%=i++%></td>
                                        <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                        <td><%=NEXMI.NMCommon.GetProductUnitName(Item.ProductId)%></td>
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
                                    <td valign="top" style="width: 60%"><%=WSI.Order.Description%></td>
                                    <td valign="top">
                                        <table style="width: 100%">
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
                                                <td align="right" style="width: 120px;"><label id="lbTotalAmount"><%=WSI.Order.Amount.ToString("N3")%></label></td>
                                            </tr>                                                        
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalDiscount"><%=WSI.Order.Discount.ToString("N3")%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalVATRate"><%=WSI.Order.Tax.ToString("N3")%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId)%>: </b></td>
                                                <td align="right"><label id="lbInvoiceAmount"><%=WSI.Order.TotalAmount.ToString("N3")%></label></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div style="padding:10px;">
                <p style="padding:8px"><%=WSI.Order.PaymentTerm%></p>
            </div>
            <div style="padding:10px;">
                <table class="frmInput">
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></td>
                        <td><%=WSI.SalesPerson.CompanyNameInVietnamese%></td>
                        <td class="lbright">Incoterm</td>
                        <td><%=WSI.Incoterm.Name%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY_METHOD", langId) %></td>
                        <td><%=WSI.ShippingPolicy.Name%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %></td>
                        <td><%=WSI.CreateInvoice.Name%></td>
                    </tr>
                    <tr>                                    
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ADVANCE", langId) %></td>
                        <td><%=WSI.Order.Advances%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REPAIR_DATE", langId) %></td>
                        <td><%=WSI.Order.RepairDate%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAID", langId) %></td>
                        <td><input type="checkbox" disabled="disabled" /></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERED", langId) %></td>
                        <td><input type="checkbox" disabled="disabled" /></td>
                    </tr>
                </table>
            </div>
        <%
            if (WSI.Order.OrderTypeId == NEXMI.NMConstant.SOType.SalesOrder)
          { %>            
            <div id='Invoice' style="padding:10px;">
                <table id="tbReceipts" class="tbDetails">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th width="15%"><%= NEXMI.NMCommon.GetInterface("INVOICE_NO", langId) %></th>
                            <th width="15%"><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></th>
                            <th width="15%"><%= NEXMI.NMCommon.GetInterface("ORDER_NO", langId) %></th>
                            <th width="15%"><%= NEXMI.NMCommon.GetInterface("REFERENCE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
                            <th width="8%"></th>
                        </tr>
                    </thead>
                    <tbody>
            <%
              double total = 0;
              if (WSI.Invoices.Count > 0)
              {
                  int count = 1;
                  double amount = 0;
                  foreach (NEXMI.SalesInvoices Item in WSI.Invoices)
                  {
                      amount = Item.TotalAmount;
                      total += amount;
                    %>
                            <tr ondblclick="javascript:fnLoadSIDetail('<%=Item.InvoiceId%>')">
                                <td ><%=count++%></td>
                                <td ><%=Item.InvoiceId%></td>
                                <td ><%=Item.InvoiceDate.ToString("dd-MM-yyyy")%></td>
                                <td ><%=Item.Reference%></td>
                                <td ><%=Item.SourceDocument%></td>
                                <td ><%=Item.DescriptionInVietnamese%></td>
                                <td align="right" ><%= amount.ToString("N3")%></td>
                                <td >
                                        <a href="javascript:fnLoadSIDetail('<%=Item.InvoiceId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                <%
if (Item.InvoiceStatus == NEXMI.NMConstant.SIStatuses.Draft)    // | Item.Invoice.InvoiceStatus == NEXMI.NMConstant.PIStatuses.Open)
{
    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
    {                        
                                %>
                                        <a href="javascript:fnCreateInvoice('<%=Item.InvoiceId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                                <%  }
    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
    {
                                %>
                                        <a href="javascript:fnDeleteSI('<%=Item.InvoiceId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                                <% 
    }
}
                                %>
                                </td>
                            </tr>
                    <%
                  }
              }
                    %>
                            <tr>
                                <td colspan='6'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                                <td align="right"><b><%=total.ToString("N3")%></b></td>
                                <td align="right"></td>
                            </tr>
                    </tbody>
                </table>
            </div>
            <div style="padding:10px;">
                <table id="Table1" class="tbDetails" border=".1" style="width: 100%;">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th><%= NEXMI.NMCommon.GetInterface("BILL_NO", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                    <%  
if (WSI.Exports.Count > 0)
{
    int count = 1;
    foreach (NEXMI.Exports Item in WSI.Exports)
    {
                        %>
                            <tr ondblclick="javascript:fnLoadExportDetail('<%=Item.ExportId %>')">
                                <td width="3%"><%=count++%></td>
                                <td><%= Item.ExportId%></td>
                                <td ><%= Item.ExportDate.ToString("dd-MM-yyyy")%></td>
                                <td><%= Item.DescriptionInVietnamese%></td>
                                <td ><%=NEXMI.NMCommon.GetStatusName(Item.ExportStatus, langId)%></td>
                                <td >
                                        <a href="javascript:fnLoadExportDetail('<%=Item.ExportId %>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                <%
if (Item.ExportStatus == NEXMI.NMConstant.EXStatuses.Draft)
{
    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
    {                        
                                %>
                                        <a href="javascript:fnExportProduct('<%=Item.ExportId %>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                                <%      }
    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
    {
                                %>
                                        <a href="javascript:fnDeleteExport('<%=Item.ExportId %>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                                <% 
    }
}
                                %>
                                </td>
                            <%} %>
                            </tr>
                    <%
}
                    %>
                    </tbody>
                </table>
            </div>
        <%} %>
        </div>
    </form>
    <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = WSI.Order.CreatedBy });%>
</div>
</div>
