<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $('#slStocks').prepend('<option value="" selected="selected">Tất cả</option>');
        $('#slStocks, #dtFrom, #dtTo').on('change', function () {
            GetContent();
        });
        GetContent();
    });

    function fnSetDate(fromDate, toDate) {
        $('#dtFrom').val(fromDate);
        $('#dtTo').val(toDate);
        GetContent();
    }

    function GetContent() {
        var categoryId = '';
        try {
            categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        LoadContentDynamic('divSalesByProductContent', 'Report/SalesByProductUC', {
            fromDate: $('#dtFrom').val(), toDate: $('#dtTo').val(),
            stockId: $("#slStocks").val(), categoryId: categoryId
        });
    }

    function ChangeCategory() {
        GetContent();
    }
</script>

<div class="divActions">
    <%Html.RenderPartial("ToolbarTimeButton"); %>
</div>
<div class="divContent">
    <div class="divStatus">
    <div class="divButtons">
    <table>
        <tr>
            <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks" });%></td>
            <td><%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%></td>
            <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 89px"/></td>
            <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" style="width: 89px"/></td>
        </tr>
    </table>
    </div>
    </div>
    <div id="divSalesByProductContent"></div>
</div>