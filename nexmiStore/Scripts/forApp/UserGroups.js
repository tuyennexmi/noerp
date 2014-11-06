var counter = 0;

function fnPopupUserGroup(UserGroupId) {
    var windowId = fnRandomString();
    OpenPopup(windowId, "Thông tin nhóm người dùng", appPath + "Users/AddOrEditUserGroup", { UserGroupId: UserGroupId }, true, true, true, true, 500, 350);
    var done = function () {
        fnSaveOrUpdateUserGroup();
    }
    var cancel = function () {
        ClosePopup(windowId);
        if (counter > 0) {
            document.location.reload();
        }
    }
    var reset = function () {
        fnResetFormUserGroup();
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

function fnSaveOrUpdateUserGroup() {
    if (NMValidator("formUG")) {
        var UserGroupId = $("#txtUserGroupId").val();
        var UserGroupName = $("#txtUserGroupName").val();
        var UserGroupDescription = $("#txtUserGroupDescription").val();
        var UserGroupManager = $("#slUserGroupManager").val();
        showProcessing();
        $.post(appPath + "Users/SaveUserGroup", { Id: UserGroupId, Name: UserGroupName, Description: UserGroupDescription, Manager: UserGroupManager },
        function(data) {
            if (data == "") {
                showSuccess();
                counter++;
            }
            else {
                alert(data);
            }
            showButtons();
        });
    } else {

    }
}

function fnDeleteUserGroup(UserGroupId) {
    var answer = confirm("Bạn muốn xóa nhóm này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Users/DeleteUserGroup", { Id: UserGroupId },
        function(data) {
            CloseProcessing();
            if (data == "") {
                document.location.reload();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnResetFormUserGroup() {
    $("#txtUserGroupId").val("");
    $("#txtUserGroupName").val("");
    $("#txtUserGroupDescription").val("");
    $("#slUserGroupManager").val(0);
    $("#txtUserGroupName").focus();
}