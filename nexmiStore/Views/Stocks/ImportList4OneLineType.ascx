<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">

    function fnRefreshImport(type) {
        $("#txtImportKeyword").val("");
        fnReloadImport('', type);
    }

    function fnReloadImport(page, type) {
        LoadContentDynamic('ImportGrid', 'Stocks/ImportList', {
            pageNum: page,
            typeId: type,
            status: $('#slStatus').val(),
            keyword: $('#txtKeywordImport').val(),
            from: $('#dtFrom').val(),
            to: $('#dtTo').val(),
            partnerId: document.getElementsByName('cbbCustomers')[0].value
        });
    }
</script>

<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th>#</th>
            <th width="10%"><%= NEXMI.NMCommon.GetInterface("ID", langId) %> </th>
            <th><%= NEXMI.NMCommon.GetInterface("IMPORT_DATE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("ID", langId) %> lượng</th>
            <%
                //if (ViewData["Mode"].ToString() != "Print")
              {%>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
            <th width="6%"></th>
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
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            int count = 1;            
            double totalQuantity = 0;
            foreach (NEXMI.NMImportsWSI Item in WSIs)
            {
                //Item.Details = Item.Import.ImportDetailsList.ToList();
                totalQuantity += Item.Details.Sum(i => i.GoodQuantity);                    
            %>
            <tr ondblclick="javascript:fnLoadImportDetail('<%=Item.Import.ImportId%>')">
                <td width="3%"><%=count++%></td>
                <td><%=Item.Import.ImportId %></td>
                <td ><%=Item.Import.ImportDate.ToString("dd-MM-yyyy")%></td>
                <td ><%= (Item.Supplier != null)? Item.Supplier.CompanyNameInVietnamese : "" %></td>
                <td><%= NEXMI.NMCommon.GetName(Item.Import.StockId, langId)%></td>                
                <td><%= NEXMI.NMCommon.GetProductCode(Item.Details[0].ProductId) %></td>
                <td><%= NEXMI.NMCommon.GetName(Item.Details[0].ProductId, langId)%></td>
                <td><%= NEXMI.NMCommon.GetUnitNameById(Item.Details[0].UnitId) %></td>
                <td align="right"><%=Item.Details[0].GoodQuantity.ToString("N3")%></td>
                
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
            <tr>
                <td colspan='8'><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</td>
                <td align="right"><%=totalQuantity.ToString("N3") %></td>
                <td></td>
            </tr>
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