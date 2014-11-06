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
                    { name: 'Id' },
                    { name: 'Name' },
                    { name: 'Purpose' },
                    { name: 'Criteria' },
                    { name: 'WorkGuids' }
                ],
                id: 'Id',
                url: appPath + 'UserControl/GetJobsForCBB'
            }, {
                formatData: function (data) {
                    data.keyword = $('#<%=ViewData["ElementId"]%>').jqxComboBox('searchString');
                    data.limit = 10;
                    data.mode = 'select';
                    return data;
                }
            }),
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            displayMember: 'Name',
            valueMember: 'Id',
            search: function (searchString) {
                var dataAdapter = $('#<%=ViewData["ElementId"]%>').jqxComboBox('source');
                dataAdapter.dataBind();
            }
        });
        $('#<%=ViewData["ElementId"]%>').on('bindingComplete', function (event) {
            var id = '<%=ViewData["Id"]%>';
            var label = '<%=ViewData["Label"]%>';
            if (id != '') {
                fnSetItemForCombobox('<%=ViewData["ElementId"]%>', id, Base64.encode(label));
            }
        });
        $('#<%=ViewData["ElementId"]%>').on('change', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var dataAdapter = $('#<%=ViewData["ElementId"]%>').jqxComboBox('source');
                var item = dataAdapter.records[index];
                if (item != null) {
                    switch (item.Id) {
                        case 'Search':
                            //fnShowCustomers('<%=ViewData["ElementId"]%>', '<%=ViewData["WindowTitle"]%>', '<%=ViewData["GroupId"]%>');
                            $(this).jqxComboBox('clearSelection');
                            break;
                        case 'CreateOrEdit':
                            //fnPopupProjectDialog('', '<%=ViewData["ElementId"]%>');
                            $(this).jqxComboBox('clearSelection');
                            break;
                        default:
                            try {
                                cbbJobChanged(item);
                            } catch (err) { }
                            break;
                    }
                } else {
                    $(this).jqxComboBox('clearSelection');
                    cbbJobChanged(null);
                }
            }
        });
    });

    
</script>
