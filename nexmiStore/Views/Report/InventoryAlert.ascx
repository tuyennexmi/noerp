<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('#stockId, #top').on('change', function () {
            change();
        });
        change();
    });
    function change() {
        var categoryId = '';
        try {
            categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        LoadContentDynamic('divInventoryAlertUC', 'Report/InventoryAlertUC', { stockId: $("#stockId").val(), categoryId: categoryId, top: $("#top").val() });
    }
    function ChangeCategory() {
        change();
    }
</script>
    
<div class="divActions">
    <%--<%Html.RenderPartial("ToolbarTimeButton"); %>--%>
</div>

<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
        <table style="width: 100%;">
            <tr>
                <td>
                    <%= NEXMI.NMCommon.GetInterface("STORE", langId) %>:
                    <select id="stockId">
                        <option value="">Tất cả</option>
                    <%
                        List<NEXMI.NMStocksWSI> stock = (List<NEXMI.NMStocksWSI>)ViewData["stock"];
                        foreach (NEXMI.NMStocksWSI item in stock)
                        {
                    %>
                        <option value="<%=item.Id%>"><%=item.Translation.Name%></option>
                    <%
                        }   
                    %>
                    </select>
                </td>
                <td><%= NEXMI.NMCommon.GetInterface("KIND", langId) %>:
                </td>
                <td><%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%>
                </td>
                <td>
                    Top:
                    <select id="top">
                        <option>5</option>
                        <option>10</option>
                        <option>25</option>
                        <option>50</option>
                    </select>
                </td>
            </tr>
        </table>
        </div>
    </div>
    <div id="divInventoryAlertUC">

    </div>
</div>
