<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">

    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $("#dtFrom, #dtTo").change(function () {
            fnReloadPISList();
        });

        var t;
        $("#txtKeywordPIS").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadPISList();
            }, 1000);
        });
    });

    function fnRefreshImport() {
        $("#txtKeywordPIS").val("");
        fnReloadImport();
    }

    function fnReloadPISList() {
        LoadContentDynamic('PISGrid', 'Stocks/ProductInStockList', {
            pageNum: '',
            stockId: $('#slStocks').val(),
            keyword: $('#txtKeywordPIS').val(),
            categoryId: document.getElementsByName('cbbCategories')[0].value,
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
        });
    }

    function ChangeCategory() {
        fnReloadPISList();
    }
</script>

<div id="divProductInStocks">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">
                    <input id="txtKeywordPIS" name="txtKeywordPIS" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <button class="button" onclick="javascript:fnReloadPISList()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                </td>
                <td align="right" style="float: right;">
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <table>
                    <tr>
                        <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ViewData["StockId"] });%></td>
                        <td><%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%></td>
                        <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text" id="dtFrom" value="" style="width: 89px"/></td>
                        <td><%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text" id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 89px"/></td>
                    </tr>
                </table>
            </div>
        </div>
        <div class='divContentDetails'>
            <div id="PISGrid">
                <%--<%Html.RenderAction("ProductInStockList", new { stockId = ViewData["StockId"] });%>--%>
            </div>
        </div>
    </div>
</div>
