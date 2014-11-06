<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<fieldset>
    <legend>Danh sách đơn đặt hàng bán</legend>
    <%Html.RenderAction("SalesOrderList"); %>
</fieldset>
<%
    if (NEXMI.NMCommon.GetSetting("HAVE_LEASEORDER")) 
    {
%>
<fieldset>
    <legend>Danh sách hợp đồng thuê</legend>
    <%Html.RenderAction("LeaseOrderList", "Sales", new { from = new DateTime(2000, 01, 01), to = new DateTime(2014, 12, 31) }); %>
</fieldset>
<%  } %>