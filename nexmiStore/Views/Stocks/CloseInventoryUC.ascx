<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMMonthlyInventoryControlWSI> WSIs = (List<NEXMI.NMMonthlyInventoryControlWSI>)ViewData["WSIs"];
%>
<table class="tbDetails" border=".1">
    <thead>
        <tr>          
            <th>#</th>                
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th>Giá</th>
            <th><%= NEXMI.NMCommon.GetInterface("BEGIN_AMOUNT", langId) %></th>
            <th>SL nhập</th>
            <th>Xuất giao</th>
            <th>Xuất hủy</th>
            <th><%= NEXMI.NMCommon.GetInterface("END_AMOUNT", langId) %></th>
            <th>Xuất bán</th>
            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>            
        </tr>
    </thead>
    <tbody>
        <% 
            if (WSIs.Count > 0)
            {
                int i = 1;
                double price = 0;
                double totalAmount = 0;
                double amount = 0;
                foreach (NEXMI.NMMonthlyInventoryControlWSI Item in WSIs)
                {
                    NEXMI.NMProductsBL BL = new NEXMI.NMProductsBL();
                    NEXMI.NMProductsWSI WSI = new NEXMI.NMProductsWSI();
                    WSI.Mode = "SEL_OBJ";
                    WSI.Product = new NEXMI.Products();
                    WSI.Product.ProductId = Item.MIC.ProductId;
                    WSI = BL.callSingleBL(WSI);
                    if (WSI.WsiError == "")
                    {
                        //vatRate = WSI.Product.VATRate.ToString("N3");
                        //unitName = (WSI.Unit == null) ? "" : WSI.Unit.Name;
                        //discount = (WSI.PriceForSales == null) ? "0.00" : WSI.PriceForSales.Discount.ToString("N3");
                        price = (WSI.PriceForSales == null) ? 0.00 : WSI.PriceForSales.Price;
                    }
                    amount = Item.MIC.SalesQuantity * price;
                    totalAmount += amount;
        %>
        <tr>
            <td><%=i++%></td>
            <td><%=NEXMI.NMCommon.GetName(Item.MIC.ProductId, langId)%></td>
            <td><%=price %></td>
            <td><%=Item.MIC.BeginQuantity.ToString("N3")%></td>
            <td><%=Item.MIC.ImportQuantity.ToString("N3")%></td>
            <td><%=Item.MIC.ExportQuantity.ToString("N3")%></td>
            <td><%=Item.MIC.OutQuantity.ToString("N3")%></td>
            <td><%=Item.MIC.EndQuantity.ToString("N3")%></td>
            <td><%=Item.MIC.SalesQuantity.ToString("N3")%></td>
            <td><%=amount.ToString("N3") %></td>            
        </tr>
        <% 
                }%>
              <tr>
                <td colspan="9"><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></td>
                <td><%=totalAmount.ToString("N3")%></td>
              </tr>  
            <%}
            else { 
        %>
        <tr><td colspan="9" align="center"><h4>Không có dữ liệu.</h4></td></tr>
        <%
            }    
        %>
    </tbody>
</table>