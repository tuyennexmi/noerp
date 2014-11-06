<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Users.js" type="text/javascript"></script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <%
                    string functionUser = NEXMI.NMConstant.Functions.Users;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionUser))
                    {
                %>
                <button onclick="javascript:fnPopupUserDialog('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                %>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td></td>
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
            <table style="width: 100%" id="tbUsers" class="tbDetails">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th><%= NEXMI.NMCommon.GetInterface("NAME", langId) %></th>
                        <th>Tên đăng nhập</th>
                        <th><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></th>
                        <th>Email</th>
                        <th>Kho mặc định</th>
                        <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <%
                        List<NEXMI.NMCustomersWSI> WSIs = (List<NEXMI.NMCustomersWSI>)ViewData["WSIs"];
                        if (WSIs != null)
                        {
                            string strStatus = "Đã khóa", stockName = "";
                            foreach (NEXMI.NMCustomersWSI Item in WSIs)
                            {
                                strStatus = "Đã khóa"; stockName = "";
                                if (Item.Customer.Discontinued == false)
                                {
                                    strStatus = "Không khóa";
                                }
                                stockName = (Item.DefaultStock != null && Item.DefaultStock.Translation != null) ? Item.DefaultStock.Translation.Name : "";
                    %>
                    <tr>
                        <td><%=Item.Customer.CustomerId %></td>
                        <td><%=Item.Customer.CompanyNameInVietnamese%></td>
                        <td><%=Item.Customer.Code%></td>
                        <td><%=Item.Customer.Cellphone%></td>
                        <td><%=Item.Customer.EmailAddress %></td>
                        <td><%=stockName%></td>
                        <td><%=strStatus%></td>
                        <td class="actionCols">
                        <%
                                if (GetPermissions.GetSelect((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionUser))
                                {
                        %>
                            <a href="javascript:fnLoadCustomerDetail('<%=Item.Customer.CustomerId%>', '<%=Item.Customer.GroupId %>')"><img alt="Xem" title="Xem" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                        <% 
                                }
                                if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionUser, "Update"))
                                {
                        %>
                            <a href="javascript:fnPopupUserDialog('<%=Item.Customer.CustomerId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                            <%--<a href="javascript:fnShowCustomerForm('<%=Item.Customer.CustomerId%>', '<%=Item.Customer.GroupId %>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>--%>
                        <% 
                                }                                
                                if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionUser, "Delete"))
                                {
                        %>
                             <a href="javascript:fnDeleteUser('<%=Item.Customer.CustomerId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                        <% 
                                }
                        %>
                             <a href="javascript:LoadContent('', 'Managements/Permissions?userId=<%=Item.Customer.CustomerId%>')"><img alt="Phân quyền" title="Phân quyền" src="<%=Url.Content("~")%>Content/UI_Images/16Permission-icon.png" /></a>
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
