<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~") %>Scripts/forApp/Planning.js" type="text/javascript"></script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Plans;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                        <button onclick="javascript:fnPlanForm('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }                    
                %>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
    </div>
    <div class='divContentDetails'>
        <table style="width: 100%" class="tbTemplate">
            <tr>
                <td>
                    <table style="width: 100%" id="tbUnits" class="tbDetails">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>ID</th>
                                <th>Ngày bắt đầu</th>
                                <th>Ngày kết thúc</th>
                                <th><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
                                <%--<th>Doanh số</th>--%>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        <% 
                                      
                            List<NEXMI.NMMasterPlanningsWSI> WSIs = (List<NEXMI.NMMasterPlanningsWSI>)ViewData["Plans"];
                            int i = 1;
                            if (WSIs != null)
                            {
                                foreach (NEXMI.NMMasterPlanningsWSI Item in WSIs)
                                {                        
                        %>
                            <tr ondblclick="javascript:fnPlanDetail('<%=Item.Planning.Id%>')">
                                <td><%=i++%></td>
                                <td><%=Item.Planning.Id%></td>
                                <td><%=Item.Planning.BeginDate.ToString("dd-MM-yyyy") %></td>
                                <td><%=Item.Planning.EndDate.ToString("dd-MM-yyyy")%></td>
                                <td><%=Item.Planning.Title%></td>
                                <td><%=Item.Planning.Descriptions%></td>
                                <%--<td><%=Item.Planning.SalesAmount%></td>--%>
                                <td >
                        <% 
                                    if (GetPermissions.GetSelect((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                {    
                        %>
                                    <a href="javascript:fnPlanDetail('<%=Item.Planning.Id%>')"><img alt="Xem" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                        <%      }
                                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                {    
                        %>
                                    <a href="javascript:fnPlanForm('<%=Item.Planning.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
                        <% 
                                }
                                if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Delete"))
                                {
                        %>
                                    <a href="javascript:fnDeletePlan('<%=Item.Planning.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
</div>