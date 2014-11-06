<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th width="3%">#</th>            
            <th><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></th>
            <th>PTVC</th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %> VAT</th>
            <th width="12%"><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
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
        List<NEXMI.NMPurchaseOrdersWSI> WSIs = (List<NEXMI.NMPurchaseOrdersWSI>)ViewData["WSIs"];
        int totalPage = 1;
        string func = NEXMI.NMConstant.Functions.PQuotation;
        if (ViewData["POType"].ToString() == NEXMI.NMConstant.POType.PurchaseOrder)
        {                
            func = NEXMI.NMConstant.Functions.PO;
        }
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            int count = 1;
            double total = 0, totalQuantity = 0;
            foreach (NEXMI.NMPurchaseOrdersWSI Item in WSIs)
            {
                Item.Details = Item.Order.OrderDetailsList.ToList();
                total += Item.Order.TotalAmount;
                totalQuantity += Item.Order.OrderDetailsList.Sum(i => i.Quantity);
            %>
            <tr ondblclick="javascript:fnLoadPODetail('<%=Item.Order.Id%>')">
                <td ><%=count++%></td>                
                <td ><%=Item.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                <td >
                <%if (ViewData["Mode"].ToString() != "Print")
                  {%>
                    <a href="javascript:fnPopupCustomerDetail('<%=Item.Supplier.CustomerId%>','POPUP')">
                        [<%=Item.Supplier.Code%>] <%=Item.Supplier.CompanyNameInVietnamese%>
                    </a>
                    <%}
                  else
                  { %>
                        [<%=Item.Supplier.Code%>] <%=Item.Supplier.CompanyNameInVietnamese%>
                    <%} %>
                </td>
                <td><%= NEXMI.NMCommon.GetName(Item.Order.ImportStockId, langId)%></td>                
                <td ><%=Item.Order.Reference%></td>
                <td ><%=Item.Order.Transportation%></td>
                <td><%= Item.Details.Count > 0? NEXMI.NMCommon.GetProductCode( Item.Details[0].ProductId) : "" %></td>
                <td><%= Item.Details.Count > 0 ? NEXMI.NMCommon.GetName(Item.Details[0].ProductId, langId) : ""%></td>
                <td align="right"><%= Item.Details.Count > 0? Item.Details[0].Quantity.ToString("N3") : "" %></td>
                <td align="right"><%= Item.Details.Count > 0? ((Item.Details[0].Price)*(1+Item.Details[0].Tax/100)).ToString("N3") : "" %></td>
                <td align="right" ><%=Item.Order.TotalAmount.ToString("N3")%></td>
            <%if (ViewData["Mode"].ToString() != "Print")
              {%>
                <td ><%=Item.CreatedUser.CompanyNameInVietnamese%></td>
                <td ><%=NEXMI.NMCommon.GetStatusName(Item.Order.OrderStatus, langId)%></td>
                <td >
                        <a href="javascript:fnLoadPODetail('<%=Item.Order.Id%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                <%
                if (Item.Order.OrderStatus == NEXMI.NMConstant.POStatuses.Draft | Item.Order.OrderStatus == NEXMI.NMConstant.POStatuses.Order)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {                        
                %>
                        <a href="javascript:fnShowPOForm('<%=Item.Order.Id%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                <%  }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {
                %>
                        <a href="javascript:fnDeletePO('<%=Item.Order.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
            <tr>
                <td colspan='8'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                <td align="right"><%=totalQuantity.ToString("N3") %></td>
                <td></td>
                <td align="right"><%=total.ToString("N3") %></td>
            </tr>
    <%
        }
    %>
    </tbody>
    <tfoot>
    
    </tfoot>
</table>

<%if (ViewData["Mode"].ToString() != "Print")
{%>
<script type="text/javascript">
    $(function () {
        $('#pagination-POrder').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>
<%} %>