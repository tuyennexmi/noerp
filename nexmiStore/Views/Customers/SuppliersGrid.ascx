<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    Html.RenderAction("Customers", "Sales", new { pageNum = "", keyword = "", groupId = NEXMI.NMConstant.CustomerGroups.Supplier, functionId = NEXMI.NMConstant.Functions.Suppilers });
 %>