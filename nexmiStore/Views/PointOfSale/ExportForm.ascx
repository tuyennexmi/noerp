<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();    
%>
<div class="windowContent">
    <form id="frmExport" method="post" action="">
        <table style="width: 100%" class="frmInput">
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { cgroupId = NEXMI.NMConstant.CustomerGroups.Customer, customerId = "", customerName = "" });%></td>
                <td class="lbright">Loại</td>
                <td valign="top"><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slExportTypes", objectName = "ExportTypes", value = "" });%></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_STOCK", langId) %></td>
                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %></td>
                <td><input type="text"  id="txtExportDate" value="<%=ViewData["ExportDate"]%>" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                <td><input type="text" id="txtReference" value="" /></td>
                <td class="lbright"><%--<%= NEXMI.NMCommon.GetInterface("EXPORT_STOCK", langId) %>--%></td>
                <td><%--<%Html.RenderAction("slStocks", "UserControl", new { elementId = "slExportStock", stockId = "" });%>--%></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTER", langId) %></td>
                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = "", customerName = "", elementId = "1" });%></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                <td><input type="text" id="txtTransport" value="" /></td> 
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                <td colspan="3"><textarea id="txtExportDescription" rows="5" cols="1" style="width: 90%"></textarea></td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        $(function () {
            $("#txtExportDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
            $("#frmExport").jqxValidator({
                rules: [
                    { input: '#txtExportDate', message: 'Bạn chưa nhập ngày xuất kho.', action: 'keyup, blur', rule:
                        function (input, commit) {
                            if ($(input).val() == '')
                                return false;
                            return true;
                        }
                    },
                    { input: '#cbbCustomers', message: 'Bạn chưa chọn khách hàng!', action: 'keyup, blur', rule:
                        function (input, commit) {
                            if ($(input).val() == '')
                                return false;
                            return true;
                        }
                    }
                ]
            });
            $('#slStocks').prop('disabled', true);
        });

        function fnSaveExport() {
            if ($('#frmExport').jqxValidator('validate')) {
                var customerId = "";
                try {
                    customerId = $("#cbbCustomers").jqxComboBox('getSelectedItem').value;
                } catch (err) { }
                var carrierId = '';
                try {
                    carrierId = $('#cbbCustomers1').jqxComboBox('getSelectedItem').value
                } catch (err) { }
                var data = {};
                data.exportDate = $('#txtExportDate').val();
                data.customerId = customerId;
                data.carrierId = carrierId;
                data.exportType = $("#slExportTypes").val();
                data.stockId = $("#slStocks").val();
                data.description = $("#txtExportDescription").val();
                data.invoiceId = $("#txtInvoiceId").val();
                data.transport = $('#txtTransport').val();
                $.ajax({
                    type: 'POST',
                    url: appPath + 'PointOfSale/SaveExport',
                    data: data,
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            fnSuccess();
                            $("#btNextOrder").click();
                            closeWindow('<%=windowId%>');
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
                alert('Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!');
            }
        }
    </script>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveExport()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>