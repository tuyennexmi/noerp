<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>


<%
    List<NEXMI.NMManufactureOrdersWSI> WSIs = (List<NEXMI.NMManufactureOrdersWSI>)ViewData["WSIs"];
    int totalPage = 1;
    try
    {
        int totalRows = int.Parse(ViewData["TotalRows"].ToString());
        if (totalRows % NEXMI.NMCommon.PageSize() == 0)
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize();
        }
        else
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
        }
    }
    catch { }
    string func = NEXMI.NMConstant.Functions.ManufactureOrders;
%>


<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <tr>
        <th>#</th>
        <th>Mã tham chiếu</th>
        <th>Ngày dự kiến</th>
        <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
        <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
        <th>Chịu trách nhiệm</th>
        <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
        <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
        <th></th>
    </tr>
    <%  int count = 1;
    foreach(NEXMI.NMManufactureOrdersWSI Item in WSIs)
    {
    %>
    <tr ondblclick="javascript:fnLoadMODetail('<%=Item.ManufactureOrder.Id%>')">
        <td><%=count++ %></td>  
        <td><%=Item.ManufactureOrder.Id %></td>
        <td><%=Item.ManufactureOrder.StartDate.ToString("dd-MM-yyyy") %></td>
        <td><%= NEXMI.NMCommon.GetName(Item.ManufactureOrder.ProductId, langId)%></td>
        <td><%=Item.ManufactureOrder.Quantity.ToString("N3") %></td>
        <td><%=Item.ManagedBy.CompanyNameInVietnamese%></td>        
        <td ><%=Item.CreatedBy.CompanyNameInVietnamese%></td>
        <td ><%=NEXMI.NMCommon.GetStatusName(Item.ManufactureOrder.Status, langId)%></td>
        <td >
                <a href="javascript:fnLoadMODetail('<%=Item.ManufactureOrder.Id%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
        <%
            if (Item.ManufactureOrder.Status == NEXMI.NMConstant.MOStatuses.Draft )
            {
                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                {                        
        %>
                    <a href="javascript:fnShowMOForm('<%=Item.ManufactureOrder.Id%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
        <%      }
                if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                {
        %>
                    <a href="javascript:fnDeleteMO('<%=Item.ManufactureOrder.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
        <% 
                }
            }
        %>
        </td>
    </tr>
    <%
    }
    %>
    
</table>