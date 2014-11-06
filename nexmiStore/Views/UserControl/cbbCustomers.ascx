<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    string windowTitle = ""; string holderText = "";
    if (ViewData["GroupId"] == NEXMI.NMConstant.CustomerGroups.Manufacturer)
    {
        windowTitle = "Danh sách nhà sản xuất";
        holderText = "Gõ chọn nhà sản xuất";
    }
    else if (ViewData["GroupId"] == NEXMI.NMConstant.CustomerGroups.Supplier)
    {
        windowTitle = "Danh sách nhà cung cấp";
        holderText = "Gõ chọn nhà cung cấp";
    }
    else if (ViewData["GroupId"] == NEXMI.NMConstant.CustomerGroups.User)
    {
        windowTitle = "Danh sách người dùng";
        holderText = "Gõ chọn người dùng";
    }
    else if (ViewData["GroupId"] == NEXMI.NMConstant.CustomerGroups.User)
    {
        windowTitle = "Danh sách khách hàng";
        holderText = "Gõ chọn khách hàng";
    }
    else
    {
        windowTitle = "Danh sách đối tác";
        holderText = "Gõ chọn đối tác";
    }
          
%>

<script type="text/javascript">
    $(function () {
        $('#cbbCustomers<%=ViewData["ElementId"]%>').jqxComboBox({
            theme: theme,
            width: 220,
            height: 22,
            source: new $.jqx.dataAdapter({
                datatype: 'json',
                datafields: [
                { name: 'CustomerId' },
                { name: 'CompanyNameInVietnamese' },
                { name: 'EmailAddress' }
            ],
                id: 'CustomerId',
                url: appPath + 'Common/GetCustomerForAutocomplete'
            }, {
                formatData: function (data) {
                    return { keyword: $('#cbbCustomers<%=ViewData["ElementId"]%>').jqxComboBox('searchString'),
                        cgroupId: '<%=ViewData["GroupId"]%>', limit: 10, mode: "select"
                    }
                }
            }),
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            promptText: '<%=holderText%>',
            displayMember: 'CompanyNameInVietnamese',
            valueMember: 'CustomerId',
            search: function (searchString) {
                var dataAdapter = $('#cbbCustomers<%=ViewData["ElementId"]%>').jqxComboBox('source');
                dataAdapter.dataBind();
            }
        });
        $('#cbbCustomers<%=ViewData["ElementId"]%>').on('bindingComplete', function (event) {
            var customerId = '<%=ViewData["CustomerId"]%>';
            var customerName = '<%=ViewData["CustomerName"]%>';
            if (customerId != '') {
                fnSetItemForCombobox('cbbCustomers<%=ViewData["ElementId"]%>', customerId, Base64.encode(customerName));
            }
            try {
                fnCallValidate("formSalesInvoice");
            } catch (err) { }
            try {
                fnCallValidate("formSalesOrder");
            } catch (err) { }
            try {
                fnCallValidate("formQuotation");
            } catch (err) { }
        });
        $('#cbbCustomers<%=ViewData["ElementId"]%>').on('change', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var dataAdapter = $('#cbbCustomers<%=ViewData["ElementId"]%>').jqxComboBox('source');
                var item = dataAdapter.records[index];
                if (item != null) {
                    switch (item.CustomerId) {
                        case 'Search':
                            fnShowCustomers('<%=ViewData["ElementId"]%>', '<%=windowTitle%>', '<%=ViewData["GroupId"]%>');
                            $(this).jqxComboBox('clearSelection');
                            break;
                        case 'CreateOrEdit':
                            fnPopupCustomerDialog('', '<%=ViewData["GroupId"]%>', '<%=ViewData["ElementId"]%>');
                            $(this).jqxComboBox('clearSelection');
                            break;
                        default:
                            try {
                                cbbCustomerChanged();
                            } catch (err) { }
                            break;
                    }
                } else {
                    $(this).jqxComboBox('clearSelection');
                    try {
                        cbbCustomerChanged(null);
                    } catch (err) { }
                }
            }
        });
    });
</script>
<div id="cbbCustomers<%=ViewData["ElementId"]%>"></div>