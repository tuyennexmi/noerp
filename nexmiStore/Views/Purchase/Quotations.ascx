<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%Html.RenderAction("PurchaseOrders", new { typeId = NEXMI.NMConstant.POType.Quotation }); %>