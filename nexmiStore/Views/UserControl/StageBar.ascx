<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    string current = ViewData["Current"].ToString();
    List<NEXMI.Stages> WSIs = (List<NEXMI.Stages>)ViewData["Stages"];    
%>
<ul class="breadcrumbs-one">
    <% 
        string strCurrent;
        foreach (NEXMI.Stages Item in WSIs)
        {
            if (!Item.Folded)
            {
                strCurrent = "";
                if (current == Item.StageId)
                {
                    strCurrent = "currentStatus";
                }
    %>
    <li class="<%=strCurrent%>"><a><%=Item.StageName%></a></li>
    <%
            }
        }
    %>       
</ul>