<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbCloseMonths" class="tbDetails" width="100%">
    <thead>
        <tr>
            <th>#</th>
            <th>Kỳ kế toán</th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <th></th>
        </tr>
    </thead>
    <tbody>
<%
    string functionId = NEXMI.NMConstant.Functions.Payments;
    List<NEXMI.NMCloseMonthsWSI> WSIs = (List<NEXMI.NMCloseMonthsWSI>)ViewData["WSIs"];
    int totalPage = 1;
    if (WSIs.Count > 0)
    {
        int totalRows = WSIs[0].Filter.TotalRows;
        if (totalRows % NEXMI.NMCommon.PageSize() == 0)
            totalPage = totalRows / NEXMI.NMCommon.PageSize();
        else
            totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
        int i = 1;
        foreach (NEXMI.NMCloseMonthsWSI Item in WSIs)
        {
    %>
        <tr ondblclick="javascript:fnShowCloseMonthDetail('<%=Item.CloseMonth.CloseMonth%>')">
            <td style="width: 15px;"><%=i++%></td>
            <td style="width: 120px;"><div><%=Item.CloseMonth.CloseMonth%></div></td>
            <td style="width: 100px;"><div><%=Item.CloseMonth.CreatedDate.ToString("dd-MM-yyyy")%></div></td>
            <td style="width: 120px;"><div><%=NEXMI.NMCommon.GetCustomerName(Item.CloseMonth.CreatedBy) %></div></td>
            <td><div title="<%=Item.CloseMonth.Descriptions%>"><%=Item.CloseMonth.Descriptions%></div></td>
            
            <td class="actionCols">
                <a href="javascript:fnShowCloseMonthDetail('<%=Item.CloseMonth.CloseMonth%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
    <%--<%          
            if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {    
    %>
                <a href="javascript:fnShowCloseMonthForm('<%=Item.CloseMonth.CloseMonth%>', '', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
    <% 
            }
            if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {
    %>
                <a href="javascript:fnDeleteCloseMonth('<%=Item.CloseMonth.CloseMonth%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
    <% 
            }
    %>--%>
            &nbsp;</td>
        </tr>
    <%
        }
    %>
        <%--<tr>
            <td colspan='5'><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</td>
            <td align="right"><%=total.ToString("N3") %></td>
        </tr>--%>
<% 
    }
    else
    { 
%>
        <tr><td colspan="9" align="center"><h3>Không có dữ liệu.</h3></td></tr>
<%
    }
%>
    </tbody>    
</table>
<script type="text/javascript">
    $(function () {
        $('#pagination-CloseMonth').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>