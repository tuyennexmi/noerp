<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = ViewData["WindowId"].ToString(), popupMode = "";
    NEXMI.NMTasksWSI WSI = new NEXMI.NMTasksWSI();
    if (ViewData["WSI"] != null)
    {
        WSI = (NEXMI.NMTasksWSI)ViewData["WSI"];
    }
    if (WSI.Task == null)
    {
        WSI.Task = new NEXMI.Tasks();
        WSI.Task.StageId = ViewData["StageId"].ToString();
        WSI.Task.StatusId = NEXMI.NMConstant.TaskStatuses.InProgress;
        WSI.Task.Deadline = DateTime.Today;
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
                    <button onclick="javascript:fnSaveOrUpdateTask('close')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:fnSaveOrUpdateTask('')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button">Thoát</button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("ProductSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
<% 
    }
%>
    <div class="divStatus">
        <div class="divButtons">
            
        </div>     
        <div class="divStatusBar">
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Tasks", current = WSI.Task.StatusId });%>
        </div>
    </div>
    <div class="windowContent" style="position: <%=pos%> !important;">
        <script type="text/javascript">
            $(function () {
                $("#txtStartDate<%=windowId%>, #txtDeadline<%=windowId%>, #txtEndDate<%=windowId%> ").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
                $('#tabs<%=windowId%>').jqxTabs({ theme: theme, keyboardNavigation: false });
                $('#txtTags<%=windowId%>').tagit({ allowSpaces: true });
                $('#form<%=windowId%>').jqxValidator({
                    rules: [
                        { input: '#txtName<%=windowId%>', message: 'Không được để trống', action: 'keyup, blur', rule: 'required' },
                        //{ input: '#txtCategory<%=windowId%>', message: 'Không được để trống', action: 'keyup, blur', rule: 'required' },
                        { input: '#cbbProjects<%=windowId%>', message: 'Chọn dự án', action: 'change', rule: function (input) {
                            if ($(input).jqxComboBox('getSelectedItem') == null)
                                return false;
                            return true;
                        }
                        },
                    ]
                });
                $('#txtName<%=windowId%>').focus();
            });

            function fnSaveOrUpdateTask(saveMode) {
                if ($('#form<%=windowId%>').jqxValidator('validate')) {
                    var name = $('#txtName<%=windowId%>').val();
                    var project = document.getElementsByName('cbbProjects<%=windowId%>')[0];
                    var jobref = document.getElementsByName('cbbJobs<%=windowId%>')[0].value;
                    $.ajax({
                        type: 'POST',
                        url: appPath + 'Projects/SaveOrUpdateTask',
                        data: {
                            id: $('#txtId<%=windowId%>').val(),
                            name: name,
                            jobref: jobref,
                            purpose: $('#txtPurpose<%=windowId%>').val(),
                            criteria: $('#txtCriteria<%=windowId%>').val(),
                            stageId: $('#slStages<%=windowId%>').val(),
                            projectId: project.value,
                            deadLine: $('#txtDeadline<%=windowId%>').val(),
                            assignedTo: $('#slUsers<%=windowId%>').val(),
                            priority: $('#slPriorities<%=windowId%>').val(),
                            sequence: $('#txtSequence<%=windowId%>').val(),
                            tags: $('#txtTags<%=windowId%>').val(),
                            description: $('#txtDescription<%=windowId%>').val(),
                            statusId: $('#txtStatus<%=windowId%>').val(),
                            start: $('#txtStartDate<%=windowId%>').val(),
                            end: $('#txtEndDate<%=windowId%>').val(),
                            checkedBy: $('#slCheckedBy<%=windowId%>').val(),
                            manager: $('#slManager<%=windowId%>').val(),
                            category: $('#txtCategory<%=windowId%>').val(),
                            reportsPeriod: $('#slReportsPeriod<%=windowId%>').val()
                        },
                        beforeSend: function () {
                            fnDoing();
                        },
                        success: function (data) {
                            if (data.error == "") {
                                fnSuccess();
                                if (saveMode == 'close') {
                                    if ('<%=popupMode%>' == '') {
                                        try {
                                            fnSetProjectItem(project.value, project.label);
                                        } catch (err) { }
                                        //fnSetItemForCombobox('<%=ViewData["ComboBoxId"]%>', data.id, Base64.encode(name));
                                        setTimeout(function () {
                                            $('#<%=ViewData["ComboBoxId"]%>').jqxComboBox('selectIndex', 0);
                                            closeWindow('<%=windowId%>');
                                        }, 300);
                                    }
                                    else {
                                        fnLoadTaskDetail(data.id);
                                    }
                                }
                                else
                                    fnResetFormTask();
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

            function fnResetFormTask() {
                $('#txtId<%=windowId%>').val('');
                $('#txtName<%=windowId%>').val('');
                $('#slStatuses<%=windowId%>').val('');
                $('#txtSequence<%=windowId%>').val('');
                document.getElementById('cbCommon<%=windowId%>').checked = false;
                document.getElementById('cbFold<%=windowId%>').checked = false;
                $('#txtDescription<%=windowId%>').val('');
            }

            function fnAppendWorkToList(no, name, timeSpent, startDate, userId) {
                var data = '<tr id="row' + no + '"><td>' + no + '</td><td>' + name + '</td><td>' + timeSpent + '</td><td>' + startDate + '</td><td>'
                + userId + '</td><td><a href="javascript:fnPopupWorkDialog(\'' + no + '\')">'
                + '<img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>'
                + '<a href="javascript:fnRemoveWork(\'' + no + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a></td></tr>';
                if (document.getElementById('row' + no) != null)
                    $('#row' + no).replaceWith(data);
                else
                    $('#tbodyWork<%=windowId%>').append(data);
                try {
                    var total = $('#lbTotalTimeSpent').text();
                    $('#lbTotalTimeSpent').text(fnNumberFormat(parseFloat(total) + parseFloat(timeSpent)));
                } catch (err) { }
            }

            function fnRemoveWork(id) {
                if (confirm('Bạn muốn xóa công việc này?')) {
                    $.ajax({
                        type: 'POST',
                        url: appPath + 'Projects/RemoveWorkFromSession',
                        data: {
                            id: id
                        },
                        success: function (data) {
                            if (data == '') {
                                //$('#row' + id).remove();
                                LoadContentDynamic('TaskItemLine', 'Projects/TaskItemLine');
                            }
                            else {
                                alert(data);
                            }
                        },
                        error: function () {
                            fnError();
                        }
                    });
                }
            }

            function cbbJobChanged(item) {
                var jobId = '';
                if (item != null) {
                    jobId = item.Id;
                    $('#txtName<%=windowId%>').val(item.Name);
                    $('#txtPurpose<%=windowId%>').val(item.Purpose);
                    $('#txtCriteria<%=windowId%>').val(item.Criteria);
                    $('#txtDescription<%=windowId%>').val(item.WorkGuids);

                } else {
                    $('#txtName<%=windowId%>').val('');
                }
            }
        </script>
        <div>
            <form id="form<%=windowId%>" class="form-to-validate" action="">
                <table style="width: 100%" class="frmInput">
                    <tr>
                        <td class="lbright">Công việc tham chiếu</td>
                        <td><%Html.RenderAction("cbbJobs", "UserControl", new { elementId = "cbbJobs" + windowId, windowTitle = "Danh sách công việc", id = WSI.Task.JobReference, label = ViewData["ProjectName"] });%></td>
                        <td></td><td></td>
                    </tr>
                    <tr>
                        <td class="lbright">Tên nhiệm vụ</td>
                        <td>
                            <input type="hidden" id="txtId<%=windowId%>" value="<%=WSI.Task.TaskId%>" />
                            <input type="hidden" id="txtStatus<%=windowId%>" value="<%=WSI.Task.StatusId%>" />
                            <input type="text" id="txtName<%=windowId%>" value="<%=WSI.Task.TaskName%>" />
                        </td>
                        <td class="lbright">Giao cho</td>
                        <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers" + windowId, userId = WSI.Task.AssignedTo });%></td>
                    </tr>
                    <tr>
                        <td class="lbright">Mục đích</td>
                        <td><input type="text" id="txtPurpose<%=windowId%>" value="<%=WSI.Task.Purpose%>" /></td>
                        <td class="lbright">Tiêu chí</td>
                        <td><input type="text" id="txtCriteria<%=windowId%>" value="<%=WSI.Task.Criteria%>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright">Dự án</td>
                        <td><%Html.RenderAction("cbbProjects", "UserControl", new { elementId = "cbbProjects" + windowId, windowTitle = "Danh sách dự án", id = ViewData["ProjectId"], label = ViewData["ProjectName"] });%></td>
                        <td class="lbright">Hạng mục</td>
                        <td><input type="text" id="txtCategory<%=windowId%>" value="<%=WSI.Task.Category %>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright">Ngày bắt đầu</td>
                        <td><input type="text" id="txtStartDate<%=windowId%>" value="<%=(WSI.Task.StartDate != null) ? WSI.Task.StartDate.Value.ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd")%>" /></td>
                        <td class="lbright">Ngày hết hạn</td>
                        <td><input type="text" id="txtDeadline<%=windowId%>" value="<%=(WSI.Task.Deadline != null) ? WSI.Task.Deadline.ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd")%>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright">Người kiểm soát</td>
                        <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slCheckedBy" + windowId, userId = WSI.Task.CheckedBy });%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ACCOUNT_MANAGER", langId) %></td>
                        <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slManager" + windowId, userId = WSI.Task.Manager });%></td>
                    </tr>
                </table>
                <div id="tabs<%=windowId%>"> 
                    <ul>
                        <li style="margin-left: 10px;">Hướng dẫn</li>
                        <li>Các công việc</li>
                        <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                    </ul>
                    <div style="padding:10px;">
                        <textarea id="txtDescription<%=windowId%>" cols="1" rows="5" style="width: 99%;"><%=WSI.Task.Description%></textarea>
                    </div>
                    <div style="padding:10px;">
                        <input type="button" class="button" value="Thêm" onclick="javascript:fnPopupWorkDialog('');" /><br /><br />
                        <div id='TaskItemLine'>
                        <table class="tbDetails">
                            <tr>
                                <th>#</th>
                                <th>Công việc</th>
                                <th>Thời gian</th>
                                <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
                                <th>Người thực hiện</th>
                                <th></th>
                            </tr>
                            <tbody id="tbodyWork<%=windowId%>">
                <% 
                    double totalTimeSpent = 0;
                    if (WSI.Details != null)
                    {
                        foreach (NEXMI.TaskDetails Item in WSI.Details)
                        {
                            totalTimeSpent += Item.TimeSpent;
                            Item.TaskId = Item.OrdinalNumber.ToString();
                %>
                            <tr id="row<%=Item.OrdinalNumber%>">
                                <td><%=Item.No%></td>
                                <td><%=Item.Name%></td>
                                <td><%=Item.TimeSpent.ToString("N3")%></td>
                                <td><%=Item.StartDate.ToString("dd/MM/yyyy HH:mm")%></td>
                                <td><%=NEXMI.NMCommon.GetCustomerName(Item.UserId)%></td>
                                <td>
                                    <a href="javascript:fnPopupWorkDialog('<%=Item.OrdinalNumber%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>
                                    <a href="javascript:fnRemoveWork('<%=Item.OrdinalNumber%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>
                                </td>
                            </tr>
                <%
                        }
                        Session["TaskDetails"] = WSI.Details;
                    }
                %>       
                            </tbody>
                            <tr>
                                <td>Tổng thời gian</td><td></td>
                                <td><b id="lbTotalTimeSpent"><%=totalTimeSpent.ToString("N3")%></b></td>
                                <td></td><td></td><td></td>
                            </tr>
                        </table>
                        </div>
                    </div>
                    <div style="padding:10px;">
                        <table style="width: 100%" class="frmInput">
                            <tr>
                                <td class="lbright">Ngày kết thúc</td>
                                <td><input type="text" id="txtEndDate<%=windowId%>" value="<%=(WSI.Task.EndDate != null) ? WSI.Task.EndDate.Value.ToString("yyyy-MM-dd") : ""%>" /></td>
                                <td class="lbright">Các đánh dấu</td>
                                <td ><input type="text" id="txtTags<%=windowId%>" value="<%=WSI.Task.Tags%>" /></td>
                            </tr>
                            <tr>
                                <td class="lbright">Độ ưu tiên</td>
                                <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slPriorities" + windowId, objectName = "TaskPriorities", current = WSI.Task.Priority });%></td>
                                <td class="lbright">Chu kỳ báo cáo</td>
                                <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slReportsPeriod" + windowId, objectName = "ReportsPeriod", current = WSI.Task.Priority });%></td>
                            </tr>
                            <tr>
                                <td class="lbright">Thứ tự</td>
                                <td><input type="number" min="0" id="txtSequence<%=windowId%>" value="<%=WSI.Task.Sequence%>" /></td>
                                <td class="lbright">Thuộc giai đoạn</td>
                                <td><%Html.RenderAction("slStages", "UserControl", new { elementId = windowId, current = WSI.Task.StageId, projectStage = ViewData["ProjectStages"] });%></td>
                            </tr>

                        </table>
                    </div>
                </div>
            </form>
        </div>
    </div>
<% 
    if (ViewData["WindowMode"] == null)
    {
%>
    <div class="windowButtons">
        <div class="NMButtons">
            <button onclick="javascript:fnSaveOrUpdateTask('close')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            <button onclick="javascript:fnSaveOrUpdateTask('')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
            <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium">Thoát</button>
        </div>
    </div>
<% 
    }    
%>
</div>