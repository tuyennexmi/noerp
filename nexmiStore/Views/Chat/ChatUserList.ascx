<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    var myTimer;
    var catchUserDataList = "";
    var CWCount = 0;
    var getMessageTimer;
    $(document).ready(function () {
        // định kỳ 5s
        getMessageTimer = setInterval(function () {
            // lấy tin nhắn về
            $.post(appPath + "Chat/GetMessage", function (result) {
                if (result != "") {
                    var msgarr = result.split("<;>");
                    var i;
                    var t = 0;
                    for (i = 0; i < msgarr.length - 1; i++) {
                        var toUser = msgarr[i].split("<:>");
                        var $dlg = $("#" + toUser[0] + "CW");
                        if ($dlg.parents('.ui-dialog:visible').length == 0) {
                            OpenChatWindows(toUser[0]);
                            catchUserDataList = result;
                            setMsg();
                            break;
                        }
                        var fromUser = $("#" + toUser[0] + "LI").attr("name");
                        $("#" + toUser[0] + "CON").append("<strong>" + fromUser + ": </strong>" + toUser[1] + "<br>");
                    }
                }
            });
            $.post(appPath + "Messages/Feeds", function (result) {
                if (result != "") {
                    $("#divFeed").html(result);
                }
            });

        }, 5000);
        //Auto refresh list
        setInterval(function () {
            $.post(appPath + "Chat/RefreshList", function (data) {
                $(".clStatusImage").html('<img alt="Offline" src="<%=Url.Content("~")%>Content/UI_Images/Status-user-offline-icon.png" style="width: 16px;" />');
                if (data.length > 0) {
                    for (var x in data) {
                        $("#STATUS" + data[x]).html('<img alt="Online" src="<%=Url.Content("~")%>Content/UI_Images/Status-user-online-icon.png" style="width: 16px;" />');
                    }
                }
            });
        }, 60000);
    });

    // h thị tin nhắn nhận được trong khi dialog chưa mở
    function setMsg() {
        myTimer = setInterval(function () {
            var msgarr = catchUserDataList.split("<;>");
            var i;
            var t = 0;
            for (i = 0; i < msgarr.length - 1; i++) {
                var toUser = msgarr[i].split("<:>");
                var $dlg = $("#" + toUser[0] + "CW");
                if ($dlg.parents('.ui-dialog:visible').length != 0) {
                    clearInterval(myTimer);
                    var fromUser = $("#" + toUser[0] + "LI").attr("name");
                    $("#" + toUser[0] + "CON").append("<strong>" + fromUser + ": </strong>" + toUser[1] + "<br>");
                }
            }
        }, 500);
    }

    // mở một cửa sổ chat
    function OpenChatWindows(ToUser) {
        var $dlg = $("#" + ToUser + "CW");
        if ($dlg.parents('.ui-dialog:visible').length == 0) {
            //if ($dlg.dialog('isOpen') != true) {
            //if ($dlg.is(':visible') != true) {
            var rPos = CWCount * 246;
            $("#" + ToUser + "CW").load(appPath + "Chat/OpenChatWindows?ToUserId=" + ToUser).dialog({
                position: [100, 100],
                modal: false,
                resizable: false,
                draggable: true,
                outline: 1,
                width: 229,
                height: 312,
                position: [rPos, 'bottom'],
                closeOnEscape: true,
                hide: { effect: 'drop' },
                open: function (event, ui) { $("#" + ToUser + "MSG").focus(); },
                focus: function (event, ui) { $("#" + ToUser + "MSG").focus(); },
                close: function (event, ui) {
                    CWCount--;
                }
            }).fadeIn(5000);
            // tăng số đếm cửa sổ chat
            CWCount++;
        }
    }

    // gửi tin nhắn
    function SendMessage(ToUser) {
        var obj = $("#" + ToUser + "MSG");
        var msg = obj.val();
        if (msg != '') {
            $.post(appPath + "Chat/SaveMessage", { ToUser: ToUser, Message: msg }, function (result) {
                var obj2 = $("#" + ToUser + "CON");
                obj2.append("<strong>Tôi: </strong>" + msg + "<br>").scrollTop(obj2.height());
                obj.val('');
            });
        }
    }
