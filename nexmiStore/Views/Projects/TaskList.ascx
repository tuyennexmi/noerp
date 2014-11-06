<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
%>
<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            var windowId = '<%=windowId%>';
            var source = {
                datatype: "json",
                datafields: [
                    { name: 'TaskId' },
                    { name: 'TaskName' }
                ],
                url: appPath + 'Common/GetTasks',
                formatdata: function (data) {
                    var txtKeyword = $('#txtKeyword' + windowId).val();
                    return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                        sortorder: data.sortorder, txtKeyword: txtKeyword
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
            $('#divTaskList' + windowId).jqxGrid({
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
                    { text: 'ID', datafield: 'CustomerId', width: 100 },
                    { text: 'Tên giai đoạn', datafield: 'CompanyNameInVietnamese' }
                ]
            });

            $('#divTaskList' + windowId).bind("rowdoubleclick", function (event) {
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
            $('#divTaskList' + windowId).on("keyup", function (event) {
                clearTimeout(t);
                t = setTimeout(function () {
                    fnReloadTasks();
                }, 1000);
            });
        });

        function fnReloadTasks() {
            $('#divTaskList' + windowId).jqxGrid('updatebounddata');
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td align="right"><input id="txtKeyword<%=windowId%>" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td><div id="divTaskList<%=windowId%>"></div></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:closeWindow("<%=windowId%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>