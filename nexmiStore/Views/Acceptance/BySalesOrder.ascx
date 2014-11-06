<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    List<NEXMI.NMAcceptanceWSI> WSIs = (List<NEXMI.NMAcceptanceWSI>)ViewData["WSIs"];
%>

<table id="tbReceipts" class="tbDetails">
    <thead>
        <tr>
            <th>#</th>
            <th width="12%"><%= NEXMI.NMCommon.GetInterface("ACCEPTANCE_NO", langId)%></th>
            <th width="12%"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
            <th width="15%"><%= NEXMI.NMCommon.GetInterface("REFERENCE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
            <th width="12%"><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
            <th width="8%"></th>
        </tr>
    </thead>
    <tbody>
    <%          
double total = 0;
if (WSIs.Count > 0)
{
    int count = 1;
    double amount = 0;
    NEXMI.Acceptance Item;
    foreach (NEXMI.NMAcceptanceWSI Acc in WSIs)
    {
        Item = Acc.Acceptance;
        amount = Item.TotalAmount;
        total += amount;
    %>
        <tr ondblclick="javascript:fnLoadAcceptanceDetail('<%=Item.Id%>')">
            <td ><%=count++%></td>
            <td ><%=Item.Id%></td>
            <td ><%=Item.AcceptanceDate.ToString("dd-MM-yyyy")%></td>
            <td ><%=Item.Reference%></td>
            <td ><%=Item.Description%></td>
            <td align="right" ><%= amount.ToString("N3")%></td>
            <td >
                <a href="javascript:fnLoadAcceptanceDetail('<%=Item.Id%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
            </td>
        </tr>
    <%
    }
}
    %>
        <tr>
            <td colspan='5'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
            <td align="right"><b><%=total.ToString("N3")%></b></td>
            <td align="right"></td>
        </tr>
    </tbody>
</table>

<script type="text/javascript">

    function fnLoadAcceptanceDetail(id) {
        LoadContent('', 'Acceptance/Details?id=' + id);
    }

</script>