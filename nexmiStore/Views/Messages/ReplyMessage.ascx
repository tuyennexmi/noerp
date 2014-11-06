<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#txtLogContent").jqte();
        $('#btnSend').click(function () {
            var title = $('#txtTitle').val();
            var content = $('#txtLogContent').val();
            var ownerId = $('#txtOwnerId').val();
            var sendTo = $('#txtSendMsgTo').val();
            var replyTo = $('#txtReplyMsgTo').val();
            
            if (content != '') {
                $.post(appPath + "Messages/LogANote", { title: title, msgContent: content, ownerId: ownerId, sendTo: sendTo, replyTo: replyTo }, function (data) {
                    if (data == "") {
                        $.post(appPath + "Messages/Logs", { ownerId: ownerId }, function (data) {
                            $('#divLogs').html('');
                            $('#divLogs').html(data);
                        });
                    }
                    else {
                        alert('Đã có lỗi xảy ra nên không thể lưu nội dùng đã gửi!' + data);
                    }
                });
            }
            else {
                alert('Vui lòng nhập nội dung cần gửi!');
            }
        });
    });
</script>

<div>
    <input type="hidden" id ="txtSendMsgTo" value="<%=ViewData["SendTo"]%>" />
    <input type="hidden" id ="txtOwnerId" value="<%=ViewData["OwnerId"]%>" />
    <input type="hidden" id ="txtReplyMsgTo" value="<%=ViewData["ReplyTo"]%>" />
    
    <div id="divComment">
        <table style="width:100%">
            <tr>
                <td></td>
                <td><div id="txtHint"></div></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
                <td>
                    <input type="text" id="txtTitle" />                    
                </td>
            </tr>
            <tr>
                <td style="width:10%; vertical-align:top" class="lbright"><%= NEXMI.NMCommon.GetInterface("CONTENT", langId) %> </td>
                <td style="width:90%"><textarea id="txtLogContent" cols="1" rows="6" style="width: 90%;"></textarea></td>
            </tr>
        </table>        
        
        <%--<button id="btnSend" class="button">Gửi</button>--%>
    </div>    
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button id="btnSend" class="button medium">Gửi</button>
        <%--<button onclick='javascript:closeWindow()' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>--%>
    </div>
</div>