<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('img').error(function () {
            $(this).attr('src', '<%=Url.Content("~")%>Content/UI_Images/noimage.png');
        });
    });
</script>
<% 
    List<NEXMI.NMTasksWSI> WSIs = (List<NEXMI.NMTasksWSI>)ViewData["WSIs"];
    string[] arrTags;
    foreach (NEXMI.NMTasksWSI Item in WSIs)
    {
%>
<div class="task-item" id="<%=Item.Task.TaskId%>">
    <table style="width: 100%;" cellpadding="5">
        <tr>
            <td><b><%=Item.Task.TaskName%></b><br /><%=(Item.Project != null)? Item.Project.ProjectName : ""%></td>
            <td valign="top">
                <dl class="dropdown" style="display: none;">
                    <dt><a class="task-child" href="javascript:showMenu('menuTask<%=Item.Task.TaskId%>')"><img alt="" class="img-12px task-child" src="<%=Url.Content("~")%>Content/UI_Images/32Arrows-Down-circular-icon.png" /></a></dt>
                    <dd>
                        <ul id="menuTask<%=Item.Task.TaskId%>">
                            <li><a class="task-child" href="javascript:fnShowTaskForm('<%=Item.Task.TaskId%>', '', '')"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>...</a></li>
                            <li><a class="task-child" href="javascript:fnDeleteTask('<%=Item.Task.TaskId%>', '1')"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></li>
                        </ul>
                    </dd>
                </dl>
            </td>
        </tr>
        <tr>
            <td valign="top">
<% 
        if (!string.IsNullOrEmpty(Item.Task.Tags))
        {
            arrTags = Item.Task.Tags.Split(',');
            for (int i = 0; i < arrTags.Length; i++)
            { 
%>
            <label class="tag-item"><%=arrTags[i]%></label>
<%
            }
        }
%>
            </td>
            <td align="right">
                <a class="task-child" href="javascript:()"><img class="task-child" style="width: 24px; height: 24px; border: solid 1px #ccc; border-radius: 3px;" alt="" title="<%=Item.AssignedUser.CompanyNameInVietnamese%>" src="<%=Url.Content("~")%>uploads/_thumbs/<%=Item.AssignedUser.Avatar%>" /></a>
            </td>
        </tr>
    </table>
</div>
<% 
    }
%>