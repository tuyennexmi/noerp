/***Loading...***/
function OpenLoading() {
    $("#loading-form").load(appPath + "Common/Loading").dialog({
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false,
        width: 400,
        heigth: 200,
        closeOnEscape: false,
        open: function(event, ui) { $(".ui-dialog-titlebar").hide(); }
    }).dialog("open");
}

function CloseLoading() {
    $("#loading-form").dialog("close");
}

/***Processing...***/
function OpenProcessing() {
    $('html').append('<div id="progress-window" style="background-color: #fff; padding: 30px;"><div align="center"><img alt="" src="' + appPath + 'Content/UI_Images/processing.gif" /><h4 style="color: Red">Đang xử lý...</h4></div></div>');
    $('#progress-window').dialog({
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false,
        width: 400,
        heigth: 200,
        closeOnEscape: false,
        open: function (event, ui) { $('.ui-dialog-titlebar').hide(); }
    }).dialog('open');
}

function CloseProcessing() {
    $('#progress-window').remove();
}

/***Popup***/
function OpenPopup(id, title, url, data, modal, autoOpen, resizable, draggable, width, height) {
    $("body").prepend("<div id='" + id + "'></div>");
    $("#" + id).append(loadingContent).load(url, data, function (responseText, textStatus, XMLHttpRequest) {
        if (textStatus == 'error') {
            $("#" + id).empty();
            $("#" + id).html("<h4>Không thể tải dữ liệu.<br/>Vui lòng thử lại hoặc liên hệ người quản trị để khắc phục.</h4>");
        }
    }).dialog({
        modal: modal,
        autoOpen: autoOpen,
        resizable: resizable,
        draggable: draggable,
        width: width,
        height: height,
        title: title,
        closeOnEscape: true,
        open: function (event, ui) { $(".ui-dialog-titlebar").show(); $(".ui-dialog-titlebar-close").hide(); },
        close: function (event, ui) { $("#" + id).dialog("destroy").remove(); }
    }).dialog("open");
}

function ClosePopup(id) {
    $("#" + id).dialog("close");
}

function CloseCurrentPopup() {
    var id = $(".ui-dialog-content:last").attr("id");
    ClosePopup(id);
}

function showProcessing() {
    $(".ui-dialog-buttonset").hide();
    $(".ui-dialog-buttonpane").append(processing);
}

function showSuccess() {
    $(".noticeArea").html(success);
}

function showError(strError) {
    try {
        $(".errorArea").html(strError);
    } catch (Error) { }
}

function clearError() {
    try {
        $(".errorArea").html("");
    } catch (Error) { }
}

function showButtons() {
    setTimeout(function() {
        $(".noticeArea").remove();
        $(".ui-dialog-buttonset").show();
    }, 1000);
}

function LoadContentDynamic(id, action, data) {
    $('#' + id).empty().append(loadingContent).load(appPath + action, data, function (response, status, XMLHttpRequest) {
        if (status == 'error') {
            $('#' + id).html('Không thể tải dữ liệu.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: ' + XMLHttpRequest.status + " " + XMLHttpRequest.statusText);
        } else { }
    });
}

function LoadContent(id, action) {    
    var currentAction = fnGetHashParameter('action');
    location.hash = location.hash.replace(currentAction, action);
}

function fnLoad(action) {
    $("#mainContent").fadeOut("slow", function () {
        $("#mainContent").load(appPath + action, function (response, status, XMLHttpRequest) {
            if (status == 'error') {
                alert('Không thể tải dữ liệu.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: ' + XMLHttpRequest.status + " " + XMLHttpRequest.statusText);
                history.back();
            } else {
                //$("#SitemapContent").append("/ <a href='javascript:LoadContent(\"\",\"" + action + "\")'>Test</a>");
            }
            $("#mainContent").fadeIn('fast');
        });
    });
}

