<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="<%=ViewData["ElementId"]%>"></div>

<script type="text/javascript">
    $(function () {
        $('#<%=ViewData["ElementId"]%>').jqxComboBox({
            theme: theme,
            width: 220,
            height: 22,
//            toggleMode: 'none',
            source: new $.jqx.dataAdapter({
                datatype: 'json',
                datafields: [
                { name: 'TaskId' },
                { name: 'TaskName' }
            ],
                id: 'TaskId',
                url: appPath + 'Common/GetTaskForAutocomplete'
            }, {
                formatData: function (data) {
                    data.keyword = $('#<%=ViewData["ElementId"]%>').jqxComboBox('searchString');
                    data.limit = 10;
                    data.mode = '<%=ViewData["Mode"]%>';
                    return data;
                }
            }),
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            displayMember: 'TaskName',
            valueMember: 'TaskId',
            search: function (searchString) {
                var dataAdapter = $('#<%=ViewData["ElementId"]%>').jqxComboBox('source');
                dataAdapter.dataBind();
            }
        });
        $('#<%=ViewData["ElementId"]%>').bind('bindingComplete', function (event) {
            var id = '<%=ViewData["Id"]%>';
            var label = '<%=ViewData["Label"]%>';
            if (id != '') {
                fnSetItemForCombobox('<%=ViewData["ElementId"]%>', id, Base64.encode(label));
            }
        });
        $('#<%=ViewData["ElementId"]%>').bind('change', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var dataAdapter = $('#<%=ViewData["ElementId"]%>').jqxComboBox('source');
                var item = dataAdapter.records[index];
                if (item != null) {
                    switch (item.TaskId) {
                        case 'Search':
                            $(this).jqxComboBox('clearSelection');
                            break;
                        case 'CreateOrEdit':
                            fnPopupTaskDialog('', '', '<%=ViewData["ElementId"]%>');
                            $(this).jqxComboBox('clearSelection');
                            break;
                        default:
                            try {
                                cbbTaskChanged(item);
                            } catch (err) { }
                            break;
                    }
                } else
                    $(this).jqxComboBox('clearSelection');
            }
        });
    });
</script>
