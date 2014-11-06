<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%Html.RenderAction("ImagesList", "Utilities", new { ownerId = "", type = "DOC" }); %>