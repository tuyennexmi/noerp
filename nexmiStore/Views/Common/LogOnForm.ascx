<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script src="<%=Url.Content("~")%>Scripts/forApp/LogOn.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function() {
        $("#txtUserCode").focus();
        validateForm("frmLogOn");
    });
</script>
<form id="frmLogOn" action="">
<table style="width: 100%;">
    <tr>
        <td style="width: 25%;">Tên đăng nhập</td>
        <td><input type="text" id="txtUserCode" class="required" onkeypress="CallEnter(event)" value="" />&nbsp;</td>
    </tr>
    <tr>
        <td>Mật khẩu</td>
        <td><input type="password" id="txtPassword" class="required" onkeypress="CallEnter(event)" value="" />&nbsp;</td>
    </tr>
</table>
<center><b class="errorArea" style="color: Red; font-style: inherit;"></b></center>
</form>