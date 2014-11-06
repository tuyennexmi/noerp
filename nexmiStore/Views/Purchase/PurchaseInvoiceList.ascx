<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("INVOICE_NO", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("ORDER_NO", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PAID", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <%--<th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>--%>
            <th width="6%"></th>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMPurchaseInvoicesWSI> WSIs = (List<NEXMI.NMPurchaseInvoicesWSI>)ViewData["WSIs"];
        int totalPage = 1;
        double total = 0, paid = 0, remain = 0;
        string func = NEXMI.NMConstant.Functions.PI;        
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;

            int count = 1;
            double PaidAmount = 0;
            double RemainingAmount = 0;
            foreach (NEXMI.NMPurchaseInvoicesWSI Item in WSIs)
            {
                PaidAmount = (Item.Payments.Where(i => i.PaymentStatus == NEXMI.NMConstant.PaymentStatuses.Done).Sum(x => x.PaymentAmount));
                RemainingAmount = (Item.Invoice.TotalAmount - PaidAmount);
                total += Item.Invoice.TotalAmount;
                paid += PaidAmount;
                remain += RemainingAmount;
    %>
            <tr ondblclick="javascript:fnLoadPIDetail('<%=Item.Invoice.InvoiceId%>')">
                <td ><%=count++%></td>
                <td ><%=Item.Invoice.InvoiceId%></td>
                <td ><%=Item.Invoice.InvoiceDate.ToString("dd-MM-yyyy")%></td>
                <td ><%=Item.Invoice.Reference%></td>
                <td >
                    <a href="javascript:fnPopupCustomerDetail('<%=Item.Supplier.CustomerId%>','POPUP')">
                        [<%=Item.Supplier.Code%>] <%=Item.Supplier.CompanyNameInVietnamese%>
                    </a>
                </td>
                <td ><%=Item.Invoice.DetailsList.Count%></td>                
                <td ><%=Item.Invoice.DescriptionInVietnamese%></td>
                <td align="right" ><%=Item.Invoice.TotalAmount.ToString("N3")%></td>
                <td align="right" ><%=PaidAmount.ToString("N3")%></td>
                <td align="right" ><%=RemainingAmount.ToString("N3")%></td>
                <td ><%=Item.CreatedBy.CompanyNameInVietnamese%></td>
                <%--<td ><%=GlobalValues.GetStatus(Item.Invoice.InvoiceStatus).Status.Name%></td>--%>
                <td >
                        <a href="javascript:fnLoadPIDetail('<%=Item.Invoice.InvoiceId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                <%
                    if (Item.Invoice.InvoiceStatus == NEXMI.NMConstant.PIStatuses.Draft)    // | Item.Invoice.InvoiceStatus == NEXMI.NMConstant.PIStatuses.Open)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {                        
                %>
                        <a href="javascript:fnPopupPIDialog('<%=Item.Invoice.InvoiceId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                <%  }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {
                %>
                        <a href="javascript:fnDeletePI('<%=Item.Invoice.InvoiceId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
                <td colspan='7'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                <td align="right"><b><%=total.ToString("N3") %></b></td>
                <td align="right"><b><%=paid.ToString("N3") %></b></td>
                <td align="right"><b><%=remain.ToString("N3") %></b></td>
            </tr>
    </tbody>
    <tfoot>
    
    </tfoot>
</table>

<script type="text/javascript">
    $(function () {
        $('#pagination-Invoice').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>