<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#txtCash").autoNumeric('init', { vMax: '1000000000000.00' });
        $("#txtCash").on('keyup', function (event) {
            var total = fnStripNonNumeric($("#totalPay").val());
            var value = $('#txtCash').autoNumeric('get');
            if (value == "") value = 0;
            var remaining = total - value;
            if (remaining < 0) {
                remaining = 0;
            }
            $("#remaining").val(fnNumberFormat(remaining));
            var change = value - total;
            if (change < 0) {
                change = 0;
            }
            $("#change").val(fnNumberFormat(change));
                <% if(NEXMI.NMCommon.GetSetting("DEBIT_ENABLE") == false) {%>
                    if (parseFloat(value) >= parseFloat(total))
                        $("#btOk").attr("disabled", false);
                    else
                        $("#btOk").attr("disabled", true);
                <%}else{ %>
                        $("#btOk").attr("disabled", false);
                <%} %>
        });
        $('#txtCustomerId').on('change', function () {
            $.post(appPath + 'PointOfSale/CheckExistCustomer', { id: $(this).val() }, function (data) {
                if (data != '') {
                    alert(data);
                    $('#txtCustomerId').val('').focus();
                }
            });
        });
        
        $("#btCancel").click(function () {
            var windowId = $("#txtWindowId").val();
            $("#" + windowId).jqxWindow('close');
            $("#" + windowId).remove();
        });

        $("#btOk").click(function () {
            var cusId = $("#txtCustomerId").val();
            if (cusId == '') {
                alert('Vui lòng chọn hoặc nhập mã số khách hàng!');
//                if ($('#txtFelixCustomerId').length == 0) {
//                    alert('Vui lòng chọn hoặc nhập mã số khách hàng!');
//                    $("#txtCustomerId").focus();
//                }
//                else{
//                    if ($('#txtFelixCustomerId').val() == '') {
//                        alert('Vui lòng nhập mã một trong hai loại khách hàng!');
//                        $("#txtCustomerId").focus();
//                    }
//                }
            }
            else {
                $.post(appPath + 'PointOfSale/CheckExistCustomer', { id: cusId }, function (data) {
                    if (data != '') {
                        $('#txtCustomerId').val('').focus();
                    }
                    else {
                        var cash = $('#txtCash').autoNumeric('get');
                        var voucherTotal = fnStripNonNumeric($('#voucherTotal').val());
                        $.post(appPath + "PointOfSale/SaveInvoice", { 
                            customerId: cusId, 
                            cash: cash,
                            voucherTotal: voucherTotal
                            }, function (data) {
                            if (data.error != "") {
                                alert(data.error);
                            } else {
                                $("#btCancel").click();
                                $("#divRight").hide();
                                $("#divReceipt").show();
                                LoadContentDynamic("toPrint", "PointOfSale/PrintReceipt", { id: data.id });
                            }
                        });
                    }
                });
            }
        });

        $("#btOk").attr("disabled", true);
        $("#txtCash").keyup();
        $("#txtCustomerId").focus();

//        $('#txtFelixCustomerId').on('change', function () {
//            var memberCode = $(this).val();
//            if(memberCode != ''){
//                $.post(appPath + "PointOfSale/FelixMemberInfoCheck", { memberCode: $(this).val() }, 
//                    function (data) {
//                        if(data != 'success'){
//                            alert('Thành viên này không tồn tại!');
//                            $('#txtFelixCustomerId').val('').focus();
//                        }
//                        //openWindow("Thông tin thành viên", "PointOfSale/FelixMemberInfoDialog", { memberCode: $(this).val() }, 600, 420);
//                    });
//            }
//        });

        $('#txtEVoucher').on('change', function () {
            var voucherCode = $(this).val();
            if(voucherCode != ''){
                $.post(appPath + "PointOfSale/FelixVoucherInfoCheck", { voucherCode: voucherCode }, 
                    function (data) {
                        if(data.status != 'success'){
                            alert('Mã khuyến mãi này không tồn tại!');
                            $('#txtEVoucher').val('').focus();
                        }
                        else{
                            $('#voucherTotal').val(fnNumberFormat(data.value));
                            var total = fnStripNonNumeric($('#total').val());
                            var voucher = data.value;
                            var pay = total - voucher;
                            $('#txtVAT').val(fnNumberFormat(data.vat));
                            $('#totalPay').val(fnNumberFormat(data.pay));
                            $('#remaining').val(fnNumberFormat(data.pay));
                        }
                        //openWindow("Thông tin thành viên", "PointOfSale/FelixVoucherInfoDialog", { voucherCode: $(this).val() }, 600, 420);
                    });
                }
        });
        });

        function fnShowFelixEVoucherInfo() {
            var voucherCode =  $("#txtEVoucher").val();
            openWindow("Thông tin thẻ khuyến mãi", "PointOfSale/FelixVoucherInfoDialog", { voucherCode: voucherCode }, 600, 420);
        }

        function fnShowFelixMemberInfo() {
            var memberCode =  $("#txtFelixCustomerId").val();
            openWindow("Thông tin thành viên", "PointOfSale/FelixMemberInfoDialog", { memberCode: memberCode }, 600, 420);
        }
    
