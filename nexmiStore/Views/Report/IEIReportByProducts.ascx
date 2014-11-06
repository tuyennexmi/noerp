<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">

    function fnRefreshImport() {
        $("#txtKeywordPIS").val("");
        fnReloadImport();
    }

    function fnReloadPISList() {
        LoadContentDynamic('PISGrid', 'Stocks/ProductInStockList', {
            pageNum: '',
            stockId: $('#slStocks').val(),
            keyword: $('#txtKeywordPIS').val(),
            categoryId: document.getElementsByName('cbbCategories')[0].value
        });
    }
</script>

<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("BEGIN_AMOUNT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("IMPORTED", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("EXPORTED", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("END_AMOUNT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("VALUE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("ABOVE_LIMIT", langId) %></th>
            <%  //if (ViewData["Mode"].ToString() != "Print")
              {%>
            <th><%= NEXMI.NMCommon.GetInterface("LOWER_LIMIT", langId) %></th>
            <%--<th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>--%>
            <%} %>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMProductInStocksWSI> WSIs = (List<NEXMI.NMProductInStocksWSI>)ViewData["WSIs"];
        
        if (WSIs.Count > 0)
        {
            int count = 1;
            double totalQuantity = 0;
            string status = "";
            
            foreach (NEXMI.NMProductInStocksWSI Item in WSIs)
            {
                totalQuantity = Item.PIS.BeginQuantity+Item.PIS.ImportQuantity-Item.PIS.ExportQuantity;
                status = "";
                if (totalQuantity > Item.PIS.MaxQuantity)
                {
                    status = "#CC3300";
                }
                else if (totalQuantity < Item.PIS.MinQuantity)
                {
                    status = "#0033CC";
                }
            %>
            <tr style='color:<%=status %>'>
                <td width="3%"><%=count++%></td>
                <td><%= NEXMI.NMCommon.GetName(Item.PIS.StockId, langId)%></td>                
                <td>[<%= NEXMI.NMCommon.GetProductCode(Item.PIS.ProductId) %>] <%= NEXMI.NMCommon.GetName(Item.PIS.ProductId, langId)%></td>
                <td><%= NEXMI.NMCommon.GetUnitNameById(Item.PIS.ProductId) %></td>
                <td align="right"><%= Item.PIS.BeginQuantity.ToString("N3")%></td>
                <td align="right"><%= Item.PIS.ImportQuantity.ToString("N3")%></td>
                <td align="right"><%= Item.PIS.ExportQuantity.ToString("N3")%></td>
                <td align="right"><%=totalQuantity.ToString("N3")%></td>
                <td align="right"><%=(totalQuantity*Item.PIS.CostPrice).ToString("N3")%></td>
                <%
                //if (ViewData["Mode"].ToString() != "Print")
                  {%>
                <td align="right"><%=Item.PIS.MaxQuantity.ToString("N3")%></td>
                <td align="right"><%=Item.PIS.MinQuantity.ToString("N3")%></td>
                <%--<td ><%= status  %></td>--%>
                <%} %>
            </tr>
    <%
            }
            %>
    <%
        }
    %>
    </tbody>
    <tfoot>
    
    </tfoot>
</table>