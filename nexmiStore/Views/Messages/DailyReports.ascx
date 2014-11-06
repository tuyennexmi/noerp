<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/DailyReport.js" type="text/javascript"></script>

<script type="text/javascript">
    $(function () {
        $('#pagination-DailyReports').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadDailyReports(page);
            }
        });
    });

</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right"><input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td>
                <%
                    string functionPayment = NEXMI.NMConstant.Functions.Payments;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionPayment))
                    {
                %>
                        <button onclick="javascript:fnShowDailyReportForm('', '')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                %>
                <button onclick="javascript:fnLoadDailyReports()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="right"><%Html.RenderAction("PaginationJS", "UserControl", new { id = "DailyReports" });%></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <%--<select id="slStatus" onchange="javascript:fnLoadDailyReports()">
                <option value="">Tất cả</option>
                <option value="<%=NEXMI.NMConstant.PaymentStatuses.Draft%>">Mới</option>
                <option value="<%=NEXMI.NMConstant.PaymentStatuses.Approved%>">Đã duyệt</option>
                <option value="<%=NEXMI.NMConstant.PaymentStatuses.Done%>">Đã chi</option>
                <option value="<%=NEXMI.NMConstant.PaymentStatuses.Cancelled%>">Đã hủy</option>
            </select>--%>
        </div>
    </div>
    <div id="dailyreports-container">
        <%Html.RenderAction("DailyReportsUC", "Messages");%>
    </div>
</div>