<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            var source = {
                datatype: "json",
                datafields: [
                    { name: 'InvoiceId' },
                    { name: 'InvoiceBatchId' },
                    { name: 'CustomerId' },
                    { name: 'CustomerName' },
                    { name: 'Details' },
                    { name: 'InvoiceStatus' },
                    { name: 'Note' },
                    { name: 'CreatedBy' }
                ],
                url: appPath + 'Common/GetPurchaseInvoices',
                formatdata: function (data) {
                    var keyword = $("#txtKeywordPIList").val();
                    return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder,
                        keyword: keyword
                    }
                },
                root: 'Rows',
                beforeprocessing: function (data) {
                    source.totalrecords = data.TotalRows;
                },
                sort: function () {
                    $("#PIList").jqxGrid('updatebounddata');
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(source);
            // initialize jqxGrid
            $("#PIList").jqxGrid({
                width: '98%',
                autoheight: true,
                pagesize: 15,
                pagesizeoptions: ['15', '20', '30'],
                source: dataAdapter,
                theme: theme,
                enabletooltips: true,
                pageable: true,
                sortable: true,
                rendergridrows: function () {
                    return dataAdapter.records;
                },
                virtualmode: true,
                columnsresize: true,
                columns: [
                    { text: '<%= NEXMI.NMCommon.GetInterface("ID", langId) %> ', datafield: 'InvoiceId', width: 150 },
                    { text: 'Lô hóa đơn', datafield: 'InvoiceBatchId', width: 150 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER_CODE", langId) %>', datafield: 'CustomerId', sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>', datafield: 'CustomerName', width: 200, sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %>', datafield: 'Details', width: 80, sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("STATUS", langId) %>', datafield: 'InvoiceStatus', width: 80 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>', datafield: 'Note', sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %>', datafield: 'CreatedBy', sortable: false, width: 120 }
                ]
            });
            $("#PIList").bind('rowdoubleclick', function (event) {
                var dataRecord = $("#PIList").jqxGrid('getrowdata', event.args.rowindex);
                try {
                    fnGetPurchaseInvoiceDetails(dataRecord.InvoiceId, dataRecord.CustomerId, dataRecord.CustomerName)
                } catch (Error) { }
            });

            var t;
            $("#txtKeywordPIList").on("keyup", function (event) {
                clearTimeout(t);
                t = setTimeout(function () {
                    fnReloadPIList();
                }, 1000);
            });
        });

        function fnReloadPIList() {
            $("#PIList").jqxGrid('updatebounddata');
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td align="right"><input id="txtKeywordPIList" name="txtKeywordPIList" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td><div id="PIList"></div></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>