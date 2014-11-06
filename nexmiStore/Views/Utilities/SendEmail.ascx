<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <button onclick="javascript:fnSend()" class="button">Gửi</button>
                    <button onclick="javascript:$('#formSalesInvoice').jqxValidator('hide'); history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                </td>
                <td align="right"></td>
            </tr>
        </table>
    </div>
    <div class="divContent">
    <%--<div class="divStatus">
        <div class="divButtons"></div>
        <div class="divStatusBar"></div>
    </div>--%>
    <script type="text/javascript">
        $(function () {
            $("#txtContent").jqte();
            $("#txtTags").tagit({
                //allowSpaces: false,
                placeholderText: "Thêm...",
                tagSource: function (request, response) {
                    $.ajax({
                        url: appPath + "Common/GetCustomerForAutocomplete",
                        type: "POST", dataType: "json",
                        data: { keyword: request.term, groupId: "" },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return { label: '"' + item.CompanyNameInVietnamese + '" <' + item.EmailAddress + '>', value: item.EmailAdrress }
                            }))
                        }
                    })
                },
                beforeTagAdded: function (event, ui) {
                    var email = GetEmailsFromString(ui.tagLabel);
                    var temp = "";
                    if (email.length > 0) {
                        temp = email[0].email;
                    } else if (CheckEmail(ui.tagLabel)) {
                        temp = ui.tagLabel;
                    }
                    if (temp != "") {
                        var emails = $("#txtTo").val();
                        if (emails != "") {
                            emails += ",";
                        }
                        emails += temp;
                        $("#txtTo").val(emails);
                        return true;
                    } else {
                        alert("Email không đúng định dạng!");
                        return false;
                    }
                },
                afterTagRemoved: function (event, ui) {
                    var emails = $("#txtTo").val();
                    var emailTags = GetEmailsFromString(ui.tagLabel);
                    if (emailTags.length > 0) {
                        emails = emails.replace(emailTags[0].email, "");
                    } else {
                        emails = emails.replace(ui.tagLabel, "");
                    }
                    $("#txtTo").val(emails);
                }
            });
            fnCheck();
        });

        function fnCheck() {
            if ($('#rdAll').is(':checked'))
                $('.tagit').hide();
            else
                $('.tagit').show();
        }

        function fnSend() {
            var emails = null;
            if ($('#rdCustom').is(':checked')) {
                emails = $('#txtTo').val()
            }
            if (emails != '') {
                $('.button').attr('disabled', true);
                var attachments = document.getElementsByName("attachFiles");
                var attachmentDatas = "";
                var attachmentNames = "";
                for (var x = 0; x < attachments.length; x++) {
                    attachmentDatas += attachments[x].value + "_NEXMI_";
                    attachmentNames += attachments[x].title + "_NEXMI_";
                }
                $.ajax({
                    type: "POST",
                    url: appPath + 'UserControl/SendEmail',
                    data: {
                        emails: emails,
                        subject: $("#txtSubject").val(),
                        content: Base64.encode($("#txtContent").val()),
                        attachmentNames: attachmentNames,
                        attachmentDatas: attachmentDatas
                    },
                    beforeSend: function () {
                    },
                    success: function (data) {
                        if (data == "") {
                            alert('Chúc mừng bạn đã gửi thành công!');
                        }
                        else {
                            alert(data);
                            $('.button').attr('disabled', true);
                        }
                    },
                    error: function () {
                    },
                    complete: function () {
                    }
                });
            } else {
                alert("Vui lòng nhập email người nhận!");
            }
        }
    </script>
    <table style="width: 100%" cellpadding="5">
        <tr>
            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECIPIENT", langId) %></td>
            <td>
                <input type="radio" name="SL" id="rdAll" value="All" onclick="javascript:fnCheck()" /> <label for="rdAll">Tất cả khách hàng</label>
                <input type="radio" name="SL" id="rdCustom" value="" checked="checked" onclick="javascript:fnCheck()" /> <label for="rdCustom">Nhập địa chỉ email</label>
                <input type="hidden" style="width: 100%;" id="txtTo" name="txtTo" value="" />
                <input type="text" id="txtTags" name="txtTags" value="<%=ViewData["Email"]%>" />
            </td>
        </tr>
        <tr>
            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
            <td><input type="text" id="txtSubject" name="txtSubject" value="<%=ViewData["Subject"]%>" style="width: 100%;" /></td>
        </tr>
        <tr>
            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CONTENT", langId) %></td>
            <td>
                <textarea id="txtContent" name="txtContent" rows="1" cols="1" style="width: 100%; height: 300px;"><%=ViewData["Content"]%></textarea>
            </td>
        </tr>
        <tr>
            <td class="lbright">File đính kèm</td>
            <td><%Html.RenderPartial("UploadFile"); %></td>
        </tr>
    </table>
    </div>
</div>