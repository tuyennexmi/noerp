<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    string elementId = ViewData["ElementId"].ToString();
%>
<script type="text/javascript">
    $(function () {
        $('#<%=elementId%>').jqxComboBox({
            theme: theme,
            width: 220,
            height: 22,
            source: new $.jqx.dataAdapter(source = {
                datatype: "json",
                datafields: [
                { name: 'ProductId' },
                { name: 'ProductNameInVietnamese' }
            ],
                id: 'ProductId',
                url: appPath + "Common/GetProductForAutocomplete"
            }, {
                formatData: function (data) {
                    return { keyword: $('#<%=elementId%>').jqxComboBox('searchString'), limit: 10, mode: "select" }
                }
            }),
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            displayMember: "ProductNameInVietnamese",
            valueMember: "ProductId",
            search: function (searchString) {
                $('#<%=elementId%>').jqxComboBox('source').dataBind();
            }
        });
        var productId = '<%=ViewData["Value"]%>';
        var productName = '<%=ViewData["Label"]%>';
        $('#<%=elementId%>').bind('bindingComplete', function (event) {
            if (productId != '' && productId != null) {
                fnSetItemForCombobox('<%=elementId%>', productId, Base64.encode(productName));
            }
        });
        $('#<%=elementId%>').on('change', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var item = args.item;
                var value = item.value;
                var label = item.label;
                switch (value) {
                    case "Search":
                        fnShowProducts('<%=elementId%>');
                        $('#<%=elementId%>').jqxComboBox('clearSelection');
                        break;
                    case "CreateOrEdit":
                        fnPopupProductDialog('', '<%=elementId%>');
                        $('#<%=elementId%>').jqxComboBox('clearSelection');
                        break;
                    default:
                        if (value != "" && value != undefined) {
                            fnGetProductInStock(value);
                        }                        
                        break;
                }
            }
        });
    });

</script>
<div id="<%=ViewData["ElementId"]%>"></div>
