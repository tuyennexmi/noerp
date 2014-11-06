<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails">
    <thead>
        <tr>
            <th>#</th>
            <th width="9%">ID</th>
            <th>Tên dự án</th>
            <th width="11%"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            <th>Thời gian bắt đầu</th>
            <th>Thời gian kết thúc</th>
            <th>Tổng thời gian (ngày)</th>
            <th>Thời gian thực hiện (ngày)</th>
            <th>Dự báo Doanh số</th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
            <th width="6%"></th>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMProjectsWSI> WSIs = (List<NEXMI.NMProjectsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        double total = 0, paid = 0, remain = 0;
        string func = NEXMI.NMConstant.Functions.Project;        
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;

            int count = 1;
            foreach (NEXMI.NMProjectsWSI Item in WSIs)
            {
                total += Item.Project.SalesForecast;
    %>
            <tr ondblclick="javascript:fnLoadProjectDetail('<%=Item.Project.ProjectId%>')">
                <td ><%=count++%></td>
                <td ><%=Item.Project.ProjectId%></td>
                <td ><%=Item.Project.ProjectName %></td>
                <td ><%=Item.Customer.CompanyNameInVietnamese%></td>
                <td ><%=Item.Project.StartDate.ToString("dd-MM-yyyy")%></td>
                <td ><%=Item.Project.EndDate.ToString("dd-MM-yyyy")%></td>
                <td><%=Item.Project.EndDate.Subtract(Item.Project.StartDate).TotalDays %></td>
                <td><%=DateTime.Today.Subtract(Item.Project.StartDate).TotalDays %></td>
                <td ><%=Item.Project.SalesForecast.ToString("N3") %></td>
                <td ><%=Item.ManagedBy.CompanyNameInVietnamese%></td>
                <td ><%=GlobalValues.GetStatus(Item.Project.StatusId).Status.Name%></td>
                <td >
                    <a href="javascript:fnLoadProjectDetail('<%=Item.Project.ProjectId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
            <%
                if (Item.Project.StatusId == NEXMI.NMConstant.ProjectStatuses.Pending)
            {
                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                {                        
            %>
                    <a href="javascript:fnShowProjectForm('<%=Item.Project.ProjectId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> " src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
            <%  }
                if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                {
            %>
                    <a href="javascript:fnDeleteProject('<%=Item.Project.ProjectId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
            <% 
                }
            }
            %>
                </td>
            </tr>
    <%
            }
        }
    %>
            <tr>
                <td colspan='8'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                <td align="right"><b><%=total.ToString("N3") %></b></td>
            </tr>
    </tbody>
    <tfoot>
    
    </tfoot>
</table>

<script type="text/javascript">
    $(function () {
        $('#pagination-Project').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>