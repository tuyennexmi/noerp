//function fnReloadImport() {
//    $("#ImportGrid").jqxGrid('updatebounddata');
//}

//function fnReloadExport() {
//    $("#ExportGrid").jqxGrid('updatebounddata');
//}

//function fnRefreshImport() {
//    $("#txtImportKeyword").val("");
//    $("#ImportGrid").jqxGrid('removesort');
//    fnReloadImport();
//}

//function fnRefreshExport() {
//    $("#txtImportKeyword").val("");
//    $("#ExportGrid").jqxGrid('removesort');
//    fnReloadExport();
//}

function fnReload() {
    fnReloadExport();
    fnReloadImport();
}

function fnRefresh(exType, imType) {
    fnRefreshExport(exType);
    fnRefreshImport(imType);
}

function fnLoadTransferForm(id, mode) {
    LoadContent('', 'Stocks/TransferProductForm');
}

function fnTransfer() {
    if ($('#tbodyDetails tr').children().length > 0) {
        var fromStock = $('#slStocks').val();
        var toStock = $('#slToStock').val();
        var txtDate = $('#txtDate').val();
        if (txtDate == '') {
            alert('Vui lòng chọn ngày chuyển'); return;
        }
        var carrierId = '';
        try {
            carrierId = $('#cbbCustomers').jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        $.ajax({
            type: 'POST',
            url: appPath + 'Stocks/Transfer',
            data: {
                fromStock: fromStock,
                toStock: toStock,
                txtDate: txtDate,
                carrierId: carrierId,
                transport: $('#txtTransport').val(),
                description: $('#txtDescription').val()
            },
            success: function (data) {
                if (data.error == '') {
                    LoadContent('', 'Stocks/Export?id=' + data.id);
                }
                else {
                    alert(data.error);
                }
            }
        });
    } else {
        alert('Không có sản phẩm nào.');
    }
}

function fnLoadImportDetail(id) {
    LoadContent("", "Stocks/Import?id=" + id);
}

function fnLoadExportDetail(id) {
    LoadContent("", "Stocks/Export?id=" + id);
}

function fnPopupExportDialog(id, mode) {
    LoadContent("", "Stocks/ExportForm?exportId=" + id + "&mode=" + mode);
}

function fnPopupImportDialog(id, mode) {
    LoadContent("", "Stocks/ImportForm?id=" + id + "&mode=" + mode);
}

function fnDeleteExport(id) {
    var answer = confirm("Bạn muốn xóa phiếu này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Stocks/DeleteExport", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnReloadExport();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnDeleteImport(id) {
    var answer = confirm("Bạn muốn xóa phiếu này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Stocks/DeleteImport", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnReloadImport();
            }
            else {
                alert(data);
            }
        });
    }
}