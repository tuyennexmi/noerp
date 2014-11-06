<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table style="width: 100%;">
    <tr>
        <td colspan="4" align="center">
            <%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %>
            <br /><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %>: <%=NEXMI.NMCommon.GetCompany().Customer.TaxCode %>
            <br /><%=NEXMI.NMCommon.GetCompany().Customer.Address %>
            <br />ĐT: <%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %>
            <br />Fax: <%=NEXMI.NMCommon.GetCompany().Customer.FaxNumber %>
            <br />Email: <%=NEXMI.NMCommon.GetCompany().Customer.EmailAddress %>
            <br />Website: <%=NEXMI.NMCommon.GetCompany().Customer.Website %>
        </td>
    </tr>
    <tr>
        <td colspan="4" align="center"><label style="font-size: 16pt; font-weight: bold;">HÓA ĐƠN BÁN LẺ</label></td>
    </tr>
    <tr>
        <td colspan="4" align="center">
            <table style="width: 100%;">
                <tr>
                    <td>Số HĐ: <%=ViewData["Id"]%></td>
                    <td align="right">Ngày: <%=ViewData["InvoiceDate"]%></td>
                </tr>
                <tr>
                    <td>Nhân viên: <%=ViewData["SalesPerson"]%></td>
                    <td align="right"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>: <%=ViewData["CustomerName"]%></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="4" align="center">-----------------------------</td>
    </tr>
    <%
        foreach (NEXMI.SalesInvoiceDetails Item in (List<NEXMI.SalesInvoiceDetails>)ViewData["Details"])
        {
    %>
    <tr>
        <td colspan="4"><%=Item.ProductId%> - <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
    </tr>
    <tr>
        <td>VAT <%=Item.Tax%>%</td>
        <td><%=Item.Quantity.ToString("N3")%>&nbsp;&nbsp;&nbsp;x&nbsp;&nbsp;&nbsp;<%=Item.Price.ToString("N3")%></td>
        <td>- <%=Item.Discount%>%</td>
        <td align="right"><%=(Item.Amount - Item.DiscountAmount).ToString("N3")%></td>
    </tr>
    <% 
        }    
    %>
    <tr>
        <td colspan="4" align="center">-----------------------------</td>
    </tr>
    <tr>
        <td colspan="2"><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</td>
        <td colspan="2" align="right"><%=ViewData["Amount"]%></td>
    </tr>
    <tr>
        <td colspan="2"><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>:</td>
        <td colspan="2" align="right"><%=ViewData["DiscountAmount"]%></td>
    </tr>
    <tr>
        <td colspan="2">Thuế:</td>
        <td colspan="2" align="right"><%=ViewData["TaxAmount"]%></td>
    </tr>
    <tr style="font-weight: bold;">
        <td colspan="2"><%= NEXMI.NMCommon.GetInterface("TOTAL_PAYMENT", langId) %>:</td>
        <td colspan="2" align="right"><%=ViewData["TotalAmount"]%></td>
    </tr>
    <tr>
        <td colspan="4" align="center">-----------------------------</td>
    </tr>
    <tr>
        <td colspan="4" align="center">
            Trân trọng cảm ơn và Hẹn gặp lại quý khách!<br />
        </td>
    </tr>
</table>
