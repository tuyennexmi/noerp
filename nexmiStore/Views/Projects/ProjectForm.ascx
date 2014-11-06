<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = ViewData["WindowId"].ToString(), popupMode = "";
    string id = "", projectName = "", cbTask = "checked", cbIssue = "checked", customerId = "", customerName = "", managedBy = "",
        startDate = DateTime.Today.ToString("yyyy-MM-dd"), endDate = "", parentId = "", parentName = "", statusId = NEXMI.NMConstant.ProjectStatuses.InProgress;
    string salesForecast ="";
    //string[] team = { }, stage = { };
    if (ViewData["WSI"] != null)
    {
        NEXMI.NMProjectsWSI WSI = (NEXMI.NMProjectsWSI)ViewData["WSI"];
        id = WSI.Project.ProjectId;
        projectName = WSI.Project.ProjectName;
        if (WSI.Project.Task == false)
            cbTask = "";
        if (WSI.Project.Issue == false)
            cbIssue = "";
        if (!string.IsNullOrEmpty(WSI.Project.StatusId))
            statusId = WSI.Project.StatusId;
        if (WSI.Customer != null)
        {
            customerId = WSI.Customer.CustomerId;
            customerName = WSI.Customer.CompanyNameInVietnamese;
        }
        managedBy = WSI.Project.ManagedBy;
        if (WSI.Project.StartDate != null)
            startDate = WSI.Project.StartDate.ToString("yyyy-MM-dd");
        if (WSI.Project.EndDate != null)
            endDate = WSI.Project.EndDate.ToString("yyyy-MM-dd");
        TempData["CurrentUser"] = WSI.Teams;
        TempData["CurrentStage"] = WSI.Stages;
        salesForecast = WSI.Project.SalesForecast.ToString();
    }
    
    if (ViewData["WindowMode"] != null)
    {
        pos = "static";
        popupMode = "1";
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
                    <button onclick="javascript:fnSaveOrUpdateProject('close')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:fnSaveOrUpdateProject('')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                    <button onclick="javascript:$('#formProject').jqxValidator('hide'); history.back();" class="button">Thoát</button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("SalesOrderSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
<%
    }
