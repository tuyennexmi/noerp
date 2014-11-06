
function fnPlanForm(id) {
    LoadContent('', 'Planning/PlanningByProducts?id=' + id);
}

function fnDeletePlan(id) {
    var answer = confirm("Bạn muốn xóa đơn vị này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Planning/DeletePlan", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnLoadContent();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnLoadContent() {
    fnLoad("Planning/Plans");
}

function fnPlanDetail(id) {
    LoadContent('', 'Planning/PlanningByProducts?id=' + id + '&mode=detail');
}