function LoadDetail(masterId, detailId, action) {
    if (masterId != "") {
        $("#" + masterId).hide('slow');
    }
    $("#" + detailId).empty();
    $("#" + detailId).append(loadingContent).load(appPath + action, function(responseText, textStatus, XMLHttpRequest) {
        if (textStatus == 'error') {
            $("#" + detailId).empty().html('<h3>Không thể tải dữ liệu.<br/>Vui lòng thử lại hoặc liên hệ người quản trị để khắc phục.</h3>');
        }
    });
}

function BackToMaster(masterId, detailId) {
    $("#" + detailId).empty();
    $("#" + masterId).show('fast');
}

function fnUpperCase(value) {
    value = value.toUpperCase();
    return value;
}

function fnOnlyNumber(evt) {
    evt = (evt) ? evt : window.event
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 46 || charCode > 57 || charCode == 47) && charCode != 189) {
        if (charCode != 45) {
            return false
        }
    }
    return true
}

function fnNumberFormat(value) {
    return new Number(value).numberFormat("#,###.##");
}

function fnCurrencyFormat(value, symbol) {
    return new Number(value).numberFormat("#,###.##");
}

/*** This function removes non-numeric characters***/
function fnStripNonNumeric(str) {
    str += '';
    var rgx = /^\d|\.|-$/;
    var out = '';
    for (var i = 0; i < str.length; i++) {
        if (rgx.test(str.charAt(i))) {
            if (!((str.charAt(i) == '.' && out.indexOf('.') != -1) ||
             (str.charAt(i) == '-' && out.length != 0))) {
                out += str.charAt(i);
            }
        }
    }
    return out;
}


/***Add commas to number when typing***/
function fnAddCommas(value) {
    var operator = "";
    var afterDot = "";
    var temp;
    if (value.indexOf("-") != -1) {
        operator = "-";
        value = value.replace(/-/g, "");
    }
    if (value.indexOf(".") != -1) {
        temp = value.split(".");
        value = temp[0];
        afterDot = "." + temp[1];
    }
    //remove any existing commas...
    value = value.replace(/,/g, "");
    //start putting in new commas...
    value = '' + value;
    if (value.length > 3) {
        var mod = value.length % 3;
        var output = (mod > 0 ? (value.substring(0, mod)) : '');
        for (i = 0; i < Math.floor(value.length / 3); i++) {
            if ((mod == 0) && (i == 0))
                output += value.substring(mod + 3 * i, mod + 3 * i + 3);
            else
                output += ',' + value.substring(mod + 3 * i, mod + 3 * i + 3);
        }
        return (operator + output + afterDot);
    }
    else return operator + value + afterDot;
}

function GetEmailsFromString(input) {
    var ret = [];
    var email = /\"([^\"]+)\"\s+\<([^\>]+)\>/g

    var match;
    while (match = email.exec(input))
        ret.push({ 'name': match[1], 'email': match[2] })

    return ret;
}

function CheckEmail(value) { 
    var emailPattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    if (emailPattern.test(value))
        return true;
    return false;
}

function fnRandomString() {
    var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
    var string_length = 7;
    var randomstring = '';
    for (var i = 0; i < string_length; i++) {
        var rnum = Math.floor(Math.random() * chars.length);
        randomstring += chars.substring(rnum, rnum + 1);
    }
    return randomstring;
}

function fnShowUploadForm() {
    var windowId = fnRandomString();
    OpenPopup(windowId, "", appPath + "UserControl/UploadForm", {}, true, true, true, true, 800, 600);
}

function fnGetParameter(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return results[1];
}

function fnGetHashParameter(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = '';
    if (name == 'action') {
        regexS = "[\\#&]" + name + "=([^#]*)";
    } else {
        regexS = "[\\#&]" + name + "=([^&#]*)";
    }
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.hash);
    if (results == null)
        return "";
    else
        return results[1];
}


