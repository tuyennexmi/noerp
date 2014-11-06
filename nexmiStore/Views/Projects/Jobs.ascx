<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>



<script type="text/javascript">
    $(function () {
        $('#pagination-Jobs').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadJobs(page);
            }
        });
    });

    function fnCreateJob(id) {
        openWindow('Định nghĩa Công việc', 'Projects/JobForm', { id: id }, 680, 450);
    }

    function fnLoadJobs(page) {
        LoadContentDynamic('JobList', 'Projects/JobList', { pageNum: page });
    }

    function fnLoadJobDetail(id) {
        openWindow('Công việc', 'Projects/JobForm', { id: id, mode: 'Detail' }, 680, 450);
    }

    function fnDeleteJob(id) {
        if (confirm("Bạn muốn xóa công việc này?")) {
            $.ajax({
                type: 'POST',
                url: appPath + 'Projects/DeleteJob',
                data: { id: id },
                beforeSend: function () {
                    OpenProcessing();
                },
                success: function (data) {
                    if (data == "") {
                        if (mode == '1')
                            $('#' + id).remove();
                        else {
                            history.back();
                        }
                    }
                    else {
                        alert(data);
                    }
                },
                error: function () {
                    fnError();
                },
                complete: function () {
                    CloseProcessing();
                }
            });
        }
    }

</script>

<div class="divActions">
    <table>
        <tr>
            <td></td>
            <td align="right"><input id="txtKeyword" name="txtKeyword" value="<%=ViewData["Keyword"]%>" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td>
        <%
            string functionId = NEXMI.NMConstant.Functions.Jobs;
            //Kiểm tra quyền Insert
            if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {
        %>
                <button onclick="javascript:fnCreateJob('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
        <% 
            }
        %>
                <button class="button" onclick="javascript:$(window).hashchange();"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td style="float: right;">
                <%Html.RenderAction("PaginationJS", "UserControl", new { id = "Jobs" });%>
                &nbsp;
                <%--<%Html.RenderPartial("ProjectSV"); %>--%>
            </td>
        </tr>
    </table>
</div>

<div class="divContent">
    <div class="divStatus">
    </div>
    <div class='divContentDetails'>
        <div id='JobList'>
            <%Html.RenderAction("JobList"); %>
        </div>
    </div>
</div>