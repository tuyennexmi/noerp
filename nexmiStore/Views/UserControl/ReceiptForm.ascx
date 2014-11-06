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
                    <button onclick="javascript:fnSaveReceipt()" class="color red button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:fnSaveReceipt('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
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
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Receipts", current = ViewData["Status"] });%>
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
            var windowId = '<%=windowId%>';
            $(function () {
                $("#txtReceiptDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
                $("#txtPaidAmount").autoNumeric('init', { vMax: '1000000000000' });
                if ('<%=ViewData["disabled"]%>' == "disabled") {
                    $("#cbbCustomers").jqxComboBox({ disabled: true });
                    $("#slPaymentTypes").attr('disabled', true);
                    $("#txtInvoiceId").attr('disabled', true);
                }
                $("#formReceipt").jqxValidator({
                    rules: [
                        { input: '#cbbCustomers', message: 'Chọn khách hàng.', action: 'change', rule: function (input) {
                            if ($(input).jqxComboBox('getSelectedItem') == null)
                                return false;
                            return true;
                        }
                        },
                        { input: '#slBankAccounts', message: 'Chọn ngân hàng.', action: 'keyup, blur', rule: function (input, commit) {
                            if ($('#slPaymentMethods') == 'PAY02') {
                                var bnk = $('#slBankAccounts').val();
                                if (bnk == null)
                                    return false;
                            }
                            return true;
                        }
                        },
                        { input: '#txtPaidAmount', message: 'Số tiền thanh toán phải lớn hơn 0.', action: 'keyup, blur', rule: function (input, commit) {
                            var amount = $('#txtPaidAmount').autoNumeric('get');
                            var result = amount > 0;
                            return result;
                        }
                        }
                    ]
                });
                $('#txtInvoiceId').on('change', function () {
                    $.post(appPath + 'Common/GetInvoiceInfo', { id: $(this).val() }, function (data) {
                        if (data.Error != '') {
                            $('#txtInvoiceId').val('');
                            $('#cbbCustomers').jqxComboBox({ disabled: false });
                            $('#cbbCustomers').jqxComboBox('clearSelection');
                        } else {
                            if (data.CustomerId != '') {
                                fnSetItemForCombobox('cbbCustomers', data.CustomerId, Base64.encode(data.CustomerName));
                                $('#cbbCustomers').jqxComboBox({ disabled: true });
                            }
                        }
                        $('#txtTotalAmount').val(data.TotalAmount);
                        $('#txtBalance').val(data.Balance);
                    });
                });
                $('#txtInvoiceId').change();
                $("#txtPaidAmount").focus();
            });

            function fnSaveReceipt(saveMode) {
                if ($("#formReceipt").jqxValidator('validate')) {
                    $.ajax({
                        async: false,
                        type: "POST",
                        url: appPath + 'UserControl/SaveReceipt',
                        data: {
                            txtReceiptId: $("#txtReceiptId").val(),
                            cbbCustomers: $("#cbbCustomers").jqxComboBox('getSelectedItem').value,
                            txtInvoiceId: $("#txtInvoiceId").val(),
                            txtReceiptDate: $("#txtReceiptDate").val(),
                            txtTotalAmount: fnStripNonNumeric($("#txtTotalAmount").val()),
                            txtDescription: $("#txtDescription").val(),
                            slReceiptTypes: $("#slReceiptTypes").val(),
                            slPaymentMethods: $("#slPaymentMethods").val(),
                            txtBalance: fnStripNonNumeric($("#txtBalance").val()),
                            txtPaidAmount: $("#txtPaidAmount").autoNumeric('get'),
                            status: '<%=ViewData["Status"]%>',
                            bank: $("#slBankAccounts").val()                            
                        },
                        beforeSend: function () {
                            fnDoing();
                        },
                        success: function (data) {
                            if (data.error == '') {
                                fnSuccess();
                                if (windowId == '') {
                                    if (saveMode == '1')
                                        fnResetReceiptForm();
                                    else
                                        fnLoadReceiptDetail(data.id);
                                } else {
                                    closeWindow(windowId);
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

            function fnResetReceiptForm() {
                $("#txtReceiptId").val('')
                $("#cbbCustomers").jqxComboBox('clearSelection')
                $("#txtInvoiceId").val('')
                //$("#txtReceiptDate").val('<%=DateTime.Today.ToString("yyyy-MM-dd")%>')
                $("#txtTotalAmount").val('')
                $("#txtDescription").val('')
                //$("#slReceiptTypes").val('')
                $("#txtBalance").val('')
                $("#txtPaidAmount").val('')
            }
        </script>
        <div class='divContentDetails'>
        <form id="formReceipt" action="" class="form-to-validate">
            <table class="frmInput">
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECEIPT_NO", langId) %></td>
                    <td><input type="text" id="txtReceiptId" name="txtReceiptId" disabled="disabled" value="<%=ViewData["ReceiptId"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("INVOICE_NO", langId) %></td>
                    <td><input type="text" id="txtInvoiceId" name="txtInvoiceId" value="<%=ViewData["InvoiceId"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYER", langId) %></td>
                    <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = ViewData["CustomerId"], customerName = ViewData["CustomerName"] });%></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></td>
                    <td><input type="text"  id="txtReceiptDate" value="<%=ViewData["ReceiptDate"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></td>
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
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></td>
                    <td><input type="text" id="txtBalance" name="txtBalance" readonly="readonly" value="<%=ViewData["Balance"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAY_AMOUNT", langId) %></td>
                    <td><input type="text" id="txtPaidAmount" name="txtPaidAmount" value="<%=ViewData["ReceiptAmount"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                    <td><textarea id="txtDescription" name="txtDescription" cols="50" rows="2" style="width: 250px"><%=ViewData["Description"]%></textarea></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TYPE", langId) %></td>
                    <%--<td><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slReceiptTypes", objectName = "ReceiptTypes", value = ViewData["TypeId"] });%></td>--%>
                    <td><%Html.RenderAction("slAccountTypes", "UserControl", new { elementId = "slReceiptTypes", objectName = "Receipts", value = ViewData["TypeId"] });%></td>
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
            <button onclick="javascript:fnSaveReceipt()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
        </div>
    </div>
</div>
<% 
    }    
%>
