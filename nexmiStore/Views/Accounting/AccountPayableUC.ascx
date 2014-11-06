<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%if(ViewData["Mode"].ToString() == "Print")
  {
      String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
 %>
    <div style="text-align: center;" align="center">
        <table>
            <tr>
                <td width="40%"><img height="60px" alt="" src="<%=localpath%>" /></td>
                <td><h3><b>CÔNG NỢ NHÀ CUNG CẤP</b></h3>
                    <label><%= NEXMI.NMCommon.GetInterface("FROM", langId) %> ngày: <%=DateTime.Parse(ViewData["dtFrom"].ToString()).ToString("dd/MM/yyyy") %> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %> ngày: <%=DateTime.Parse(ViewData["dtTo"].ToString()).ToString("dd/MM/yyyy")%></label>
                </td>
            </tr>
        </table>
    </div>
    <br />
<%} %>
<table class="frmInput" style="width: 100%;">
    <tr>
        <td valign="top" id="divPrintMIC">
            <div id="divMIC">
                <% 
                    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
                    List<NEXMI.NMCustomersWSI> Suppliers = (List<NEXMI.NMCustomersWSI>)ViewData["Supplier"];
                    DateTime from = DateTime.Parse(ViewData["dtFrom"].ToString());
                %>

                <table class="tbDetails" border=".1" style="width: 100%;">
                    <thead>
                        <tr>          
                            <th>#</th>                
                            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_CODE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_NAME", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("BALANCE_AT_BEGINNING", langId)%></th>
                            <th><%= NEXMI.NMCommon.GetInterface("BUY_IN_PERIOD", langId)%></th>
                            <th><%= NEXMI.NMCommon.GetInterface("PAID", langId)%></th>
                            <th><%= NEXMI.NMCommon.GetInterface("DEBIT", langId)%></th>
                            <%if (ViewData["Mode"].ToString() != "Print")
                              {%>
                            <th><%= NEXMI.NMCommon.GetInterface("MAX_DEBT", langId) %></th>
                            <%--<th>Trạng thái nợ</th>--%>
                            <th></th>
                            <%} %>

                        </tr>
                    </thead>
                    <tbody>
                        <% 
                            if (WSIs.Count > 0)
                            {
                                int count = 1;
                                double begin = 0, buyinmonth = 0, paid = 0, debit = 0;
                                double totalBegin =0, totalBuy = 0, totalPaid =0, totalDebit = 0;
                                string status = "";
                                foreach (NEXMI.NMCustomersWSI Item in Suppliers)
                                {
                                    begin = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == true & c.MGJ.IssueDate == from)).Sum(i => i.MGJ.CreditAmount);
                                    buyinmonth = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.CreditAmount);                                            
                                    paid = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.DebitAmount);
                                    debit = begin + buyinmonth - paid;
                                    if (begin == 0 && buyinmonth == 0 && paid == 0)
                                        continue;
                                    totalBegin += begin;
                                    totalBuy += buyinmonth;
                                    totalPaid += paid;
                                    totalDebit += debit;
                                    if (debit > Item.Customer.MaxDebitAmount)
                                        status = "#CC3300";
                                    else
                                        status = "";
                        %>
                        <tr style='color:<%=status %>'>
                            <td width="4%"><%=count++%></td>
                            <td width="10%"><%=Item.Customer.Code%></td>
                            <td><%=Item.Customer.CompanyNameInVietnamese%></td>
                            <td align="right"><%=begin.ToString("N3")%></td>
                            <td align="right"><%=buyinmonth.ToString("N3") %></td>
                            <td align="right"><%=paid.ToString("N3") %></td>
                            <td align="right"><%=debit.ToString("N3") %></td>
                            <%if (ViewData["Mode"].ToString() != "Print")
                              {%>
                            <td align="right"><%=Item.Customer.MaxDebitAmount.ToString("N3")%></td>
                            <%--<td><%=status%></td>--%>
                            <td class="actionCols">
                                <a href="javascript:fnLoadAPDetail('<%=Item.Customer.CustomerId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                <a href="javascript:fnCreatePaymentRequire('<%=Item.Customer.CustomerId%>')"><img alt="Đề nghị thanh toán" title="Đề nghị thanh toán" src="<%=Url.Content("~")%>Content/UI_Images/billing-icon.png" /></a>
                                <a href="javascript:fnPrintAPDetails('<%=Item.Customer.CustomerId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("PRINT", langId) %> chi tiết công nợ NCC" title="<%= NEXMI.NMCommon.GetInterface("PRINT", langId) %> chi tiết công nợ NCC" src="<%=Url.Content("~")%>Content/UI_Images/16print-icon.png" /></a>
                            </td>
                            <%} %>
                        </tr>
                        <% 
                                }%>
                                <tr>
                                <td colspan="3"><strong><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></strong> </td>
                                <td align="right"><%=totalBegin.ToString("N3") %></td>
                                <td align="right"><%=totalBuy.ToString("N3") %></td>
                                <td align="right"><%=totalPaid.ToString("N3") %></td>
                                <td align="right"><%=totalDebit.ToString("N3") %></td>
                                </tr>  
                            <%}
                            else { 
                        %>
                        <tr><td colspan="10" align="center"><h4>Không có dữ liệu.</h4></td></tr>
                        <%
                            }    
                        %>
                    </tbody>
                </table>
            </div>
        </td>
    </tr>
</table>