function fnPopupStageDialog(id, parentId) {
    var title = 'Thêm giai đoạn';
    if (id != '' && id != null)
        title = 'Sửa giai đoạn';
    openWindow(title, 'Projects/StageForm', { id: id, parentId: parentId }, '60%', 500);
}

function fnDeleteStage(id) {
    var answer = confirm('Bạn muốn xóa giai đoạn này?');
    if (answer) {
        OpenProcessing();
        $.post(appPath + 'Projects/DeleteStage', { id: id }, function (data) {
            CloseProcessing();
            if (data == "") {
            }
            else {
                alert(data);
            }
        });
    }
}