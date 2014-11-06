<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<input type="hidden" value='<%= ViewData["ViewType"] %>' id ='productViewType' />

<table id="tbReceipts" class="tbDetails" width="100%">
    <thead>
        <tr>
            <th>#</th>
            <th>ID</th>
            <th>Mã sản phẩm</th>
            <th>Mã vạch</th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
            <th>Thuế (%)</th>
            <th>Ngưng bán?</th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <th></th>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMProductsWSI> WSIs = (List<NEXMI.NMProductsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            string functionReceipt = NEXMI.NMConstant.Functions.Products;
            int i = 1;
            foreach (NEXMI.NMProductsWSI Item in WSIs)
            {   
    %>
        <tr ondblclick="javascript:fnLoadProductDetail('<%=Item.Product.ProductId%>')">
            <td style="width: 15px;"><%=i++%></td>
            <td style="width: 120px;"><div><%=Item.Product.ProductId%></div></td>
            <td style="width: 120px;"><div><%=Item.Product.ProductCode%></div></td>
            <td style="width: 100px;"><div><%=Item.Product.BarCode%></div></td>
            <td style="width: 120px;"><div><%=Item.Translation.Name%></div></td>
            <td><%=Item.Unit.Name%></td>
            <td><%=Item.Product.VATRate%></td>
            <td style="width: 120px;" align="right"><input type='checkbox' value='<%= Item.Product.Discontinued%>' /></td>
            <td><%=Item.Product.Description%></td>
            <td class="actionCols">
                <a href="javascript:fnLoadProductDetail('<%=Item.Product.ProductId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
    <%          
                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                {    
    %>
                <a href="javascript:fnShowProductForm('<%=Item.Product.ProductId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
    <% 
                }
    %>
    <% 
                if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                {
%>
                <a href="javascript:fnDeleteProduct('<%=Item.Product.ProductId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
    <% 
                }
            }
    %>
            </td>
        </tr>
    <%
        }
        else
        { 
    %>
        <tr><td colspan="10" align="center"><h3>Không có dữ liệu.</h3></td></tr>
    <%
        }
    %>
    </tbody>    
</table>
<script type="text/javascript">
    $(function () {
        $('#pagination-Product').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>