<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>


<script type="text/javascript">
    $(function () {
        $('#pagination-TaskCard').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadTaskCards(page);
            }
        });
    });

    function fnLoadTaskCards(page) {
        LoadContentDynamic('TaskCardList', 'Projects/TaskCardList', {
            pageNum: page,
            status: $('#slStatus').val(),
            keyword: $('#txtKeywordImport').val(),
            from: $('#dtFrom').val(),
            to: $('#dtTo').val(),
            user: document.getElementsByName('cbbCustomers')[0].value
        });
    }

    function fnChanged() {
        
    }

</script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
            </td>
            <td align="right">
                <input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
            </td>
        </tr>
        <tr>
            <td>
        <%
            string functionId = NEXMI.NMConstant.Functions.Task;
            //Kiểm tra quyền Insert
            if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {
        %>
                <button onclick="javascript:fnShowTaskForm('', '', '')" class="color red button">Thêm nhiệm vụ</button>
        <%  
            }
        %>
                <button onclick="javascript:fnLoadTaskCards('')" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="right" style="float: right;">
                <%Html.RenderAction("PaginationJS", "UserControl", new { id = "TaskCard" });%>
                <%//Html.RenderPartial("ProjectSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "Tasks" }); %>
        </div>
    </div>
    
    <div class='divContentDetails'>
        <div id="TaskCardList">
            <%Html.RenderAction("TaskCardList", "Projects");%>
        </div>
    </div>
</div>