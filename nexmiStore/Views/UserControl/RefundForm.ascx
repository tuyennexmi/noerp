<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            $("#txtPayAmount, #txtPaid").autoNumeric('init', { vMax: '1000000000000' });
            if ('<%=ViewData["disabled"]%>' == "disabled") {
                $("#cbbCustomers").jqxComboBox({ disabled: true });
                $("#slPaymentTypes").attr('disabled', true);
                $("#txtInvoiceId").attr('disabled', true);
            }
            $("#formPayment").jqxValidator({
                rules: [
                    { input: '#cbbCustomers', message: 'Chọn khách hàng.', action: 'change', rule: function (input) {
                        if ($(input).jqxComboBox('getSelectedItem') == null)
                            return false;
                        return true;
                    }
                    },
                    { input: '#txtPayAmount', message: 'Số tiền trả lại nhỏ hơn hoặc bằng số tiền đã thu.', action: 'keyup, blur', rule: function (input, commit) {
                        var amount = $('#txtPayAmount').autoNumeric('get');
                        var paid = $('#txtPaid').autoNumeric('get');
                        if (amount <= 0 && amount > paid) {
                            return false;
                        }
                        return true;
                    }
                    }
                ]
            });
            $("#txtPayAmount").focus();
        });

        function fnSaveRefund() {
            if ($("#formPayment").jqxValidator('validate')) {
                $.ajax({
                    type: "POST",
                    url: appPath + 'UserControl/SaveRefund',
                    data: {
                        txtPaymentId: $("#txtPaymentId").val(),
                        cbbCustomers: $("#cbbCustomers").jqxComboBox('getSelectedItem').value,
                        txtInvoiceId: $("#txtInvoiceId").val(),
                        txtPaymentDate: $("#txtPaymentDate").val(),
                        txtTotalAmount: fnStripNonNumeric($("#txtTotalAmount").val()),
                        txtDescription: $("#txtDescription").val(),
                        slPaymentTypes: $("#slPaymentTypes").val(),
                        slPaymentMethods: $("#slPaymentMethods").val(),
                        txtPaid: $('#txtPaid').autoNumeric('get'),
                        txtPayAmount: $("#txtPayAmount").autoNumeric('get')
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            fnSuccess();
                            document.location.reload();
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
            } else {
                alert("Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!");
            }
        }
    </script>
    <div>
        <form id="formPayment" action="">
            <table class="frmInput">
                <tr>
                    <td class="lbright">Số phiếu chi</td>
                    <td><input type="text" id="txtPaymentId" name="txtPaymentId" disabled="disabled" value="<%=ViewData["PaymentId"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                    <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = ViewData["CustomerId"], customerName = ViewData["CustomerName"] });%></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                    <td><input type="text" id="txtInvoiceId" name="txtInvoiceId" value="<%=ViewData["InvoiceId"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright">Ngày chi trả</td>
                    <td><input type="text" id="txtPaymentDate" value="<%=ViewData["PaymentDate"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright">Hình thức chi trả</td>
                    <td><%Html.RenderAction("slPaymentMethods", "UserControl");%></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></td>
                    <td><input type="text" id="txtTotalAmount" name="txtTotalAmount" readonly="readonly" value="<%=ViewData["TotalAmount"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAID", langId) %></td>
                    <td><input type="text" id="txtPaid" name="txtPaid" readonly="readonly" value="<%=ViewData["Paid"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright">Số tiền chi</td>
                    <td><input type="text" id="txtPayAmount" /></td>
                </tr>
                <tr>
                    <td class="lbright">Lý do</td>
                    <td><textarea id="txtDescription" name="txtDescription" cols="50" rows="2" style="width: 250px"></textarea></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TYPE", langId) %></td>
                    <td><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slPaymentTypes", objectName = "PaymentTypes", value = ViewData["slPaymentTypes"] });%></td>
                </tr>
            </table>
        </form>
    </div>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveRefund()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>
