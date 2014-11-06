<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();    
%>
<div class="windowContent">
    <form id="frmImport" method="post" action="">
        <table style="width: 100%" class="frmInput">
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></td>
                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { cgroupId = NEXMI.NMConstant.CustomerGroups.Supplier, customerId = "", customerName = "" });%></td>
                <td class="lbright">Loại</td>
                <td valign="top"><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slImportTypes", objectName = "ImportTypes", value = "" });%></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("IMPORT_DATE", langId) %></td>
                <td><input type="text"  id="txtImportDate" value="<%=ViewData["ImportDate"]%>" /></td>
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
                <td colspan="3"><textarea id="txtImportDescription" rows="5" cols="1" style="width: 90%"></textarea></td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        $(function () {
            $("#txtImportDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
            $("#frmImport").jqxValidator({
                rules: [
                    { input: '#txtImportDate', message: 'Chọn ngày nhập kho.', action: 'keyup, blur', rule:
                        function (input, commit) {
                            if ($(input).val() == '')
                                return false;
                            return true;
                        }
                    },
                    { input: '#cbbCustomers', message: 'Chọn Nhà cung cấp!', action: 'keyup, blur', rule:
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

        function fnSaveImport() {
            if ($('#frmImport').jqxValidator('validate')) {
                var supplierId = "";
                try {
                    supplierId = $("#cbbCustomers").jqxComboBox('getSelectedItem').value;
                } catch (err) { }
                var carrierId = '';
                try {
                    carrierId = $('#cbbCustomers1').jqxComboBox('getSelectedItem').value
                } catch (err) { }
                var data = {};
                data.importDate = $('#txtImportDate').val();
                data.supplierId = supplierId;
                data.carrierId = carrierId;
                data.importType = $("#slImportTypes").val();
                data.stockId = $("#slStocks").val();
                data.description = $("#txtImportDescription").val();
                data.invoiceId = $("#txtInvoiceId").val();
                data.transport = $('#txtTransport').val();
                $.ajax({
                    type: 'POST',
                    url: appPath + 'PointOfSale/SaveImport',
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
        <button onclick="javascript:fnSaveImport()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>