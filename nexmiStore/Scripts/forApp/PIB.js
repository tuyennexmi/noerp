var counter = 0;
var idTemp = "";

$(document).ready(function () {
    var source = {
        datatype: "json",
        datafields: [
            { name: 'InvoiceBatchId' },
            { name: 'InvoiceBatchDate' },
            { name: 'BatchStatus' },
            { name: 'Description' },
            { name: 'CreatedUserName' },
            { name: 'CreatedDate' }
        ],
        url: appPath + 'Common/GetPurchaseInvoiceBatchs',
        formatdata: function (data) {
            var keyword = $("#txtKeywordPIB").val();
            return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                sortorder: data.sortorder, keyword: keyword
            }
        },
        root: 'Rows',
        beforeprocessing: function (data) {
            source.totalrecords = data.TotalRows;
        },
        sort: function () {
            fnReloadPIB();
        },
        deleterow: function (rowid, commit) {
            var dataRecord = $("#PIBGrid").jqxGrid('getrowdata', rowid);
            fnDeleteCustomer(dataRecord.InvoiceBatchId);
        }
    };
    var dataAdapter = new $.jqx.dataAdapter(source);
    // initialize jqxGrid
    $("#PIBGrid").jqxGrid({
        width: '98%',
        autoheight: true,
        pagesize: 15,
        pagesizeoptions: ['15', '20', '30'],
        source: dataAdapter,
        theme: theme,
        pageable: true,
        sortable: true,
        enabletooltips: true,
        rendergridrows: function () {
            return dataAdapter.records;
        },
        virtualmode: true,
        columnsresize: true,
        columns: [
            { text: 'Số phiếu', datafield: 'InvoiceBatchId', width: 150 },
            { text: 'Ngày', datafield: 'InvoiceBatchDate', width: 120 },
            { text: 'Trạng thái', datafield: 'BatchStatus', sortable: false, width: 100 },
            { text: 'Ghi chú', datafield: 'Description', sortable: false },
            { text: 'Người tạo', datafield: 'CreatedUserName', sortable: false, width: 120 },
            { text: 'Ngày tạo', datafield: 'CreatedDate', sortable: false, width: 150 }
        ]
    });

    // create context menu
    var contextMenu = $("#PIBMenu").jqxMenu({ width: 200, height: 'auto', autoOpenPopup: false, mode: 'popup', theme: theme });
    $("#PIBGrid").bind('contextmenu', function () {
        return false;
    });
    // handle context menu clicks.
    $("#PIBMenu").bind('itemclick', function (event) {
        var args = event.args;
        var rowindex = $("#PIBGrid").jqxGrid('getselectedrowindex');
        var dataRecord = $("#PIBGrid").jqxGrid('getrowdata', rowindex);
        switch ($(args).text()) {
            case "Chi tiết":
                fnShowPIBDetail(dataRecord.InvoiceBatchId);
                break;
            case "Sửa":
                fnPopupPIBDialog(dataRecord.InvoiceBatchId);
                break;
            case "Xóa":
                var rowid = $("#PIBGrid").jqxGrid('getrowid', rowindex);
                $("#PIBGrid").jqxGrid('deleterow', rowid);
                break;
        }
    });
    $("#PIBGrid").bind('rowclick', function (event) {
        if (event.args.rightclick) {
            $("#PIBGrid").jqxGrid('selectrow', event.args.rowindex);
            var dataRecord = $("#PIBGrid").jqxGrid('getrowdata', event.args.rowindex);
            var scrollTop = $(window).scrollTop();
            var scrollLeft = $(window).scrollLeft();
            contextMenu.jqxMenu('open', parseInt(event.args.originalEvent.clientX) + 5 + scrollLeft, parseInt(event.args.originalEvent.clientY) + 5 + scrollTop);
            return false;
        } else {
            contextMenu.jqxMenu('close');
        }
    });

    $("#PIBGrid").bind("rowdoubleclick", function (event) {
        var dataRecord = $("#PIBGrid").jqxGrid('getrowdata', event.args.rowindex);
        fnShowPIBDetail(dataRecord.InvoiceBatchId);
    });
});

function fnPopupPIBDialog(id) {
    var windowId = fnRandomString();
    OpenPopup(windowId, "Thông tin lô hóa đơn", appPath + "Purchase/PurchaseInvoiceBatchForm", { id: id }, true, true, true, true, '50%', 400);
    var done = function () {
        fnSaveOrUpdatePIB();
    }
    var cancel = function () {
        ClosePopup(windowId);
        if (counter > 0) {
            fnRefreshPIBList();
        }
    }
    var reset = function () {
        fnResetFormPIB();
    };
    var dialogOpts = {
        buttons: {
            "Hoàn tất": done,
            "Đóng": cancel,
            "Nhập lại": reset
        }
    };
    $("#" + windowId).dialog(dialogOpts);
}

function fnSaveOrUpdatePIB() {
    if (NMValidator("formPIB")) {
        var id = $("#txtPIBId").val();
        var batchDate = $("#txtPIBDate").val();
        var description = $("#txtPIBDescription").val();
        var status = "False";
        if ($("#cbPIBStatus").is(":checked")) {
            status = "True";
        }
        showProcessing();
        $.post(appPath + "Purchase/SaveOrUpdatePurchaseInvoiceBatch", { id: id, batchDate: batchDate, description: description, status: status }, function (data) {
            if (data == "") {
                showSuccess();
                counter++;
                fnResetFormPIB();
            }
            else {
                alert(data);
            }
            showButtons();
        });
    }
}

function fnDeletePIB(batchId) {
    var answer = confirm("Bạn muốn xóa lô hóa đơn này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Purchase/DeletePurchaseInvoiceBatch", { batchId: batchId },
        function (data) {
            CloseProcessing();
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined) {
                    fnRefreshPIBList();
                } else {
                    fnLoadPIBDetail(nextId);
                }
            }
            else {
                alert(data);
            }
        });
    }
}

function fnLoadPIBs() {
    LoadContent("", "Purchase/PurchaseInvoiceBatchs");
}

function fnLoadPIBDetail(batchId) {
    if (batchId == "") {
        var rowindex = $("#PIBGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#PIBGrid").jqxGrid('getrowdata', rowindex);
        batchId = dataRecord.InvoiceBatchId;
    }
    LoadContent("", "Purchase/PurchaseInvoiceBatchDetail?batchId=" + batchId);
}

function fnReloadPIB() {
    $("#PIBGrid").jqxGrid('updatebounddata');
}

function fnRefreshPIBList() {
    $("#txtKeywordPIB").val("");
    $("#PIBGrid").jqxGrid('removesort');
    fnReloadPIB();
}