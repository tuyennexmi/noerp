<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
$(function () {
    $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
});

function fnSaveCloseMonth(saveMode) {
    $.ajax({
        type: "POST",
        url: appPath + 'Accounting/SaveOrUpdateCloseMonth',
        data: {
            from: $("#dtFrom").val(), to: $("#dtTo").val(),
            descriptions: $('#txtDescriptions').val()
        },
        beforeSend: function () {
            fnDoing();
        },
        success: function (data) {
            if (data.error == "") {
                fnSuccess();
                fnShowCloseMonthDetail(data.id);
            }
            else {
                alert(data.error);
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

function fnCalculateCloseMonth() {
    $.post(appPath + 'Accounting/CloseMonthCalculate', { from: $("#dtFrom").val(), to: $("#dtTo").val() }, function (data) {
        $('#formCloseMonth').html(data);
    });
}

</script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnCalculateCloseMonth()" class="button color red">Tính toán </button>
                    <button onclick="javascript:fnSaveCloseMonth()" class="button color red">Lưu sổ</button>
                    <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %> </button>
                </div>
            </td>
            <td align="right">
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divStatusBar">
            <%--<%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Payments", current = ViewData["Status"] });%>--%>
        </div>
    </div>
    <div class='divContentDetails'>
        <div class="windowContent" style="position: !important;">
            <label class="lbId" style="padding-left:20px">Khóa sổ kỳ kế toán </label>
            <form id="formCloseMonth" action="">
                <table class="frmInput">
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("FROM", langId) %> ngày</td>
                        <td><input type="text" id="dtFrom" value="<%=ViewData["FromDate"] %>" style="width: 89px"/></td>
                        <td class="lbright">Đến ngày</td>
                        <td><input type="text" id="dtTo" value="<%=ViewData["ToDate"]%>" style="width: 89px"/></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                        <td colspan="3"><input type="text" id="txtDescriptions" value="" style="width: 689px"/></td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
</div>
