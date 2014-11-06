<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = "";
    string groupId=ViewData["GroupId"].ToString();
    string cusTitle = NEXMI.NMCommon.GetInterface("CUSTOMER", langId);
    if (groupId == "G000")
        cusTitle = NEXMI.NMCommon.GetInterface("COMPANY", langId);
    else if (groupId == "G004")
        cusTitle = NEXMI.NMCommon.GetInterface("STAFF", langId);
    else if (groupId == "G002")
        cusTitle = NEXMI.NMCommon.GetInterface("SUPPLIER", langId);
    else if (groupId == "G003")
        cusTitle = NEXMI.NMCommon.GetInterface("MANUFACTURE", langId);
        
    if (ViewData["WindowId"] == null)
    {
        pos = "static";
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnSaveOrUpdateCustomer('')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:fnSaveOrUpdateCustomer('', '1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("CustomerSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
<% 
    }
    else
    {
        windowId = ViewData["WindowId"].ToString(); 
    }
%>
    <div class="windowContent" style="position: <%=pos%> !important;">
    <script type="text/javascript">
        function fnChangeType() {
            var avatar = $("#avatar").attr('src');
            if (avatar == "" || avatar.indexOf('personal.png') > 0 || avatar.indexOf('company.png') > 0) {
                var personalImage = appPath + "Content/avatars/personal.png";
                var enterpriseImage = appPath + "Content/avatars/company.png";
                if ($("#cbCustomerTypes").is(":checked")) {
                    $('#avatar').attr('src', enterpriseImage);
                } else {
                    $('#avatar').attr('src', personalImage);
                }
            }
            if ($("#cbCustomerTypes").is(":checked")) {
                $('#tabsCustomerEdit').jqxTabs('enableAt', 1);
                $('#lbDOB').text('<%= NEXMI.NMCommon.GetInterface("ESTABLISH_DATE", langId) %>');
                $('.type2').css({ 'display': '' });
            } else {
                $('#tabsCustomerEdit').jqxTabs('disableAt', 1);
                $('#lbDOB').text('<%= NEXMI.NMCommon.GetInterface("BIRTHDAY", langId) %>');
                $('.type2').css({ 'display': 'none' });
            }
        }

        function uploadClick() {
            $("#uploadFileImage").click();
        }

        function imageHandler(evt2) {
            $('#avatar').attr('src', evt2.target.result);
        }

        function loadImage(evt1) {
            var fileName = evt1.files[0];
            var reader = new FileReader();
            reader.onload = imageHandler;
            reader.readAsDataURL(fileName);
        }

        function fnSaveOrUpdateCustomer(mode, saveMode) {
            if ($("#formCustomer").jqxValidator('validate')) {
                var typeId = "PERSONAL";
                if ($("#cbCustomerTypes").is(":checked")) {
                    typeId = "";
                    var contactPerson = $("#tbContactPersons tbody").html();
                    //alert(contactPerson);
                    if (contactPerson == undefined) {
                        alert('Nhập thông tin người đại diện!');
                        return;
                    }
                }
                var areaId = "";
                try {
                    areaId = $("#cbbAreas").jqxComboBox('getSelectedItem').value;
                } catch (err) { }
                var name = $("#txtName").val();
                $.ajax({
                    type: "POST",
                    url: appPath + 'Sales/SaveOrUpdateCustomer',
                    data: {
                        id: $("#txtId").val(),
                        name: name,
                        typeId: typeId,
                        taxCode: $("#txtTaxCode").val(),
                        address: $("#txtAddress").val(),
                        telephone: $("#txtTelephone").val(),
                        cellphone: $("#txtCellphone").val(),
                        faxNumber: $("#txtFaxNumber").val(),
                        email: $("#txtEmail").val(),
                        groupId: $("#txtGroupId").val(),
                        description: $("#txtDescription").val(),
                        maxDebitAmount: fnStripNonNumeric($("#txtMaxDebitAmount").val()),
                        areaId: areaId,
                        fileName: $("#uploadFileImage").val().split('\\').pop(),
                        imageUrl: $("#avatar").attr('src'),
                        managedBy: $("#slUsers").val(),
                        dueDate: $("#txtDueDate").val(),
                        website: $("#txtWebsite").val(),
                        code: $('#txtCustomerCode').val(),
                        dateDOB: $('#dateDOB').val(),
                        KOI: $('#slKindOfIndustries').val(),
                        KOE: $('#slKindOfEnterprises').val(),
                        bonusPoints: $('#txtBonusPoints').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            if (mode == 'popup') {
                                fnSetItemForCombobox('cbbCustomers<%=ViewData["ParentElement"]%>', data.result, Base64.encode(name));
                                closeWindow('<%=ViewData["WindowId"]%>');
                            } else {
                                if (saveMode == '1')
                                    fnResetFormCustomer();
                                else
                                    fnLoadCustomerDetail(data.result, $("#txtGroupId").val());
                            }
                        }
                        else
                            alert(data.error);
                        fnSuccess();
                    },
                    error: function () {
                        fnError();
                    },
                    complete: function () {
                        fnComplete();
                    }
                });
            } else {
                alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu màu đỏ!");
            }
        }

        function fnResetFormCustomer() {
            $("#txtId").val("");
            $("#txtName").val("");
            $("#txtTaxCode").val("");
            $("#txtAddress").val("");
            $("#txtTelephone").val("");
            $("#txtCellphone").val("");
            $("#txtAreaId").val("");
            $("#txtAreaName").val("");
            $("#txtMaxDebitAmount").val("");
            $("#txtWebsite").val("");
            $("#txtFaxNumber").val("");
            $("#txtEmail").val("");
            $('#txtBonusPoints').val('');
            $("#txtDescription").val("");
            try {
                fnChangeType();
            } catch (Error) { }
            $("#tbContactPersons tbody").html("");
            $("#txtName").focus();
        }

        function fnPopupContactPersonDialog(id) {
            openWindow("Thông tin người đại diện", "Customers/ContactPersonForm", { id: id }, 500, 350);
        }

        function fnSaveOrUpdateContactPerson() {
            if ($("#formContactPerson").jqxValidator('validate')) {
                var id = $("#txtCPId").val();
                var name = $("#txtCPName").val();
                var position = $("#txtCPPosition").val();
                var phoneNumber = $("#txtCPPhoneNumber").val();
                var cellphone = $("#txtCPCellphone").val();
                var email = $("#txtCPEmail").val();
                $.ajax({
                    type: "POST",
                    url: appPath + 'Customers/SaveContactPerson',
                    data: {
                        id: id, name: name, position: position, phoneNumber: phoneNumber, cellphone: cellphone, email: email
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            id = data.result;
                            var elm = document.getElementById(id);
                            var row = "";
                            row += "<td>" + name + "</td>";
                            row += "<td>" + position + "</td>";
                            row += "<td>" + phoneNumber + "</td>";
                            row += "<td>" + cellphone + "</td>";
                            row += "<td>" + email + "</td>";
                            row += "<td class='actionCols'><button type='button' class='btActions update' onclick='javascript:fnPopupContactPersonDialog(\"" + id + "\")'></button> <button type='button' class='btActions delete' onclick='javascript:fnDeleteContactPerson(\"" + id + "\")'></button></td>";
                            if (elm == undefined) {
                                $("#tbContactPersons tbody").append("<tr id='" + id + "'>" + row + "</tr>");
                            } else {
                                $("#tbContactPersons tbody tr#" + id).replaceWith("<tr id='" + id + "'>" + row + "</tr>");
                            }
                            fnResetFormContactPerson();
                        }
                        else
                            alert(data.error);
                        fnSuccess();
                    },
                    error: function () {
                        fnError();
                    },
                    complete: function () {
                        fnComplete();
                    }
                });
            }
        }

        function fnDeleteContactPerson(id) {
            if (confirm("Bạn muốn xóa người đại diện này?")) {
                //OpenProcessing();
                $.post(appPath + "Customers/DeleteContactPerson", { id: id }, function (data) {
                    //CloseProcessing();
                    if (data == "") {
                        $("#tbContactPersons tbody tr#" + id).remove();
                    }
                    else {
                        alert(data);
                    }
                });
            }
        }

        function fnResetFormContactPerson() {
            $("#txtCPId").val("");
            $("#txtCPName").val("");
            $("#txtCPPosition").val("");
            $("#txtCPPhoneNumber").val("");
            $("#txtCPCellphone").val("");
            $("#txtCPEmail").val("");
            $("#txtCPName").focus();
        }
        </script>
