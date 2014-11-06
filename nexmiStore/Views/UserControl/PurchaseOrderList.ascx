<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            var source = {
                datatype: "json",
                datafields: [
                    { name: 'OrderId' },
                    { name: 'CustomerId' },
                    { name: 'CustomerName' },
                    { name: 'Details' },
                    { name: 'OrderStatus' },
                    { name: 'Note' },
                    { name: 'CreatedBy' }
                ],
                url: appPath + 'Common/GetPurchaseOrders',
                formatdata: function (data) {
                    var keyword = $("#txtKeywordPOList").val();
                    return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder,
                        keyword: keyword
                    }
                },
                root: 'Rows',
                beforeprocessing: function (data) {
                    source.totalrecords = data.TotalRows;
                },
                sort: function () {
                    $("#POList").jqxGrid('updatebounddata');
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(source);
            // initialize jqxGrid
            $("#POList").jqxGrid({
                width: '100%',
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
                    { text: '<%= NEXMI.NMCommon.GetInterface("ID", langId) %> ', datafield: 'OrderId', width: 150 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER_CODE", langId) %>', datafield: 'CustomerId', sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>', datafield: 'CustomerName', width: 200, sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %>', datafield: 'Details', width: 80, sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("STATUS", langId) %>', datafield: 'OrderStatus', width: 80 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>', datafield: 'Note', sortable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %>', datafield: 'CreatedBy', sortable: false, width: 120 }
                ]
            });
            $("#POList").bind('rowdoubleclick', function (event) {
                var dataRecord = $("#POList").jqxGrid('getrowdata', event.args.rowindex);
                try {
                    fnGetPurchaseOrderDetails(dataRecord.OrderId, '<%=ViewData["WindowId"]%>')
                } catch (Error) { }
            });

            var t;
            $("#txtKeywordPOList").on("keyup", function (event) {
                clearTimeout(t);
                t = setTimeout(function () {
                    fnReloadPOList();
                }, 1000);
            });
        });

        function fnReloadPOList() {
            $("#POList").jqxGrid('updatebounddata');
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td align="right"><input id="txtKeywordPOList" name="txtKeywordPOList" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td><div id="POList"></div></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>