var counter = 0;

$(document).ready(function () {
    
});

function fnPopupUnitDialog(id) {
    openWindow('Thêm/Sửa Đơn Vị Tính', 'Directories/UnitForm', { id: id }, 500, 400);
//    var windowId = fnRandomString();
//    OpenPopup(windowId, "Thông tin đơn vị tính", appPath + "Directories/UnitForm", { id: id }, true, true, true, true, 500, 350);
//    var done = function () {
//        fnSaveOrUpdateUnit();
//    }
//    var cancel = function () {
//        ClosePopup(windowId);
//        if (counter > 0 && $('.ui-dialog:visible').length == 0) {
//            fnLoadContent();
//        }
//    }
//    var reset = function () {
//        fnResetUnitsForm();
//    };
//    var dialogOpts = {
//        buttons: {
//            "Hoàn tất": done,
//            "Đóng": cancel,
//            "Nhập lại": reset
//        }
//    };
//    $("#" + windowId).dialog(dialogOpts);
}

function fnSaveOrUpdateUnit() {
    var name = $("#txtName").val();

    if (name != '') {
        $.ajax({
            type: "POST",
            url: appPath + 'Directories/SaveOrUpdateUnit',
            data: {
                id: $("#txtId").val(), name: $("#txtName").val()
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data == "") {
                    fnSuccess();
                    fnResetUnitForm();
                }
                else {
                    alert(data);
                }
            },
            error: function () {
                fnError();
            },
            complete: function () {
                fnComplete();
            }
        });

        //        var id = $("#txtId").val();
        //        var name = $("#txtName").val();
        //        showProcessing();
        //        $.post(appPath + "Directories/SaveOrUpdateUnit", { id: id, name: name }, function (data) {
        //            if (data == "") {
        //                showSuccess();
        //                counter++;
        //            }
        //            else {
        //                alert(data);
        //            }
        //            showButtons();
        //        });
    } else {
        alert("Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!");
        $("#txtName").focus();
    }
}

function fnDeleteUnit(id) {
    var answer = confirm("Bạn muốn xóa đơn vị này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Directories/DeleteUnit", { id: id },
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

function fnResetUnitForm() {
    $("#txtId").val("");
    $("#txtName").val("");
    $("#txtName").focus();
    //$("#txtstart").val("");
    //$("#txtfinish").val("");
    //$("#txtdes").val("");
}

function fnLoadContent() {
    fnLoad("Directories/Units");
}