<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('.PISInput').autoNumeric('init', { vMax: '1000000000000' });
        $('.PISInput').on('change', function () {
            var elm = $(this);
            var stockId = $('#slStocks').val();
            var productId = elm.parent().parent().attr('id');
            var goodQty = $('#txtGoodQty' + productId).autoNumeric('get');
            var price = $('#txtPrice' + productId).autoNumeric('get');
            $("#txtTotalAmount" + productId).val(fnNumberFormat(goodQty * price));            
            //var badQty = $('#txtBadQty' + productId).autoNumeric('get');
            $.post(appPath + 'Stocks/AddToList', { stockId: stockId, productId: productId, goodQty: goodQty, price: price}, function (data) {
                if (data != '') {
                    alert(data);
                    elm.val('').focus();
                }
            });
        });
    });

</script>
<table style="width: 100%" class="tbTemplate">
    <tr>
        <td>
            <table style="width: 100%" id="tbUpdateStock" class="tbDetails">
                <thead>
                    <tr>
                        <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %> </th>
                        <th><%= NEXMI.NMCommon.GetInterface("RECORD_AMOUNT", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("REAL_AMOUNT", langId)%></th>
                        <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                        <%--<th>SL xấu</th>--%>
                        <%--<th>SL xấu thực tế</th>--%>
                    </tr>
                </thead>
                <tbody id="rows">
                <%
                    List<NEXMI.ImportDetails> list = new List<NEXMI.ImportDetails>();
                    if (Session["List"] != null)
                        list = (List<NEXMI.ImportDetails>)Session["List"];
                    NEXMI.ImportDetails obj;
                    List<NEXMI.NMProductInStocksWSI> WSIs = (List<NEXMI.NMProductInStocksWSI>)ViewData["WSIs"];
                    if (WSIs != null)
                    {
                        string goodQty = "";    //, badQty = "";
                        string price = "", amount = "";
                        foreach (NEXMI.NMProductInStocksWSI Item in WSIs)
                        {
                            goodQty = ""; //badQty = "";
                            obj = list.Select(x => x).Where(x => x.ProductId == Item.PIS.ProductId && x.StockId == Item.PIS.StockId).FirstOrDefault();
                            if (obj != null)
                            {
                                goodQty = obj.GoodQuantity.ToString("N3");
                                price = obj.Price.ToString("N3");
                                amount = obj.Amount.ToString("N3");                                    
                                //badQty = obj.BadQuantity.ToString("N3");
                            }
                %>
                    <tr id="<%=Item.PIS.ProductId%>">
                        <td>[<%=Item.PIS.StockId %>] <%= NEXMI.NMCommon.GetName(Item.PIS.StockId, langId)%></td>
                        <td>[<%=NEXMI.NMCommon.GetProductCode(Item.PIS.ProductId) %>] <%= NEXMI.NMCommon.GetName(Item.PIS.ProductId, langId)%></td>
                        <td style="width: 100px;" align="right"><%=Item.PIS.BeginQuantity.ToString("N3")%></td>
                        <td style="width: 100px;">
                            <input type="hidden" id="txtProductId<%=Item.PIS.ProductId%>" value="<%=Item.PIS.ProductId%>" />
                            <input type="hidden" id="txtStockId<%=Item.PIS.ProductId%>" value="<%=Item.PIS.StockId%>" />
                            <input type="text" class="PISInput" value="<%=goodQty%>" id="txtGoodQty<%=Item.PIS.ProductId%>" style="width: 150px; height: 12px; text-align: right;" />
                        </td>
                        <td style="width: 100px;">                            
                            <input type="text" class="PISInput" value="<%=price %>" id="txtPrice<%=Item.PIS.ProductId%>" style="width: 150px; height: 12px; text-align: right;" />
                        </td>
                        <td style="width: 100px;">
                            <input type="text" class="PISInput" value="<%=amount %>" id="txtTotalAmount<%=Item.PIS.ProductId%>" style="width: 200px; height: 12px; text-align: right;" readonly="readonly"/>
                        </td>
                        <%--<td style="width: 100px;" align="right"><%=Item.PIS.BadProductInStock.ToString("N3")%></td>--%>
                        <%--<td style="width: 100px;">
                            <input type="text" class="PISInput" value="<%=badQty%>" id="txtBadQty<%=Item.PIS.ProductId%>" style="width: 100px; height: 12px; text-align: right;" />
                        </td>--%>
                    </tr>
                <%
                        }
                    }
                %>
                </tbody>
            </table>
        </td>
    </tr>
</table>