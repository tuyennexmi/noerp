<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeywordProduct").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadProduct();
            }, 1000);
        });

        $('#pagination-Product').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnReloadProduct(page);
            }
        });

//        var source = {
//            datatype: "json",
//            datafields: [
//            { name: 'ProductId' },
//            { name: 'ProductCode' },
//            { name: 'Name' },
//            { name: 'Price', type: 'number' },
//            { name: 'Discontinued', type: 'bool' },
//            { name: 'VATRate' }
//        ],
//            url: appPath + 'Common/GetProducts',
//            formatdata: function (data) {
//                var keyword = $('#txtKeywordProduct').val();
//                var categoryId = '';
//                try {
//                    categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
//                } catch (err) { }
//                return { categoryId: categoryId, pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder, keyword: keyword }
//            },
//            root: 'Rows',
//            beforeprocessing: function (data) {
//                source.totalrecords = data.TotalRows;
//            },
//            sort: function () {
//                fnReloadProduct();
//            }
//        };
//        var actionrenderer = function (row, column, value) {
//            var dataRecord = $('#ProductGrid').jqxGrid('getrowdata', row);
//            return '<a href="javascript:fnLoadProductDetail(\'' + dataRecord.ProductId + '\')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" class="gridButtons" /></a>'
//            + '<a href="javascript:fnShowProductForm(\'' + dataRecord.ProductId + '\', \'\')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>'
//            + '<a href="javascript:fnDeleteProduct(\'' + dataRecord.ProductId + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>';
//        }
//        var dataAdapter = new $.jqx.dataAdapter(source);
//        // initialize jqxGrid
//        $('#ProductGrid').jqxGrid({
//            width: '100%',
//            autoheight: true,
//            pagesize: 15,
//            pagesizeoptions: ['15', '20', '30'],
//            source: dataAdapter,
//            theme: theme,
//            //editable: true,
//            pageable: true,
//            sortable: true,
//            rendergridrows: function () {
//                return dataAdapter.records;
//            },
//            virtualmode: true,
//            columnsresize: true,
//            columns: [
//            { text: 'ID', datafield: 'ProductId', width: 100 },
//            { text: 'Mã sản phẩm', datafield: 'ProductCode', width: 120 },
//            { text: '<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %>', datafield: 'Name' },
//            { text: 'Giá sản phẩm', datafield: 'Price', sortable: false, cellsformat: 'f', cellsalign: 'right' },
//            { text: 'Ngưng bán', datafield: 'Discontinued', columntype: 'checkbox' },
//            { text: 'Thuế (%)', datafield: 'VATRate', cellsalign: 'right' },
//            { text: '', datafield: 'link', width: 75, cellsrenderer: actionrenderer, cellsalign: 'right' }
//        ]
//        });

//        $('#ProductGrid').bind('rowdoubleclick', function (event) {
//            var dataRecord = $('#ProductGrid').jqxGrid('getrowdata', event.args.rowindex);
//            fnLoadProductDetail(dataRecord.ProductId);
//        });
//        $('#ProductGrid').on('bindingcomplete', function (event) {
//            $(this).jqxGrid('localizestrings', language);
//        });
    });

//    function fnReloadProduct() {
//        $("#ProductGrid").jqxGrid('updatebounddata');
//    }

    function fnRefreshProduct() {
        $("#txtKeywordProduct").val("");
        //$("#ProductGrid").jqxGrid('removesort');
        fnReloadProduct();
    }

    function ChangeCategory() {
        fnReloadProduct();
    }
</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">
                <input id="txtKeywordProduct" name="txtKeywordProduct" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
            </td>
        </tr>
        <tr>
            <td>
                <%
                string functionId = NEXMI.NMConstant.Functions.Products;
                //Kiểm tra quyền Insert
                if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                {
                %>
                    <button onclick='javascript:fnShowProductForm("", "<%=ViewData["TypeId"]%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                }
                %>
                    <button class="button" onclick="javascript:fnRefreshProduct()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    <button onclick="javascript:fnLoadImportFromExcel('Products')" class="button" ti='Chỉ nhập sản phẩm'>Nhập từ Excel 1</button>
                    <button onclick="javascript:fnLoadImportFromExcel('Mix')" class="button" title='Nhập nhóm sản phẩm và sản phẩm'>Nhập từ Excel 2</button>
                    <button onclick="javascript:fnExport2Excel()" class="button">Xuất ra Excel</button>
            </td>
            <td align="right" style="float: right;">
                <%Html.RenderAction("PaginationJS", "UserControl", new { id = "Product" });%>
                <%Html.RenderPartial("ProductSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%>
        </div>
    </div>
    
    <div class='divContentDetails'>
        <table style="width: 100%">
            <tr>
                <td>
                    <div id="ProductGrid">
                        <%Html.RenderAction("ProductList", "Directories", new { view = ViewData["ViewType"] }); %>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>