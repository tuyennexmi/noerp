<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>


<table class="frmInput" style="width: 100%;">
    <tr>
        <td valign="top" id="divPrintMIC">
            <div id="divMIC">
                <% 
                    NEXMI.NMCloseMonthsWSI WSI = (NEXMI.NMCloseMonthsWSI)ViewData["WSI"];
                    List<NEXMI.NMCustomersWSI> Suppliers = (List<NEXMI.NMCustomersWSI>)ViewData["Supplier"];
                %>

                <table class="tbDetails" border=".1" style="width: 100%;">
                    <thead>
                        <tr>          
                            <th>#</th>                
                            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_CODE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_NAME", langId) %></th>
                            <th>Số dư đầu kỳ</th>
                            <th>Mua trong kỳ</th>
                            <th>Trả trong kỳ</th>
                            <th>Nợ cuối kỳ</th>
                            
                        </tr>
                    </thead>
                    <tbody>
                        <% 
                            if (WSI.Details.Count > 0)
                            {
                                int count = 1;
                                double begin = 0, buyinmonth = 0, paid = 0, debit = 0;
                                double totalBegin =0, totalBuy = 0, totalPaid =0, totalDebit = 0;
                                string status = "";
                                foreach (NEXMI.NMCustomersWSI Item in Suppliers)
                                {
                                    begin = WSI.Details.Where(c => (c.PartnerId == Item.Customer.CustomerId & c.IsBegin == true)).Sum(i => i.CreditAmount);
                                    buyinmonth = WSI.Details.Where(c => (c.PartnerId == Item.Customer.CustomerId & c.IsBegin == false)).Sum(i => i.CreditAmount);                                            
                                    paid = WSI.Details.Where(c => (c.PartnerId == Item.Customer.CustomerId & c.IsBegin == false)).Sum(i => i.DebitAmount);
                                    debit = begin + buyinmonth - paid;
                                    if (debit == 0)
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