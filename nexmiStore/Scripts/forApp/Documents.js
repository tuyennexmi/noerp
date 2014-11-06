var counter = 0;

function fnShowDocumentForm(id, typeId) {
    LoadContent('', 'CMS/DocumentForm?id=' + id + '&typeId=' + typeId);
}

function fnDeleteDocument(id) {
    var answer = confirm("Bạn muốn xóa dòng này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "CMS/DeleteDocument", { id: id }, function (data) {
            CloseProcessing();
            if (data == "") {
                fnReloadDocument();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnReloadDocument() {
    $("#DocumentGrid").jqxGrid('updatebounddata');
}

function fnRefreshDocument() {
    $("#txtKeyword").val("");
    $("#DocumentGrid").jqxGrid('removesort');
    fnReloadDocument();
}

function fnPreviousPage(typeId) {
    var pageNum = parseInt($("#lbPage").text()) - 2;
    fnLoadDocumentList(pageNum, typeId);
}

function fnNextPage(typeId) {
    var pageNum = parseInt($("#lbPage").text());
    fnLoadDocumentList(pageNum, typeId);
}