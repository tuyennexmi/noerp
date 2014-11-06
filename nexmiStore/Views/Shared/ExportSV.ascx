<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

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
	<%--<li title="Xem dạng lưới"><button class="button <%=strCheckKanban%>" onclick="javascript:fnLoadExports('kanban')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/item-view.png" class="btSwitchView" />&nbsp;</button></li>--%>
    <li title="Xem dạng danh sách"><button class="button <%=strCheckList%>" onclick="javascript:fnLoadExports('list')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/list-view.png" class="btSwitchView" />&nbsp;</button></li>
    <li title="Xem chi tiết"><button class="button <%=strCheckDetail%>" onclick="javascript:fnLoadExportDetail('')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/detail-view.png" class="btSwitchView" />&nbsp;</button></li>
</ul>