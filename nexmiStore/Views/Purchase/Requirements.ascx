<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Requirements.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
//        fnLoadRequirementList();
    });
</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                
            </td>
            <td align="right"><input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Requirement;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                        <button onclick="javascript:fnShowRequirementForm('', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }    
                %>
                <button onclick="javascript:fnLoadRequirementList()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="right"><%Html.RenderAction("PaginationJS", "UserControl", new { id = "Requirement" });%></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
           
        </div>
    </div>
    
    <div class='divContentDetails'>
        <div id="requirements-container">
            <%Html.RenderAction("RequirementList", "Purchase", new { pageNum = "1" });%>
        </div>
    </div>
</div>