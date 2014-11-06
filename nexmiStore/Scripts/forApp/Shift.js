var counter = 0;

$(document).ready(function () {
    
});

function fnPopupShiftDialog(id) {
    var windowId = fnRandomString();
    OpenPopup(windowId, "Thông tin đơn vị tính", appPath + "Directories/ShiftForm", { id: id }, true, true, true, true, 500, 350);
    var done = function () {
        fnSaveOrUpdateShift();
    }
    var cancel = function () {
        ClosePopup(windowId);
        if (counter > 0 && $('.ui-dialog:visible').length == 0) {
            fnLoadContent();
        }
    }
    var reset = function () {
        fnResetShiftsForm();
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

function fnSaveOrUpdateShift() {
    if (NMValidator("formShift")) {
        var id = $("#txtId").val();
        var name = $("#txtName").val();
        var start = $("#txtstart").val();
        var finish = $("#txtfinish").val();
        var des = $("#txtdes").val();
        showProcessing();
        $.post(appPath + "Directories/SaveOrUpdateShift", { id: id, name: name, start: start, finish: finish, des: des },
        function (data) {
            if (data == "") {
                showSuccess();
                counter++;
            }
            else {
                alert(data);
            }
            showButtons();
        });
    }
}

function fnDeleteShift(id) {
    var answer = confirm("Bạn muốn xóa ca này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Directories/DeleteShift", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnLoadContent();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnResetShiftForm() {
    $("#txtId").val("");
    $("#txtName").val("");
    $("#txtstart").val("");
    $("#txtfinish").val("");
    $("#txtdes").val("");
}

function fnLoadContent() {
    fnLoad("Directories/Shifts");
}