%>
    <div class="windowContent" style="position: <%=pos%> !important;">
        <%--<div class="divStatus">     
            <div class="divStatusBar">
                <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Projects", current = statusId });%>
            </div>
        </div>--%>
        <script type="text/javascript">
            $(function () {
                $("#txtStartDate<%=windowId%>, #txtEndDate<%=windowId%>").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
                $('#tabs<%=windowId%>').jqxTabs({ theme: theme, keyboardNavigation: false });
                $('#form<%=windowId%>').jqxValidator({
                    rules: [
                        { input: '#txtName<%=windowId%>', message: 'Không được để trống', action: 'keyup, blur', rule: 'required' }
                    ]
                });
                fnCheckTask();
            })

            function fnCheckTask() {
                if (document.getElementById('cbTask<%=windowId%>').checked) {
                    $('#tabs<%=windowId%>').jqxTabs('enableAt', 2);
                } else {
                    $('#tabs<%=windowId%>').jqxTabs('disableAt', 2);
                    $('#tabs<%=windowId%>').jqxTabs('select', 0);
                }
            }

            function fnSaveOrUpdateProject(saveMode) {
                if ($('#form<%=windowId%>').jqxValidator('validate')) {
                    var parentId = '';
                    try {
                        parentId = $('#cbbProjects<%=windowId%>').jqxComboBox('getSelectedItem').value;
                    } catch (err) { }
                    var members = '';
                    $('.slMultiUsers<%=windowId%>').each(function () {
                        members += $(this).attr('id') + '_NM_';
                    });
                    var stages = '';
                    $('.slMultiStages<%=windowId%>').each(function () {
                        stages += $(this).attr('id') + '_NM_';
                    });
                    var name = $('#txtName<%=windowId%>').val();
                    $.ajax({
                        type: 'POST',
                        async: false,
                        url: appPath + 'Projects/SaveOrUpdateProject',
                        data: {
                            id: $('#txtId<%=windowId%>').val(),
                            name: name,
                            customerId: document.getElementsByName('cbbCustomers<%=windowId%>')[0].value,
                            managedBy: $('#slUsers<%=windowId%>').val(),
                            task: (document.getElementById('cbTask<%=windowId%>').checked) ? 'true' : 'false',
                            issue: (document.getElementById('cbIssue<%=windowId%>').checked) ? 'true' : 'false',
                            team: members,
                            stage: stages,
                            description: $('#txtDescription<%=windowId%>').val(),
                            startDate: $('#txtStartDate<%=windowId%>').val(),
                            endDate: $('#txtEndDate<%=windowId%>').val(),
                            parentId: parentId,
                            statusId: $('#txtStatusId<%=windowId%>').val(),
                            salesForecast: $('#txtSalesForecast<%=windowId%>').val()
                        },
                        beforeSend: function () {
                            fnDoing();
                        },
                        success: function (data) {
                            fnSuccess();
                            if (data.error == "") {
                                if (saveMode == 'close') {
                                    if ('<%=popupMode%>' == '') {
                                        fnSetItemForCombobox('<%=ViewData["ComboBoxId"]%>', data.id, Base64.encode(name));
                                        closeWindow('<%=windowId%>');
                                    }
                                    else {
                                        fnLoadProjectDetail(data.id);
                                    }
                                }
                                else
                                    fnResetProjectForm();
                            }
                            else {
                                alert(data.error);
                            }
                        },
                        error: function () {
                            fnError();
                        },
                        complete: function () {
                            fnComplete();
                        }
                    });
                } else {
                    alert('Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!');
                }
            }

            function fnResetProjectForm() {
                $('#txtId<%=windowId%>').val('');
                $('#txtName<%=windowId%>').val('');
                document.getElementById('cbTask<%=windowId%>').checked = true;
                document.getElementById('cbIssue<%=windowId%>').checked = true;
                $('#txtDescription<%=windowId%>').val('');
                $('#txtStartDate<%=windowId%>').val('<%=DateTime.Today.ToString("yyyy-MM-dd")%>');
                $('#txtEndDate<%=windowId%>').val('');
            }
        </script>
        <form id="form<%=windowId%>" action="">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td>
                        <table style="width: 100%" class="frmInput">
                            <tr>
                                <td class="lbright">Tên dự án</td>
                                <td>
                                    <input type="hidden" id="txtId<%=windowId%>" value="<%=id%>" />
                                    <input type="hidden" id="txtStatusId<%=windowId%>" value="<%=statusId%>" />
                                    <input type="text" id="txtName<%=windowId%>" value="<%=projectName%>" />
                                </td>
                                <td colspan="2">
                                    <%--<button type="button" class="button">Công việc</button>
                                    <button type="button" class="button">Tài liệu</button>
                                    <button type="button" class="button">Chấm công</button>
                                    <button type="button" class="button">Vấn đề</button>--%>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <b><label for="cbTask<%=windowId%>" title="Đánh dấu nếu dự án có quản lí các nhiệm vụ"><input type="checkbox" id="cbTask<%=windowId%>" <%=cbTask%> onchange="javascript:fnCheckTask()" /> Nhiệm vụ</label>
                                    <label for="cbIssue<%=windowId%>" title="Đánh dấu nếu dự án có quản lí các vấn đề"><input type="checkbox" id="cbIssue<%=windowId%>" <%=cbIssue%> /> Vấn đề</label></b>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { elementId = windowId, customerId = customerId, customerName = customerName });%></td>
                                <td class="lbright">Quản lý dự án</td>
                                <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers" + windowId, userId = managedBy });%></td>
                            </tr>
                            <tr>
                                <td class="lbright">Doanh số dự kiến</td>
                                <td><input type="text" id="txtSalesForecast<%=windowId%>" value="<%=salesForecast%>" /></td>
                                <td></td><td></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="tabs<%=windowId%>"> 
                            <ul>
                                <li style="margin-left: 10px;">Thành viên</li>
                                <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                                <li>Các giai đoạn</li>
                            </ul>
                            <div style="padding:10px;">
                                <%Html.RenderAction("slMultiUsers", "UserControl", new { elementId = "slMultiUsers" + windowId });%>
                            </div>
                            <div style="padding:10px;">
                                <table style="width:100%" class="frmInput">
                                    <tr>
                                        <td class="lbright">Ngày bắt đầu</td>
                                        <td><input type="text"  id="txtStartDate<%=windowId%>" value="<%=startDate%>" /></td>
                                    </tr>
                                    <tr>
                                        <td class="lbright">Ngày kết thúc</td>
                                        <td><input type="text"  id="txtEndDate<%=windowId%>" value="<%=endDate%>" /></td>
                                    </tr>
                                    <tr>
                                        <td class="lbright">Thuộc dự án</td>
                                        <td>
                                            <%Html.RenderAction("cbbProjects", "UserControl", new { elementId = "cbbProjects" + windowId, windowTitle = "Danh sách dự án", id = parentId, label = parentName });%>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div style="padding:10px;">
                                <%Html.RenderAction("slMultiStages", "UserControl", new { elementId = "slMultiStages" + windowId });%>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </form>
    </div>
<% 
    if (ViewData["WindowMode"] == null)
    {
%>
    <div class="windowButtons">
        <div class="NMButtons">
            <button onclick="javascript:fnSaveOrUpdateProject('close')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            <button onclick="javascript:fnSaveOrUpdateProject('')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
            <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium">Thoát</button>
        </div>
    </div>
<% 
    }
    else
    {
%>
</div>
<% 
    }    
%>