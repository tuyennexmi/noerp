<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = "";
    if (ViewData["WindowId"] == null)
    {
        pos = "static";
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnSavePayment()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:fnSavePayment('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
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
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Payments", current = ViewData["Status"] });%>
        </div>
    </div>
<% 
    }
    else
    {
        windowId = ViewData["WindowId"].ToString();
    } 
%>
    <div class="windowContent" style="position: <%=pos%> !important;">
        <script type="text/javascript">
            $(function () {

                $("#txtPaymentDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

                $("#txtPayAmount, #txtPaid").autoNumeric('init', { vMax: '1000000000000' });
                if ('<%=ViewData["disabled"]%>' == "disabled") {
                    $("#cbbCustomers").jqxComboBox({ disabled: true });
                    $("#slReceiptTypes").attr('disabled', true);
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
                        },
                        { input: '#txtPayAmount', message: 'Số tiền chi phải lớn hơn 0.', action: 'keyup, blur', rule: function (input, commit) {
                            var amount = $('#txtPayAmount').autoNumeric('get');
                            var result = amount > 0;
                            return result;
                        }
                        }
                    ]
                });
                $("#txtPayAmount").focus();
            });

            function fnSavePayment(saveMode) {
                if ($("#formPayment").jqxValidator('validate')) {
                    $.ajax({
                        type: "POST",
                        url: appPath + 'Accounting/SaveOrUpdatePayment',
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
                            txtPayAmount: $("#txtPayAmount").autoNumeric('get'),
                            status: '<%=ViewData["Status"]%>',
                            bank: $("#slBankAccounts").val(),
                            receiveBank: $("#slBankFor112").val()
                        },
                        beforeSend: function () {
                            fnDoing();
                        },
                        success: function (data) {
                            if (data.error == "") {
                                fnSuccess();
                                if ('<%=windowId%>' == '') {
                                    if (saveMode == '1')
                                        fnResetPaymentForm();
                                    else
                                        fnLoadPaymentDetail(data.id);
                                } else {
                                    closeWindow('<%=windowId%>');
                                    $(window).hashchange();
                                }
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

            function fnResetPaymentForm() {
                $("#txtPaymentId").val('')
                $("#cbbCustomers").jqxComboBox('clearSelection')
                $("#txtInvoiceId").val('')
                $("#txtTotalAmount").val('')
                $("#txtDescription").val('')
                $("#txtPayAmount").val('')
                $("#txtPaid").val('')
            }
        </script>
        <div class='divContentDetails'>
            <form id="formPayment" action="">
                <table class="frmInput">
                    <tr>
                        <td class="lbright">Số chứng từ</td>
                        <td><input type="text" id="txtPaymentId" name="txtPaymentId" disabled="disabled" value="<%=ViewData["PaymentId"]%>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECIPIENT", langId) %></td>
                        <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = ViewData["CustomerId"], customerName = ViewData["CustomerName"] });%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                        <td><input type="text" id="txtInvoiceId" name="txtInvoiceId" value="<%=ViewData["InvoiceId"]%>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></td>
                        <td><input type="text"  id="txtPaymentDate" value="<%=ViewData["PaymentDate"]%>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright">Hình thức chi trả</td>
                        <td><%Html.RenderAction("slPaymentMethods", "UserControl", new { paymentMethod = ViewData["PaymentMethodId"] });%></td>
                    </tr>
                    <tr>
                        <td class="lbright">Qua Ngân hàng</td>
                        <td><%Html.RenderAction("slBankAccounts", "UserControl", new { bankAccount = ViewData["bankAccount"] });%></td>
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
                        <td><input type="text" id="txtPayAmount" value="<%=ViewData["PaymentAmount"]%>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
                        <td><textarea id="txtDescription" name="txtDescription" cols="50" rows="2" style="width: 250px"><%=ViewData["Description"]%></textarea></td>
                    </tr>
                    <tr>
                        <td class="lbright">Lý do chi </td>
                        <%--<td><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slPaymentTypes", objectName = "PaymentTypes", value = ViewData["TypeId"] });%></td>--%>
                        <td><%Html.RenderAction("slAccountForPayments", "UserControl", new { elementId = "slPaymentTypes", objectName = "Payments", value = ViewData["TypeId"] });%></td>
                    </tr>
                    <tr>
                        <td class="lbright">Tại Ngân hàng</td>
                        <td><%Html.RenderAction("slBankAccounts", "UserControl", new { bankAccount = ViewData["bankAccount"], elementId = "slBankFor112" });%></td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
<% 
    if (windowId != "")
    {
%>
    <div class="windowButtons">
        <div class="NMButtons">
            <button onclick="javascript:fnSavePayment('')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
        </div>
    </div>
<% 
    }    
%>
</div>
