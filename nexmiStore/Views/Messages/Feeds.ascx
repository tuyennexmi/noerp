<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<style type="text/css">
    .feedItem
    {
        background-color:White ;
        border-bottom: 1px solid Orange;
        padding:6px;
        width:94%;
    }
</style>

<ul style='list-style-type: none; -webkit-padding-start: 0px; padding:0px;'>
<% List<NEXMI.NMMessagesWSI> wsi = (List<NEXMI.NMMessagesWSI>)ViewData["WSIs"];
    foreach (NEXMI.NMMessagesWSI Item in wsi)
    {
     %>
    <li class='feedItem'>
        <%--<p class="author"><b> <%=Item.Message.MessageName%></b></p>
        <p><%=Item.Message.Description%></p>--%>
        <%Html.RenderAction("RenderMessages", "Messages", new { msg = Item });%>
    </li>
<%} %>
</ul>