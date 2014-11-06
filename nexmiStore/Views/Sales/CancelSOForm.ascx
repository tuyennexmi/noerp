<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString(), id = ViewData["Id"].ToString();
%>
<script type="text/javascript">
    function fnCancelSO() {
        $.ajax({
            type: 'POST',
            url: appPath + 'Sales/CancelSO',
            data: {
                orderId: '<%=id%>',
                message: $('#txtMessage<%=windowId%>').val()
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data == '') {
                    closeWindow('<%=windowId%>');
                    $(window).hashchange();
                }
                else {
                    alert(data);
                }
            },
            error: function () {
                fnError();
            },
            complete: function () {
                fnComplete();
            }
        });
    }
</script>
<div class="windowContent">
    <table style="width: 100%;" cellpadding="3">
        <tr>
            <td><textarea cols="1" rows="7" style="width: 98%;" id="txtMessage<%=windowId%>"></textarea></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:fnCancelSO()' class="color red button medium"><%= NEXMI.NMCommon.GetInterface("CONFIRM", langId) %></button>
        <button onclick='javascript:closeWindow("<%=windowId%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>