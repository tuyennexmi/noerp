<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="divDocuments">
    <%Html.RenderAction("DocumentList", "CMS", new { typeId = NEXMI.NMConstant.DocumentTypes.Introduce });%>
</div>
