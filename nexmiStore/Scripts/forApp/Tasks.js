function fnShowTaskForm(id, stageId, projectId) {
    LoadContent('', 'Projects/TaskForm?id=' + id + '&stageId=' + stageId + '&projectId=' + projectId);
}

function fnPopupTaskDialog(id, projectId, comboboxId) {
    if (projectId == undefined || projectId == null) projectId = '';
    var title = 'Thêm nhiệm vụ';
    if (id != '' && id != null)
        title = 'Sửa nhiệm vụ';
    openWindow(title, 'Projects/TaskForm', { id: id, projectId: projectId, comboboxId: comboboxId }, '60%', 500);
}

function fnPopupWorkDialog(id) {
    openWindow('Công việc', 'Projects/WorkForm', { id: id }, 450, 300);
}

function fnDeleteTask(id, mode) {
    if (confirm("Bạn muốn xóa nhiệm vụ này?")) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Projects/DeleteTask',
            data: { id: id },
            beforeSend: function () {
                OpenProcessing();
            },
            success: function (data) {
                if (data == "") {
                    if (mode == '1')
                        $('#' + id).remove();
                    else {
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

function fnLoadTasks(viewType) {
    switch (viewType) {
        case 'list':
            LoadContent('', 'Projects/TaskList');
            break;
        case 'detail':
            break;
        default:
            LoadContent('', 'Projects/Tasks');
            break;
    }
}

function fnLoadTaskDetail(id) {
    LoadContent('', 'Projects/TaskDetail?id=' + id);
}

function fnSetStageForTask(taskId, stageStatusId, statusId) {
    $.ajax({
        type: 'POST',
        url: appPath + 'Projects/SetStageForTask',
        data: {
            taskId: taskId,
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