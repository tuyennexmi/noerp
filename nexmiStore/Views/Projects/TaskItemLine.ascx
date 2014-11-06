<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    List< NEXMI.TaskDetails> Details = (List< NEXMI.TaskDetails>)Session["TaskDetails"];
%>

<table class="tbDetails">
    <tr>
        <th>#</th>
        <th>Công việc</th>
        <th>Thời gian</th>
        <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
        <th>Người thực hiện</th>
        <th></th>
    </tr>
    <tbody id="tbodyWork">
<% 
double totalTimeSpent = 0;
if (Details != null)
{
foreach (NEXMI.TaskDetails Item in Details)
{
    totalTimeSpent += Item.TimeSpent;
    Item.TaskId = Item.OrdinalNumber.ToString();
%>
    <tr id="row<%=Item.OrdinalNumber%>">
        <td><%=Item.No%></td>
        <td><%=Item.Name%></td>
        <td><%=Item.TimeSpent.ToString("N3")%></td>
        <td><%=Item.StartDate.ToString("dd/MM/yyyy HH:mm")%></td>
        <td><%=NEXMI.NMCommon.GetCustomerName(Item.UserId)%></td>
        <td>
            <a href="javascript:fnPopupWorkDialog('<%=Item.No%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>
            <a href="javascript:fnRemoveWork('<%=Item.No%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>
        </td>
    </tr>
<%
}

}
%>       
    </tbody>
    <tr>
        <td>Tổng thời gian</td><td></td>
        <td><b id="lbTotalTimeSpent"><%=totalTimeSpent.ToString("N3")%></b></td>
        <td></td><td></td><td></td>
    </tr>
</table>