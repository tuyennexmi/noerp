function fnDeleteRequirement(id) {
    if (confirm("Bạn muốn xóa phiếu này?")) {
        OpenProcessing();
        $.post(appPath + "Purchase/DeleteRequirement", { id: id }, function (data) {
            CloseProcessing();
            if (data == "") {
                fnLoadRequirementList();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnShowRequirementForm(id, mode) {
    LoadContent('', 'Purchase/RequirementForm?id=' + id + '&mode='+mode);
}

function fnLoadRequirementList() {
    var pageNum = $('#pagination-Requirement input').first().val().split(' /')[0];
    LoadContentDynamic('requirements-container', 'Purchase/RequirementList', { pageNum: pageNum, keyword: $('#txtKeyword').val() });
}

function fnLoadRequirementDetail(id) {
    LoadContent('', 'Purchase/RequirementForm?id=' + id + '&mode=detail');
}

function fnSendRQ (id) {
    openWindow("Đề nghị vật tư thiết bị", "Messages/ComposeEmail", '', 880, 500);

//    if (confirm("Bạn muốn gửi phiếu này qua email ?")) {
//        OpenProcessing();
//        $.post(appPath + "Purchase/", { id: id }, function (data) {
//            CloseProcessing();
//            if (data == "") {
//                fnLoadRequirementDetail(id);
//            }
//            else {
//                alert(data);
//            }
//        });
//    }
}

function fnConfirmRQ (id, status, str ) {
    if (confirm("Bạn muốn " + str + " phiếu này?")) {
        OpenProcessing();
        $.post(appPath + "Purchase/SetStatus", { id: id, status: status }, function (data) {
            CloseProcessing();
            if (data.error == '') {                
                $(window).hashchange();
            }
            else {
                alert(data.error);
            }
        });
    }
}