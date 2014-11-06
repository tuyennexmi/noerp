<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table style="width: 100%" class="frmInput">
    <tr>
        <td>
            <table class="tbDetails" style="width: 100%">
                <tr>
                    <th><%= NEXMI.NMCommon.GetInterface("INVOICE_NO", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("PAID", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></th>
                </tr>
                <%
                    List<NEXMI.NMSalesInvoicesWSI> WSIs = (List<NEXMI.NMSalesInvoicesWSI>)ViewData["WSIs"];
                    if (WSIs.Count > 0)
                    {
                        double total1 = 0, total2 = 0, total3 = 0;
                        double amout = 0, rpt = 0, pay = 0;
                        foreach (NEXMI.NMSalesInvoicesWSI Item in WSIs)
                        {
                            amout = Item.Invoice.DetailsList.Sum(i => i.Amount);
                            rpt = Item.Receipts.Sum(x => x.Amount); 
                            pay = Item.Payments.Sum(x => x.PaymentAmount);
                            total1 += amout;
                            total2 += rpt - pay;
                            total3 += amout - (Item.Receipts.Sum(x => x.Amount) - Item.Payments.Sum(x => x.PaymentAmount));
                %>
                <tr>
                    <td><%=Item.Invoice.InvoiceId%></td>
                    <td><%=Item.Invoice.InvoiceDate.ToString("dd-MM-yyyy")%></td>
                    <td><%= (Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese%></td>
                    <td><%= (Item.SalesPerson == null) ? "" : Item.SalesPerson.CompanyNameInVietnamese%></td>
                    <td align="right"><%= amout.ToString("N0")%></td>
                    <td align="right"><%= (rpt-pay).ToString("N0")%></td>
                    <td align="right"><%= (amout-rpt+pay).ToString("N0")%></td>
                </tr>
                <%
                        }
                %>
                <tr style="font-weight: bold;">
                    <td colspan="4" align="right">Tổng số: </td>
                    <td align="right"><%=total1.ToString("N0")%></td>
                    <td align="right"><%=total2.ToString("N0")%></td>
                    <td align="right"><%=total3.ToString("N0")%></td>
                </tr>
                <% 
                    }    
                %>
            </table>
        </td>
    </tr>
</table>