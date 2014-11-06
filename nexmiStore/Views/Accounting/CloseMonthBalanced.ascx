<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 

    List<NEXMI.MonthlyGeneralJournals> Details = ((NEXMI.NMCloseMonthsWSI)ViewData["WSI"]).Details.OrderBy(i=>i.IssueDate).ToList();
    List<NEXMI.MonthlyGeneralJournals> begin = Details.Where(i => i.IsBegin == true).ToList();
        
%>

<table class="frmInput" style="width: 100%;">
    <tr>
        <td valign="top" id="divPrintMIC">            
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
                          foreach (var bg in begin)
                          {
                              totalDebit += bg.DebitAmount;
                          %>
                    <tr>
                        <td><%=count++ %></td>
                        <td><%= bg.IssueId%></td>
                        <td><%= bg.IssueDate.ToString("dd-MM-yyyy")%></td>
                        <td></td>
                        <td><%=bg.Descriptions%></td>
                        <td></td>
                        <td ></td>
                        <td align="right"><%=bg.DebitAmount.ToString("N3")%></td>
                    </tr>
                    <%  }
                      }
                        if (Details.Count > 0)
                        {   
                            foreach (var Item in Details)
                            {
                                if (Item.IsBegin)
                                    continue;
                                totalCredit += Item.CreditAmount;
                                totalDebit += Item.DebitAmount;
                                balance = totalDebit - totalCredit;
                    
                    %>
                    <tr>
                        <td><%=count++ %></td>
                    <%if (!string.IsNullOrEmpty(Item.RPTID))
                      {%>
                        <td><%=Item.RPTID%></td>
                    <%}
                      else
                      { %>
                        <td><%=Item.PADID%></td>
                    <%} %>
                        <td><%=Item.IssueDate.ToString("dd-MM-yyyy")%></td>
                        <td><%= NEXMI.NMCommon.GetCustomerName( Item.PartnerId )%></td>
                        <td><%=Item.Descriptions%></td>
                        <td align="right"><%=Item.DebitAmount.ToString("N3")%></td>
                        <td align="right"><%=Item.CreditAmount.ToString("N3")%></td>
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
                        <%--<td><%=DateTime.Parse(ViewData["toDate"].ToString()).ToString("dd-MM-yyyy")%></td>--%>
                        <td></td><td></td>
                        <td>Số dư cuối kỳ</td>
                        <td align="right"><%=totalDebit.ToString("N3")%></td>
                        <td align="right"><%=totalCredit.ToString("N3")%></td>
                        <td align="right"><%=balance.ToString("N3")%></td>
                    </tr>
                </tfoot>
            </table>
        </td>
    </tr>
</table>