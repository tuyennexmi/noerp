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
    <li title="Xem dạng danh sách"><button class="button <%=strCheckList%>" onclick="javascript:fnReturnPIList()"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/list-view.png" class="btSwitchView" />&nbsp;</button></li>
    <li title="Xem chi tiết"><button class="button <%=strCheckDetail%>" onclick="javascript:fnLoadPIDetail('')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/detail-view.png" class="btSwitchView" />&nbsp;</button></li>
</ul>