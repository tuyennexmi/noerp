<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            var source = {
                datatype: "json",
                datafields: [
                    { name: 'DateOfPrice' },
                    { name: 'Price' },
                    { name: 'CreatedBy' }
                ],
                url: appPath + 'Common/GetPriceForSales',
                formatdata: function (data) {
                    return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder, productId: '<%=ViewData["ProductId"]%>' }
                },
                root: 'Rows',
                beforeprocessing: function (data) {
                    source.totalrecords = data.TotalRows;
                },
                sort: function () {
                    fnReloadCostPriceGrid();
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(source);
            // initialize jqxGrid
            $("#CostPriceGrid").jqxGrid({
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
                    { text: 'Ngày', datafield: 'DateOfPrice'},
                    { text: 'Giá bán', datafield: 'Price' },
                    { text: 'Người cập nhật', datafield: 'CreatedBy' },
                ]
            });

            $("#CostPriceGrid").bind("rowdoubleclick", function (event) {
                //                var dataRecord = $("#CostPriceGrid").jqxGrid('getrowdata', event.args.rowindex);
                //                try {
                //                    fnSetItemForCombobox('<%=ViewData["ParentId"]%>', dataRecord.Id, Base64.encode(dataRecord.FullName));
                //                } catch (err) { }
                //                closeWindow('<%=ViewData["WindowId"]%>');
            });
        });

        function fnReloadCostPriceGrid() {
            $("#CostPriceGrid").jqxGrid('updatebounddata');
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td align="right"></td>
        </tr>
        <tr>
            <td><div id="CostPriceGrid"></div></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>