<% if (NEXMI.NMCommon.GetSetting("CHECK_BEFORE_SAVE") == true)
   {  %>
    <script type="text/javascript">
        $(function () {
            $("#dateDOB, #txtDueDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
            $("#tabsCustomerEdit").jqxTabs({ theme: theme, keyboardNavigation: false });
            $('#slKindOfEnterprises').prepend('<option value="" selected="selected"></option>');
            $('#slKindOfEnterprises').val('<%=ViewData["KOE"]%>');
            $('#slKindOfIndustries').prepend('<option value="" selected="selected"></option>');
            $('#slKindOfIndustries').val('<%=ViewData["KOI"]%>');
            $("#formCustomer").jqxValidator({
                rules: [
                            { input: '#txtName', message: 'Nhập tên <%=cusTitle%>.', action: 'keyup, blur', rule: 'required' },
                            { input: '#txtAddress', message: 'Nhập địa chỉ.', action: 'keyup, blur', rule: 'required' },
                            { input: '#txtEmail', message: 'Email không đúng định dạng.', action: 'keyup, blur', rule: 'email' },
                            { input: '#txtTelephone', message: 'Nhập số điện thoại liên lạc.', action: 'keyup, blur', rule: function (input, commit) {
                                if ($("#cbCustomerTypes").is(":checked") && input.val() === '') {
                                    return true;
                                }
                                return false;
                            }
                            },
                            { input: '#txtCellphone', message: 'Nhập số điện thoại liên lạc.', action: 'keyup, blur', rule: function (input, commit) {
                                if ($("#cbCustomerTypes").is(":checked") == false && input.val() === '') {
                                    return false;
                                }
                                return true;
                            }
                            }
                        ]
            });
            fnChangeType();
            $("#uploadFileImage").change(function () {
                loadImage(document.getElementById("uploadFileImage"));
            });
            $("#cbCustomerTypes").click(function () {
                fnChangeType();
            });
            $("#txtName").focus();
        });
    </script>
<%}
else
{ %>
   <script type="text/javascript">
       $(function () {
           $("#dateDOB, #txtDueDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
           $("#tabsCustomerEdit").jqxTabs({ theme: theme, keyboardNavigation: false });
           $('#slKindOfEnterprises').prepend('<option value="" selected="selected"></option>');
           $('#slKindOfEnterprises').val('<%=ViewData["KOE"]%>');
           $('#slKindOfIndustries').prepend('<option value="" selected="selected"></option>');
           $('#slKindOfIndustries').val('<%=ViewData["KOI"]%>');
           $("#formCustomer").jqxValidator({
               rules: [
                        { input: '#txtName', message: 'Nhập tên <%=cusTitle%>.', action: 'keyup, blur', rule: 'required' },
                        { input: '#txtAddress', message: 'Nhập địa chỉ.', action: 'keyup, blur', rule: 'required' },
                        { input: '#txtTelephone', message: 'Nhập số điện thoại liên lạc.', action: 'keyup, blur', rule: function (input, commit) {
                            if ($("#cbCustomerTypes").is(":checked") && input.val() === '') {
                                return false;
                            }
                            return true;
                        }
                        },
                        { input: '#txtCellphone', message: 'Nhập số điện thoại liên lạc.', action: 'keyup, blur', rule: function (input, commit) {
                            if ($("#cbCustomerTypes").is(":checked") == false && input.val() === '') {
                                return false;
                            }
                            return true;
                        }
                        }
                    ]
           });
           fnChangeType();
           $("#uploadFileImage").change(function () {
               loadImage(document.getElementById("uploadFileImage"));
           });
           $("#cbCustomerTypes").click(function () {
               fnChangeType();
           });
           $("#txtName").focus();
       });
</script>
<%} %>
        
        <form id="formCustomer" action="" style="margin: 10px;">
            <input type="hidden" id="txtGroupId" value="<%=groupId%>" />
            <table class="frmInput">
                <tr>
                    <td>
                        <table class="frmInput">
                            <tr>
                                <td style="width: 120px;" align="center">
                                    <input type="file" id="uploadFileImage" style="display: none;" />
                                    <img id="avatar" alt="" src="<%=ViewData["avatar"]%>" style="width: 100px; height: 100px;" /><br />
                                    <a href='javascript:uploadClick()'>Thay đổi</a>
                                </td>
                                <td valign="top" style="font-weight: bold">
                            <%if (groupId != "G004" & groupId != "G000")
                            { %> 
                                    <input type="checkbox" id="cbCustomerTypes" <%=ViewData["strChecked"]%> /> <%= NEXMI.NMCommon.GetInterface("IS_COMPANY", langId) %><br />
                            <%} %>
                                    <%=NEXMI.NMCommon.GetInterface("CODE", langId)%> <%=cusTitle%>: <br /><input type="text" id="txtCustomerCode" value="<%=ViewData["code"]%>" />
                                    <input type='hidden' id='cbCustomerTypes' />
                                    <br /><%=NEXMI.NMCommon.GetInterface("NAME", langId)%>  <%=cusTitle%> 
                                    <br /><input type="text" id="txtName" value="<%=ViewData["name"]%>" /><input type="hidden" id="txtId" value="<%=ViewData["id"]%>" style="width: 80%;" />    
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %></td>
                                <td><input type="text" id="txtTaxCode" value="<%=ViewData["taxCode"]%>" maxlength="26" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></td>
                                <td><input type="text" id="txtTelephone" value="<%=ViewData["telephone"]%>" maxlength="100" /></td>
                                <td class="lbright"><label id="lbDOB"></label></td>
                                <td><input type="text"  id="dateDOB" value="<%=ViewData["DateOfBirth"]%>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %></td>
                                <td><input type="text" id="txtAddress" value="<%=ViewData["address"]%>" maxlength="200" /></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MOBILEPHONE", langId) %></td>
                                <td><input type="text" id="txtCellphone" value="<%=ViewData["cellphone"]%>" maxlength="100" /></td>
                                <td class="lbright type2"><%= NEXMI.NMCommon.GetInterface("CATEGORIES", langId) %></td>
                                <td class="type2"><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slKindOfIndustries", objectName = NEXMI.NMConstant.ObjectNames.KindOfIndustry });%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("AREA", langId) %></td>
                                <td><%Html.RenderAction("cbbAreas", "UserControl", new { areaId = ViewData["AreaId"], areaName = ViewData["AreaName"], elementId = ViewData["WindowId"] });%></td>
                                <td class="lbright">Fax</td>
                                <td><input type="text" id="txtFaxNumber" value="<%=ViewData["faxNumber"]%>" maxlength="100" /></td>
                                <td class="lbright type2"><%= NEXMI.NMCommon.GetInterface("KIND_OF_ENTERPRISE", langId) %></td>
                                <td class="type2"><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slKindOfEnterprises", objectName = NEXMI.NMConstant.ObjectNames.KindOfEnterprise });%></td>
                            </tr>
                            <tr>
                                <td class="lbright">Website</td>
                                <td><input type="text" id="txtWebsite" value="<%=ViewData["website"]%>" maxlength="100" /></td>
                                <td class="lbright">Email</td>
                                <td><input type="text" id="txtEmail" value="<%=ViewData["email"]%>" maxlength="60" /></td>                            
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="tabsCustomerEdit">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("CONTACT_PERSON", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div style="padding: 10px;">
                    <textarea id="txtDescription" cols="30" rows="5" style="width: 99%"><%=ViewData["description"]%></textarea>
                </div>
                <div style="padding: 10px;">
                    <button type="button" onclick="javascript:fnPopupContactPersonDialog('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <br />
                    <table id="tbContactPersons" class="tbDetails">
                        <thead>
                            <tr>
                                <th><%= NEXMI.NMCommon.GetInterface("NAME", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("POSITION", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("MOBILEPHONE", langId) %></th>
                                <th>Email</th>
                                <th width="8%"></th>
                            </tr>
                        </thead>
                        <tbody>
                        <% 
                            List<NEXMI.Customers> ContactPersons = (List<NEXMI.Customers>)Session["ContactPersons"];
                            if (ContactPersons != null)
                            {
                                foreach (NEXMI.Customers Item in ContactPersons)
                                {
                        %>
                            <tr id="<%=Item.CustomerId%>">
                                <td><%=Item.CompanyNameInVietnamese%></td>
                                <td><%=Item.JobPosition%></td>
                                <td><%=Item.Telephone%></td>
                                <td><%=Item.Cellphone%></td>
                                <td><%=Item.EmailAddress%></td>
                                <td class="actionCols">
                                    <button type="button" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" class="btActions update" onclick="javascript:fnPopupContactPersonDialog('<%=Item.CustomerId%>')"></button>
                                    <button type="button" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" class="btActions delete" onclick="javascript:fnDeleteContactPerson('<%=Item.CustomerId%>')"></button>
                                </td>
                            </tr>
                        <%
                                }
                            }
                        %>
                        </tbody>
                    </table>
                </div>
                <div style="padding: 10px;">
                    <table style="width: 100%" class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ACCOUNT_MANAGER", langId) %></td>
                            <td>
                                <%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers", userId = ViewData["slUsers"] }); %></td>
                        <%if (groupId == "G004")
                        { %>
                            <td class="lbright">Nơi làm việc</td>
                            <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slToStock", stockId = ViewData["slStocks"] });%></td>
                        <%}
                        else
                        { %>
                            <td id='slUsers'></td>
                            <td id='slToStock'></td>
                        <%} %>                            
                        </tr>
                        <tr>
                        <%if (groupId != "G004")
                          { %>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MAX_DEBT", langId) %></td>
                            <td><input id="txtMaxDebitAmount" type="text" value="<%=ViewData["maxDebitAmount"]%>" onkeypress="return fnOnlyNumber(event);" onkeyup="this.value = fnAddCommas(this.value)" maxlength="15" /></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_DATE", langId) %></td>
                            <td><input type="text"  id="txtDueDate" value="<%=ViewData["dueDate"]%>" /></td>
                        <%}
                          else
                          { %>
                          <td id="txtMaxDebitAmount" ></td>
                          <td id="txtDueDate" ></td>
                        <%} %>
                        </tr>
                        <tr>
                            <%if(groupId != "G004"){ %>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BONUS_POINT", langId) %></td>
                                <td><input type="text" id="txtBonusPoints" value="<%=ViewData["BonusPoints"]%>" /></td>
                            <%} %>
                            <td></td>
                            <td id="txtBonusPoints"></td>
                        </tr>
                        
                    </table>
                </div>
            </div>
        </form>
    </div>
<% 
    if (ViewData["WindowId"] != null)
    {
%>
    <div class="windowButtons">
        <div class="NMButtons">
            <button onclick="javascript:fnSaveOrUpdateCustomer('popup')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
        </div>
    </div>
</div>
<% 
    }    
%>

