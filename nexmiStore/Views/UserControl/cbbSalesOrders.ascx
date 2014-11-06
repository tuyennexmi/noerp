<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(document).ready(function () {
        var source = {
//            toggleMode: 'none',
            datatype: "json",
            datafields: [
                { name: 'OrderId' }
            ],
            id: 'OrderId',
            url: appPath + "Common/GetSalesOrdersForAutocomplete"
        };
        var dataAdapter = new $.jqx.dataAdapter(source, {
            formatData: function (data) {
                return { keyword: $('#cbbSalesOrders<%=ViewData["elementId"]%>').jqxComboBox('searchString'), limit: 10, mode: "select" }
            }
        });
        $('#cbbSalesOrders<%=ViewData["elementId"]%>').jqxComboBox({
            theme: theme,
            width: 220,
            height: 22,
            source: dataAdapter,
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            displayMember: "OrderId",
            valueMember: "OrderId",
            search: function (searchString) {
                dataAdapter.dataBind();
            }
        });
        $('#cbbSalesOrders<%=ViewData["elementId"]%>').bind('bindingComplete', function (event) {
            var orderId = '';
            if (orderId != "") {
                fnSetSOItem(orderId, orderId);
            }
        });
        $('#cbbSalesOrders<%=ViewData["elementId"]%>').bind('select', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var item = args.item;
                var value = item.value;
                var label = item.label;
                switch (value) {
                    case "Tìm...":
                        $(this).jqxComboBox('clearSelection');
                        fnShowSalesInvoice();
                        break;
                    default:
                        try {
                        } catch (err) { }
                        break;
                }
            }
        });
    });

    function fnSetSOItem(value, label) {
        $('#cbbSalesOrders<%=ViewData["elementId"]%>').jqxComboBox('insertAt', { value: value, label: label }, 0);
        $('#cbbSalesOrders<%=ViewData["elementId"]%>').jqxComboBox('selectIndex', 0);
    }
</script>
<div id="cbbSalesOrders<%=ViewData["elementId"]%>"></div>
