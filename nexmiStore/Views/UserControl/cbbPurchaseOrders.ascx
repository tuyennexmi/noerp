<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(document).ready(function () {
        var source = {
//            toggleMode: 'none',
            datatype: "json",
            datafields: [
                { name: 'Id' }
            ],
            id: 'Id',
            url: appPath + "Common/GetPurchaseOrdersForAutocomplete"
        };
        var dataAdapter = new $.jqx.dataAdapter(source, {
            formatData: function (data) {
                return { keyword: $('#cbbPurchaseOrders<%=ViewData["elementId"]%>').jqxComboBox('searchString'), limit: 10, mode: "select" }
            }
        });
        $('#cbbPurchaseOrders<%=ViewData["elementId"]%>').jqxComboBox({
            theme: theme,
            width: 220,
            height: 22,
            source: dataAdapter,
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            displayMember: "Id",
            valueMember: "Id",
            search: function (searchString) {
                dataAdapter.dataBind();
            }
        });
        $('#cbbPurchaseOrders<%=ViewData["elementId"]%>').bind('bindingComplete', function (event) {
            var orderId = '';
            if (orderId != "") {
                fnSetPOItem(orderId, orderId);
            }
        });
        $('#cbbPurchaseOrders<%=ViewData["elementId"]%>').bind('select', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var item = args.item;
                var value = item.value;
                var label = item.label;
                switch (value) {
                    case "Tìm...":
                        fnShowPurchaseOrder();
                        $(this).jqxComboBox('clearSelection');
                        break;
                    default:
                        try {
                        } catch (err) { }
                        break;
                }
            }
        });
    });

    function fnSetPOItem(value, label) {
        $('#cbbPurchaseOrders<%=ViewData["elementId"]%>').jqxComboBox('insertAt', { value: value, label: label }, 0);
        $('#cbbPurchaseOrders<%=ViewData["elementId"]%>').jqxComboBox('selectIndex', 0);
    }
</script>
<div id="cbbPurchaseOrders<%=ViewData["elementId"]%>"></div>
