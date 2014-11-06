<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMProductsWSI> product = (List<NEXMI.NMProductsWSI>)ViewData["product"];
    List<NEXMI.NMStocksWSI> stock = (List<NEXMI.NMStocksWSI>)ViewData["stock"];
    NEXMI.NMProductInStocksWSI item;    
%>
<table class="tbDetails">
    <tr>
        <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
<%
    if (stock != null)
    {
        foreach (NEXMI.NMStocksWSI its in stock)
        {
%>
        <th><%=its.Translation.Name%></th>
<%
        }
    }
%>
        <th>Tổng cộng</th>
    </tr>
    <tbody>
<%
    if (product != null && product.Count > 0)
    {
        foreach (NEXMI.NMProductsWSI itp in product)
        {
            double totalStock =0;
            double totalAllStock =0;
            List<NEXMI.NMProductInStocksWSI> lpis = (List<NEXMI.NMProductInStocksWSI>)ViewData[itp.Product.ProductId];
%>
        <tr>
            
            <td>[<%=itp.Product.ProductCode %>] <%=(itp.Translation == null) ? "" : itp.Translation.Name%></td>
<%
        foreach (NEXMI.NMStocksWSI its in stock)
        {
            totalStock = lpis.Where(p => p.PIS.StockId == its.Id).Select(p => p.PIS.BeginQuantity+p.PIS.ImportQuantity-p.PIS.ExportQuantity).FirstOrDefault();
            totalAllStock += totalStock;
%>
            <td><%=totalStock%></td>
<%
        }
%>
            <td><%=totalAllStock %></td>
        </tr>
<%        
        }
    }
    else
    {
%>
        <tr><td colspan="<%=stock.Count + 2%>" align="center"><h4>Không có dữ liệu.</h4></td></tr>
<%
    }
%>
    </tbody>
</table>