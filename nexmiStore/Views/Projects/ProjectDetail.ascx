<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Projects.js" type="text/javascript"></script>
<% 
    string windowId = ViewData["WindowId"].ToString();
    string id = "", projectName = "", cbTask = "checked", cbIssue = "checked", customerId = "", customerName = "", managedBy = "",
        startDate = DateTime.Today.ToString("yyyy-MM-dd"), endDate = "", parentId = "", parentName = "", status = "";
    //string[] team = { }, stage = { };
    IList<NEXMI.Customers> teams = new List<NEXMI.Customers>();
    IList<NEXMI.Stages> stages = new List<NEXMI.Stages>();
    NEXMI.NMProjectsWSI WSI = (NEXMI.NMProjectsWSI)ViewData["WSI"];
    id = WSI.Project.ProjectId;
    projectName = WSI.Project.ProjectName;
    if (WSI.Project.Task == false)
        cbTask = "";
    if (WSI.Project.Issue == false)
        cbIssue = "";
    if (WSI.Customer != null)
    {
        customerId = WSI.Customer.CustomerId;
        customerName = WSI.Customer.CompanyNameInVietnamese;
    }
    managedBy = WSI.ManagedBy.CompanyNameInVietnamese;
    if (WSI.Project.StartDate != null)
        startDate = WSI.Project.StartDate.ToString("yyyy-MM-dd");
    if (WSI.Project.EndDate != null)
        endDate = WSI.Project.EndDate.ToString("yyyy-MM-dd");
    teams = WSI.Teams;
    stages = WSI.Stages;
    status = WSI.Project.StatusId;
%>
<script type="text/javascript">
    $(function () {
        $('#tabs<%=windowId%>').jqxTabs({ theme: theme, keyboardNavigation: false });
    });
</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
            </td>
            <td align="right">&nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnShowProjectForm('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <button onclick="javascript:fnShowProjectForm('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                    <button onclick="javascript:fnDeleteProject('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("SalesOrderSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
<% 
    if(!string.IsNullOrEmpty(id))
    {
        if (status == NEXMI.NMConstant.ProjectStatuses.InProgress)
        {
%>
            <input type="button" value="Đóng" class="button" onclick="javascript:fnChangeProjectStatus('<%=id%>', '<%=NEXMI.NMConstant.ProjectStatuses.Closed%>')" />
            <input  type="button" value="Dự án treo" class="button" onclick="javascript:fnChangeProjectStatus('<%=id%>', '<%=NEXMI.NMConstant.ProjectStatuses.Pending%>')"  />
            <%--<input type="button" value="Set as Template" class="button"/>--%>
            <input  type="button" value="<%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %>" class="button" onclick="javascript:fnChangeProjectStatus('<%=id%>', '<%=NEXMI.NMConstant.ProjectStatuses.Cancelled%>')" />
<% 
        }
        else if (status == NEXMI.NMConstant.ProjectStatuses.Pending)
        { 
%>
            <input  type="button" value="Mở lại" class="button" onclick="javascript:fnChangeProjectStatus('<%=id%>', '<%=NEXMI.NMConstant.ProjectStatuses.InProgress%>')" />
            <input  type="button" value="Đóng" class="button" onclick="javascript:fnChangeProjectStatus('<%=id%>', '<%=NEXMI.NMConstant.ProjectStatuses.Closed%>')" />
            <input  type="button" value="<%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %>" class="button" onclick="javascript:fnChangeProjectStatus('<%=id%>', '<%=NEXMI.NMConstant.ProjectStatuses.Cancelled%>')" />
<%
        }
        else
        { 
%>
            <input  type="button" value="Mở lại" class="button" onclick="javascript:fnChangeProjectStatus('<%=id%>', '<%=NEXMI.NMConstant.ProjectStatuses.InProgress%>')" />
<%         
        }
    }    
%>
        </div>     
        <div class="divStatusBar">
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Projects", current = status });%>
        </div>
    </div>
    <form id="form<%=windowId%>" action="">
        <table style="width: 100%" class="frmInput">
            <tr>
                <td>
                    <table style="width: 100%" class="frmInput">
                        <tr>
                            <td colspan="2">
                                <h2><%=projectName%></h2>
                                <b><label for="cbTask<%=windowId%>" title="Đánh dấu nếu dự án có quản lí các nhiệm vụ"><input type="checkbox" id="cbTask<%=windowId%>" <%=cbTask%> disabled="disabled" onchange="javascript:fnCheckTask()" /> Nhiệm vụ</label>
                                <label for="cbIssue<%=windowId%>" title="Đánh dấu nếu dự án có quản lí các vấn đề"><input type="checkbox" id="cbIssue<%=windowId%>" <%=cbIssue%> disabled="disabled" /> Vấn đề</label></b>
                            </td>
                            <td colspan="2" valign="top" align="right">
                                <button type="button" class="button" onclick="javascript:LoadContent('', 'Projects/Tasks?projectId=<%=id%>')">Nhiệm vụ</button>
                                <%--<button type="button" class="button">Tài liệu</button>
                                <button type="button" class="button">Chấm công</button>--%>
                                <button type="button" class="button" onclick="javascript:LoadContent('', 'Projects/Issues?projectId=<%=id%>')">Vấn đề</button>
                            </td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                            <td><%=customerName%></td>
                            <td class="lbright">Quản lý dự án</td>
                            <td><%=managedBy%></td>
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
                            <% 
                                foreach (NEXMI.Customers Item in teams)
                                { 
                            %>
                            <div style="padding:6px; width: 120px; height: 120px; position: relative; float: left;">
                                <img class="full" title="<%=Item.CompanyNameInVietnamese%>" src="<%=Url.Content("~")%>Content/Avatars/<%=Item.Avatar%>" />
                                <%--<span> <%=Item.CompanyNameInVietnamese + ", "%></span>--%>
                            </div>
                            <%   
                                }
                            %>
                        </div>
                        <div style="padding:10px;">
                            <table style="width:100%" class="frmInput">
                                <tr>
                                    <td class="lbright">Ngày bắt đầu</td>
                                    <td><%=startDate%></td>
                                </tr>
                                <tr>
                                    <td class="lbright">Ngày kết thúc</td>
                                    <td><%=endDate%></td>
                                </tr>
                                <tr>
                                    <td class="lbright">Doanh số dự kiến</td>
                                    <td>
                                        <%=WSI.Project.SalesForecast.ToString("N3") %>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="padding:10px;">
                            <table style="width: 100%;" class="tbDetails">
                                <tr>
                                    <th>Tên giai đoạn</th>
                                    <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
                                </tr>
                                <% 
                                    foreach (NEXMI.Stages Item in stages)
                                    { 
                                %>
                                <tr>
                                    <td><%=Item.StageName%></td>
                                    <td><%=NEXMI.NMCommon.GetStatusName(Item.RelatedStatus, langId)%></td>
                                </tr>
                                <%
                                    }    
                                %>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </form>
    <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = WSI.Project.CreatedBy });%>
</div>