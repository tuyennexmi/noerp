<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string id = ViewData["Id"].ToString();
    string current = ViewData["ExportStatus"].ToString(); 
%>
<script type="text/javascript">
    $(function () {
        $("#txtExportDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $("#tabsExport").jqxTabs({ theme: theme, keyboardNavigation: false });
        $("#formExport").jqxValidator({
            rules: [
                { input: '#txtExportDate', message: 'Không được để trống!', action: 'keyup, blur', rule:
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
        $('#slImportStock option[value=' + $('#slStocks').val() + ']').remove();
        fnCheckExportType();
        $('#slExportTypes').on('change', function () {
            fnCheckExportType();
        });
        if ('<%=current%>' == '<%=NEXMI.NMConstant.EXStatuses.Ready%>') {
            $('#formExportDetail, .actionCols').css({ 'display': 'none' });
        }
    });

    function fnCheckExportType() {
        if ($('#slExportTypes').val() == '<%=NEXMI.NMConstant.ExportType.Transfer%>') {
            $('.importStock').show();
        } else {
            $('.importStock').hide();
        }
    }

    function fnSaveOrUpdateExportProduct(saveMode) {
        if ($('#tbodyDetails tr').children().length > 0) {
            if ($("#formExport").jqxValidator('validate')) {
                var customerId = "";
                try {
                    customerId = $("#cbbCustomers").jqxComboBox('getSelectedItem').value;
                } catch (err) { }
                var carrierId = '';
                try {
                    carrierId = $('#cbbCustomers1').jqxComboBox('getSelectedItem').value
                } catch (err) { }
                $.ajax({
                    type: "POST",
                    url: appPath + 'Stocks/SaveOrUpdateExport',
                    data: {
                        id: $("#txtId").text(),
                        exportDate: $("#txtExportDate").val(),
                        customerId: customerId,
                        statusId: '<%=current%>',
                        reference: $("#txtReference").val(),
                        description: $("#txtDescription").val(),
                        exportTypeId: $("#slExportTypes").val(),
                        stockId: $("#slStocks").val(),
                        carrierId: carrierId,
                        transport: $('#txtTransport').val(),
                        deliveryMethod: $('#slDeliveryMethods').val(),
                        importStockId: $('#slImportStock').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            fnSuccess();
                            if (saveMode == '1')
                                fnResetFormExport();
                            else
                                fnLoadExportDetail(data.result);
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
            else {
                alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!");
            }
        } else {
            alert('Không có sản phẩm nào');
        }
    }

    function fnResetFormExport() {
        $.post(appPath + "Common/ClearSession", { sessionName: "Details" });
        $("#tbodyDetails").html("");
        $("#cbbCustomers").jqxComboBox('clearSelection');
        $("#txtId").val("");
        //$("#cbExportStatus").attr('checked', false);
        $("#txtExportDescription").val("");
    }
</script>

<div id="divExport">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <button onclick="javascript:fnSaveOrUpdateExportProduct()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button onclick="javascript:fnSaveOrUpdateExportProduct('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                        <button onclick="javascript:$('#formExport').jqxValidator('hide'); history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                    </div>
                </td>
                <td align="right">
                    <%Html.RenderPartial("ExportSV");%>
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
                Html.RenderAction("StatusBar", "UserControl", new { objectName = "Exports", current = current });
            %>
        </div>
    </div>
    <div class='divContentDetails'>
        <form id="formExport" action="" style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%= NEXMI.NMCommon.GetInterface("EXPORT_ORDER", langId) %> <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = ViewData["CustomerId"], customerName = ViewData["CustomerName"] });%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                                <td><input type="text" id="txtReference" value="<%=ViewData["Reference"]%>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %></td>
                                <td><input type="text"  id="txtExportDate" value="<%=ViewData["ExportDate"]%>" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TYPE", langId) %></td>
                                <td valign="top"><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slExportTypes", objectName = "ExportTypes", value = ViewData["ExportTypeId"] });%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_STOCK", langId) %></td>
                                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ViewData["slStocks"] });%></td>
                                <td class="lbright importStock"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                                <td class="importStock"><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slImportStock", stockId = ViewData["ImportStockId"] }); %></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="tabsExport">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td id='Details'>
                                <table style="width: 100%" id="tbExportProductDetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("INVENTORY", langId) %></th>
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
                                        <%Html.RenderPartial("ExportDetails", "Stocks");%>
                                    </tbody>
                                    <tfoot id="formExportDetail" class="form-to-validate">
                                        <%Html.RenderAction("ExportDetailForm", "Stocks");%>
                                    </tfoot>
                                </table>
                            </td>
                        </tr>
                        <%--<tr>
                            <td><a class="btActions" href="javascript:fnShowExportProductDetail('')">Thêm chi tiết</a></td>
                        </tr>--%>
                        <tr>
                            <td><textarea id="txtDescription" rows="5" cols="0" style="width: 100%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."><%=ViewData["Description"]%></textarea></td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY_METHOD", langId) %></td>
                            <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slDeliveryMethods", objectName = "ShippingPolicy", current = "" });%></td>
                            <td></td>
                            <td></td>
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