<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Shift.js" type="text/javascript"></script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Shifts;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                    {
                %>
                <button onclick="javascript:fnPopupShiftDialog('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                    else
                    {
                %>
                <button class="button disabled"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <%
                    }    
                %>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
<table style="width: 100%" class="tbTemplate">
    <tr>
        <td>
            <table style="width: 100%" id="tbShifts" class="tbDetails">
                <thead>
                    <tr>
                        <th>#</th>                        
                        <th>Tên ca</th>
                        <th>Bắt đầu</th>
                        <th>Kết thúc</th>
                        <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                <% 
                                                           
                    List<NEXMI.NMShiftsWSI> PCWSIs = (List<NEXMI.NMShiftsWSI>)ViewData["PCWSIs"];
                    int i = 1;
                    if (PCWSIs != null)
                    {
                        foreach (NEXMI.NMShiftsWSI Item in PCWSIs)
                        {                        
                %>
                    <tr>
                        <td><%=i++%></td>                        
                        <td><%=Item.Name%></td>
                        <td><%=Item.Start%></td>
                        <td><%=Item.Finish%></td>
                        <td><%=Item.Description%></td>
                        <td class="actionCols">
                <% 
                            if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Update"))
{    
                %>
                            <a href="javascript:fnPopupShiftDialog('<%=Item.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/24Edit-icon.png" /></a>          
                <% 
}
else
{ 
                %>
                            - <a href="javascript:alert('Bạn không có quyền cập nhật!')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/24Edit-icon.png" /></a>         
                <%    
}
                %>
                <% 
                            if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Delete"))
{
                %>
                            - <a href="javascript:fnDeleteShift('<%=Item.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/24Delete-icon.png" /></a>
                <% 
}
else
{ 
                %>
                            - <a href="javascript:alert('Bạn không có quyền xóa!')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/24Delete-icon.png" /></a>
                <%
}    
                %>
                        </td>
                    </tr>
                <%
}
                    }
                %>
                </tbody>    
            </table>
        </td>
    </tr>
</table>
</div>