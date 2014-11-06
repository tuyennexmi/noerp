<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMMessagesWSI Msg = (NEXMI.NMMessagesWSI)ViewData["Msg"];
    String title = "", avatar = "personal.png", name = "";
    if (Msg.Message.TypeId == NEXMI.NMConstant.MessageTypes.SysLog)
    {
        title = " đã " + Msg.Message.MessageName + " " + Msg.Message.Owner;
    }
    else
    {
        title = "đã nói về " + Msg.Message.Owner;
    }
    try
    {
        name = Msg.CreatedBy.CompanyNameInVietnamese;
        if (!string.IsNullOrEmpty(Msg.CreatedBy.Avatar))
            avatar = Msg.CreatedBy.Avatar;
    }
    catch { }
%>

<div class="logItem">
    <table style="width: 100%;">
        <tr>
            <td valign="top" style="width: 46px;">
                <img alt="" src="<%=Url.Content("~")%>Content/avatars/<%=avatar%>" style="width: 38px;" />
            </td>
            <td valign="top">                
        <%
            if (Msg.Message.TypeId != NEXMI.NMConstant.MessageTypes.SysLog)
            {
        %>
                <label> <b> <%=Msg.Message.MessageName%></b></label>
                <p><%=Msg.Message.Description%></p>
                <%
            }
            else
            {
                if (!string.IsNullOrEmpty(Msg.Message.Description))
                {
                    string[] txt = Regex.Split(Msg.Message.Description, ";");
        %>
                    <ul>
        <%
                    if (txt.Length > 1)
                    {
                        for (int i = 0; i < txt.Length - 1; i++)
                        {
        %>
                        <li><%=txt[i]%></li>
                <%          
                        }
                    }
                    else
                    { 
        %>
                        <li><%=Msg.Message.Description.Replace("\n", "<br/>")%></li>
        <%
                    }
        %>
                    </ul>
        <%
                }
            }
        %>                
            </td>
        </tr>
        <tr>
            <td colspan='2' align="right">
    <%  
        if (Msg.Message.TypeId == NEXMI.NMConstant.MessageTypes.Message)
        {
    %>
            <a id="reply" href="javascript:reply('<%=Msg.Message.CreatedBy %>', '<%=Msg.Message.MessageId %>', '<%=Msg.Message.Owner %>')"> Trả lời</a>
                <%--<a id="A1">Tô màu</a> <br />
                <a id="A2">Việc cần làm</a>--%>
    <%  
            if (Msg.Message.IsRead == false)
            {   
        %>
                    | <a id="readMsg" href="javascript:read('<%=Msg.Message.MessageId %>')"> Đã đọc</a> <br />
        <%  
            }
        }
    %>
            </td>
        </tr>
        <tr>
            <td colspan='3'>
                <p class="author"><a href="javascript:()"><%=name%></a> <%=title %><label title="<%=Msg.Message.CreatedDate%>">, khoảng <%=NEXMI.NMCommon.ToRelativeDate(Msg.Message.CreatedDate)%>.</label> </p>
            </td>
        </tr>
    </table>
</div>

<script type="text/javascript">

    function reply(userId, msgId, ownerId) {
        openWindow("Soạn tin nhắn", "Messages/ReplyMessage", { ownerId: ownerId, sendTo: userId, replyTo: msgId }, 880, 400);
    }

    function read(msgId) {
        $.post(appPath + 'Common/SetStatus', { objectName: 'Messages', id: msgId, status: true, typeId: '', description: '' }, function (data) {
            if (data != '') {
                alert(data);
            }
        });
    }
    
</script>