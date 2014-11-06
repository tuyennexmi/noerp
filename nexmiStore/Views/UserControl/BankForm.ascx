<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    function fnSaveBank() {
        if (NMValidator("formBank")) {
            var bankId = $("#txtBankId").val();
            var bankCode = $("#txtBankCode").val();
            var bankName = $("#txtBankName").val();
            var bankAddress = $("#txtBankAddress").val();
            var areaId = $("#txtAreaId").val();
            $.post(appPath + "UserControl/SaveBank", { bankId: bankId, bankCode: bankCode, bankName: bankName, bankAddress: bankAddress, areaId: areaId }, function (data) {
                if (data.error == "") {
                    try {
                        fnSetBankItem(data.result, bankName + " - " + bankCode);
                    } catch (err) { }
                    CloseCurrentPopup();
                } else {
                    alert(data.error);
                }
            });
        }
    }

    function fnResetBankForm() {
        $("#txtBankId").val("");
        $("#txtBankCode").val("");
        $("#txtBankName").val("");
        $("#txtBankAddress").val("");
        $("#txtAreaId").val("");
        $("#txtAreaName").val("");
    }
</script>
<div>
    <form id="formBank" action="">
        <table class="frmInput">
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BANK_CODE", langId) %></td>
                <td><input type="hidden" id="txtBankId" /><input type="text" id="txtBankCode" class="required" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BANK_NAME", langId) %></td>
                <td><input type="text" id="txtBankName" class="required" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %></td>
                <td><input type="text" id="txtBankAddress" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("AREA", langId) %></td>
                <td><%Html.RenderPartial("slAreas");%></td>
            </tr>
        </table>
    </form>
</div>