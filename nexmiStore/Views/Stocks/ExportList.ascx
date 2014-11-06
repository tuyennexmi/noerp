<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    function fnRefreshExport(type) {
        $("#txtExportKeyword").val("");
        fnReloadExport('', type);
    }
</script>


<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("ID", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId)%></th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
            <%
                //if (ViewData["Mode"].ToString() != "Print")
              {%>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
            <th></th>
            <%} %>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMExportsWSI> WSIs = (List<NEXMI.NMExportsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        string func = NEXMI.NMConstant.Functions.Export;
        
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            int count = 1;            
            double totalQuantity = 0;
            foreach (NEXMI.NMExportsWSI Item in WSIs)
            {
                //Item.Details = Item.Import.ImportDetailsList.ToList();
                //totalQuantity += Item.Details.Sum(i => i.Quantity);                    
            %>
            <tr ondblclick="javascript:fnLoadExportDetail('<%=Item.Export.ExportId %>')">
                <td width="3%"><%=count++%></td>
                <td><%= Item.Export.ExportId %></td>
                <td ><%= Item.Export.ExportDate.ToString("dd-MM-yyyy")%></td>
                <td >
                <%  if (Item.Customer != null)
                    {  %>
                    <a href="javascript:fnPopupCustomerDetail('<%=Item.Customer.CustomerId%>','POPUP')">
                        [<%=Item.Customer.Code%>] <%=Item.Customer.CompanyNameInVietnamese%>
                    </a>
                <%  } %>
                </td>
                <td><%= NEXMI.NMCommon.GetName(Item.Export.StockId, langId)%></td>
                <td><%= Item.Details.Count %></td>
                <td><%= Item.Export.DescriptionInVietnamese %></td>
                <%
                //if (ViewData["Mode"].ToString() != "Print")
                  {%>
                <td ><%=Item.CreatedBy.CompanyNameInVietnamese%></td>
                <td ><%=NEXMI.NMCommon.GetStatusName(Item.Export.ExportStatus, langId)%></td>
                <td >
                        <a href="javascript:fnLoadExportDetail('<%=Item.Export.ExportId %>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                <%
                    if (Item.Export.ExportStatus == NEXMI.NMConstant.EXStatuses.Draft)
                    {
                        if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        {                        
                %>
                        <a href="javascript:fnExportProduct('<%=Item.Export.ExportId %>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                <%  }
                        if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        {
                %>
                        <a href="javascript:fnDeleteExport('<%=Item.Export.ExportId %>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
    <%
        }
    %>
    </tbody>
    <tfoot>
    
    </tfoot>
</table>

<script type="text/javascript">
    $(function () {
        $('#pagination-Export').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>