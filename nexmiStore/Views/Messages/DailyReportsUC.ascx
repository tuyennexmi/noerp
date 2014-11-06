<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbPayments" class="tbDetails" width="100%">
    <thead>
        <tr>
            <th>#</th>
            <th>Số BC</th>            
            <th><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></th>
            <th>Tóm tắc</th>
            <th>Thuận lợi</th>
            <th>Khó khăn</th>
            <th>Đề xuất</th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_DATE", langId) %></th>            
            <th></th>
        </tr>
    </thead>
    <tbody>
    <%
        string functionId = NEXMI.NMConstant.Functions.Payments;
        List<NEXMI.NMDailyReportsWSI> WSIs = (List<NEXMI.NMDailyReportsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].Filter.TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            string customerId = "", customerName = "", strStatus = "";
            int i = 1;
            foreach (NEXMI.NMDailyReportsWSI Item in WSIs)
            {                
    %>
        <tr ondblclick="javascript:fnLoadDailyReportDetail('<%=Item.DailyReport.Id%>')">
            <td style="width: 15px;"><%=i++%></td>
            <td style="width: 120px;"><div><%=Item.DailyReport.Id%></div></td>
            <td style="width: 120px;"><div><%=Item.DailyReport.Title%></div></td>
            <td style="width: 120px;"><div><%=Item.DailyReport.Contents%></div></td>
            <td style="width: 120px;"><div><%=Item.DailyReport.Advantages%></div></td>
            <td style="width: 120px;"><div><%=Item.DailyReport.Hards%></div></td>
            <td style="width: 120px;"><div><%=Item.DailyReport.Promotes%></div></td>
            <td style="width: 120px;"><div><%=Item.CreatedBy.CompanyNameInVietnamese%></div></td>
            <td style="width: 100px;"><div><%=Item.DailyReport.CreatedDate.ToString("dd-MM-yyyy")%></div></td>
            <td class="actionCols">
                <a href="javascript:fnLoadDailyReportDetail('<%=Item.DailyReport.Id%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                <a href="javascript:fnEditDailyReportDetail('<%=Item.DailyReport.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> nội dung này" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
            </td>
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
        $('#pagination-Payment').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>