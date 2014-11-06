<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    var flag = '<%=Session["Flag"]%>';
    $(function () {
        var t;
        $("#txtKeywordPIS").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadPIS();
            }, 1000);
        });

        var source = {
            datatype: "json",
            datafields: [
                    { name: 'STT' },
            //{ name: 'StockName' },
                    {name: 'ProductId' },
                    { name: 'ProductNameInVietnamese' },
                    { name: 'GoodQuantity', type: 'number' },
                    { name: 'BadQuantity', type: 'number' },
                    { name: 'CurrentQuantity', type: 'number' },
                    { name: 'ExportQuantity', type: 'number' }
                ],
            url: appPath + 'Common/GetProductInStocksNoPaging',
            formatdata: function (data) {
                var categoryId = "";
                try {
                    categoryId = $("#cbbCategories").jqxComboBox('getSelectedItem').value;
                } catch (err) { }
                return { keyword: $("#txtKeywordPIS").val(), categoryId: categoryId }
            }
        };
        var dataAdapter = new $.jqx.dataAdapter(source);
        $("#PISGrid").jqxGrid({
            width: '100%',
            autoheight: true,
            source: dataAdapter,
            theme: theme,
            editable: true,
            columnsresize: true,
            columns: [
                    { text: 'STT', datafield: 'STT', width: 50, editable: false },
            //{ text: '<%= NEXMI.NMCommon.GetInterface("STORE", langId) %> hàng', datafield: 'StockName', width: 200, editable: false },
                    {text: 'Mã sản phẩm', datafield: 'ProductId', width: 100, editable: false },
                    { text: '<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %>', datafield: 'ProductNameInVietnamese', editable: false },
                    { text: 'Tồn theo sổ sách', datafield: 'GoodQuantity', cellsformat: 'f', cellsalign: 'right', width: 150, editable: false },
                    { text: 'Tồn thực trên kệ', datafield: 'CurrentQuantity', cellsformat: 'f', cellsalign: 'right', width: 150, editable: true, columntype: 'textbox' },
                    { text: 'SL bán', datafield: 'ExportQuantity', cellsformat: 'f', cellsalign: 'right', width: 150, editable: false }
                ]
        });

        $("#PISGrid").on('cellendedit', function (event) {
            var row = args.rowindex;
            var value = args.value;
            if (isNaN(value) || value < 0) {
                alert("Vui lòng nhập đúng số lượng thực!");
                $(this).jqxGrid('setcellvalue', row, "ExportQuantity", '');
                $(this).jqxGrid('setcellvalue', row, "CurrentQuantity", '');
                return;
            }
            else {
                var dataRecord = $(this).jqxGrid('getrowdata', row);
                var quantity = dataRecord.GoodQuantity;
                if (quantity < 0) {
                    alert("Số lượng sổ sách đang âm, vui lòng kiểm tra lại việc nhập kho!");
                    return;
                }
                else {
                    var exportQuantity = quantity - value;
                    if (value > quantity) {
                        alert("Số lượng Tồn thực tế phải nhỏ hơn hoặc bằng với sổ sách!");
                        $(this).jqxGrid('setcellvalue', row, 'ExportQuantity', 0);
                        $(this).jqxGrid('setcellvalue', row, 'CurrentQuantity', value);
                        return;
                    }
                    //if (value >= 0 && quantity > 0 && exportQuantity >= 0) {
                    if (exportQuantity >= 0) {
                        fnSaveOne(row, dataRecord.ProductId, exportQuantity);
                        //$(this).jqxGrid('setcellvalue', row, "CurrentQuantity", quantity);
                        //            } else {
                        //                $(this).jqxGrid('setcellvalue', row, 'CurrentQuantity', '');
                        //                $(this).jqxGrid('setcellvalue', row, 'ExportQuantity', '');
                    } 
                }
            }
        });
    });

    function fnReloadPIS() {
        //$.post(appPath + "Common/ClearSession", { sessionName: "InventoryDetails" });
        $("#PISGrid").jqxGrid('updatebounddata');
    }

    function fnSaveOne(row, productId, exportQuantity) {
        $.post(appPath + "Sales/AddToInventoryList", { productId: productId, quantity: exportQuantity }, function (data) {
            if (data != "")
                alert(data);
            else {
                flag = 'OK';
                $("#PISGrid").jqxGrid('setcellvalue', row, "ExportQuantity", exportQuantity);
            }
        });
    }

    function fnSaveAll() {
        if (flag == 'OK')
            openWindow('Thông tin giao ca', 'Sales/InventoryForm', {}, 500, 400);
        else
            alert('Không có sản phẩm nào!');
    }

    function ChangeCategory() {
        fnReloadPIS();
    }
</script>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordPIS" name="txtKeywordPIS" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <%
                            string functionId = NEXMI.NMConstant.Functions.ProductInStock;
                        %>
                        <button class="button red" onclick="javascript:fnSaveAll()"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button class="button" onclick="javascript:fnReloadPIS()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    </div>
                </td>
                <td align="right">
                        
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
    <div class="divStatus">
    </div>
    <div class='divContentDetails'>
    <table style="width: 100%">
        <tr>
            <td>
                <%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%>
            </td>            
        </tr>
        <tr>
            <td><div id="PISGrid"></div></td>
        </tr>
    </table>
    </div>
    </div>
</div>
