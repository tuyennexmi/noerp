<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    string strCheckList = "", strCheckDetail = "";
    try
    {
        string viewType = ViewData["ViewType"].ToString();
        switch (viewType)
        {
            case "detail": strCheckDetail = "selected"; break;
            default: strCheckList = "selected"; break;
        }
    }
    catch { }
%>
<ul class="button-group">
    <li title="Xem dạng danh sách"><button class="button <%=strCheckList%>" onclick="javascript:fnLoadSIs('list')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/list-view.png" class="btSwitchView" />&nbsp;</button></li>
    <li title="Xem chi tiết"><button class="button <%=strCheckDetail%>" onclick="javascript:fnLoadSIDetail('')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/detail-view.png" class="btSwitchView" />&nbsp;</button></li>
</ul>