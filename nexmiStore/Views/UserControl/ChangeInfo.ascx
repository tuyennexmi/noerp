<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            $("#formUser").jqxValidator({
                rules: [
                { input: '#txtName', message: 'Nhập tên đầy đủ.', action: 'keyup, blur', rule: 'required' },
                { input: '#txtCode', message: 'Nhập tên đăng nhập.', action: 'keyup, blur', rule: 'required' },
                { input: '#txtPassword', message: 'Nhập mật khẩu.', action: 'keyup, blur', rule: 'required' },
                { input: '#txtEmail', message: 'Nhập email.', action: 'keyup, blur', rule: 'required' },
                { input: '#txtEmail', message: 'Email không đúng định dạng.', action: 'keyup, blur', rule: 'email' },
                { input: '#txtEmailPassword', message: 'Nhập mật khẩu của email.', action: 'keyup, blur', rule: 'required' }
            ]
            });
            $("#txtName").focus();
        });

        function fnSaveOrUpdateUserInfo() {
            if ($("#formUser").jqxValidator('validate')) {
                $.ajax({
                    type: "POST",
                    url: appPath + 'Managements/SaveOrUpdateUser',
                    data: {
                        id: $("#txtId").val(),
                        name: $("#txtName").val(),
                        code: $("#txtCode").val(),
                        password: $("#txtPassword").val(),
                        phoneNumber: $("#txtPhoneNumber").val(),
                        email: $('#txtEmail').val(),
                        emailPassword: $('#txtEmailPassword').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data == "") {
                            fnSuccess();
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
            } else alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!");
        }
    </script>
    <form id="formUser" action="">
        <table style="width: 100%">
            <tr>
                <td class="lbright">Mã</td>
                <td><input type="text" id="txtId" value="<%=ViewData["id"]%>" disabled="disabled" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Tên đầy đủ</td>
                <td><input type="text" id="txtName" value="<%=ViewData["name"]%>" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Tên đăng nhập</td>
                <td><input type="text" id="txtCode" value="<%=ViewData["code"]%>" disabled="disabled" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Mật khẩu</td>
                <td><input type="password" id="txtPassword" value="<%=ViewData["password"]%>" disabled="disabled" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></td>
                <td><input type="text" id="txtPhoneNumber" value="<%=ViewData["phoneNumber"]%>" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Email</td>
                <td><input type="text" id="txtEmail" value="<%=ViewData["Email"]%>" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Mật khẩu email</td>
                <td><input type="password" id="txtEmailPassword" value="<%=ViewData["EmailPassword"]%>" />&nbsp;</td>
            </tr>
        </table>
    </form>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveOrUpdateUserInfo()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>