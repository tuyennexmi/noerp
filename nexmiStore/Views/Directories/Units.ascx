<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Units.js" type="text/javascript"></script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Units;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                    {
                %>
                <button onclick="javascript:fnPopupUnitDialog('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
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
                                <th><%= NEXMI.NMCommon.GetInterface("UNIT_ID", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("UNIT_NAME", langId) %></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        <% 
                                      
                            List<NEXMI.NMProductUnitsWSI> WSIs = (List<NEXMI.NMProductUnitsWSI>)ViewData["WSIs"];
                            int i = 1;
                            if (WSIs != null)
                            {
                                foreach (NEXMI.NMProductUnitsWSI Item in WSIs)
                                {                        
                        %>
                            <tr>
                                <td><%=i++%></td>                        
                                <td><%=Item.Id%></td>
                                <td><%=Item.Name%></td>
                                <td >
                        <% 
                                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                    {    
                        %>
                                        <a href="javascript:fnPopupUnitDialog('<%=Item.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
                        <% 
                                    }
                        %>
                        <% 
                                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                    {
                        %>
                                        <a href="javascript:fnDeleteUnit('<%=Item.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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