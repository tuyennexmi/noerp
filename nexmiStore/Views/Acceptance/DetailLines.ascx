<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string mode = ViewData["Mode"].ToString();
List<NEXMI.AcceptanceDetails> Details = (List<NEXMI.AcceptanceDetails>)Session["Details"];
foreach (NEXMI.AcceptanceDetails Item in Details)
{
    string unitName =NEXMI.NMCommon.GetUnitNameById(Item.UnitId);
%>
    <tr id="<%=Item.ProductId%>">
        <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
        <td><%= unitName%></td>
        <td><%=Item.Quantity.ToString("N3")%></td>
        <td><%=Item.Price.ToString("N3")%></td>
        <td><%=Item.Discount.ToString("N3")%></td>
        <td><%=Item.Tax.ToString("N3")%></td>
        <td align="right"><%=Item.Amount.ToString("N3")%></td>
        <td><%=Item.Description%></td>
    <%if (mode != "Detail")
        { %>
        <td>
            <button type="button" class="btActions update" onclick="javascript:fnShowDetailLineForm('<%=Item.ProductId%>')"></button>
            <button type="button" class="btActions delete" onclick="javascript:fnRemoveDetailLine('<%=Item.ProductId%>')"></button>
        </td>
    <%} %>
    </tr>
<%
}
%>
<input type='hidden' id='txtLineAmount' value='<%=Details.Sum(i => i.Amount).ToString("N3") %>' />
<input type='hidden' id='txtTaxAmount' value='<%=Details.Sum(i => i.TaxAmount).ToString("N3") %>' />
<input type='hidden' id='txtDiscountAmount' value='<%=Details.Sum(i => i.DiscountAmount).ToString("N3") %>' />
<input type='hidden' id='txtTotalAmount' value='<%=Details.Sum(i => i.TotalAmount).ToString("N3") %>' />

<script type="text/javascript">

    $(function () {
        $('#lbAmount').html($('#txtLineAmount').val());
        $('#lbTotalVATRate').html($('#txtTaxAmount').val());
        $('#lbTotalDiscount').html($('#txtDiscountAmount').val());
        $('#lbTotalAmount').html($('#txtTotalAmount').val());
    });
</script>