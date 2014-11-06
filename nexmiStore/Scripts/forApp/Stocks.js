var counter = 0;

$(document).ready(function () {
    
});

function fnReloadStock() {
    $("#StockGrid").jqxGrid('updatebounddata');
}

function fnRefreshStock() {
    $("#txtKeywordStock").val("");
    $("#StockGrid").jqxGrid('removesort');
    fnReloadStock();
}

function fnPopupStockDialog(id) {
    openWindow("Thông tin kho - bãi", "Stocks/StockForm", { id: id }, 700, 450);
}

function fnShowStockForm(id) {
    LoadContent('', 'Stocks/StockForm?id=' + id);
}

function fnDeleteStock(id) {
    var answer = confirm("Bạn muốn xóa kho này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Stocks/DeleteStock", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnRefreshStock();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnCloseFormStock(id) {
    closeWindow(id);
    fnRefreshStock();
}