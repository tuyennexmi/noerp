<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('.menu').fixedMenu();
        //fnShowHideFeed();
    });

    function ShowInfo() {
        openWindow('<%=NEXMI.NMCommon.GetInterface("APP_INFO", langId)%>', "Home/AppInfo", {}, 500, 300);
    }

    function ShowHelp() {
        window.open('http://www.facebook.com/Nexmi.IBMSolution', "_blank");
    }
    
    function fnShowHideList() {
        $("#divFeed").hide();
        $.post(appPath + "Chat/RefreshList", function (data) {
            $(".clStatusImage").html('<img alt="Offline" src="<%=Url.Content("~")%>Content/UI_Images/Status-user-offline-icon.png" style="width: 16px;" />');
            if (data.length > 0) {
                for (var x in data) {
                    $("#STATUS" + data[x]).html('<img alt="Online" src="<%=Url.Content("~")%>Content/UI_Images/Status-user-online-icon.png" style="width: 16px;" />');
                }
            }
        });
        $("#divChatSidebar").toggle();
    }

    function fnShowHideFeed() {
        $("#divChatSidebar").hide();
        $.post(appPath + "Messages/Feeds", function (result) {
            if (result != "") {
                $("#divFeed").html(result);
            }
        });
        $("#divFeed").toggle();
    }
</script>
<div class="menu">
    <ul>
    <%
        string functionId = "", action = "", name = "";
        List<NEXMI.NMModulesWSI> WSIs = (List<NEXMI.NMModulesWSI>)ViewData["WSIs"];
        foreach (NEXMI.NMModulesWSI Item in WSIs)
        {
            if (langId == "vi")
                name = Item.Module.Name;
            else if (langId == "kr")
                name = Item.Module.NameInKorea;
            else if (langId == "en")
                name = Item.Module.NameInEnglish;
             
            if (Item.DefaultFunction != null)
            {
                //functionId = Item.DefaultFunction.Id;
                //action = Item.DefaultFunction.Action;
            }
            else
            {
                action = Item.Module.Action;
            }
    %>
        <li id="<%=Item.Module.Id%>" title='<%= Item.Module.Description%>'>
            <a href="javascript:fnChangeHash('<%=Item.Module.Id%>', '<%=functionId%>', '<%=action%>')"><span><%=name%></span></a>
        </li>
    <% 
        }
    %>
        <li class="itemRight">
            <a href="javascript:fnLogOut()">[<%=NEXMI.NMCommon.GetInterface("LOGOUT", langId)%>]</a>
        </li>
        <li class="itemRight">
            <a href="javascript:()">
                <img alt="" src="<%=Url.Content("~")%>Content/avatars/<%=ViewData["Avatar"]%>" style="height: 35px; vertical-align: middle;" />
                <%=ViewData["UserName"]%><span class="arrow">&nabla;</span>
            </a>
            <!-- Drop Down menu Items -->
            <ul>
                <li><a href="javascript:fnShowChangeInfo()"><%=NEXMI.NMCommon.GetInterface("MY_PROFILE", langId)%></a></li>
                <li><a href="javascript:fnShowChangePassword()"><%=NEXMI.NMCommon.GetInterface("CHANGE_PASSWORD", langId)%></a></li>
                <li><div class="mid-line"></div></li>
                <li><a href="javascript:ShowInfo()"><%=NEXMI.NMCommon.GetInterface("APP_INFO", langId)%></a></li>
                <li><a href="javascript:ShowHelp()"><%=NEXMI.NMCommon.GetInterface("HELP", langId)%></a></li>
		    </ul>
        </li>
        <li class="itemRight">
            <a href="javascript:fnShowHideList()">
                <img id="ChatIcon" alt="Chat!" src="<%=Url.Content("~")%>Content/UI_Images/WeChat-Icon.png" style="margin-top: 7px; height: 22px;" />
            </a>
        </li>
        <li class="itemRight">
            <a href="javascript:fnShowHideFeed()">
                <img id="Img1" alt="Feed!" src="<%=Url.Content("~")%>Content/UI_Images/info_header.png" style="margin-top: 7px; height: 22px;" />
            </a>
        </li>
        
    </ul>
</div>