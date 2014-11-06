function fnLoadDailyReports() {
    var pageNum = $('#pagination-DailyReports input').first().val().split(' /')[0];
    LoadContentDynamic('dailyreports-container', 'Messages/DailyReportsUC', { pageNum: pageNum, keyword: $('#txtKeyword').val() });
}

function fnShowDailyReportForm(Id, mode) {
    openWindow('Thêm một báo cáo mới', 'Messages/DailyReport', { Id: Id, mode: mode }, 800, 460);
}

function fnLoadDailyReportDetail(id) {
    openWindow('Báo cáo số ' + id, 'Messages/DailyReport', { Id: id, mode: "detail" }, 800, 460);
}

function fnEditDailyReportDetail(id) {
    openWindow('Cập nhật báo cáo số ' + id, 'Messages/DailyReport', { Id: id, mode: "edit" }, 800, 460);
}