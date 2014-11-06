<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    
    function fnReloadExport(page, type) {
        LoadContentDynamic('ExportGrid', 'Stocks/ExportList', {
            pageNum: page,
            typeId: type,
            status: $('#slStatus').val(),
            keyword: $('#txtKeywordExport').val(),
            from: $('#dtFrom').val(),
            to: $('#dtTo').val(),
            partnerId: document.getElementsByName('cbbCustomers')[0].value
        });
    }

    function fnRefreshExport(type) {
        $("#txtExportKeyword").val("");
        fnReloadExport('', type);
    }
</script>


<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th>#</th>
            <th width="10%"><%= NEXMI.NMCommon.GetInterface("ID", langId) %> </th>
            <th><%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
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
                totalQuantity += Item.Details.Sum(i => i.Quantity);                    
            %>
            <tr ondblclick="javascript:fnLoadExportDetail('<%=Item.Export.ExportId %>')">
                <td width="3%"><%=count++%></td>
                <td><%=Item.Export.ExportId %></td>
                <td ><%=Item.Export.ExportDate.ToString("dd-MM-yyyy")%></td>
                <td ><%= (Item.Customer != null)? Item.Customer.CompanyNameInVietnamese : "" %></td>
                <td><%= NEXMI.NMCommon.GetName(Item.Export.StockId, langId)%></td>                
                <td><%= Item.Details.Count > 0? NEXMI.NMCommon.GetProductCode(Item.Details[0].ProductId) : "" %></td>
                <td><%= Item.Details.Count > 0 ? NEXMI.NMCommon.GetName(Item.Details[0].ProductId, langId) : ""%></td>
                <td><%= Item.Details.Count > 0 ? NEXMI.NMCommon.GetUnitNameById(Item.Details[0].UnitId) : ""%></td>
                <td align="right"><%= Item.Details.Count > 0 ? Item.Details[0].Quantity.ToString("N3") : ""%></td>
                
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
        $('#pagination-Export').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>