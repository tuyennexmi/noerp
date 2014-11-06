<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    Html.RenderAction("Customers", "Sales", new { pageNum = "", keyword = "", groupId = NEXMI.NMConstant.CustomerGroups.Manufacturer, functionId = NEXMI.NMConstant.Functions.Manufacturers });
 %>