<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        function fnChange() {
            //if (NMValidator("formChangePassword")) {
            var oldPass = $("#txtOldPass").val();
            var newPass = $("#txtNewPass").val();
            var confirmPass = $("#txtConfirmPass").val();
            var mess = "";
            if (oldPass == newPass) {
                mess = "Mật khẩu mới không được trùng với mật khẩu cũ";
            }
            if (newPass != confirmPass) {
                mess = "Mật khẩu xác nhận không đúng";
            }
            if (mess != "") {
                alert(mess);
            } else {
                $.post(appPath + "Common/ChangePassword", { oldPass: oldPass, newPass: newPass }, function (data) {
                    alert(data);
                    if (data == "Mật khẩu đã được thay đổi.") {
                        closeWindow('<%=ViewData["WindowId"]%>');
                    }
                });
            }
            //}
        }
    </script>
    <div>
        <form id="formChangePassword" action="">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td></td>
                    <td><label class="summaryError validation-summary-errors"></label></td>
                </tr>
                <tr>
                    <td class="lbright">Mật khẩu cũ</td>
                    <td><input type="password" id="txtOldPass" class="required" /></td>
                </tr>
                <tr>
                    <td class="lbright">Mật khẩu mới</td>
                    <td><input type="password" id="txtNewPass" class="required" /></td>
                </tr>
                <tr>
                    <td class="lbright">Nhập lại mật khẩu mới</td>
                    <td><input type="password" id="txtConfirmPass" class="required" /></td>
                </tr>
            </table>
        </form>
    </div>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:fnChange()' class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>