<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });
</script>

<table style="width: 100%;">
    <tr>
        <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = "", customerName = "" });%></td>
        <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text" id="dtFrom" value="" style="width: 89px"/></td>
        <td><%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text" id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 89px"/></td>
        <td><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %>:<%Html.RenderAction("slStatuses", "UserControl", new { elementId = "slStatus", objectName = ViewData["ObjectName"], current = "" });%></td>
    </tr>
</table>