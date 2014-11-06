<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    function fnChanged() {
        try {
            fnReloadPIS();
            $("#PISGrid").jqxGrid('gotopage', 0);
        } catch (err) { }
    }
</script>

<select id="slStocks" onchange="javascript:fnChanged()">
    <option value="">Tất cả</option>
<%
    String stockId = "";
    if (ViewData["StockId"] != null)
    {
        stockId = ViewData["StockId"].ToString();
    }
    ViewData["StockId"] = stockId;
    String selectItem = "";
    NEXMI.NMStocksWSI StockWSI = new NEXMI.NMStocksWSI();
    NEXMI.NMStocksBL StockBL = new NEXMI.NMStocksBL();
    StockWSI.Mode = "SRC_OBJ";
    List<NEXMI.NMStocksWSI> StockWSIs = StockBL.callListBL(StockWSI);
    foreach (NEXMI.NMStocksWSI Item in StockWSIs)
    {
        selectItem = "";
        if (Item.Id == stockId)
        {
            selectItem = "selected";
        }
%>
    <option value="<%=Item.Id%>" <%=selectItem%>><%=Item.Translation.Name%></option>
<%
    }
%>
</select>