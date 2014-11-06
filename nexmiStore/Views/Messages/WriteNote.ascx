<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $(document).mousedown(function (e) {
            var container = document.getElementById('commentContainer');
            if (container != null) {
                if ($.contains(container, e.target) == false) {
                    var content = $('#txtLogContent').val();
                    if (content == '') {
                        $('#divComment').hide();
                        $('#btnComment').show();
                        $('#btnSendMsg').show();
                        $('#btnSendSMS').show();
                    }
                }
            }
        });
        var IsSendMsg;
        var IsSendSMS;
        $('#divComment').hide();

        $('#btnComment').click(function () {
            $('#btnComment').hide();
            $('#btnSendMsg').hide();
            $('#btnSendSMS').hide();
            $('#divComment').show();
            $('#txtHint').html('Lưu một ghi chú về tài liệu này!');
            IsSendMsg = false;
        });

        $('#btnSendMsg').click(function () {
            $('#btnComment').hide();
            $('#btnSendMsg').hide();
            $('#btnSendSMS').hide();
            $('#divComment').show();
            $('#txtHint').html('Gửi tin nhắn đến người tạo và quản lý nội dung này!');
            IsSendMsg = true;
            IsSendSMS = false;
        });

        $('#btnSendSMS').click(function () {
            $('#btnComment').hide();
            $('#btnSendMsg').hide();
            $('#btnSendSMS').hide();
            $('#divComment').show();
            $('#txtHint').html('Gửi tin nhắn SMS đến khách hàng này!');
            IsSendSMS = true;
            IsSendMsg = false;
        });

        $('#btnSend').click(function () {
            var title = $('#txtTitle').val();
            var content = $('#txtLogContent').val();
            var ownerId = $('#Owner').val();
            var sendTo = '';
            var msgType = '';
            if (IsSendMsg == true) {    //trường hợp gửi tin nhắn
                sendTo = $('#SendMsgTo').val();
                msgType = 'MES01';
            }
            else if (IsSendSMS == true) {    //trường hợp gửi tin nhắn SMS
                sendTo = $('#SendSMSTo').val();
                msgType = 'MES05';
            }
            if (content != '') {
                $.post(appPath + "Messages/LogANote", { title: title, msgContent: content, ownerId: ownerId, sendTo: sendTo, type: msgType }, function (data) {
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

<div class="comment">
    <button id="btnSendMsg" class="button" title='Gửi tin nhắn cho người tạo nội dung này!'><%= NEXMI.NMCommon.GetInterface("SEND_MESSAGE", langId) %></button>
    <button id="btnSendSMS" class="button" title='Gửi SMS đến khách hàng này!'><%= NEXMI.NMCommon.GetInterface("SEND_SMS", langId)%></button>
    <button id="btnComment" class="button" title='Ghi một ghi chú cho nội dung này!'><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></button>
    
    <div id="divComment">
        <table style="width:100%">
            <tr>
                <td></td>
                <td><div id="txtHint"></div></td>
            </tr>
            <%--<tr>
                <td>Người nhận:</td>
                <td></td>
            </tr>--%>
            <tr>
                <td><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %>: </td>
                <td>
                    <input type="text" id="txtTitle" />                    
                </td>
            </tr>
            <tr>
                <td style="width:16%; vertical-align:top"><%= NEXMI.NMCommon.GetInterface("CONTENT", langId) %>: </td>
                <td style="width:84%"><textarea id="txtLogContent" cols="1" rows="4" style="width: 90%;"></textarea></td>
            </tr>
        </table>        
        
        <button id="btnSend" class="button"><%= NEXMI.NMCommon.GetInterface("SEND_MESSAGE", langId)%></button>
    </div>
</div>