function fnCheckPermission(strAction) {
    var isOK = false;
    var functionId = fnGetParameter('functionId');
    $.ajax({
        async: false,
        url: appPath + "Common/CheckPermission",
        data: { functionId: functionId, strAction: strAction },
        success: function (data) {
            isOK = data;
        }
    });
    return isOK;
}

/////////////////////
function openWindow(title, url, parameters, w, h) {
    if (w == "")
        w = "94%";
    if (h == "")
        h = $(document).height() - 80;
    var windowId = fnRandomString();
    $("body").append("<div id='" + windowId + "' style='border: 5px solid #ddd;'><div style='font-size: 16pt;'>" + title + "</div><div style='overflow: hidden; padding: 0;'></div></div>");
    var window = $("#" + windowId);
    window.jqxWindow({
        theme: theme,
        isModal: true,
        closeButtonAction: 'close',
        width: w,
        height: h,
        showCollapseButton: true,
        maxHeight: h,
        maxWidth: w,
        minHeight: 200,
        minWidth: 200,
        position: 'center',
        resizable: false
    });
    window.jqxWindow('open');
    parameters.windowId = windowId;
    loadWindowContent(url, parameters, window);
}

function loadWindowContent(url, parameters, window) {
    window.jqxWindow('setContent', loadingContent);
    $.ajax({
        type: 'POST',
        dataType: 'html',
        url: appPath + url,
        data: parameters,
        success: function (data) {
            window.jqxWindow('setContent', data);
        },
        error: function () {
            window.jqxWindow('setContent', '<center><h2>Không thể tải dữ liệu!</h2></center>');
        }
    });
}

function closeWindow(windowId) {
    $("#" + windowId).jqxWindow("close");
    $(".jqx-window-modal:last").remove();
    $("#" + windowId).remove();
}

function fnResetAllField(formId) { 
    var elements = document.getElementById(formId).elements;
    for (var i = 0; i < elements.length; i++) {
        elements[i].value = "";
    }
}

function fnCallValidate(formId) {
    $('#' + formId).jqxValidator('validate');
}

$.fn.selectRange = function (start, end) {
    return this.each(function () {
        if (this.setSelectionRange) {
            this.focus();
            this.setSelectionRange(start, end);
        } else if (this.createTextRange) {
            var range = this.createTextRange();
            range.collapse(true);
            range.moveEnd('character', end);
            range.moveStart('character', start);
            range.select();
        }
    });
};

function fnDoing() {
    $(".NMButtons").last().hide();
    $(".NMButtons").last().parent().append(processing);
    //$(".windowButtons").append(processing);
}

function fnSuccess() {
    $(".noticeArea").last().html(success);
}

function fnError() {
    alert("error...");
}

function fnComplete() {
    setTimeout(function () {
        $(".noticeArea").last().remove();
        $(".NMButtons").last().show();
    }, 800);
}

function fnSetItemForCombobox(id, value, label) {
    var item = $('#' + id).jqxComboBox('getItemByValue', value);
    if (item == null) {
        $('#' + id).jqxComboBox('insertAt', { value: value, label: Base64.decode(label) }, 0);
        $('#' + id).jqxComboBox('selectIndex', 0);
    } else {
        $('#' + id).jqxComboBox('selectIndex', item.index);
    }
    try {
        $('.form-to-validate:last').jqxValidator('validate');
    } catch (err) { }
}

function fnShowPriceForSales(productId) {
    openWindow("Lịch sử thay đổi giá bán", "UserControl/CostPriceHistory", { productId: productId }, '80%', '');
}

function fnStatusItemClick(elementId, statusId, ownerId, objectName) {
    $.post(appPath + 'Common/SetStatus', { objectName: objectName, id: ownerId, status: statusId }, function (data) {
        if (data != '') {
            alert(data);
        } else {
            $('#' + elementId).find('li').removeClass('currentStatus');
            $('#' + elementId + statusId).addClass('currentStatus');
        }
    });
}