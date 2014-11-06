var counter = 0;

function fnPopupUserDialog(id) {
    openWindow("Thêm/Sửa Người Dùng", "Managements/UserForm", { id: id }, 600, 500);
}

function fnSaveOrUpdateUser() {
    if ($("#formUser").jqxValidator('validate')) {
        var status = "False";
        if ($("#cbStatus").is(":checked")) {
            status = "True";
        }
        $.ajax({
            type: "POST",
            url: appPath + 'Managements/SaveOrUpdateUser',
            data: {
                id: $("#txtId").val(),
                name: $("#txtName").val(),
                code: $("#txtCode").val(),
                password: $("#txtPassword").val(),
                phoneNumber: $("#txtPhoneNumber").val(),
                email: $('#txtEmail').val(),
                emailPassword: $('#txtEmailPassword').val(),
                gender: $('input:radio[name="rdGenders"]').val(), //$('#slGenders').val(),
                status: status,
                stockId: $("#slStocks").val()
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data == "") {
                    counter++;
                    fnSuccess();
                    fnResetFormUser();
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
    } else alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!");
}

function fnDeleteUser(id) {
    var answer = confirm("Bạn muốn xóa người dùng này?");
    if (answer) {
        //OpenProcessing();
        $.post(appPath + "Managements/DeleteUser", { id: id },
        function (data) {
            //CloseProcessing();
            if (data == "") {
                fnLoadContent();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnResetFormUser() {
    $("#txtId").val("");
    $("#txtName").val("");
    $("#txtName").focus();
    $("#txtCode").val("");
    $("#txtPassword").val("");
    $("#txtPhoneNumber").val("");
    $("#txtEmail").val("");
    $("#txtEmailPassword").val("");
}

function fnCloseFormUser(id) {
    closeWindow(id);
    if (counter > 0) {
        fnLoadContent();
    }
}

function fnLoadContent() {
    //location.reload();
    $(window).hashchange();
}

function fnPermissions(userId) {
    LoadContent("", "Managements/Permissions?userId=" + userId);
}