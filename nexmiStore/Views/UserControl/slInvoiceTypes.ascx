<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%=Html.DropDownList(ViewData["ElementId"].ToString(), (IEnumerable<SelectListItem>)ViewData["List"])%>