<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<div style="padding:6px;">
    <%--<div class="divStatus">
        <div class="divButtons"></div>
        <div class="divStatusBar"></div>
    </div>--%>
    <script type="text/javascript">
        $(function () {
            $('#txtContent').jqte();
            $('#txtTags').tagit({
                allowSpaces: false,
                placeholderText: 'Thêm...',
                tagSource: function (request, response) {
                    $.ajax({
                        url: appPath + 'Common/GetCustomerForAutocomplete',
                        type: 'POST', dataType: 'json',
                        data: { keyword: request.term, groupId: '' },
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
            //fnCheck();
        });

        function fnSend() {
            var emails = null;
            emails = $('#txtTo').val();
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
                            $('.button').attr('disabled', false);
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
    <div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSend()" class="button medium">Gửi</button>
        <%--<button onclick='javascript:closeWindow()' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>--%>
    </div>
</div>