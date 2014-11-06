<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('#slStocks, #cbbCategories').on('change', function () {
            fnReloadStock();
        });
        fnReloadStock();
    });

    function fnReloadStock() {
        var categoryId = '';
        try {
            categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        LoadContentDynamic('divInventoryNormsUC', 'Stocks/InventoryNormsUC', { stockId: $('#slStocks').val(), categoryId: categoryId });
    }

    function fnAddToList() {

    }

    function fnUpdateStock() {
        $.ajax({
            type: 'POST',
            url: appPath + 'Stocks/UpdateMMProductInStock',
            //data: { productIds: productIds.toString() },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data == "") {
                    fnSuccess();
                }
                else {
                    alert(data);
                }
                fnReloadStock();
            },
            error: function () {
                fnError();
            },
            complete: function () {
                fnComplete();
            }
        });
    }
</script>
<div id="divUpdateStock">
    <div class="divActions">
        <table style="width: 100%">
            <tr>
                <td>&nbsp;</td>
                <td align="right"></td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <button onclick="javascript:fnReloadStock()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                        <button onclick="javascript:fnUpdateStock()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button onclick="javascript:history.back();" class="button">Thoát</button>
                    </div>
                </td>
                <td></td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <table>
                <tr>
                    <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks" });%></td>
                    <td><%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%></td>
                </tr>
            </table>
        </div>
        <div class='divContentDetails'>
            <div id="divInventoryNormsUC"></div>
        </div>
    </div>
</div>
