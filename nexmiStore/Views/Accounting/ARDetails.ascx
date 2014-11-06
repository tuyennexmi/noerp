<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
    DateTime from = DateTime.Today;
    if(ViewData["dtFrom"] != null)
        from = DateTime.Parse(ViewData["dtFrom"].ToString());
%>
<div style='height:100%; overflow:auto;'>

<table class="tbDetails" border=".1">
    <thead>
        <tr>          
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("RECORD_NO", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("RECORD_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <%--<th>Đối chiếu</th>--%>
            <th><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></th>            
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
            <td><%= (Item.MGJ.IssueId != null)? Item.MGJ.IssueId : (Item.MGJ.RPTID != null)? Item.MGJ.RPTID : (Item.MGJ.SIID != null)? Item.MGJ.SIID : "" %></td>
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
                <% if (totalDebit > totalCredit)
                   { %>
                <td colspan="4"><strong>Phải thanh toán tiếp</strong> </td>
                <td align="right"><%=(totalDebit - totalCredit).ToString("N3")%></td>
                <%}
                   else
                   { %>
                <td colspan="5"><strong><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></strong> </td>
                <td align="right"><%=(totalCredit - totalDebit).ToString("N3")%></td>
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

<%
if(ViewData["Mode"] != null && ViewData["Mode"].ToString() == "Print")
{
    Html.RenderPartial("ReportFooter");
}
%>

</div>