<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/TransferProducts.js" type="text/javascript"></script>
<script type='text/javascript'>
    function fnChanged() {

    }
</script>
<div id="divImport">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordImport" name="txtKeywordImport" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <%
                        string functionImport = NEXMI.NMConstant.Functions.TransferProduct;
                        string stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
                        //Kiểm tra quyền Insert
                        if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
                        {
                    %>
                    <%--<button onclick="javascript:fnLoadTransferForm()" class="button">Chuyển kho</button>--%>
                    <button onclick="javascript:fnExportProduct('', '', '', '<%=NEXMI.NMConstant.ExportType.Transfer%>')" class="color red button"><%= NEXMI.NMCommon.GetInterface("STOCK_MOVE", langId) %></button>
                    <% 
                        }                        
                    %>
                    <button class="button" onclick="javascript:fnRefresh('<%=ViewData["EXTypeId"]%>', '<%=ViewData["IMTypeId"]%>')"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                </td>
                <td align="right" style="float: right;"></td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">            
                <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "Imports" }); %>
            </div>
        </div>
        <div class='divContentDetails'>
        <table style="width: 100%">
            <tr>
                <td>
                    <h3>Danh sách xuất</h3>
                    <div id="ExportGrid">
                        <%  Html.RenderAction("ExportList", "Stocks", new { typeId = NEXMI.NMConstant.ExportType.Transfer });%>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <h3>Danh sách nhập</h3>
                    <div id="ImportGrid">
                        <%  Html.RenderAction("ImportList", "Stocks", new { typeId = NEXMI.NMConstant.ImportType.Transfer });%>
                    </div>
                </td>
            </tr>
        </table>
        </div>
    </div>
</div>
