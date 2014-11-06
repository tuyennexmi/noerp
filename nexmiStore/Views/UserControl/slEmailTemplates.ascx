<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%=Html.DropDownList(ViewData["ElementId"].ToString(), (IEnumerable<SelectListItem>)ViewData["List"], new { style = "width: 100%;" })%>