<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">

    function fnLoadARDetail(customerId) {
        openWindow('Chi tiết công nợ của khách hàng', 'Sales/ARDetails', { customerId: customerId }, 900, 500);
    }

</script>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <% 
                            if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.AccountReceivable))
                            {    
                        %>
                        <button class="button red" onclick="javascript:fnCloseInventory()">Kết sổ</button>
                        <% 
                            }
                            if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.AccountReceivable))
                            {    
                        %>
                        <button class="button" onclick="javascript:fnPrint()"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                        <% 
                            }
                        %>
                        <button class="button" onclick="javascript:fnReloadData()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    </div>
                </td>
                <td align="right">
                        
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <%--<table>
                    <tr>
                        <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>
                        <td style="padding-left: 5px;"><%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%></td>
                    </tr>
                </table>--%>
            </div>
        </div>
        <div class='divContentDetails'>
        <table class="frmInput">
            <tr>
                <td valign="top" id="divPrintMIC">
                    <%--<h4>TÌNH HÌNH CÔNG NỢ PHẢI THU</h4>--%>
                    <div id="divMIC">
                        <%--<%Html.RenderAction("ARDetails", "Sales", new { customerId = "" });%>--%>
                        <% 
                            List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
                            List<NEXMI.NMCustomersWSI> Customers = (List<NEXMI.NMCustomersWSI>)ViewData["Customers"];
                        %>

                        <table class="tbDetails" border=".1">
                            <thead>
                                <tr>          
                                    <th>#</th>                
                                    <th>Mã <%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
                                    <th>Tên <%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
                                    <th>Số dư đầu kỳ</th>
                                    <th>Mua trong kỳ</th>
                                    <th>Trả trong kỳ</th>
                                    <th>Nợ cuối kỳ</th>
                                    <th><%= NEXMI.NMCommon.GetInterface("MAX_DEBT", langId) %></th>
                                    <th>Trạng thái nợ</th>
                                    <th></th>
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
                                        foreach (NEXMI.NMCustomersWSI Item in Customers)
                                        {
                                            begin = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == true)).Sum(i => i.MGJ.DebitAmount);
                                            buyinmonth = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.DebitAmount);
                                            paid = WSIs.Where(c => (c.MGJ.PartnerId == Item.Customer.CustomerId & c.MGJ.IsBegin == false)).Sum(i => i.MGJ.CreditAmount);
                                            debit = buyinmonth - paid;
                                            totalBegin += begin;
                                            totalBuy += buyinmonth;
                                            totalPaid += paid;
                                            totalDebit += debit;
                                            if (debit > Item.Customer.MaxDebitAmount)
                                                status = "Vượt hạn mức";
                                            else
                                                status = "";
                                %>
                                <tr>
                                    <td><%=count++%></td>
                                    <td><%=Item.Customer.Code%></td>
                                    <td><%=Item.Customer.CompanyNameInVietnamese%></td>
                                    <td><%=begin.ToString("N3")%></td>
                                    <td><%=buyinmonth.ToString("N3") %></td>
                                    <td><%=paid.ToString("N3") %></td>
                                    <td><%=debit.ToString("N3") %></td>
                                    <td><%=Item.Customer.MaxDebitAmount.ToString("N3")%></td>
                                    <td><%=status%></td>
                                    <td class="actionCols">
                                        <a href="javascript:fnLoadARDetail('<%=Item.Customer.CustomerId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                    </td>
                                </tr>
                                <% 
                                        }%>
                                        <tr>
                                        <td colspan="3"><strong><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></strong> </td>
                                        <td><%=totalBegin.ToString("N3") %></td>
                                        <td><%=totalBuy.ToString("N3") %></td>
                                        <td><%=totalPaid.ToString("N3") %></td>
                                        <td><%=totalDebit.ToString("N3") %></td>
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
                    </div>
                </td>
            </tr>
        </table>
        </div>
    </div>
</div>