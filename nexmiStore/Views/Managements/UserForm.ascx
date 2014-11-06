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
            //$('#slGenders').val('<%=ViewData["Gender"]%>');
            $('input:radio[name="rdGenders"][value="<%=ViewData["Gender"]%>"]').prop('checked', true);
            $("#txtName").focus();
        });
    </script>
    <form id="formUser" action="">
        <table style="width: 100%" class="frmInput">
            <tr>
                <td class="lbright">ID</td>
                <td><input type="text" id="txtId" readonly="readonly" value="<%=ViewData["id"]%>" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Tên đầy đủ</td>
                <td><input type="text" id="txtName" value="<%=ViewData["name"]%>" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Tên đăng nhập</td>
                <td><input type="text" id="txtCode" value="<%=ViewData["code"]%>" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Mật khẩu</td>
                <td><input type="password" id="txtPassword" value="<%=ViewData["password"]%>" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright">Giới tính</td>
                <td>
                    <%//Html.RenderAction("slGenders", "UserControl", new { elementId = "slGenders" });%>
                    <input type="radio" id="rdMale" name="rdGenders" value="1" checked="checked" /> Nam
                    <input type="radio" id="rdFemale" name="rdGenders" value="0" /> Nữ
                </td>
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
            <tr>
                <td class="lbright">Kho mặc định</td>
                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks" });%></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></td>
                <td><input type="checkbox" id="cbStatus" <%=ViewData["strStatus"]%> /> Kích hoạt</td>
            </tr>
        </table>
    </form>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveOrUpdateUser()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:fnResetFormUser()" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLEAR_ALL", langId) %></button>
        <button onclick='javascript:fnCloseFormUser("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>