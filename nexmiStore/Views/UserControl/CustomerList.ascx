<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            var source = {
                datatype: "json",
                datafields: [
                    { name: 'CustomerId' },
                    { name: 'CompanyNameInVietnamese' },
                    { name: 'TaxCode' },
                    { name: 'Address' },
                    { name: 'Telephone' },
                    { name: 'FaxNumber' },
                    { name: 'EmailAddress' }
                ],
                url: appPath + 'Common/GetCustomers',
                formatdata: function (data) {
                    var txtKeyword = $("#txtKeywordCustomerList").val();
                    return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                        sortorder: data.sortorder, groupId: '<%=ViewData["GroupId"]%>', txtKeyword: txtKeyword
                    }
                },
                root: 'Rows',
                beforeprocessing: function (data) {
                    source.totalrecords = data.TotalRows;
                },
                sort: function () {
                    fnReloadCustomers();
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(source);
            // initialize jqxGrid
            $("#divCustomerList").jqxGrid({
                width: '100%',
                autoheight: true,
                pagesize: 15,
                pagesizeoptions: ['15', '20', '30'],
                source: dataAdapter,
                theme: theme,
                pageable: true,
                sortable: true,
                rendergridrows: function () {
                    return dataAdapter.records;
                },
                virtualmode: true,
                columnsresize: true,
                columns: [
                    { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER_CODE", langId) %>', datafield: 'CustomerId', width: 100 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>', datafield: 'CompanyNameInVietnamese' },
                    { text: '<%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %>', datafield: 'TaxCode', sortable: false, width: 100 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>', datafield: 'Address', sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>', datafield: 'Telephone', sortable: false, width: 120 },
                    { text: 'Fax', datafield: 'FaxNumber', sortable: false, width: 120 },
                    { text: 'Email', datafield: 'EmailAddress', sortable: false, width: 120 }
                ]
            });

            $("#divCustomerList").bind("rowdoubleclick", function (event) {
                var dataRecord = $("#divCustomerList").jqxGrid('getrowdata', event.args.rowindex);
                try {
                    $("#txtCustomerId").val(dataRecord.CustomerId);
                    $("#txtCustomerName").val(dataRecord.CompanyNameInVietnamese);
                } catch (err) { }
                try {
                    fnSetItemForCombobox('cbbCustomers<%=ViewData["ParentId"]%>', dataRecord.CustomerId, Base64.encode(dataRecord.CompanyNameInVietnamese));
                } catch (err) { }
                try {
                    $('#<%=ViewData["ParentId"]%>').val(dataRecord.CustomerId);
                } catch (err) { }
                closeWindow('<%=ViewData["WindowId"]%>');
            });

            var t;
            $("#txtKeywordCustomerList").on("keyup", function (event) {
                clearTimeout(t);
                t = setTimeout(function () {
                    fnReloadCustomers();
                }, 1000);
            });
        });

        function fnReloadCustomers() {
            $("#divCustomerList").jqxGrid('updatebounddata');
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td align="right"><input id="txtKeywordCustomerList" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td><div id="divCustomerList"></div></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>