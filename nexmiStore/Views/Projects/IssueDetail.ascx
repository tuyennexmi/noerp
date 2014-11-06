<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMIssuesWSI WSI = (NEXMI.NMIssuesWSI)ViewData["WSI"];
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
                    <button onclick="javascript:fnShowIssueForm('', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <button onclick="javascript:fnShowIssueForm('<%=WSI.Issue.IssueId%>', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                    <button onclick="javascript:fnDeleteIssue('<%=WSI.Issue.IssueId%>', '');" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
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
        <div class="divButtons">
            <% 
                if (WSI.Stage.RelatedStatus == NEXMI.NMConstant.StageStatuses.Done || WSI.Stage.RelatedStatus == NEXMI.NMConstant.StageStatuses.Cancelled)
                { 
            %>
            <button onclick="javascript:fnSetStageForIssue('<%=WSI.Issue.IssueId%>', '', '')" class="button">Mở</button>
            <%
                }
                else
                {
            %>
            <button onclick="javascript:fnSetStageForIssue('<%=WSI.Issue.IssueId%>', '<%=NEXMI.NMConstant.StageStatuses.Done%>', '<%=NEXMI.NMConstant.IssueStatuses.Done%>')" class="button">Xong</button>
            <button onclick="javascript:fnSetStageForIssue('<%=WSI.Issue.IssueId%>', '<%=NEXMI.NMConstant.StageStatuses.Cancelled%>', '<%=NEXMI.NMConstant.IssueStatuses.Blocked%>')" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %> vấn đề</button>
            <% 
                }    
            %>
        </div>     
        <div class="divStatusBar">
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Issues", current = WSI.Issue.StatusId });%>
        </div>
    </div>
    <div class="windowContent">
        <script type="text/javascript">
            $(function () {
                $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
            });
        </script>
        <div style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%=WSI.Issue.IssueContent%></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright">Thuộc giai đoạn</td>
                                <td style="min-width: 250px;"><%=WSI.Stage.StageName%></td>
                                <td></td>
                                <td style="min-width: 250px;"></td>
                            </tr>
                            <tr>
                                <td class="lbright">Các đánh dấu</td>
                                <td colspan="3"><%=WSI.Issue.Tags%></td>
                            </tr>
                            <tr>
                                <td class="lbright">Dự án</td>
                                <td><%=(WSI.Project != null) ? WSI.Project.ProjectName : ""%></td>
                                <td class="lbright">Nhiệm vụ</td>
                                <td><%=(WSI.Task != null) ? WSI.Task.TaskName : ""%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <td><%=(WSI.Customer != null) ? WSI.Customer.CompanyNameInVietnamese : ""%></td>
                                <td class="lbright">Email</td>
                                <td><%=WSI.Issue.Email%></td>
                            </tr>
                            <tr>
                                <td class="lbright">Giao cho</td>
                                <td><%=WSI.AssignedUser.CompanyNameInVietnamese%></td>
                                <td class="lbright">Độ ưu tiên</td>
                                <td><%=WSI.Priority.Name%></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div class="tabs"> 
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></li>
                    <li>Thông tin thêm</li>
                </ul>
                <div style="padding:10px;">
                    <%=WSI.Issue.Description%>
                </div>
                <div style="padding:10px;">
                                
                </div>
            </div>
        </div>
    </div>
</div>