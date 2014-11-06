<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>


<% 
    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
%>

<table class="frmInput">
    <tr>
        <td>
            <table class="tbDetails" border=".1">
            <thead>
                <tr>          
                    <th width="5%">#</th>
                    <th><%= NEXMI.NMCommon.GetInterface("RECORD_DATE", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("REFERENCE_ACC", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("IMPORT", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("EXPORT", langId) %></th>            
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
                            totalCredit += Item.MGJ.ExportQuantity;
                            totalDebit += Item.MGJ.ImportQuantity;
                %>
                        <tr>
                            <td><%=count++%></td>
                            <td><%=Item.MGJ.IssueDate.ToString("dd-MM-yyyy")%></td>
                            <td><%=Item.MGJ.Descriptions%></td>
                            <td><%=Item.MGJ.AccountId %></td>
                            <td align="right"><%=Item.MGJ.ImportQuantity.ToString("N3")%></td>
                            <td align="right"><%=Item.MGJ.ExportQuantity.ToString("N3")%></td>            
                        </tr>
                <% 
                        }%>
                        <tr>
                            <td colspan="4"><strong><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></strong> </td>                
                            <td align="right"><%=totalDebit.ToString("N3")%></td>
                            <td align="right"><%=totalCredit.ToString("N3") %></td>
                        </tr>
                    <%}
                    else 
                    { 
                %>
                        <tr><td colspan="9" align="center"><h4>Không có dữ liệu.</h4></td></tr>
                <%
                    }    
                %>
            </tbody>
            </table>

        </td>
    </tr>
</table>