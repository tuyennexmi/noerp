<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
    NEXMI.NMCustomersWSI Supplier = (NEXMI.NMCustomersWSI)ViewData["Supplier"];
    DateTime from = DateTime.Today;
    if (ViewData["dtFrom"] != null)
        from = DateTime.Parse(ViewData["dtFrom"].ToString());
%>

<div style='height:100%; overflow:auto;'>
<%
if(ViewData["Mode"] != null && ViewData["Mode"].ToString() == "Print")
{
    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
    %>
    <div style="text-align: center;" align="center">
        <table>
            <tr>
                <td>
                    <img height="60px" alt="" src="<%=localpath%>" />
                </td>
                <td>
                    <h3><b>CÔNG NỢ NHÀ CUNG CẤP</b></h3>
                    <label><%=Supplier.Customer.CompanyNameInVietnamese%></label><br />
                    <%--<label><%=Supplier.Customer.Address%></label>--%>
                    <label><%= NEXMI.NMCommon.GetInterface("FROM", langId)%> ngày: <%=from.ToString("dd/MM/yyyy")%> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId)%> ngày: <%=DateTime.Parse(ViewData["dtTo"].ToString()).ToString("dd/MM/yyyy")%></label>    
                </td>
            </tr>
        </table>
    </div>
<%} %>
<br />
<table class="tbDetails" border=".1">
    <thead>
        <tr>          
            <th width="5%">#</th>
            <th><%= NEXMI.NMCommon.GetInterface("RECORD_NO", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("RECORD_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <%--<th>Đối chiếu</th>--%>
            <th><%= NEXMI.NMCommon.GetInterface("PAID", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></th>            
        </tr>
    </thead>
    <tbody>
        <% 
            if (WSIs.Count > 0)
            {
                int count = 1;
                double totalCredit = 0, totalDebit = 0;
                string status = "";
                foreach (NEXMI.NMMonthlyGeneralJournalsWSI Item in WSIs)
                {
                    if (Item.MGJ.IsBegin == true & Item.MGJ.IssueDate != from)
                        continue;
                    totalCredit += Item.MGJ.CreditAmount;
                    totalDebit += Item.MGJ.DebitAmount;
        %>
        <tr>
            <td><%=count++%></td>
            <td><%= (Item.MGJ.IssueId != null)? Item.MGJ.IssueId : (Item.MGJ.PADID != null)? Item.MGJ.PADID : (Item.MGJ.PIID != null)? Item.MGJ.PIID : "" %></td>
            <td><%=Item.MGJ.IssueDate.ToString("dd-MM-yyyy")%></td>
            <td><%=Item.MGJ.Descriptions%></td>
            <%--<td><%=Item.MGJ.AccountId %></td>--%>
            <td align="right"><%=Item.MGJ.DebitAmount.ToString("N3")%></td>
            <td align="right"><%=Item.MGJ.CreditAmount.ToString("N3")%></td>            
        </tr>
        <% 
                }%>
            <tr>
                <td colspan="4"><strong><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></strong> </td>                
                <td align="right"><%=totalDebit.ToString("N3")%></td>
                <td align="right"><%=totalCredit.ToString("N3") %></td>
            </tr>  
            <tr>                
                <%--<% if (totalDebit > totalCredit)
                   { %>
                <td colspan="4"><strong>Phải thanh toán tiếp</strong> </td>
                <td align="right"><%=(totalDebit - totalCredit).ToString("N3")%></td>
                <%}
                   else
                   { %>--%>
                <td colspan="5"><strong>Nợ cuối kỳ</strong> </td>
                <td align="right"><%=(totalCredit - totalDebit).ToString("N3")%></td>
                <%--<%} %>--%>
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

<%
if(ViewData["Mode"] != null && ViewData["Mode"].ToString() == "Print")
{
    Html.RenderPartial("ReportFooter");
}
%>
</div>