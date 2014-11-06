<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    List<NEXMI.NMTypesWSI> Languages = (List<NEXMI.NMTypesWSI>)ViewData["Languages"];
    string mode = ViewData["Mode"].ToString();
    string ownerId = "", windowId = "";
    try
    {
        ownerId = ViewData["Data"].ToString();
    }
    catch { }
    try {
        windowId = ViewData["WindowId"].ToString();
    }
    catch { }
%>
<script type="text/javascript">
    $(function () {
        $('.tabsLanguage<%=mode%>').jqxTabs({ theme: theme, keyboardNavigation: false });
    });
</script>
<div class="tabsLanguage<%=mode%>">
    <ul>
        <%
            string styleCSS = "";
            for (int i = Languages.Count - 1; i >= 0; i--)
            {
                styleCSS = "";
                if (i == Languages.Count - 1)
                {
                    styleCSS = "margin-left: 25px;";
                }
        %>
        <li style="<%=styleCSS%>"><%=Languages[i].Name%></li>
        <%
            }
        %>
    </ul>
    <% 
        for (int i = Languages.Count - 1; i >= 0; i--)
        { 
    %>
    <div style="padding: 10px;"><%Html.RenderAction("TranslationForm", "UserControl", new { mode = mode, ownerId = ownerId, languageId = Languages[i].Id, windowId = windowId });%></div>
    <%
        }
    %>
</div>