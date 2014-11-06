<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string id = ViewData["Id"].ToString(),
        current = ViewData["StatusId"].ToString();
%>

<script type="text/javascript">
    $(function () {
        $("#txtImportDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $('#tabsImport').jqxTabs({ theme: theme, keyboardNavigation: false });
        $('#formImport').jqxValidator({
            rules: [
                { input: '#txtImportDate', message: 'Vui lòng chọn ngày nhập !', action: 'keyup, blur', rule:
                    function (input, commit) {
                        if ($(input).val() == '')
                            return false;
                        return true;
                    }
                },
                { input: '#cbbCustomers', message: 'Vui lòng chưa chọn Nhà cung cấp !', action: 'change', rule: function (input) {
                    if ($(input).jqxComboBox('getSelectedItem') == null)
                        return false;
                    return true;
                }
                }
            ]
            });
        <%if((bool)ViewData["StockInput"] == false){ %>
            $('#slStocks').prop('disabled', true);
        <%} %>

        try {
            $('#slExportStock').prop('disabled', true);
        } catch (err) { }
        
        if ('<%=current%>' == '<%=NEXMI.NMConstant.IMStatuses.Ready%>') {
            $('#formImportDetail, .actionCols').css({ 'display': 'none' });
        }
    });

    function fnSaveOrUpdateImport(saveMode) {
        if ($('#tbodyDetails tr').children().length > 0) {
            if ($("#formImport").jqxValidator('validate')) {
                var supplierId = "";
                try {
                    supplierId = $("#cbbCustomers").jqxComboBox('getSelectedItem').value;
                } catch (err) { }
                var carrierId = '';
                try {
                    carrierId = $('#cbbCustomers1').jqxComboBox('getSelectedItem').value
                } catch (err) { }
                var exportStockId = '';
                try {
                    exportStockId = $('#slExportStock').val();
                } catch (err) { }
                $.ajax({
                    type: "POST",
                    url: appPath + "Stocks/SaveOrUpdateImport",
                    data: {
                        id: $("#txtId").text(),
                        importDate: $("#txtImportDate").val(),
                        reference: $('#txtReference').val(),
                        statusId: '<%=current%>',
                        supplierId: supplierId,
                        importType: $("#slImportTypes").val(),
                        stockId: $("#slStocks").val(),
                        description: $("#txtImportDescription").val(),
                        invoiceId: $("#txtInvoiceId").val(),
                        carrierId: carrierId,
                        transport: $('#txtTransport').val(),
                        deliveryMethod: $('#slDeliveryMethod').val(),
                        exportStockId: exportStockId
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            fnSuccess();
                            if (saveMode == '1')
                                fnResetFormImport();
                            else
                                fnLoadImportDetail(data.result);
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

    function fnResetFormImport() {
        $.post(appPath + "Common/ClearSession", { sessionName: "Details" });
        $("#tbodyDetails").html("");
        $("#txtImportDescription").val("");
    }
</script>

<div id="divImport">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <button onclick="javascript:fnSaveOrUpdateImport()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button onclick="javascript:fnSaveOrUpdateImport('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                        <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                    </div>
                </td>
                <td align="right">
                    <%Html.RenderPartial("ImportSV"); %>
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
                Html.RenderAction("StatusBar", "UserControl", new { objectName = "Imports", current = current });
            %>
        </div>
    </div>

    <div class='divContentDetails'>
        <form id="formImport" action="" style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%= NEXMI.NMCommon.GetInterface("IMPORT_ORDER", langId) %> <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></td>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = ViewData["CustomerId"], customerName = ViewData["CustomerName"] });%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("INVOICE_NO", langId) %></td>
                                <td><input type="text" disabled="disabled" value="<%=ViewData["InvoiceId"]%>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("IMPORT_DATE", langId) %></td>
                                <td><input type="text"  id="txtImportDate" value="<%=ViewData["ImportDate"]%>" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                                <td><input type="text" id="txtReference" value="<%=ViewData["Reference"]%>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TYPE", langId) %></td>
                                <td valign="top"><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slImportTypes", objectName = "ImportTypes", value = ViewData["ImportType"] });%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ViewData["StockId"] }); %></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="tabsImport">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" id="tbImportDetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("GOOD_QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("BAD_QUANTITY", langId) %></th>
                                    <% 
                                        if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
                                        {
                                    %>
                                            <th><%= NEXMI.NMCommon.GetInterface("SERIAL", langId) %></th>
                                    <% 
                                        }    
                                    %>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                            <th class="actionCols"></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails">
                                        <%Html.RenderPartial("ImportDetails", "Stocks");%>
                                    </tbody>
                                    <tfoot id="formImportDetail" class="form-to-validate">
                                        <%Html.RenderAction("ImportDetailForm", "Stocks");%>
                                    </tfoot>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td><textarea id="txtImportDescription" rows="5" cols="0" style="width: 100%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."><%=ViewData["Description"]%></textarea></td>
                                        <td valign="top"></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY_METHOD", langId) %></td>
                            <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slDeliveryMethod", objectName = "DeliveryMethod", current = ViewData["DeliveryMethod"] });%></td>
                    <%
                        string exportStock = ViewData["ExportStock"].ToString();
                        if (!String.IsNullOrEmpty(exportStock))
                        {
                    %>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_STOCK", langId) %></td>
                            <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slExportStock", stockId = exportStock }); %></td>
                    <%
                        }
                        else
                        { 
                    %>
                            <td></td>
                            <td></td>
                    <%
                        }
                    %>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTER", langId) %></td>
                            <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = ViewData["CarrierId"], customerName = ViewData["CarrierName"], elementId = "1" });%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                            <td><input type="text" id="txtTransport" value="<%=ViewData["Transport"]%>" /></td> 
                        </tr>
                    </table>
                </div>
            </div>
        </form>  
    </div>
    </div>
</div>