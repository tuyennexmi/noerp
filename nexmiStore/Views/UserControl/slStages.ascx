<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%=Html.DropDownList("slStages" + ViewData["ElementId"].ToString(), (IEnumerable<SelectListItem>)ViewData["Stages"])%>