<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMTasksWSI WSI = (NEXMI.NMTasksWSI)ViewData["WSI"];
%>
<script src="<%=Url.Content("~")%>Scripts/forApp/DailyReport.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
    });

    function fnShowDailyReportForm(Id, mode, taskId) {
        openWindow('Thêm một báo cáo mới', 'Messages/DailyReport', { Id: Id, mode: mode, taskId: taskId }, 800, 460);
    }

    $('.tabs').bind('selected', function (event) {
        var item = event.args.item;
        switch (item) {
            case 3:
                LoadContentDynamic('Reports', 'Messages/DailyReportsUC', { taskId: '<%=WSI.Task.TaskId %>' });
                break;
        }
    });

    function fnPrintTaskCard(id) {
        $.ajaxSetup({ async: false });
        
        $.get(appPath + 'Projects/TaskDetail', { id: id, mode: 'Print', page: '1' }, function (data) {
            $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'ORD', size: 'A4' }, function (data2) {
                window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }

</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnShowTaskForm('', '', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <button onclick="javascript:fnShowTaskForm('<%=WSI.Task.TaskId%>', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                    <button onclick="javascript:fnDeleteTask('<%=WSI.Task.TaskId%>', '');" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                    <button onclick="javascript:fnPrintTaskCard('<%=WSI.Task.TaskId%>');" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %> thẻ</button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("ProductSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons NMButtons">
    <% 
        if (WSI.Stage.RelatedStatus == NEXMI.NMConstant.TaskStatuses.Done || WSI.Stage.RelatedStatus == NEXMI.NMConstant.TaskStatuses.Blocked)
        { 
    %>
            <button onclick="javascript:fnSetStageForTask('<%=WSI.Task.TaskId%>', '', '')" class="button">Mở</button>
    <%
        }
        else
        {
    %>
            <button onclick="javascript:fnShowDailyReportForm('', '', '<%=WSI.Task.TaskId%>')" class="button">Báo cáo</button>
            <%--<button onclick="javascript:fnSetStageForTask('<%=WSI.Task.TaskId%>', '<%=NEXMI.NMConstant.StageStatuses.Done%>', '<%=NEXMI.NMConstant.TaskStatuses.Done%>')" class="button">Xong</button>
            <button onclick="javascript:fnSetStageForTask('<%=WSI.Task.TaskId%>', '<%=NEXMI.NMConstant.StageStatuses.Cancelled%>', '<%=NEXMI.NMConstant.TaskStatuses.Blocked%>')" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %> nhiệm vụ</button>--%>
    <% 
        }
    %>
        </div>     
        <div class="divStatusBar">
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Tasks", current = WSI.Task.StatusId, clickable = "yes", ownerId = WSI.Task.TaskId, tableName = "Tasks" });%>
        </div>
    </div>
    <div style="margin: 10px;">
        <table class="frmInput">
            <tr>
                <td>
                    <label class="lbId"><%=WSI.Task.TaskName%></label>
                    <table style="width: 100%" class="frmInput">
                        <tr>
                            <td class="lbright">Mục đích</td>
                            <td><%=WSI.Task.Purpose%></td>
                            <td class="lbright">Tiêu chí</td>
                            <td><%=WSI.Task.Criteria%></td>
                        </tr>
                        <tr>
                            <td class="lbright">Giao cho</td>
                            <td><%=WSI.AssignedUser.CompanyNameInVietnamese%></td>
                            <td class="lbright">Báo cáo đúng hạn</td>
                            <td><input type='checkbox' <%=(WSI.Task.IsReportTimeRight)? "checked": ""%> /></td>
                        </tr>
                        <tr>
                            <td class="lbright">Người kiểm soát</td>
                            <td><%= (WSI.CheckedBy != null)? WSI.CheckedBy.CompanyNameInVietnamese : ""%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ACCOUNT_MANAGER", langId) %></td>
                            <td><%= (WSI.Manager != null)? WSI.Manager.CompanyNameInVietnamese : ""%></td>
                        </tr>
                        <tr>
                            <td class="lbright">Ngày bắt đầu</td>
                            <td><%=(WSI.Task.StartDate != null) ? WSI.Task.StartDate.Value.ToString("yyyy-MM-dd") : ""%></td>
                            <td class="lbright">Ngày hết hạn</td>
                            <td><%=(WSI.Task.Deadline != null) ? WSI.Task.Deadline.ToString("dd-MM-yyyy") : ""%></td>
                        </tr>
                        <tr>
                            <td class="lbright">Dự án</td>
                            <td><%= (WSI.Project != null)? WSI.Project.ProjectName : ""%></td>
                            <td class="lbright">Hạng mục</td>
                            <td><%=WSI.Task.Category %></td>
                        </tr>
                        <tr>
                            <td class="lbright">Ngày kết thúc</td>
                            <td><%=(WSI.Task.EndDate != null) ? WSI.Task.EndDate.Value.ToString("yyyy-MM-dd") : ""%></td>
                            <td class="lbright">Các đánh dấu</td>
                            <td colspan="3"><%=WSI.Task.Tags.Replace(",", "; ")%></td>
                        </tr>
                    </table>    
                </td>
            </tr>
            <tr>
                <td>
                    <div class="tabs"> 
                        <ul>
                            <li style="margin-left: 10px;">Hướng dẫn</li>
                            <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                            <li>Các công việc</li>
                            <li>Báo cáo</li>
                        </ul>
                        <div style="padding:10px;">
                            <%=WSI.Task.Description.Replace("\n", "<br />")%>
                        </div>
                        <div>
                            <table style="width: 100%" class="frmInput">
                                <tr>
                                    <td class="lbright">Chu kỳ báo cáo</td>
                                    <td><%=WSI.ReportPeriod.Name%></td>
                                    <td class="lbright">Thuộc giai đoạn</td>
                                    <td><%=WSI.Stage.StageName%></td>
                                </tr>
                                <tr>
                                    <td class="lbright">Độ ưu tiên</td>
                                    <td><%=WSI.Priority.Name%></td>
                                    <td class="lbright">Thứ tự</td>
                                    <td><%=WSI.Task.Sequence%></td>
                                </tr>
                            </table>
                        </div>
                        <div style="padding:10px;">
                            <table class="tbDetails">
                                <tr>
                                    <th>#</th>
                                    <th>Công việc</th>
                                    <th>Thời gian</th>
                                    <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
                                    <th>Người thực hiện</th>
                                </tr>
                                <tbody>
                    <% 
                        double totalTimeSpent = 0;
                        if (WSI.Details != null)
                        {   
                            foreach (NEXMI.TaskDetails Item in WSI.Details)
                            {
                                totalTimeSpent += Item.TimeSpent;
                    %>
                                <tr>
                                    <td><%=Item.No%></td>
                                    <td><%=Item.Name%></td>
                                    <td><%=Item.TimeSpent.ToString("N3")%></td>
                                    <td><%=Item.StartDate.ToString("dd/MM/yyyy HH:mm")%></td>
                                    <td><%=NEXMI.NMCommon.GetCustomerName(Item.UserId)%></td>
                                </tr>
                    <%
                            }
                        }
                    %>
                                </tbody>
                                <tr>
                                    <td>Tổng thời gian</td><td></td>
                                    <td><b><%=totalTimeSpent.ToString("N3")%></b></td>
                                    <td></td><td></td>
                                </tr>
                            </table>
                        </div>
                        <div id='Reports'>
                            
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>