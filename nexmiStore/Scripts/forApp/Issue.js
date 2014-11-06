function fnShowIssueForm(id, stageId, projectId) {
    LoadContent('', 'Projects/IssueForm?id=' + id + '&stageId=' + stageId + '&projectId=' + projectId);
}

function fnPopupIssueDialog(id) {
    var title = 'Thêm nhiệm vụ';
    if (id != '' && id != null)
        title = 'Sửa nhiệm vụ';
    openWindow(title, 'Projects/IssueForm', { id: id }, '60%', 500);
}

function fnPopupWorkDialog(id) {
    openWindow('Công việc', 'Projects/WorkForm', { id: id }, 450, 300);
}

function fnDeleteIssue(id, mode) {
    if (confirm("Bạn muốn xóa nhiệm vụ này?")) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Projects/DeleteIssue',
            data: { id: id },
            beforeSend: function () {
                OpenProcessing();
            },
            success: function (data) {
                if (data == "") {
                    if (mode == '1')
                        $(window).hashchange();
                    else {
                        //fnLoadIssues();
                        history.back();
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
                CloseProcessing();
            }
        });
    }
}

function fnLoadIssues(viewType) {
    switch (viewType) {
        case 'list':
            LoadContent('', 'Projects/IssueList');
            break;
        case 'detail':
            break;
        default:
            LoadContent('', 'Projects/Issues');
            break;
    }
}

function fnLoadIssueDetail(id) {
    LoadContent('', 'Projects/IssueDetail?id=' + id);
}

function fnSetStageForIssue(issueId, stageStatusId, statusId) {
    $.ajax({
        type: 'POST',
        url: appPath + 'Projects/SetStageForIssue',
        data: {
            issueId: issueId,
            stageStatusId: stageStatusId,
            statusId: statusId
        },
        beforeSend: function () {
            fnDoing();
        },
        success: function (data) {
            if (data == "") {
                $(window).hashchange();
            }
            else if (data != "None") {
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