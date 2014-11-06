<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    string elementId = NEXMI.NMCommon.RandomString(8, true);
    string current = ViewData["Current"].ToString();
    List<NEXMI.NMStatusesWSI> WSIs = (List<NEXMI.NMStatusesWSI>)ViewData["WSIs"];
    string stName = "";
    if (ViewData["Clickable"] == "")
    {
%>
<ul class="breadcrumbs-one">
<% 
if (WSIs.Where(s => s.Status.Id == current).FirstOrDefault().Status.OrdinalNumber == 0)
{
    if (Session["Lang"].ToString() == "vi")
        stName = "Đã hủy";
    else if (Session["Lang"].ToString() == "en")
        stName = "Cancelled";
    else if (Session["Lang"].ToString() == "kr")
        stName = "Cancelled";
%>      
            <li class="currentStatus"><a><%=stName %></a></li>
<%
}
else
{
    WSIs.RemoveAt(0);
    string strCurrent;
    foreach (NEXMI.NMStatusesWSI Item in WSIs)
    {
        strCurrent = "";
        if (current == Item.Id)
        {
            strCurrent = "currentStatus";
        }
        if (Session["Lang"].ToString() == "vi")
            stName = Item.Status.Name;
        else if (Session["Lang"].ToString() == "en")
            stName = Item.Status.English;
        else if (Session["Lang"].ToString() == "kr")
            stName = Item.Status.Korea;
    %>
                <li class="<%=strCurrent%>"><a><%=stName%></a></li>
    <%
    }
}
    %>       
</ul>
<% 
}
    else
    { 
%>
<ul id="<%=elementId%>" class="breadcrumbs-one" style="cursor: pointer;">
<% 
if (WSIs.Where(s => s.Status.Id == current).FirstOrDefault().Status.OrdinalNumber == 0)
{
    if (Session["Lang"].ToString() == "vi")
        stName = "Đã hủy";
    else if (Session["Lang"].ToString() == "en")
        stName = "Cancelled";
    else if (Session["Lang"].ToString() == "kr")
        stName = "Cancelled";
%>      
            <li class="currentStatus"><a><%=stName %></a></li>
<%
}
else
{
    WSIs.RemoveAt(0);
    string strCurrent;
    
    foreach (NEXMI.NMStatusesWSI Item in WSIs)
    {
        strCurrent = "";
        if (current == Item.Status.Id)
        {
            strCurrent = "currentStatus";
        }
        if (Session["Lang"].ToString() == "vi")
            stName = Item.Status.Name;
        else if (Session["Lang"].ToString() == "en")
            stName = Item.Status.English;
        else if (Session["Lang"].ToString() == "kr")
            stName = Item.Status.Korea;
    %>
            <li id="<%=elementId + Item.Status.Id%>" class="<%=strCurrent%>"><a href="javascript:fnStatusItemClick('<%=elementId%>', '<%=Item.Status.Id%>', '<%=ViewData["OwnerId"]%>', '<%=ViewData["TableName"]%>')"><%=stName%></a></li>
    <%
}
    %>       
</ul>
<%
}
    }
%>