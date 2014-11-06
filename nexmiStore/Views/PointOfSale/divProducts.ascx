<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script type="text/javascript">
    $(function () {
        $('img').on('error', function () {
            $(this).attr('src', appPath + 'Content/UI_Images/noimage.png');
        });
    });
</script>
<% 
    List<NEXMI.NMProductsWSI> WSIs = (List<NEXMI.NMProductsWSI>)ViewData["WSIs"];
    string productName = "", strDisc = "", avatar;
    foreach (NEXMI.NMProductsWSI Item in WSIs)
    {
        strDisc = ""; avatar = "Content/UI_Images/noimage.png";
        productName = (Item.Translation == null) ? "" : Item.Translation.Name;
        if (Item.PriceForSales != null && Item.PriceForSales.Discount > 0)
        {
            strDisc = "<div class='toprightPOS1'>-" + Item.PriceForSales.Discount + "%</div>";
        }
        if (Item.Images.Count > 0)
        {
            var ImageDefault = Item.Images.Where(i => i.IsDefault == true).FirstOrDefault();
            if (ImageDefault == null)
            {
                ImageDefault = Item.Images[0];
            }
            avatar = "uploads/_thumbs/" + ImageDefault.Name;
        }
%>
    <div class="live-tile product-itemPOS" onclick='javascript:fnItemClick("<%=Item.Product.ProductId%>", "<%=NEXMI.NMCryptography.base64Encode(productName)%>", <%=(Item.PriceForSales == null) ? 0 : Item.PriceForSales.Price%>, 1, <%=Item.Product.VATRate%>, <%=(Item.PriceForSales == null) ? 0 : Item.PriceForSales.Discount%>, "+")'>
        <div>
            <%=strDisc%>
            <div class="toprightPOS"><%=(Item.PriceForSales == null) ? "0" : String.Format("{0:0,0}", Item.PriceForSales.Price)%><%=(Item.Unit == null) ? "" : "/" + Item.Unit.Name%></div>
            <img class="full" src="<%=Url.Content("~")%><%=avatar%>" alt="" />
            <span class="tile-title" style="background-color: rgba(0,0,0,0.5); font-size: 110%;"><%=productName%></span>
        </div>
    </div>
<%
    }
%>