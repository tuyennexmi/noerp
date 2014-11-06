<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString(); 
%>
<div class="windowContent">
    <form id="frmOrder" method="post" action="">
        <table class="frmInput">
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = "", customerName = "" });%></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                <td><input type="text" id="txtReference" name="txtReference" value="" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %></td>
                <td><input type="text"  id="txtOrderDate" value="<%=ViewData["OrderDate"]%>" /></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY_DATE", langId) %></td>
                <td><input type="text"  id="txtDeliveryDate" name="txtDeliveryDate" value="<%=DateTime.Today.AddDays(7).ToString("yyyy-MM-dd") %>" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></td>
                <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers", userId = Page.User.Identity.Name });%></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALE_AT", langId) %></td>
                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>
            </tr>
            <tr>
                <td class="lbright">Điều khoản<br />thương mại quốc tế</td>
                <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slIncoterms", objectName = "Incoterms", current = ViewData["slIncoterms"] });%></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY", langId) %></td>
                <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slShippingPolicy", objectName = "ShippingPolicy", current = ViewData["slShippingPolicy"] });%></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ADVANCE", langId) %></td>
                <td><input type="text" id="txtAdvances" value="" /></td>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %></td>
                <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slCreateInvoice", objectName = "CreateInvoice" });%></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                <td colspan="3"><textarea id="txtDescription" rows="5" cols="0" style="width: 95%"></textarea></td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        $(function () {
            $("#txtOrderDate, #txtDeliveryDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
            $("#frmOrder").jqxValidator({
                rules: [
                    { input: '#cbbCustomers', message: 'Chọn khách hàng', action: 'change', rule: function (input) {
                        if ($(input).jqxComboBox('getSelectedItem') == null)
                            return false;
                        return true;
                    }
                    },
                    { input: '#txtOrderDate', message: 'Chọn ngày đặt hàng.', action: 'keyup, blur', rule:
                        function (input, commit) {
                            if ($(input).val() == '')
                                return false;
                            return true;
                        }
                    }
                ]
            });
            $('#slStocks, #slUsers').prop('disabled', true);
        });

            function fnSaveOrder() {
            if ($('#frmOrder').jqxValidator('validate')) {
                $.ajax({
                    type: 'POST',
                    url: appPath + 'PointOfSale/SaveOrder',
                    data: {
                        orderDate: $('#txtOrderDate').val(),
                        deliveryDate: $("#txtDeliveryDate").val(),
                        customerId: $("#cbbCustomers").jqxComboBox('getSelectedItem').value,
                        reference: $("#txtReference").val(),
                        description: $("#txtDescription").val(),
                        salePerson: $("#slUsers").val(),
                        incoterm: $("#slIncoterms").val(),
                        shippingPolicy: $("#slShippingPolicy").val(),
                        createInvoice: $("#slCreateInvoice").val(),
                        saleAt: $("#slStocks").val()
                    },
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
        <button onclick="javascript:fnSaveOrder()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>