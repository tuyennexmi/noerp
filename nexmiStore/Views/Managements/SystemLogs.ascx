<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>



<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                
            </td>
            <td align="right"></td>
        </tr>
    </table>
</div>
<div class="divContent">
<%--<div class="divStatus">
    <div class="divButtons"></div>
    <div class="divStatusBar"></div>
</div>--%>
    <% Html.RenderAction("LogsTracking", "Messages", new { ownerId = "" });%>
</div>
    