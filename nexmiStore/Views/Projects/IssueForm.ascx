<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = ViewData["WindowId"].ToString();
    NEXMI.NMIssuesWSI WSI = new NEXMI.NMIssuesWSI();
    if (ViewData["WSI"] != null)
    {
        WSI = (NEXMI.NMIssuesWSI)ViewData["WSI"];
    }
    if (WSI.Issue == null)
    {
        WSI.Issue = new NEXMI.Issues();
        WSI.Issue.StageId = ViewData["StageId"].ToString();
        WSI.Issue.StatusId = NEXMI.NMConstant.IssueStatuses.InProgress;
    }
    if (ViewData["WindowMode"] != null)
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
                    <button onclick="javascript:fnSaveOrUpdateIssue('close')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:fnSaveOrUpdateIssue('')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
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
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Issues", current = WSI.Issue.StatusId });%>
        </div>
    </div>
<div class="windowContent" style="position: <%=pos%> !important;">
    <script type="text/javascript">
        $(function () {
            $('#tabs<%=windowId%>').jqxTabs({ theme: theme, keyboardNavigation: false });
            $('#txtTags<%=windowId%>').tagit({ allowSpaces: true });
            $('#form<%=windowId%>').jqxValidator({
                rules: [
                    { input: '#txtName<%=windowId%>', message: 'Không được để trống', action: 'keyup, blur', rule: 'required' },
                    { input: '#txtEmail<%=windowId%>', message: 'Email không đúng định dạng.', action: 'keyup, blur', rule: 'email' }
                ]
            });
            $('#txtName<%=windowId%>').focus();
        });

        function fnSaveOrUpdateIssue(saveMode) {
            if ($('#form<%=windowId%>').jqxValidator('validate')) {
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Projects/SaveOrUpdateIssue',
                    data: {
                        id: $('#txtId<%=windowId%>').val(),
                        name: $('#txtName<%=windowId%>').val(),
                        stageId: $('#slStages<%=windowId%>').val(),
                        tags: $('#txtTags<%=windowId%>').val(),
                        assignedTo: $('#slUsers<%=windowId%>').val(),
                        priority: $('#slPriorities<%=windowId%>').val(),
                        projectId: ($('#cbbProjects<%=windowId%>').jqxComboBox('getSelectedItem') == null) ? "" : $('#cbbProjects<%=windowId%>').jqxComboBox('getSelectedItem').value,
                        taskId: ($('#cbbTasks<%=windowId%>').jqxComboBox('getSelectedItem') == null) ? "" : $('#cbbTasks<%=windowId%>').jqxComboBox('getSelectedItem').value,
                        customerId: ($('#cbbCustomers<%=windowId%>').jqxComboBox('getSelectedItem') == null) ? "" : $('#cbbCustomers<%=windowId%>').jqxComboBox('getSelectedItem').value,
                        email: $('#txtEmail<%=windowId%>').val(),
                        description: $('#txtDescription<%=windowId%>').val(),
                        statusId: $('#txtStatus<%=windowId%>').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            fnSuccess();
                            if (saveMode == 'close') {
                                if ('<%=ViewData["WindowMode"]%>' == null)
                                    closeWindow('<%=windowId%>');
                                else {
                                    //fnLoadTasks();
                                    history.back();
                                }
                            }
                            else
                                fnResetFormIssue();
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

        function fnResetFormIssue() {
            $('#txtId<%=windowId%>').val('');
            $('#txtName<%=windowId%>').val('');
            $('#slStatuses<%=windowId%>').val('');
            $('#txtSequence<%=windowId%>').val('');
            document.getElementById('cbCommon<%=windowId%>').checked = false;
            document.getElementById('cbFold<%=windowId%>').checked = false;
            $('#txtDescription<%=windowId%>').val('');
        }

        function cbbCustomerChanged(item) {
            if (item != null) {
                $('#txtEmail<%=windowId%>').val(item.EmailAddress);
            } else {
                $('#txtEmail<%=windowId%>').val('');
            }
        }

        function cbbProjectChanged(item) {
            var projectId = '';
            if (item != null) {
                projectId = item.ProjectId;
                fnSetItemForCombobox('cbbCustomers<%=windowId%>', item.CustomerId, item.CustomerName);
            } else {
                cbbCustomerChanged(null);
                $('#cbbCustomers<%=windowId%>').jqxComboBox('clearSelection');
            }
            $('#cbbTasks<%=windowId%>').jqxComboBox({
                source: new $.jqx.dataAdapter({
                    datatype: 'json',
                    datafields: [
                        { name: 'TaskId' },
                        { name: 'TaskName' }
                    ],
                    id: 'TaskId',
                    url: appPath + 'Common/GetTaskForAutocomplete'
                }, {
                    formatData: function (data) {
                        data.Mode = 'select1';
                        data.projectId = projectId;
                        return data;
                    }
                })
            });
        }

        function fnSetProjectItem(value, label) {
            $('#cbbProjects<%=windowId%>').jqxComboBox('clearSelection');
            fnSetItemForCombobox('cbbProjects<%=windowId%>', value, label);
        }
    </script>
    <div>
        <form id="form<%=windowId%>" class="form-to-validate" action="">
            <table class="frmInput">
                <tr>
                    <td>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright">Vấn đề</td>
                                <td>
                                    <input type="hidden" id="txtId<%=windowId%>" value="<%=WSI.Issue.IssueId%>" />
                                    <input type="hidden" id="txtStatus<%=windowId%>" value="<%=WSI.Issue.StatusId%>" />
                                    <input type="text" id="txtName<%=windowId%>" value="<%=WSI.Issue.IssueContent%>" />
                                </td>
                                <td class="lbright">Thuộc giai đoạn</td>
                                <td><%Html.RenderAction("slStages", "UserControl", new { elementId = windowId, current = WSI.Issue.StageId, projectStage = ViewData["ProjectStages"] });%></td>
                            </tr>
                            <tr>
                                <td class="lbright">Dự án</td>
                                <td><%Html.RenderAction("cbbProjects", "UserControl", new { elementId = "cbbProjects" + windowId, windowTitle = "Danh sách dự án", id = ViewData["ProjectId"], label = ViewData["ProjectName"] });%></td>
                                <td class="lbright">Nhiệm vụ</td>
                                <%if (WSI.Task == null) WSI.Task = new NEXMI.Tasks(); %>
                                <td><%Html.RenderAction("cbbTasks", "UserControl", new { elementId = "cbbTasks" + windowId, windowTitle = "Danh sách nhiệm vụ", id = WSI.Task.TaskId, label = WSI.Task.TaskName, mode = "select1" });%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <% if (WSI.Customer == null) WSI.Customer = new NEXMI.Customers(); %>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { elementId = windowId, customerId = WSI.Customer.CustomerId, customerName = WSI.Customer.CompanyNameInVietnamese });%></td>
                                <td class="lbright">Email</td>
                                <td><input type="text" id="txtEmail<%=windowId%>" value="<%=WSI.Issue.Email%>" maxlength="60" /></td>
                            </tr>
                            <tr>
                                <td class="lbright">Giao cho</td>
                                <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers" + windowId, userId = WSI.Issue.UserId });%></td>
                                <td class="lbright">Độ ưu tiên</td>
                                <td><%Html.RenderAction("slParameters", "UserControl", new { elementId = "slPriorities" + windowId, objectName = "TaskPriorities", current = WSI.Issue.Priority });%></td>
                            </tr>
                            <tr>
                                <td class="lbright">Các đánh dấu</td>
                                <td colspan="3"><input type="text" id="txtTags<%=windowId%>" value="<%=WSI.Issue.Tags%>" style="width: 99%;" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="tabs<%=windowId%>"> 
                            <ul>
                                <li style="margin-left: 10px;"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></li>
                                <li>Thông tin thêm</li>
                            </ul>
                            <div style="padding:10px;">
                                <textarea id="txtDescription<%=windowId%>" cols="1" rows="5" style="width: 99%;"><%=WSI.Issue.Description%></textarea>
                            </div>
                            <div style="padding:10px;">
                                
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</div>
<% 
    if (ViewData["WindowMode"] == null)
    {
%>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveOrUpdateIssue('close')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:fnSaveOrUpdateIssue('')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium">Thoát</button>
    </div>
</div>
<% 
    }    
%>
</div>