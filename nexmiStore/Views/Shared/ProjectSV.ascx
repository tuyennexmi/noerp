<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string strCheckKanban = "", strCheckList = "", strCheckDetail = "";
    try
    {
        string viewType = ViewData["ViewType"].ToString();
        switch (viewType)
        {
            case "list": strCheckList = "selected"; break;
            case "detail": strCheckDetail = "selected"; break;
            default: strCheckKanban = "selected"; break;
        }
    }
    catch { }
%>
<ul class="button-group">
	<li title="Lưới"><button class="button <%=strCheckKanban%>" onclick="javascript:LoadContent('', 'Projects/ProjectKanban')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/item-view.png" class="btSwitchView" />&nbsp;</button></li>
    <li title="Danh sách"><button class="button <%=strCheckList%>" onclick="javascript:LoadContent('', 'Projects/Projects')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/list-view.png" class="btSwitchView" />&nbsp;</button></li>
    <li title="<%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %>"><button class="button <%=strCheckDetail%>" onclick="javascript:fnLoadProjectDetail('')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/detail-view.png" class="btSwitchView" />&nbsp;</button></li>
</ul>