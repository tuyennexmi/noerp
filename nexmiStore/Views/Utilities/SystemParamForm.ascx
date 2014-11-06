<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMSystemParamsWSI WSI = (NEXMI.NMSystemParamsWSI)ViewData["WSI"];
    string id = "", name = "", subject = "", content = "", email = "", actionDate = "", objectName = "", type = "", autoSend = "",
        strUser = "", strCustomer = "", strSupplier = "", strManufacturer = "", strIndividual = "", strEnterprise = "", strMale = "", strFemale = "";
    try
    {
        id = WSI.SystemParam.Id;
        name = WSI.SystemParam.Name;
        subject = WSI.SystemParam.Subject;
        content = WSI.SystemParam.Contents;
        email = WSI.SystemParam.Email;
        objectName = WSI.SystemParam.ObjectName;
        type = WSI.SystemParam.Type;
        autoSend = (WSI.SystemParam.Value == 1) ? "checked" : "";
        actionDate = WSI.SystemParam.ActionDate.ToString("yyyy-MM-dd");
        if (WSI.SystemParam.CustomerGroup.Contains(NEXMI.NMConstant.CustomerGroups.User))
            strUser = "checked";
        if (WSI.SystemParam.CustomerGroup.Contains(NEXMI.NMConstant.CustomerGroups.Customer))
            strCustomer = "checked";
        if (WSI.SystemParam.CustomerGroup.Contains(NEXMI.NMConstant.CustomerGroups.Supplier))
            strSupplier = "checked";
        if (WSI.SystemParam.CustomerGroup.Contains(NEXMI.NMConstant.CustomerGroups.Manufacturer))
            strManufacturer = "checked";
        if (WSI.SystemParam.CustomerType.Contains(NEXMI.NMConstant.CustomerTypes.Individual))
            strIndividual = "checked";
        if (WSI.SystemParam.CustomerType.Contains(NEXMI.NMConstant.CustomerTypes.Enterprise))
            strEnterprise = "checked";
        if (WSI.SystemParam.Gender.Contains("1"))
            strMale = "checked";
        if (WSI.SystemParam.Gender.Contains("0"))
            strFemale = "checked";
    }
    catch { }
