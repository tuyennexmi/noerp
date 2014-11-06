<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails" border=".1">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("ID", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DELIVERY_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
    <%  if (ViewData["Mode"].ToString() != "Print")
        {   %>
            <th></th>
    <%  } %>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMSalesOrdersWSI> WSIs = (List<NEXMI.NMSalesOrdersWSI>)ViewData["WSIs"];
        int totalPage = 1;
        double amount = 0, total = 0;
        string func = NEXMI.NMConstant.Functions.SalesOrder;
        if (ViewData["SOType"].ToString() == NEXMI.NMConstant.SOType.Quotation)
        {                
            func = NEXMI.NMConstant.Functions.Quotation;
        }
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            int count = 1;
            foreach (NEXMI.NMSalesOrdersWSI Item in WSIs)
            {
                amount = Item.Order.OrderDetailsList.Sum(i=>i.TotalAmount);
                total += amount;
    %>
            <tr ondblclick="javascript:fnLoadSalesOrderDetail('<%=Item.Order.OrderId%>')">
                <td width="3%"><%=count++%></td>
                <td ><%=Item.Order.OrderId%></td>
                <td ><%=Item.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                <td ><%=Item.Order.DeliveryDate.Value.ToString("dd-MM-yyyy")%></td>
                <td >
            <%  if (ViewData["Mode"].ToString() != "Print")
                {%>
                    <a href="javascript:fnPopupCustomerDetail('<%=Item.Customer.CustomerId%>','POPUP')">
                        [<%=Item.Customer.Code%>] <%=Item.Customer.CompanyNameInVietnamese%>
                    </a>
            <%  } else { %>
                    [<%=Item.Customer.Code%>] <%=Item.Customer.CompanyNameInVietnamese%>
            <%  } %>
                </td>
                <td ><%=Item.Order.OrderDetailsList.Count%></td>
                <%--<td ><%=Item.Order.Reference%></td>
                <td ><%=Item.Order.Transportation%></td>--%>
                <td ><%= NEXMI.NMCommon.GetName(Item.Order.StockId, langId)%></td>
                <td align="right" ><%= amount.ToString("N3")%></td>
                <td ><%=Item.CreatedBy.CompanyNameInVietnamese%></td>
                <td ><%=NEXMI.NMCommon.GetStatusName(Item.Order.OrderStatus, langId)%></td>

        <%  if (ViewData["Mode"].ToString() != "Print")
            {   %>
                <td >
                        <a href="javascript:fnLoadSalesOrderDetail('<%=Item.Order.OrderId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                <%
                    if (Item.Order.OrderStatus == NEXMI.NMConstant.SOStatuses.Draft | Item.Order.OrderStatus == NEXMI.NMConstant.SOStatuses.Order)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {                        
                %>
                        <a href="javascript:fnPopupSalesOrderDialog('<%=Item.Order.OrderId%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                <%  }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {
                %>
                        <a href="javascript:fnDeleteSalesOrder('<%=Item.Order.OrderId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                <% 
                    }
                }
                %>
                </td>
        <%  } %>
            </tr>
    <%
            }
        }
    %>
            <tr>
                <td colspan='7'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                <td align="right"><%=total.ToString("N3") %></td>
            </tr>
    </tbody>
    <tfoot>
    
    </tfoot>
</table>

<%
if (ViewData["Mode"].ToString() != "Print")
{
    %>
<script type="text/javascript">
    $(function () {
        $('#pagination-SOrder').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>
<%
} %>