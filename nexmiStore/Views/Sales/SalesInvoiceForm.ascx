<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#txtInvoiceDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" }); 
        //$("#txtShipCost, #txtOtherCost").autoNumeric('init', { mDec: '3' });
        $("#tabsSalesInvoice").jqxTabs({ theme: theme, keyboardNavigation: false });
//        $("#txtShipCost, #txtOtherCost").keyup(function () {
//            calculateTotalAmount();
//        });
        $("#formSalesInvoice").jqxValidator({
            rules: [
                { input: '#cbbCustomers', message: 'Chọn khách hàng', action: 'change', rule: function (input) {
                    if ($(input).jqxComboBox('getSelectedItem') == null)
                        return false;
                    return true;
                }
                },
                 { input: '#txtInvoiceDate', message: 'Không được để trống.', action: 'keyup, blur', rule:
                        function (input, commit) {
                            if ($(input).val() == '')
                                return false;
                            return true;
                        }
                 }
            ]
        });
        $('#slUsers').prop('disabled', true);
    });

    function fnSaveOrUpdateSalesInvoice(saveMode) {
        if ($('#tbodyDetails tr').children().length > 0) {
            if ($("#formSalesInvoice").jqxValidator('validate')) {
                $.ajax({
                    type: "POST",
                    url: appPath + 'Sales/SaveOrUpdateSalesInvoice',
                    data: {
                        id: $("#txtId").text(),
                        invoiceDate: $("#txtInvoiceDate").val(),
                        customerId: $("#cbbCustomers").jqxComboBox('getSelectedItem').value,
                        description: $("#txtDescription").val(),
                        reference: $("#txtReference").val(),
                        salePerson: $("#slUsers").val(),
                        sourceDocument: $('#txtSourceDocument').val(),
                        stockId: $('#slStocks').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            if (saveMode == '1')
                                fnResetFormSI();
                            else
                                fnLoadSIDetail(data.result);
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
                alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!");
            }
        } else {
            alert('Không có sản phẩm nào');
        }
    }

    function fnResetFormSI() {
        $.post(appPath + "Common/ClearSession", { sessionName: "InvoiceDetails" });
        $("#txtId").text("");
        $('#cbbCustomers').jqxComboBox('clearSelection');
        //$("#txtInvoiceDate").val("");
        $("#txtReceiptAmount").val("");
        //$("#txtShipCost").autoNumeric('set', '');
        //$("#txtOtherCost").autoNumeric('set', '');
        $("#txtDescription").val("");
        $("#lbTotalDiscount").html("");
        $("#lbTotalVATRate").html("");
        $("#lbTotalAmount").html("");
        $("#lbInvoiceAmount").html("");
        $("#lbInvoiceAmount2").html("");
        $("#tbodyDetails").html("");
    }

    function fnShowSalesInvoiceDetail(productId) {
        $.get(appPath + 'Sales/SalesInvoiceDetailForm', { productId: productId }, function (data) {
            $('#formSIDetail').html(data);
        });
    }

    function fnSaveSalesInvoiceDetail() {
        if ($('#formSIDetail').jqxValidator('validate')) {
            var Product = $("#cbbProducts").jqxComboBox('getSelectedItem');
            var productId = Product.value;
            var productName = Product.label;
            var quantity = $("#txtQuantity").autoNumeric('get');
            var price = $("#txtPrice").autoNumeric('get');
            if (price == "") {
                price = 0;
            }
            var discount = $("#txtDiscount").autoNumeric('get');
            if (discount == "") {
                discount = 0;
            }
            var VATRate = $("#txtVATRate").autoNumeric('get');
            if (VATRate == "") {
                VATRate = 0;
            }
            $.post(appPath + "Sales/AddSalesInvoiceDetail", {
                ordinalNumber: $("#txtOrdinalNumber").val(),
                productId: productId, unitId: $('#slProductUnits').val(),
                quantity: quantity, price: price, 
                discount: discount, VATRate: VATRate
            }, function (data) {
                $('#tbodyDetails').html(data);
                $('#lbAmount').html($('#txtLineAmount').val());
                $('#lbTotalVATRate').html($('#txtTaxAmount').val());
                $('#lbTotalDiscount').html($('#txtDiscountAmount').val());
                $('#lbTotalAmount').html($('#txtTotalAmount').val());
                //calculateTotalAmount();
                fnResetSalesInvoiceDetail();
            });
        }
    }

    function fnRemoveSalesInvoiceDetail(productId) {
        $.post(appPath + 'Sales/RemoveSalesInvoiceDetail', { productId: productId }, function (data) {
            $('#' + productId).remove();
            $('#lbAmount').html(data.amount);
            $('#lbTotalVATRate').html(data.taxAmount);
            $('#lbTotalDiscount').html(data.discountAmount);
            $('#lbTotalAmount').html(data.totalAmount);
            calculateTotalAmount();
        });
    }

    function calculateTotalAmount() {
        var totalAmount = $("#lbAmount").html().replace(/,/g, "");
        if (totalAmount == "") totalAmount = 0;
        var VATRate = $("#lbTotalVATRate").html().replace(/,/g, "");
        if (VATRate == "") VATRate = 0;
        var totalDiscount = $("#lbTotalDiscount").html().replace(/,/g, "");
        if (totalDiscount == "") totalDiscount = 0;
        //var shipCost = $("#txtShipCost").autoNumeric('get');
        //if (shipCost == "") shipCost = 0;
        //var otherCost = $("#txtOtherCost").autoNumeric('get');
        //if (otherCost == "") otherCost = 0;
        var invoiceAmount = parseFloat(totalAmount) + parseFloat(VATRate) - parseFloat(totalDiscount);
        $("#lbTotalAmount").html(fnNumberFormat(invoiceAmount));
    }

    function fnResetSalesInvoiceDetail() {
        $("#txtOrdinalNumber").val('0')
        $('#cbbProducts').jqxComboBox('clearSelection');
        //$('#txtDetailDescription').val('');
        $('#txtQuantity').autoNumeric('set', '1.00');
        $('#txtPrice').autoNumeric('set', '0.00');
        $('#txtDiscount').autoNumeric('set', '0.00');
        $('#txtVATRate').autoNumeric('set', '0.00');
        $('#txtAmount').val('0.00');
        $('#cbbProducts').find(".jqx-combobox-input").focus();
    }
</script>
<% 
    NEXMI.NMSalesInvoicesWSI WSI = (NEXMI.NMSalesInvoicesWSI)ViewData["WSI"];
    string id = WSI.Invoice.InvoiceId;
    string current = WSI.Invoice.InvoiceStatus;
%>
<div id="divSalesInvoice">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <button onclick="javascript:fnSaveOrUpdateSalesInvoice()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button onclick="javascript:fnSaveOrUpdateSalesInvoice('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                        <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                    </div>
                </td>
                <td align="right">
                    <%//Html.RenderPartial("SalesInvoiceSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
            </div>     
            <div class="divStatusBar">
                <%
                    Html.RenderAction("StatusBar", "UserControl", new { objectName = "SalesInvoices", current = current });
                %>
            </div>
        </div>
        <div class='divContentDetails'>
        <form id="formSalesInvoice" class="form-to-validate" action="" style="margin: 10px;">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td>
                        <label class="lbId">Hóa đơn bán hàng <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = WSI.Invoice.CustomerId, customerName = ViewData["CustomerName"] });%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></td>
                                <td><input type="text"  id="txtInvoiceDate" value="<%= WSI.Invoice.InvoiceDate.ToString("yyyy-MM-dd") %>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></td>
                                <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers", userId = Page.User.Identity.Name }); %></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                                <td><input type="text" id="txtReference" value="<%=WSI.Invoice.Reference %>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = WSI.Invoice.StockId }); %></td>
                                <td class="lbright">Tham chiếu khách hàng</td>
                                <td><input type="text" id="txtSourceDocument" value="<%= WSI.Invoice.SourceDocument %>" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="tabsSalesInvoice">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" id="tbSalesInvoiceDetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %> (%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails">
                                        <%Html.RenderPartial("SalesInvoiceDetails", "Sales");%>
                                    </tbody>
                                    <tfoot id="formSIDetail" class="form-to-validate">
                                        <%Html.RenderAction("SalesInvoiceDetailForm", "Sales");%>
                                    </tfoot>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td>
                                            <textarea id="txtDescription" rows="5" cols="0" style="width: 100%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>...">
                                                <%= WSI.Invoice.DescriptionInVietnamese %>
                                            </textarea>
                                        </td>
                                        <td valign="top">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
                                                    <td align="right" style="width: 120px;"><label id="lbAmount"><%= WSI.Invoice.Amount.ToString("N3")%></label></td>
                                                </tr>                                                
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalDiscount"><%= WSI.Invoice.Discount.ToString("N3")%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %> : </b></td>
                                                    <td align="right"><label id="lbTotalVATRate"><%= WSI.Invoice.Tax.ToString("N3")%></label></td>
                                                </tr>
                                                <%--<tr>
                                                    <td colspan="6" align="right"><b>Phí vận chuyển: </b></td>
                                                    <td align="right"><input type="text" style="text-align: right; width: 150px;" id="txtShipCost" value="<%=ViewData["ShipCost"]%>" /></td>
                                                </tr>--%>
                                                <%--<tr>
                                                    <td colspan="6" align="right"><b>Phí khác: </b></td>
                                                    <td align="right"><input type="text" style="text-align: right; width: 150px;" id="txtOtherCost" value="<%=ViewData["OtherCost"]%>" /></td>
                                                </tr>--%>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalAmount"><%= WSI.Invoice.TotalAmount.ToString("N3")%></label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
        </div>
    </div>
</div>