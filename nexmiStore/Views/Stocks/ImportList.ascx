<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">

    function fnRefreshImport(type) {
        $("#txtImportKeyword").val("");
        fnReloadImport(type);
    }

</script>

<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("ID", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("IMPORT_DATE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></th>
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
        List<NEXMI.NMImportsWSI> WSIs = (List<NEXMI.NMImportsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        string func = NEXMI.NMConstant.Functions.Import;
        
        if (WSIs.Count > 0)
        {
            int pageSize = NEXMI.NMCommon.PageSize();
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % pageSize == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            int count = 1;            
            double totalQuantity = 0;
            foreach (NEXMI.NMImportsWSI Item in WSIs)
            {
                //Item.Details = Item.Import.ImportDetailsList.ToList();
                //totalQuantity += Item.Details.Sum(i => i.GoodQuantity);                    
            %>
            <tr ondblclick="javascript:fnLoadImportDetail('<%=Item.Import.ImportId%>')">
                <td width="3%"><%=count++%></td>
                <td><%= Item.Import.ImportId %></td>
                <td ><%= Item.Import.ImportDate.ToString("dd-MM-yyyy")%></td>
                <td >
                <% if(Item.Supplier != null){
                    %>
                    <a href="javascript:fnPopupCustomerDetail('<%=Item.Supplier.CustomerId%>','POPUP')">
                        [<%=Item.Supplier.Code%>] <%=Item.Supplier.CompanyNameInVietnamese%>
                    </a>
                <%}
                    %>
                </td>
                <td><%= NEXMI.NMCommon.GetName(Item.Import.StockId, langId)%></td>
                <td><%= Item.Details.Count %></td>
                <td><%= Item.Import.DescriptionInVietnamese %></td>
                <%
                //if (ViewData["Mode"].ToString() != "Print")
                  {%>
                <td ><%=Item.CreatedUser.CompanyNameInVietnamese%></td>
                <td ><%=NEXMI.NMCommon.GetStatusName(Item.Import.ImportStatus, langId)%></td>
                <td >
                        <a href="javascript:fnLoadImportDetail('<%=Item.Import.ImportId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                <%
                    if (Item.Import.ImportStatus == NEXMI.NMConstant.IMStatuses.Draft )
                    {
                        if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        {                        
                %>
                        <a href="javascript:fnPopupImportDialog('<%=Item.Import.ImportId%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                <%  }
                        if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        {
                %>
                        <a href="javascript:fnDeleteImport('<%=Item.Import.ImportId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
        $('#pagination-Import').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>