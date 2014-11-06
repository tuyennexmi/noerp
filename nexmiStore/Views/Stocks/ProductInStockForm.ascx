<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        validateForm("formProductInStock");
        $("#txtProductName").focus();
        $("#txtProductName").autocomplete({
            source: productData,
            select: function (event, ui) {
                $("#txtProductId").val(ui.item.data);
            }
        });
    });
</script>
<% 
    String productId = "", productName = "", goodQuantity = "", badQuantity = "";
    if (ViewData["ProductId"] != null)
    {
        productId = ViewData["ProductId"].ToString();
        ArrayList objs = (ArrayList)Session["ImportDetails"];
        if (objs != null)
        {
            ArrayList item;
            for (int i = 0; i < objs.Count; i++)
            {
                item = (ArrayList)objs[i];
                if (item[0].ToString() == productId)
                {
                    productName = item[1].ToString();
                    goodQuantity = item[2].ToString();
                    badQuantity = item[3].ToString();
                    break;
                }
            }
        }
    }    
%>
<div>
    <form id="formProductInStock" action="">
        <table style="width: 100%">
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
                <td>
                    <input id="txtProductId" type="hidden" value="<%=productId%>" />
                    <input id="txtProductName" type="text" class="required" value="<%=productName%>" />
                    <input type="button" value="Chọn" onclick="javascript:fnShowProducts()" />&nbsp;
                </td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("GOOD_QUANTITY", langId) %></td>
                <td>
                    <input id="txtGoodQuantity" type="text" class="required" value="<%=goodQuantity%>" onkeypress="return fnOnlyNumber(event);" onkeyup="this.value = fnAddCommas(this.value)" />&nbsp;
                </td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BAD_QUANTITY", langId) %></td>
                <td>
                    <input id="txtBadQuantity" type="text" value="<%=badQuantity%>" onkeypress="return fnOnlyNumber(event);" onkeyup="this.value = fnAddCommas(this.value)" />&nbsp;
                </td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                <td>
                    <select id="slStocks">
                    <%
                        String stockId = "";
                        if(ViewData["StockId"]!= null)
                        {
                            stockId = ViewData["StockId"].ToString();
                        }
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
                        <option value="<%=Item.Id%>" <%=selectItem%>><%=Item.Name%></option>
                    <%
                        }
                    %>
                    </select>
                </td>
            </tr>
        </table>
    </form>
</div>