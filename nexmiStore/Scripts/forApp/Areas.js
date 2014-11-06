function fnReloadArea() {
    $("#AreaGrid").jqxGrid('updatebounddata');
}

function fnRefreshArea() {
    $("#txtKeywordArea").val("");
    $("#AreaGrid").jqxGrid('removesort');
    fnReloadArea();
}

function fnPopupAreaDialog(id, parentElement) {
    openWindow('Thêm/Sửa Khu Vực', 'Directories/AreaForm', { id: id, parentElement: parentElement }, 500, 250);
}
function fnSaveArea(windowId, parentElement) {
    if ($('#formArea' + windowId).jqxValidator('validate')) {
        var parentId = "";
        try {
            parentId = $('#cbbAreas' + windowId).jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        var name = $('#txtAreaName' + windowId).val();
        $.ajax({
            type: 'POST',
            url: appPath + 'Directories/SaveOrUpdateArea',
            data: {
                id: $('#txtAreaId' + windowId).val(),
                name: name,
                zipCode: $('#txtZipCode').val(),
                parentId: parentId
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data.error == "") {
                    if (parentElement != null && parentElement != '') {
                        fnSetItemForCombobox('cbbAreas' + parentElement, data.id, Base64.encode(name));
                        closeWindow(windowId);
                    } else {
                        fnSuccess();
                        fnResetArea();
                    }
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
    }
    else {
        alert("Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!");
    }
}
function fnDeleteArea(id) {
    if (confirm('Bạn có muốn xóa?')) {
        var selectedItem = $('#jqxTree').jqxTree('selectedItem');
        $.post(appPath + 'Directories/DeleteArea', { id: id }, function (data) {
            if (data == '') {
                fnReloadArea();
            } else {
                alert(data);
            }
        });
    }
}
function fnResetArea(windowId) {
    $('#txtAreaId' + windowId).val('');
    $('#txtAreaName' + windowId).val('');
    $('#txtAreaId' + windowId).focus();
}
function fnCloseAreaForm(windowId) {
    closeWindow(windowId);
    fnReloadArea();
}