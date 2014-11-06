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
        url: appPath + 'Common/GetSalesInvoiceBatchs',
        formatdata: function (data) {
            var keyword = $("#txtKeywordSIB").val();
            return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                sortorder: data.sortorder, keyword: keyword
            }
        },
        root: 'Rows',
        beforeprocessing: function (data) {
            source.totalrecords = data.TotalRows;
        },
        sort: function () {
            fnReloadSIB();
        },
        deleterow: function (rowid, commit) {
            var dataRecord = $("#SIBGrid").jqxGrid('getrowdata', rowid);
            fnDeleteSIB(dataRecord.InvoiceBatchId);
        }
    };
    var dataAdapter = new $.jqx.dataAdapter(source);
    // initialize jqxGrid
    $("#SIBGrid").jqxGrid({
        width: '100%',
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
            { text: 'Trạng thái', datafield: 'BatchStatus', width: 100 },
            { text: 'Ghi chú', datafield: 'Description', sortable: false },
            { text: 'Người tạo', datafield: 'CreatedUserName', sortable: false, width: 120 },
            { text: 'Ngày tạo', datafield: 'CreatedDate', sortable: false, width: 150 }
        ]
    });

    // create context menu
    var contextMenu = $("#SIBMenu").jqxMenu({ width: 200, height: 'auto', autoOpenPopup: false, mode: 'popup', theme: theme });
    $("#SIBGrid").bind('contextmenu', function () {
        return false;
    });
    // handle context menu clicks.
    contextMenu.bind('itemclick', function (event) {
        var args = event.args;
        var rowindex = $("#SIBGrid").jqxGrid('getselectedrowindex');
        var dataRecord = $("#SIBGrid").jqxGrid('getrowdata', rowindex);
        switch ($(args).text()) {
            case "Chi tiết":
                fnLoadSIBDetail(dataRecord.InvoiceBatchId);
                break;
            case "Sửa":
                fnPopupSIBDialog(dataRecord.InvoiceBatchId);
                break;
            case "Xóa":
                var rowid = $("#SIBGrid").jqxGrid('getrowid', rowindex);
                $("#SIBGrid").jqxGrid('deleterow', rowid);
                break;
        }
    });
    $("#SIBGrid").bind('rowclick', function (event) {
        if (event.args.rightclick) {
            $("#SIBGrid").jqxGrid('selectrow', event.args.rowindex);
            var dataRecord = $("#SIBGrid").jqxGrid('getrowdata', event.args.rowindex);
            var scrollTop = $(window).scrollTop();
            var scrollLeft = $(window).scrollLeft();
            contextMenu.jqxMenu('open', parseInt(event.args.originalEvent.clientX) + 5 + scrollLeft, parseInt(event.args.originalEvent.clientY) + 5 + scrollTop);
            if (dataRecord.BatchStatus == "False") {
                contextMenu.jqxMenu('disable', 'menuEdit', false);
                contextMenu.jqxMenu('disable', 'menuDelete', false);
            } else {
                contextMenu.jqxMenu('disable', 'menuEdit', true);
                contextMenu.jqxMenu('disable', 'menuDelete', true);
            }
            return false;
        } else {
            contextMenu.jqxMenu('close');
        }
    });

    $("#SIBGrid").bind("rowdoubleclick", function (event) {
        var dataRecord = $("#SIBGrid").jqxGrid('getrowdata', event.args.rowindex);
        fnLoadSIBDetail(dataRecord.InvoiceBatchId);
    });
});

function fnPopupSIBDialog(id) {
    var windowId = fnRandomString();
    OpenPopup(windowId, "Thông tin lô hóa đơn", appPath + "Sales/SalesInvoiceBatchForm", { id: id }, true, true, true, true, '50%', 400);
    var done = function () {
        fnSaveOrUpdateSIB();
    }
    var cancel = function () {
        ClosePopup(windowId);
        if (counter > 0) {
            if (idTemp != "") {
                fnLoadSIBDetail(idTemp);
            } else {
                fnRefreshSIBList();
            }
        }
    }
    var reset = function () {
        fnResetFormSIB();
    }
    var dialogOpts = {
        buttons: {
            "Hoàn tất": done,
            "Đóng": cancel,
            "Nhập lại": reset
        }
    };
    $("#" + windowId).dialog(dialogOpts);
}

function fnSaveOrUpdateSIB() {
    if (NMValidator("formSIB")) {
        var id = $("#txtSIBId").val();
        var batchDate = $("#txtSIBDate").val();
        var description = $("#txtSIBDescription").val();
        var status = "False";
        if ($("#cbSIBStatus").is(":checked")) {
            status = "True";
        }
        showProcessing();
        $.post(appPath + "Sales/SaveOrUpdateSalesInvoiceBatch", { id: id, batchDate: batchDate, description: description, status: status }, function (data) {
            idTemp = data.result;
            if (data.error == "") {
                showSuccess();
                counter++;
                fnResetFormSIB();
            }
            else {
                alert(data.error);
            }
            showButtons();
        });
    }
}

function fnDeleteSIB(batchId) {
    var answer = confirm("Bạn muốn xóa lô hóa đơn này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Sales/DeleteSalesInvoiceBatch", { batchId: batchId },
        function (data) {
            CloseProcessing();
            if (data == "") {
                var nextId = $("#btNext").val();
                if (nextId == undefined) {
                    fnRefreshSIBList();
                } else {
                    fnLoadSIBDetail(nextId);
                }
            }
            else {
                alert(data);
            }
        });
    }
}

function fnResetFormSIB()
{ }

function fnLoadSIBs() {
    LoadContent("", "Sales/SalesInvoiceBatchs");
}

function fnLoadSIBDetail(batchId) {
    if (batchId == "") {
        var rowindex = $("#SIBGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#SIBGrid").jqxGrid('getrowdata', rowindex);
        batchId = dataRecord.InvoiceBatchId;
    }
    LoadContent("", "Sales/SalesInvoiceBatchDetail?batchId=" + batchId);
}

function fnReloadSIB() {
    $("#SIBGrid").jqxGrid('updatebounddata');
}

function fnRefreshSIBList() {
    $("#txtKeywordSIB").val("");
    $("#SIBGrid").jqxGrid('removesort');
    fnReloadSIB();
}