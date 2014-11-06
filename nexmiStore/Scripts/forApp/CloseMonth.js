var counter = 0;

function fnLoadContent() {
    var strError = "";
    var monthYear = "";
    var closeType = "";
    monthYear = $("#txtMonthYear").val();
    if (monthYear == "") {
        strError = "Chưa chọn ngày!";
    }
    closeType = $('input[name=cbType]:checked').val();
    if (strError == "") {
        LoadContent("", "Accounting/CloseMonth?monthYear=" + monthYear + "&closeType=" + closeType);
    } else {
        alert(strError);
    }
}