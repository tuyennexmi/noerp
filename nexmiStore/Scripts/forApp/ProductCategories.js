function fnReloadPC() {
    $("#PCGrid").jqxGrid('updatebounddata');
}

function fnRefreshPC() {
    $("#txtKeywordPC").val("");
    $("#PCGrid").jqxGrid('removesort');
    fnReloadPC();
}

function fnPopupPCDialog(id, parentElement, objectName) {
    openWindow("Thêm/Sửa Loại Sản Phẩm", "Directories/ProductCategoryForm", { id: id, parentElement: parentElement, objectName: objectName }, 700, 500);
}

function fnShowPCForm(id, parentElement, objectName) {
    LoadContent('', 'Directories/ProductCategoryForm?id=' + id + '&parentElement=' + parentElement + '&objectName=' + objectName);
}

function fnDeletePC(id) {
    var answer = confirm("Bạn muốn xóa nhóm sản phẩm này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Directories/DeleteProductCategory", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnRefreshPC();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnCloseFormPC(windowId) {
    closeWindow(windowId);
    fnRefreshPC();
}

function loadImage(fileUpload, imageId) {
    var reader = new FileReader();
    reader.onload = function (event) {
        $('#' + imageId).attr('src', event.target.result);
    }
    reader.readAsDataURL(fileUpload.files[0]);
}