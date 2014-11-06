
function fnLoadMODetail(id) {
    LoadContent('', 'Manufacture/ManufactureOrder?id=' + id + '&mode=detail');
}

function fnDeleteMO(id) {
    var answer = confirm("Bạn muốn xóa phiếu này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Manufacture/DeleteMO", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnLoadMOList();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnShowMOForm(id) {
    LoadContent('', 'Manufacture/ManufactureOrder?id=' + id);
}

function fnLoadMOList(pageNum) {
//    if (pageNum == undefined)
//        pageNum = $('#pagination-Receipt input').first().val().split(' /')[0];
    LoadContentDynamic('divContent', 'Manufacture/ManufactureOrderList', '');
}
