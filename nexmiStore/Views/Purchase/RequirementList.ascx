<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails">
    <thead>
        <tr>
            <th>#</th>
            <th>Mã đề xuất</th>
            <th>Ngày đề xuất</th>
            <th>Lý do</th>
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <th>TG đáp ứng</th>
            <th>Tổng giá trị</th>            
            <th>Người đề xuất</th>
            <th></th>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMRequirementsWSI> WSIs = (List<NEXMI.NMRequirementsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].Filter.TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            string functionId = NEXMI.NMConstant.Functions.Requirement;
            int count=0;
            foreach (NEXMI.NMRequirementsWSI Item in WSIs)
            {
    %>
        <tr ondblclick="javascript:fnLoadRequirementDetail('<%=Item.Requirement.Id%>')">
            <td><%= ++count %></td>
            <td ><%=Item.Requirement.Id%></td>
            <td><%=Item.Requirement.RequireDate.ToString("dd-MM-yyyy")%></td>
            <td ><%=GlobalValues.GetType( Item.Requirement.RequirementTypeId)%></td>
            <td ><%=(Item.Customer == null) ? "" : Item.Customer.CompanyNameInVietnamese %></td>
            <td ><%=Item.Requirement.Description%></td>            
            <td align="right"><%=Item.Requirement.ResponseDays%></td>
            <td align="right"><%=Item.Requirement.Amount.ToString("N3")%></td>
            <td ><%=Item.RequiredBy.CompanyNameInVietnamese%></td>
            <td class="actionCols">
    <% 
                if (GetPermissions.GetSelect((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                {    
    %>
                <a href="javascript:fnLoadRequirementDetail('<%=Item.Requirement.Id%>')"><img alt="Xem" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
    <% 
                }
                if (Item.Requirement.Status  != NEXMI.NMConstant.RequirementStatuses.Approved)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {    
            %>
                        <a href="javascript:fnShowRequirementForm('<%=Item.Requirement.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
            <% 
                    }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                            <a href="javascript:fnDeleteRequirement('<%=Item.Requirement.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                <% 
                    }
                }
    %>
            </td>
        </tr>
    <%
            }
        }
        else
        { 
    %>
        <tr>
            <td colspan="10" align="center">
                <h3>Không có dữ liệu.</h3>
            </td>
        </tr>
    <%
        }
    %>
    </tbody>    
</table>
<script type="text/javascript">
    $(function () {
        $('#pagination-Requirement').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadReceipt();
            },
            max_page: '<%=totalPage%>'
        });
    });
</script>