</script>

<%    
    List<NEXMI.NMCustomersWSI> UserList = (List<NEXMI.NMCustomersWSI>)ViewData["WSIs"];
%>
<div id="divChatSidebar" class="nmChatSidebar">
    <div>
        <table style="width:100%">
            <tr>
                <td style="padding:6px; width:85%">Trò chuyện trực tuyến</td>
                <td style="width:15%">
                    <a title="Ẩn danh sách" aria-label="Ẩn danh sách" href="javascript:fnShowHideList()" style="float:right">
                        <img alt="" src="<%=Url.Content("~")%>Content/UI_Images/hide_right.png" />
                    </a>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <input type="text" placeholder="Tìm kiếm" style='width:277px' />
    </div>
    <table id="tbUsers" class="noneborder" cellpadding="3" style="width: 100%">                
<%
    for (int i = 0; i < UserList.Count; i++)
    {
%>
        <tr>
            <td>
                <table style="width: 100%" class="noneborder">
        <% 
                if (UserList[i].Customer.CustomerId.ToString() != Page.User.Identity.Name)
                {
        %>
                    <tr class="nmChatUser">
                        <td>                            
                            <a id="<%=UserList[i].Customer.CustomerId + "LI"%>" name="<%=UserList[i].Customer.CompanyNameInVietnamese%>" href="javascript:OpenChatWindows('<%=UserList[i].Customer.CustomerId%>')">
                                <img alt="" src="<%=Url.Content("~")%><%=(string.IsNullOrEmpty(UserList[i].Customer.Avatar)) ? "Content/avatars/personal.png" : "Content/avatars/" + UserList[i].Customer.Avatar%>" style="height: 28px; vertical-align: middle;" />
                                <%=UserList[i].Customer.CompanyNameInVietnamese%> 
                            </a>                            
                        </td>
                        <td style="width: 28px" id="STATUS<%=UserList[i].Customer.CustomerId%>" class="clStatusImage">
        <%
                    if (SessionChecker.CheckExist(UserList[i].Customer.CustomerId))
                    {
        %>
                            <%--hiện thị ảnh biểu tượng online--%>
                            <img alt="Online" src="<%=Url.Content("~")%>Content/UI_Images/Status-user-online-icon.png" style="width: 16px;" /><%
                    }
                    else
                    { 
        %>
                            <%--hiện thị ảnh biểu tượng offline--%>
                            <img alt="Offline" src="<%=Url.Content("~")%>Content/UI_Images/Status-user-offline-icon.png" style="width: 16px;" />
        <%
                    }
        %>  
                        </td>                        
                    </tr>
            <%
                }
            %>
                </table>
                <div id="<%=UserList[i].Customer.CustomerId + "CW"%>" title="<%=UserList[i].Customer.CompanyNameInVietnamese%>" style="overflow:hidden" >
                </div>
            </td>
        </tr>
<%
    }
%>
</table>
</div>

<style type="text/css">

.nmChatSidebar {
    -webkit-background-clip: padding-box;
    background-color: #f0f8f0;
    border: 1px solid rgba(0, 10, 0, .4);
    -webkit-box-shadow: inset 2px 0 2px -2px #aaff80;
    height: 96%;
    overflow: auto;
    position: fixed !important;
    right: 0;
    top: 39px;    
    width: 278px;
    z-index: 300;
}

.nmChatUser:hover
{
    background-color:blueviolet;
}

div.auto
{
    /*background-color:#5A9D5E;
    height: 239px;*/
    height: 90%;
    overflow: auto;
    border:0;
}

.nmChatWindow
{
    width:100%;
    height: 100%;
    background-color: White;
    padding:0px; 
    border:0px; 
    margin:0px; 
    overflow:hidden;
}

</style>