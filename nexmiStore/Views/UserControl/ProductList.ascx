<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            var source = {
                datatype: "json",
                datafields: [
                    { name: 'ProductId' },
                    { name: 'ProductCode' },
                    { name: 'Name' },
                    { name: 'Price', type: 'number' },
                    { name: 'Discontinued', type: 'bool' },
                    { name: 'VATRate', type: 'number' },
                    { name: 'CustomerName' }
                ],
                url: appPath + 'Common/GetProducts',
                formatdata: function (data) {
                    var keyword = $('#txtKeyword<%=ViewData["WindowId"]%>').val();
                    return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder, keyword: keyword }
                },
                root: 'Rows',
                beforeprocessing: function (data) {
                    source.totalrecords = data.TotalRows;
                },
                sort: function () {
                    fnReloadProduct();
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(source);
            // initialize jqxGrid
            $('#ProductGrid<%=ViewData["WindowId"]%>').jqxGrid({
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
                    { text: '#', datafield: 'ProductId', width: 100 },
                    { text: 'Mã sản phẩm', datafield: 'ProductCode', width: 100 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %>', datafield: 'Name' },
                    { text: 'Giá sản phẩm', datafield: 'Price', sortable: false, cellsformat: 'f', cellsalign: 'right' },
                    { text: 'Ngưng bán', datafield: 'Discontinued', columntype: 'checkbox' },
                    { text: 'Thuế (%)', datafield: 'VATRate', cellsalign: 'right' },
                    { text: 'Nhà sản xuất', datafield: 'CustomerName', sortable: false }
                ]
            });

            $('#ProductGrid<%=ViewData["WindowId"]%>').bind('rowdoubleclick', function (event) {
                var dataRecord = $('#ProductGrid<%=ViewData["WindowId"]%>').jqxGrid('getrowdata', event.args.rowindex);
                try {
                    $("#txtProductId").val(dataRecord.ProductId);
                    $("#txtProductName").val(dataRecord.ProductNameInVietnamese);
                } catch (err) { }
                try {
                    fnSetItemForCombobox('<%=ViewData["ParentId"]%>', dataRecord.ProductId, Base64.encode(dataRecord.Name));
                } catch (err) { }
                closeWindow('<%=ViewData["WindowId"]%>');
            });

            var t;
            $('#txtKeyword<%=ViewData["WindowId"]%>').on('keyup', function (event) {
                clearTimeout(t);
                t = setTimeout(function () {
                    fnReloadProduct();
                }, 1000);
            });
        });

        function fnReloadProduct() {
            $('#ProductGrid<%=ViewData["WindowId"]%>').jqxGrid('updatebounddata');
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td align="right"><input id="txtKeyword<%=ViewData["WindowId"]%>" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td><div id="ProductGrid<%=ViewData["WindowId"]%>"></div></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>

