<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(function () {
        var source = {
            datatype: 'json',
            datafields: [
                { name: 'Id' },
                { name: 'Name' },
                { name: 'FullName' }
            ],
            id: 'Id',
            url: appPath + 'Common/GetAreaForAutocomplete'
        };
        var dataAdapter = new $.jqx.dataAdapter(source, {
            formatData: function (data) {
                return { keyword: $('#cbbAreas<%=ViewData["ElementId"]%>').jqxComboBox('searchString'), limit: 10, mode: 'select' }
            }
        });
        $('#cbbAreas<%=ViewData["ElementId"]%>').jqxComboBox({
            theme: theme,
            width: 220,
            height: 22,
            source: dataAdapter,
            minLength: 0,
            autoDropDownHeight: true,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            promptText: 'Gõ chọn khu vực...',
            displayMember: 'FullName',
            valueMember: 'Id',
            search: function (searchString) {
                dataAdapter.dataBind();
            }
        });
        $('#cbbAreas<%=ViewData["ElementId"]%>').bind('bindingComplete', function (event) {
            var id = '<%=ViewData["AreaId"]%>';
            var name = '<%=ViewData["AreaName"]%>';
            if (id != null && id != '') {
                fnSetItemForCombobox('cbbAreas<%=ViewData["ElementId"]%>', id, Base64.encode(name));
            }
        });
        $('#cbbAreas<%=ViewData["ElementId"]%>').bind('select', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var item = args.item;
                var value = item.value;
                var label = item.label;
                switch (value) {
                    case 'Search':
                        $('#cbbAreas<%=ViewData["ElementId"]%>').jqxComboBox('clearSelection');
                        fnShowAreas();
                        break;
                    default:
                        try {
                            fnLoadCustomers();
                        } catch (err) { }
                        break;
                }
            }
        });
    });
</script>
<div id="cbbAreas<%=ViewData["ElementId"]%>"></div>
