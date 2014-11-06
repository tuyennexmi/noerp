<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.EmailTemplate;
                %>
                <%--<%
                    //Kiểm tra quyền Insert
                    if (GetPermissions.Get(functionId, "Insert"))
                    {
                %>
                <button onclick="javascript:fnPopupUserDialog('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                    else
                    {
                %>
                <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <%
                    }    
                %>--%>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td></td>
        </tr>
    </table>
</div>
<div class="divContent">
<table style="width: 100%" class="tbTemplate">
    <tr>
        <td>
            <table style="width: 100%" id="tbUsers" class="tbDetails">
                <thead>
                    <tr>
                        <th>#</th>
                        <th><%= NEXMI.NMCommon.GetInterface("NAME", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></th>
                        <th>Ngày gửi</th>
                        <th>Tóm tắc nội dung</th>
                        <th style="width: 50px;"></th>
                    </tr>
                </thead>
                <tbody>
                    <%
                        List<NEXMI.SystemParams> WSIs = (List<NEXMI.SystemParams>)ViewData["WSIs"];
                        if (WSIs != null)
                        {
                            int i = 1;
                            foreach (NEXMI.SystemParams Item in WSIs)
                            {
                    %>
                    <tr>
                        <td><%=i++%></td>
                        <td><%=Item.Name%></td>
                        <td><%=Item.Subject %></td>
                        <td><%=Item.ActionDate.ToString("dd-MM-yyyy")%></td>
                        <%if (Item.Contents.Length > 20)
                          { %>
                            <td><%=Item.Contents.Substring(0, 20)%></td>
                        <%}
                          else
                          {%>
                            <td><%=Item.Contents%></td>
                            <%} %>
                        <td class="actionCols">
                        <%
                                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                {
                        %>
                            <a href='javascript:LoadContent("", "Utilities/SystemParamForm?id=<%=Item.Id%>")'><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
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
