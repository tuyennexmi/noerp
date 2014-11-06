<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/ProductCategories.js" type="text/javascript"></script>
<% 
    string elementId = ViewData["ElementId"].ToString();
    string objectName = ViewData["ObjectName"].ToString();
    string promptText = "";
    switch (objectName)
    {
        case "Products": promptText = NEXMI.NMCommon.GetInterface("KIND", langId); break;
        case "Introduce": promptText = ""; break;
    }
    string id = "", name = "";
    try
    {
        id = ViewData["CurrentId"].ToString();
    }
    catch { }
    try
    {
        name = NEXMI.NMCryptography.base64Encode(ViewData["CurrentName"].ToString());
    }
    catch { }
%>
<script type="text/javascript">
    $(function () {
        $('#cbbCategories<%=elementId%>').jqxComboBox({
            theme: theme,
            width: 220,
            height: 22,
            source: new $.jqx.dataAdapter({
                datatype: "json",
                datafields: [
                { name: 'Id' },
                { name: 'Name' },
                { name: 'FullName' }
            ],
                id: 'Id',
                url: appPath + 'Common/GetPCForAutocomplete'
            }, {
                formatData: function (data) {
                    var keyword = $('#cbbCategories<%=elementId%>').jqxComboBox('searchString');
                    return { keyword: keyword, limit: 10, mode: 'select', objectName: '<%=objectName%>' }
                }
            }),
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            displayMember: 'FullName',
            valueMember: 'Id',
            promptText: '<%=promptText%>',
            search: function (searchString) {
                var dataAdapter = $('#cbbCategories<%=elementId%>').jqxComboBox('source');
                dataAdapter.dataBind();
            }
        });
        $('#cbbCategories<%=elementId%>').on('bindingComplete', function (event) {
            if ('<%=id%>' != '') {
                fnSetItemForCombobox('cbbCategories<%=elementId%>', '<%=id%>', '<%=name%>');
            }
        });
        $('#cbbCategories<%=elementId%>').on('change', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var dataAdapter = $('#cbbCategories<%=elementId%>').jqxComboBox('source');
                var item = dataAdapter.records[index];
                if (item != null) {
                    switch (item.Id) {
                        case "Search":
                            $(this).jqxComboBox('clearSelection');
                            fnShowCategories('cbbCategories<%=elementId%>', '<%=objectName%>');
                            break;
                        case "CreateOrEdit":
                            $(this).jqxComboBox('clearSelection');
                            fnPopupPCDialog('', 'cbbCategories<%=elementId%>', '<%=objectName%>');
                            break;
                        default:
                            try {
                                ChangeCategory();
                            } catch (err) { }
                            break;
                    }
                } else {
                    $(this).jqxComboBox('clearSelection');
                    try {
                        ChangeCategory();
                    } catch (err) { }
                }
            }
        });
    });
</script>
<div id="cbbCategories<%=elementId%>"></div>
