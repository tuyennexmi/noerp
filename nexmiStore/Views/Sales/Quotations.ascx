<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%Html.RenderAction("SalesOrders", new { SOType = NEXMI.NMConstant.SOType.Quotation }); %>