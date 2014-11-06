<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {

    });
</script>
<%     
    List<NEXMI.NMPricesForSalesInvoiceWSI> PriceWSIs = (List<NEXMI.NMPricesForSalesInvoiceWSI>)ViewData["PriceWSIs"];
%>
<h4>Lịch sử thay đổi giá của sản phẩm: <%= ViewData["productId"] + " - " + ViewData["productName"]%></h4>
<table style="width: 100%" id="tbPricesForSalesInvoice">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
            <th>Giá</th>
            <th>Người nhập</th>
        </tr>
    </thead>
    <tbody>
        <%
            int i = 1;
            if (PriceWSIs != null)
            {
                foreach (NEXMI.NMPricesForSalesInvoiceWSI Item in PriceWSIs)
                {
        %>
        <tr>
            <td><%=i++%></td>
            <td><%=Item.PriceForSale.DateOfPrice.ToString("dd-MM-yyyy")%></td>
            <td><%=Item.PriceForSale.Price.ToString("N3")%></td>
            <td><%=Item.PriceForSale.CreatedBy%></td>
        </tr>
        <%
}
            }
        %>
    </tbody>
</table>