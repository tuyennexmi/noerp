<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string currentMonth = DateTime.Today.ToString("MM/yyyy");
    string previousMonth = DateTime.Today.AddMonths(-1).ToString("MM/yyyy");
    DateTime from = DateTime.Parse(ViewData["fromDate"].ToString());
    
    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
    List<NEXMI.NMMonthlyGeneralJournalsWSI> begin = WSIs.Where(i => i.MGJ.IsBegin == true & i.MGJ.IssueDate == from).ToList();
    //double beginingAmount = WSIs.Where(i => i.MGJ.IsBegin == true).Sum(i => i.MGJ.DebitAmount);
        
%>
<table id="tbCashBalances" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th width="3%">#</th>
            <th>Số chứng từ</th>
            <th><%= NEXMI.NMCommon.GetInterface("RECORD_DATE", langId) %></th>
            <th>Đối tác</th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <th>Tiền thu</th>
            <th>Tiền chi</th>
            <th>Tồn quỹ</th>
        </tr>
    </thead>
    <%
        int count = 1;
        double totalCredit = 0, totalDebit = 0, balance = 0;
            %>
    <tbody>
        <%if (begin.Count > 0)
          {
              foreach (NEXMI.NMMonthlyGeneralJournalsWSI bg in begin)
              {
                  totalDebit += bg.MGJ.DebitAmount;
              %>
        <tr>
            <td><%=count++ %></td>
            <td><%= bg.MGJ.IssueId%></td>
            <td><%= bg.MGJ.IssueDate.ToString("dd-MM-yyyy")%></td>
            <td></td>
            <td><%=bg.MGJ.Descriptions%></td>
            <td></td>
            <td ></td>
            <td align="right"><%=bg.MGJ.DebitAmount.ToString("N3")%></td>
        </tr>
        <%  }
          }
            if (WSIs.Count > 0)
            {   
                foreach (NEXMI.NMMonthlyGeneralJournalsWSI Item in WSIs)
                {
                    if (Item.MGJ.IsBegin)
                        continue;
                    totalCredit += Item.MGJ.CreditAmount;
                    totalDebit += Item.MGJ.DebitAmount;
                    balance = totalDebit - totalCredit;
                    
        %>
        <tr>
            <td><%=count++ %></td>
        <%if (!string.IsNullOrEmpty(Item.MGJ.RPTID))
          {%>
            <td><%=Item.MGJ.RPTID%></td>
        <%}
          else
          { %>
            <td><%=Item.MGJ.PADID%></td>
        <%} %>
            <td><%=Item.MGJ.IssueDate.ToString("dd-MM-yyyy")%></td>
            <td><%= NEXMI.NMCommon.GetCustomerName( Item.MGJ.PartnerId )%></td>
            <td><%=Item.MGJ.Descriptions%></td>
            <td align="right"><%=Item.MGJ.DebitAmount.ToString("N3")%></td>
            <td align="right"><%=Item.MGJ.CreditAmount.ToString("N3")%></td>
            <td align="right"><%=balance.ToString("N3")%></td>
        </tr>
        <%
                }
            }
        %>
    </tbody>
    <tfoot>
        <tr>
            <td></td><td></td>
            <td><%=DateTime.Parse(ViewData["toDate"].ToString()).ToString("dd-MM-yyyy")%></td>
            <td></td>
            <td>Số dư cuối kỳ</td>
            <td align="right"><%=totalDebit.ToString("N3")%></td>
            <td align="right"><%=totalCredit.ToString("N3")%></td>
            <td align="right"><%=balance.ToString("N3")%></td>
        </tr>
    </tfoot>
</table>