</script>
<% 
    string remaining = "0.00", change = "0.00";
    List<NEXMI.SalesInvoiceDetails> OrderDetails = new List<NEXMI.SalesInvoiceDetails>();
    if (Session["InvoiceDetails"] != null)
    {
        OrderDetails = (List<NEXMI.SalesInvoiceDetails>)Session["InvoiceDetails"];
    }
    string voucherCode= "";
    if (Session["VoucherCode"] != null)
        voucherCode = Session["VoucherCode"].ToString();
%>
<div style="padding: 10px;">
    <input type="hidden" id="txtWindowId" value="<%=ViewData["WindowId"]%>" />
    <table style="width: 100%; font-size: 16pt;" cellpadding="5" cellspacing="5">
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>: </td>
            <td align="right"><input type="text" id="txtCustomerId" style="height: 30px; font-size: 17pt;" /></td>
            <td><button onclick="javascript:fnShowCustomers('txtCustomerId', 'Danh sách khách hàng', '');" class="button keyboardPOS textPOS longPOS" >Tìm</button></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %>: </td>
            <td align="right"><input type="text" id="total" value="<%=OrderDetails.Sum(i=>i.Amount).ToString("N3") %>" style="height: 30px; font-size: 17pt; text-align: right;" disabled=disabled /></td>
            <td></td>
        </tr>
    <% if (NEXMI.NMCommon.GetSetting("FELIX_CONN") == true)
    {%>
        <%--<tr>
            <td><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %> Felix: </td>
            <td align="right"><input type="text" id="txtFelixCustomerId" style="height: 30px; font-size: 17pt;" /></td>
            <td><button onclick="javascript:fnShowFelixMemberInfo();" class="button keyboardPOS textPOS longPOS">Chi tiết</button></td>
        </tr>--%>
        <tr>
            <td>Mã Khuyến mãi: </td>
            <td align="right"><input type="text" id="txtEVoucher" value="<%=voucherCode %>" style="height: 30px; font-size: 17pt;" /></td>
            <td><button onclick="javascript:fnShowFelixEVoucherInfo();" class="button keyboardPOS textPOS longPOS"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></button></td>
        </tr>
        
        <tr>
            <td>Số tiền khuyến mãi: </td>
            <td align="right"><input type="text"  id="voucherTotal" value="<%=OrderDetails.Sum(i=>i.DiscountAmount).ToString("N3") %>" style="height: 30px; font-size: 17pt; text-align: right;" disabled=disabled /></td>
            <td></td>
        </tr>
    <%
        } %>
        
        <tr>
            <td>VAT: </td>
            <td align="right"><input type="text" id="txtVAT" value="<%=OrderDetails.Sum(i=>i.TaxAmount).ToString("N3") %>" style="height: 30px; font-size: 17pt; text-align: right;" disabled=disabled /></td>
            <td></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("TOTAL_PAYMENT", langId) %>: </td>
            <td align="right"><input type="text" id="totalPay" value="<%=OrderDetails.Sum(i=>i.TotalAmount).ToString("N3") %>" disabled=disabled style="height: 30px; font-size: 17pt; text-align: right;" /></td>
            <td></td>
        </tr>
        <tr >
            <td>Số tiền khách trả: </td>
            <td align="right"><input type="text" id="txtCash" value="0" style="height: 30px; font-size: 17pt; text-align: right;" /></td>
            <td><button class="btActions reset" onclick="javascript:$('#txtCash').val('').focus();"></button></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %>: </td>
            <td align="right"><input type="text" id="remaining" value="<%=OrderDetails.Sum(i=>i.TotalAmount).ToString("N3")  %>" style="height: 30px; font-size: 17pt; text-align: right;" disabled=disabled /></td>
            <td></td>
        </tr>
        <tr>
            <td>Tiền thối: </td>
            <td align="right"><input type="text" id="change" value="<%=change %>" style="height: 30px; font-size: 17pt; text-align: right;" disabled=disabled /></td>
            <td style="width: 20px"></td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <button id="btOk" class="button green large"><b>Đồng ý</b></button>
                <button id="btCancel" class="button green large"><b>Thoát</b></button>
            </td>
        </tr>
    </table>
</div>