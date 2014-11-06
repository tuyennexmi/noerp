<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>


<%
    List<NEXMI.NMJobsWSI> WSIs = (List<NEXMI.NMJobsWSI>)ViewData["WSIs"];
    int totalPage = 1;
    try
    {
        int totalRows = WSIs[0].Filter.TotalRows;
        if (totalRows % NEXMI.NMCommon.PageSize() == 0)
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize();
        }
        else
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
        }
    }
    catch { }
%>

<table id="tbReceipts" class="tbDetails">
    <thead>
        <tr>
            <th>#</th>
            <th width="9%">ID</th>
            <th>Tên công việc</th>
            <th width="11%">Mục đích</th>
            <th>Tiêu chí</th>
            <th>Tóm tắc</th>
            <th>Thời gian thực hiện</th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
            <th width="6%"></th>
        </tr>
    </thead>
    <tbody>
<%
    string func = NEXMI.NMConstant.Functions.Project;        
    if (WSIs.Count > 0)
    {   
        int count = 1;
        foreach (NEXMI.NMJobsWSI Item in WSIs)
        {   
%>
        <tr ondblclick="javascript:fnLoadJobDetail('<%=Item.Job.Id%>')">
            <td ><%=count++%></td>
            <td ><%=Item.Job.Id %></td>
            <td ><%=Item.Job.Name %></td>
            <td ><%=Item.Job.Purpose %></td>
            <td ><%=Item.Job.Criteria %></td>
            <td ><%=Item.Job.WorkSummary %></td>
            <td ><%=Item.Job.TimeSpent %></td>
            <td><%= NEXMI.NMCommon.GetCustomerName(Item.Job.CreatedBy) %></td>
            <td ><%=NEXMI.NMCommon.GetStatusName(Item.Job.Status, langId)%></td>
            <td >
                <a href="javascript:fnLoadJobDetail('<%=Item.Job.Id%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
        <%
        if (Item.Job.Status == NEXMI.NMConstant.JobsStatus.Draft)
        {
            if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
            {                        
        %>
                <a href="javascript:fnCreateJob('<%=Item.Job.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
        <%  }
            if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
            {
        %>
                <a href="javascript:fnDeleteJob('<%=Item.Job.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
    </tbody>
</table>