%>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <button onclick="javascript:fnSaveTemplate()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:$('#formEmailTemplate').jqxValidator('hide'); history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                </td>
                <td align="right"></td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <form id="formEmailTemplate" action="" style="margin: 10px;">        
            <script type="text/javascript">
                $(function () {
                    $("#txtContent").jqte();
                    $("#dtActionDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
                    $("#formEmailTemplate").jqxValidator({
                        rules: [
                            { input: '#txtName', message: 'Nhập tên email mẫu.', action: 'keyup, blur', rule: 'required' },
                            { input: '#txtSubject', message: 'Nhập tiêu đề của email.', action: 'keyup, blur', rule: 'required' },
                            { input: '#dtActionDate', message: 'Nhập ngày email tự động gởi.', action: 'keyup, blur', rule: function (input, commit) {
                                if ($('#dtActionDate').val() == '') {
                                    return false;
                                }
                                return true;
                            }
                            },
                            { input: '#txtContent', message: 'Nhập nội dung của email.', action: 'keyup, blur', rule: 'required' }
                        ]
                    });
                });

                function fnSaveTemplate() {
                    if ($("#formEmailTemplate").jqxValidator('validate')) {
                        $('.button').attr('disabled', true);
                        var group = new Array(), type = new Array(), gender = new Array();
                        var groups = $('input[name="Groups"]');
                        $.each(groups, function () {
                            if ($(this).attr('checked')) {
                                group.push($(this).val());
                            }
                        });
                        var types = $('input[name="Types"]');
                        $.each(types, function () {
                            if ($(this).attr('checked')) {
                                type.push($(this).val());
                            }
                        });
                        var genders = $('input[name="Genders"]');
                        $.each(genders, function () {
                            if ($(this).attr('checked')) {
                                gender.push($(this).val());
                            }                            
                        });
                        $.ajax({
                            type: "POST",
                            url: appPath + 'Utilities/SaveSystemParam',
                            data: {
                                id: $("#slEmailTypes").val(),
                                name: $("#txtName").val(),
                                subject: $("#txtSubject").val(),
                                actionDate: $("#dtActionDate").val(),
                                autoSend: ($('#cbAllow').is(':checked')) ? 1 : 0,
                                group: group.toString(),
                                customerType: type.toString(),
                                gender: gender.toString(),
                                content: $("#txtContent").val(),
                                objectName: $('#txtObjectName').val(),
                                type: $('#txtType').val()
                            },
                            success: function (data) {
                                if (data == "") {
                                    alert('Chúc mừng bạn đã lưu thành công!');
                                }
                                else {
                                    alert(data);
                                    $('.button').attr('disabled', true);
                                }
                            }
                        });
                    } else alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!");
                }
            </script>
            <input type="hidden" id="txtObjectName" value="<%=objectName%>" />
            <input type="hidden" id="txtType" value="<%=type%>" />
            <table style="width: 100%" cellpadding="5">
                <tr>
                    <td class="lbright">Loại mẫu</td>
                    <td>
                        <%Html.RenderAction("slEmailTemplates", "UserControl", new { elementId = "slEmailTypes", value = id });%>
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Tên mẫu</td>
                    <td><input type="text" id="txtName" name="txtName" value="<%=name%>" style="width: 100%;" /></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
                    <td><input type="text" id="txtSubject" name="txtSubject" value="<%=subject%>" style="width: 100%;" /></td>
                </tr>
                <tr>
                    <td class="lbright">Ngày gửi</td>
                    <td><input type="text"  id="dtActionDate" value="<%=actionDate%>" /> <input type="checkbox" id="cbAllow" <%=autoSend%> /> <label for="cbAllow">Cho phép tự động gửi</label></td>
                </tr>
                <tr>
                    <td class="lbright">Đối tượng</td>
                    <td>
                        <table>
                            <tr id="rowGroup">
                                <td>
                                    <% 
                                        
                                    %>
                                    <input type="checkbox" name="Groups" id="cbUser" value="<%=NEXMI.NMConstant.CustomerGroups.User%>" <%=strUser%> /> <label for="cbUser">Người dùng</label>
                                    <input type="checkbox" name="Groups" id="cbCustomer" value="<%=NEXMI.NMConstant.CustomerGroups.Customer%>" <%=strCustomer%> /> <label for="cbCustomer"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></label>
                                    <input type="checkbox" name="Groups" id="cbSupplier" value="<%=NEXMI.NMConstant.CustomerGroups.Supplier%>" <%=strSupplier%> /> <label for="cbSupplier"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></label>
                                    <input type="checkbox" name="Groups" id="cbManufacturer" value="<%=NEXMI.NMConstant.CustomerGroups.Manufacturer%>" <%=strManufacturer%> /> <label for="cbManufacturer">Nhà sản xuất</label>
                                </td>
                            </tr>
                            <tr id="rowType">
                                <td>
                                    <input type="checkbox" name="Types" id="cbIndividual" value="<%=NEXMI.NMConstant.CustomerTypes.Individual%>" <%=strIndividual%>/> <label for="cbIndividual">Cá nhân</label>
                                    <input type="checkbox" name="Types" id="cbEnterprise" value="<%=NEXMI.NMConstant.CustomerTypes.Enterprise%>" <%=strEnterprise%> /> <label for="cbEnterprise">Doanh nghiệp</label>
                                </td>
                            </tr>
                            <tr id="rowGender">
                                <td>
                                    <input type="checkbox" name="Genders" id="cbMale" value="1" <%=strMale%> /> <label for="cbMale">Nam</label>
                                    <input type="checkbox" name="Genders" id="cbFemale" value="0" <%=strFemale%> /> <label for="cbFemale">Nữ</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CONTENT", langId) %></td>
                    <td>
                        <textarea id="txtContent" name="txtContent" rows="1" cols="1" style="width: 100%; height: 300px;"><%=content%></textarea>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</div>