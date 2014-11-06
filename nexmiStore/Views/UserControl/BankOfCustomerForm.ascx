<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div>
    <form id="formBankOfCustomer" action="">
        <table class="frmInput">
            <tr>
                <td class="lbright">Ngân hàng</td>
                <td><%Html.RenderPartial("cbbBanks");%></td>
            </tr>
            <tr>
                <td class="lbright">Số tài khoản</td>
                <td><input type="text" id="txtAccountNumber" value="" /></td>
            </tr>
            <tr>
                <td class="lbright">Chủ tài khoản</td>
                <td><input type="text" /></td>
            </tr>
        </table>
    </form>
</div>