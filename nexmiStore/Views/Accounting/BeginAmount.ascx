<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/CashBalances.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $("#dtFrom").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    function fnBeginCashBalance() {
        LoadContentDynamic('BeginAmountUC', 'Accounting/CBBeginAmount','');
    }

    function fnBeginAccountReceivable() {
        LoadContentDynamic('BeginAmountUC', 'Accounting/BeginARAmount', '');
    }

    function fnBeginAccountPayable() {
        LoadContentDynamic('BeginAmountUC', 'Accounting/BeginAPAmount', '');
    }

    function fnSaveBeginCash() {
        $.ajax({
            type: 'POST',
            url: appPath + 'Accounting/SaveBeginCash',
            data: { period: $('#dtFrom').val() },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data == "") {
                    alert("Đã lưu thành công!");
                }
                else {
                    alert(data);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi nên không lưu được!");
            },
            complete: function () {
            }
        });
    }

</script>

<%
    string from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01).ToString("yyyy-MM-dd");
    %>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <%--<button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>--%>
                <button onclick="javascript:fnBeginCashBalance()" class="button">Số dư tiền</button>
                <button onclick="javascript:fnBeginAccountReceivable()" class="button">Số dư nợ phải thu</button>
                <button onclick="javascript:fnBeginAccountPayable()" class="button">Số dư nợ phải trả</button>
                <button onclick="javascript:fnSaveBeginCash()" class="color red button">Lưu thay đổi</button>
            </td>
            <td></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">            
            <table>
                <tr>
                    <td>Kỳ bắt đầu <input type="text"  id="dtFrom" value="<%=from %>" /></td>
                    <%--<td><%Html.RenderAction("slPaymentMethods", "UserControl", new { paymentMethod = ViewData["PaymentMethodId"] });%></td>--%>
                    <%--<td><%Html.RenderAction("slBankAccounts", "UserControl", new { bankAccount = ViewData["bankAccount"] });%></td>--%>
                </tr>
            </table>            
        </div>
    </div>
    <div class='divContentDetails'>
    <div id="BeginAmountUC"></div>
    </div>
</div>