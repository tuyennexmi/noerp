<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.PurchaseInvoiceDetails> Details = (List<NEXMI.PurchaseInvoiceDetails>)Session["Details"];
    if (Details != null)
    {
        foreach (NEXMI.PurchaseInvoiceDetails Item in Details)
        {
%>
        <tr id="<%=Item.ProductId%>">
            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId)%>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
            <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
            <td><%=Item.Quantity.ToString("N3")%></td>
            <td><%=Item.Price.ToString("N3")%></td>
            <td><%=Item.Discount.ToString("N3")%></td>
            <td><%=Item.Tax.ToString("N3")%></td>
            <td align="right"><%=Item.Amount.ToString("N3")%></td>
            <td><%=Item.Description%></td>
            <td class="actionCols">
                <button type="button" class="btActions update" onclick="javascript:fnShowPurchaseInvoiceDetail('<%=Item.ProductId%>')"></button>
                <button type="button" class="btActions delete" onclick="javascript:fnRemovePurchaseInvoiceDetail('<%=Item.ProductId%>')"></button>
            </td>
        </tr>
<% 
        }
%>
        <input type='hidden' id='txtLineAmount' value='<%=Details.Sum(i => i.Amount).ToString("N3") %>' />
        <input type='hidden' id='txtTaxAmount' value='<%=Details.Sum(i => i.TaxAmount).ToString("N3") %>' />
        <input type='hidden' id='txtDiscountAmount' value='<%=Details.Sum(i => i.DiscountAmount).ToString("N3") %>' />
        <input type='hidden' id='txtTotalAmount' value='<%=Details.Sum(i => i.TotalAmount).ToString("N3") %>' />
<%
    }    
%>

