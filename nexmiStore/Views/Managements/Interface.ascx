<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button class="button" onclick="javascript:fnSaveChanges()"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                </div>
            </td>
            <td align="right"></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="windowContent" style="position: static;">
        <script type="text/javascript">
            $(function () {
                $('#btnBrowser_bg').on("click", function () {
                    var finder = new CKFinder();
                    finder.selectActionFunction = function (fileUrl, data) {
                        setImageUrl(fileUrl, 'txtBG');
                    };
                    finder.popup();
                });

                $('#btnBrowser_logo').on("click", function () {
                    var finder = new CKFinder();
                    finder.selectActionFunction = function (fileUrl, data) {
                        setImageUrl(fileUrl, 'txtLogo');
                    };
                    finder.popup();
                });

                $('#btnBrowser_siteIcon').on("click", function () {
                    var finder = new CKFinder();
                    finder.selectActionFunction = function (fileUrl, data) {
                        setImageUrl(fileUrl, 'txtSiteIcon');
                    };
                    finder.popup();
                });

                if (CKEDITOR.instances['txtFooter1'])
                    delete CKEDITOR.instances['txtFooter1'];
                CKEDITOR.replace('txtFooter1', {
                    height: '80px'
                });
                if (CKEDITOR.instances['txtFooter2'])
                    delete CKEDITOR.instances['txtFooter2'];
                CKEDITOR.replace('txtFooter2');

                $('.color-picker').colorPicker();
            });

            function setImageUrl(fileUrl, elementId) {
                if (appPath.length > 1) {
                    fileUrl = fileUrl.replace(appPath, 'quantri/');
                } else {
                    fileUrl = fileUrl.replace('//', 'quantri/');
                }
                $('#' + elementId).val(fileUrl);
            }

            function fnSaveChanges() {
                $.post(appPath + 'Managements/SaveInterface', {
                    logo: $('#txtLogo').val(),
                    background: $('#txtBG').val(),
                    siteIcon: $('#txtSiteIcon').val(),
                    footer1BG: '#' + $('#clpFooter1').val(),
                    footer2BG: '#' + $('#clpFooter2').val(),
                    footer1: Base64.encode(CKEDITOR.instances['txtFooter1'].getData()),
                    footer2: Base64.encode(CKEDITOR.instances['txtFooter2'].getData())
                }, function (data) {
                    if (data != '')
                        alert(data);
                });
            }
        </script>
        <table style="width: 100%" class="frmInput">
            <tr>
                <td class="lbright">Site icon</td>
                <td>
                    <input type="text" id="txtSiteIcon" value="<%=ViewData["SiteIcon"]%>" />
                    <input class="button" type="button" id="btnBrowser_siteIcon" value="Browser..." />
                </td>
            </tr>
            <tr>
                <td class="lbright">Logo</td>
                <td>
                    <input type="text" id="txtLogo" value="<%=ViewData["Logo"]%>" />
                    <input class="button" type="button" id="btnBrowser_logo" value="Browser..." />
                </td>
            </tr>
            <tr>
                <td class="lbright">Hình nền</td>
                <td>
                    <input type="text" id="txtBG" value="<%=ViewData["Background"]%>" />
                    <input class="button" type="button" id="btnBrowser_bg" value="Browser..." />
                </td>
            </tr>
            <tr>
                <td class="lbright">Chân trang<br />(Footer bar) </td>
                <td>
                    <p><b>Phần trên</b></p>
                        Màu nền: <div id="clpFooter1" class="color-picker"><%=ViewData["Footer1BG"]%></div>
                        <textarea cols="1" rows="1" id="txtFooter1"><%=ViewData["Footer1"]%></textarea>
                    <p><b>Phần dưới</b></p>
                        Màu nền: <div id="clpFooter2" class="color-picker"><%=ViewData["Footer2BG"]%></div>
                        <textarea cols="1" rows="1" id="txtFooter2"><%=ViewData["Footer2"]%></textarea>
                </td>
            </tr>
        </table>
    </div>
</div>