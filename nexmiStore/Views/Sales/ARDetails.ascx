<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
    List<NEXMI.NMCustomersWSI> Customers = (List<NEXMI.NMCustomersWSI>)ViewData["Customers"];
%>

<table class="tbDetails" border=".1">
    <thead>
        <tr>          
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("RECORD_NO", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("RECORD_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <th>Đối chiếu</th>
            <th><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></th>            
        </tr>
    </thead>
    <tbody>
        <% 
            if (WSIs.Count > 0)
            {
                int count = 1;
                //double begin = 0, buyinmonth = 0, paid = 0, debit = 0;
                double totalCredit = 0, totalDebit = 0;
                string status = "";
                foreach (NEXMI.NMMonthlyGeneralJournalsWSI Item in WSIs)
                {
                    //begin = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == true)).Sum(i => i.MGJ.DebitAmount);
                    //buyinmonth = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.DebitAmount);
                    //paid = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.CreditAmount);
                    //debit = buyinmonth - paid;
                    //totalBegin += begin;
                    //totalBuy += buyinmonth;
                    totalCredit += Item.MGJ.CreditAmount;
                    totalDebit += Item.MGJ.DebitAmount;
                    //if (debit > Item.Customer.MaxDebitAmount)
                    //    status = "Vượt hạn mức";
                    //else
                    //    status = "";
        %>
        <tr>
            <td><%=count++%></td>
            <td><%= (Item.MGJ.IssueId != null)? Item.MGJ.IssueId : (Item.MGJ.PADID != null)? Item.MGJ.PADID : (Item.MGJ.SIID != null)? Item.MGJ.SIID : "Hic" %></td>
            <td><%=Item.MGJ.IssueDate%></td>
            <td><%=Item.MGJ.Descriptions%></td>
            <td><%=Item.MGJ.AccountId %></td>
            <td><%=Item.MGJ.DebitAmount.ToString("N3")%></td>
            <td><%=Item.MGJ.CreditAmount.ToString("N3")%></td>            
        </tr>
        <% 
                }%>
            <tr>
                <td colspan="5"><strong><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></strong> </td>                
                <td><%=totalDebit.ToString("N3")%></td>
                <td><%=totalCredit.ToString("N3") %></td>
            </tr>  
            <tr>                
                <% if (totalDebit > totalCredit)
                   { %>
                <td colspan="5"><strong>Phải thanh toán tiếp</strong> </td>
                <td><%=(totalDebit - totalCredit).ToString("N3")%></td>
                <%}
                   else
                   { %>
                <td colspan="6"><strong><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></strong> </td>
                <td><%=(totalCredit - totalDebit).ToString("N3")%></td>
                <%} %>
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