function ShowLoadingIndicator() {
    $(this).css("cursor", "wait");
    if (typeof(disableLoadingIndicator) != 'undefined' && disableLoadingIndicator) {
	    return;
    }
    var windowWidth = $(window).width();
    var scrollTop;
    if (self.pageYOffset) {
	    scrollTop = self.pageYOffset;
    }
	else if (document.documentElement && document.documentElement.scrollTop) {
	    //alert('s');
	    scrollTop = document.documentElement.scrollTop;
    }
	else if (document.body) {
	    //alert('s');
	scrollTop = document.body.offsetTop;//  document.documentElement.scrollTop; // document.body.offsetTop;
        
    }
    $('#AjaxLoading').css('position', 'absolute');
    $('#AjaxLoading').css('top', scrollTop+'px');
    $('#AjaxLoading').css('left', parseInt((windowWidth-150)/2)+"px");
    $('#AjaxLoading').show();
}

function HideLoadingIndicator() {
    $(this).css("cursor", "auto");
    $('#AjaxLoading').delay(800).slideUp(300);
}

$(document).ready(function () {
    $('html').ajaxStart(function () {
        ShowLoadingIndicator();
        var posting = $.post(appPath + 'Common/CheckAuthentication');
        posting.done(function (data) {
            if (data == false)
                document.location = appPath + 'Account/LogOff?returnUrl=' + Base64.encode(location.hash);
        });
        posting.fail(function () {
            document.location = appPath + 'Account/LogOff?returnUrl=' + Base64.encode(location.hash);
        });

    });
    $('html').ajaxComplete(function () {
        HideLoadingIndicator();
    });
    $('.InitialFocus').focus();
});