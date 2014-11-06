<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th>#</th>            
            <th><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></th>
            <th>PTVC</th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %> VAT</th>
            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
            
            <%if (ViewData["Mode"].ToString() != "Print")
              {%>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
            <th width="6%"></th>
            <%} %>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMSalesOrdersWSI> WSIs = (List<NEXMI.NMSalesOrdersWSI>)ViewData["WSIs"];
        int totalPage = 1;
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
            double total = 0;
            double totalQuantity = 0;
            foreach (NEXMI.NMSalesOrdersWSI Item in WSIs)
            {
                Item.Details = Item.Order.OrderDetailsList.ToList();
                total += Item.Order.TotalAmount;
                totalQuantity += Item.Order.OrderDetailsList.Sum(i => i.Quantity);                    
            %>
            <tr ondblclick="javascript:fnLoadSalesOrderDetail('<%=Item.Order.OrderId%>')">
                <td width="3%"><%=count++%></td>                
                <td ><%=Item.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
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
                <td><%= NEXMI.NMCommon.GetName(Item.Order.StockId, langId)%></td>
                <td ><%=Item.Order.Reference%></td>
                <td ><%=Item.Order.Transportation%></td>
                <td><%= Item.Details.Count > 0? NEXMI.NMCommon.GetProductCode(Item.Details[0].ProductId) : "" %></td>
                <td><%= Item.Details.Count > 0 ? NEXMI.NMCommon.GetName(Item.Details[0].ProductId, langId) : ""%></td>
                <td align="right"><%= Item.Details.Count > 0? Item.Details[0].Quantity.ToString("N3") : "" %></td>
                <td align="right"><%= Item.Details.Count > 0? ((Item.Details[0].Price)*(1+Item.Details[0].Tax/100)).ToString("N0") : "" %></td>
                <td align="right" width="12%"><%=Item.Order.TotalAmount.ToString("N0")%></td>
                
        <%  if (ViewData["Mode"].ToString() != "Print")
            {%>
                <td ><%=Item.CreatedBy.CompanyNameInVietnamese%></td>
                <td ><%=NEXMI.NMCommon.GetStatusName(Item.Order.OrderStatus, langId)%></td>
                <td >
                    <a href="javascript:fnLoadSalesOrderDetail('<%=Item.Order.OrderId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
            <%
                if (Item.Order.OrderStatus == NEXMI.NMConstant.SOStatuses.Draft | Item.Order.OrderStatus == NEXMI.NMConstant.SOStatuses.Order)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {                        
            %>
                    <a href="javascript:fnPopupSalesOrderDialog('<%=Item.Order.OrderId%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
            <%      }
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
            %>
            <tr>
                <td colspan='8'><b> <%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                <td align="right"><b><%=totalQuantity.ToString("N3") %></b></td>
                <td></td>
                <td align="right"><b><%=total.ToString("N0") %></b></td>
            </tr>
    <%
        }
    %>
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