<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbPayments" class="tbDetails" width="100%">
    <thead>
        <tr>
            <th>#</th>
            <th>Số thẻ</th>
            <th>Tên công việc</th>
            <th>Ngày giao</th>
            <th>Ngày hết hạn</th>
            <th><%= NEXMI.NMCommon.GetInterface("RECIPIENT", langId) %></th>
            <th>Người kiểm tra</th>
            <th><%= NEXMI.NMCommon.GetInterface("ACCOUNT_MANAGER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
            <th></th>
        </tr>
    </thead>
    <tbody>
    <%
        string functionId = NEXMI.NMConstant.Functions.Payments;
        List<NEXMI.NMTasksWSI> WSIs = (List<NEXMI.NMTasksWSI>)ViewData["WSIs"];
        int totalPage = 1;
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            string customerId = "", customerName = "", strStatus = "";
            int i = 1;
            double total = 0;
            foreach (NEXMI.NMTasksWSI Item in WSIs)
            {
                customerId = ""; customerName = "";
    %>
        <tr ondblclick="javascript:fnLoadTaskDetail('<%=Item.Task.TaskId%>')">
            <td style="width: 15px;"><%=i++ %></td>
            <td style="width: 120px;"><div><%=Item.Task.TaskId %></div></td>
            <td style="width: 100px;"><div><%=Item.Task.TaskName %></div></td>
            <td style="width: 120px;"><div><%=Item.Task.StartDate.Value.ToString("dd-MM-yyyy") %></div></td>
            <td><div"><%=(Item.Task.Deadline != null)? Item.Task.Deadline.ToString("dd-MM-yyyy") : ""%></div></td>
            <td style="width: 120px;" align="right"><div><%= Item.AssignedUser.CompanyNameInVietnamese %></div></td>
            <td><div title=""><%=Item.CheckedBy.CompanyNameInVietnamese %></div></td>
            <td><div title=""><%=Item.Manager.CompanyNameInVietnamese %></div></td>
            <td><input type='checkbox' <%=(Item.Task.IsReportTimeRight)? "checked": ""%> /></td>
            <td class="actionCols">
                <a href="javascript:fnLoadTaskDetail('<%=Item.Task.TaskId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
    <% 
        if (Item.Task.StatusId == NEXMI.NMConstant.TaskStatuses.InProgress)
        {
            if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {    
    %>
                <a href="javascript:fnShowTaskForm('<%=Item.Task.TaskId%>', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
    <% 
            }
%>
<% 
            if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {
    %>
                <a href="javascript:fnDeleteTask('<%=Item.Task.TaskId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
    <% 
            }
        } 
    %>
            &nbsp;</td>
        </tr>
    <%
            }
        }
        else
        { 
    %>
        <tr><td colspan="10" align="center"><h3>Không có dữ liệu.</h3></td></tr>
    <%
        }
    %>
    </tbody>    
</table>

<script type="text/javascript">
    $(function () {
        $('#pagination-TaskCard').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>