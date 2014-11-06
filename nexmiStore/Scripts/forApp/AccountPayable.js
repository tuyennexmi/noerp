var counter = 0;

function fnDeleteProductInStock(id) {
    var answer = confirm("Bạn muốn xóa sản phẩm này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Stocks/DeleteProductInStock", { id: id },
        function(data) {
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
    var strError = "";
    var monthYear = "";
    var fromDate = "";
    var toDate = "";
    switch ($('input[name=cbType]:checked').val()) {
        case "1":
            break;
        case "2":
            //monthYear = $("#slMonths").val() + "/" + $("#slYears").val();
            monthYear = $("#txtMonthYear").val();
            if (monthYear == "") {
                strError = "Chưa chọn ngày!";
            }
            break;
        case "3":
            fromDate = $("#txtFromDate").val();
            toDate = $("#txtToDate").val();
            if (fromDate == "" || toDate == "") {
                strError = "Chưa chọn ngày!";
            }
            break;
    }
    if (strError == "") {
        LoadContent("", "Purchase/AccountPayable?monthYear=" + monthYear + "&fromDate=" + fromDate + "&toDate=" + toDate);
    } else {
        alert(strError);
    }
}