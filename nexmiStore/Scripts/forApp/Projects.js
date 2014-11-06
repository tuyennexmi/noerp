function fnShowProjectForm(id) {
    LoadContent('', 'Projects/ProjectForm?id=' + id);
}

function fnPopupProjectDialog(id, comboBoxId) {
    var title = 'Thêm dự án';
    if (id != '' && id != null)
        title = 'Sửa dự án';
    openWindow(title, 'Projects/ProjectForm', { id: id, comboBoxId: comboBoxId }, '80%', '');
}

function fnDeleteProject(id) {
    var answer = confirm('Bạn muốn xóa dự án này?');
    if (answer) {
        OpenProcessing();
        $.post(appPath + 'Projects/DeleteProject', { id: id }, function (data) {
            if (data == '') {
                try {
                    fnReloadProject();
                } catch (err) {
                    LoadContent('', 'Projects/Projects');
                }
            }
            else {
                alert(data);
            }
            CloseProcessing();
        });
    }
}

function fnLoadProjectDetail(id) {
    if (id == '') {
        var rowindex = $('#ProjectGrid').jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $('#ProjectGrid').jqxGrid('getrowdata', rowindex);
        if (dataRecord != null) {
            id = dataRecord.ProjectId;
        }
    }
    if (id != '') {
        LoadContent('', 'Projects/ProjectDetail?id=' + id);
    }
}

function fnChangeProjectStatus(id, statusId) {
    $.post(appPath + 'Projects/UpdateProject', { id: id, statusId: statusId }, function (data) {
        if (data == '') {
            $(window).hashchange();
        } else {
            alert(data);
        }
    });
}


//function fnExport2Excel() {
//    var categoryId = '';
//    try {
//        categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
//    } catch (err) { }
//    $.download(appPath + 'Common/Projects2Excel', 'keyword=' + $('#txtKeyword').val() + '&categoryId=' + categoryId);
//}

function fnLoadProjectGrid(pageNum) {
    if (pageNum == undefined)
        pageNum = '';
    LoadContent('', 'Projects/ProjectKanban?pageNum=' + pageNum + '&keyword=' + Base64.encode($('#txtKeyword').val()));
}