<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    List<NEXMI.NMCustomersWSI> WSIs = (List<NEXMI.NMCustomersWSI>)ViewData["WSIs"];
    string elementId = ViewData["ElementId"].ToString();
%>
<script type="text/javascript">
    $(function () {
        
    });

    function fnAddUserToSession(id, name) {
        if (document.getElementById(id) == null) {
            $('#div<%=elementId%>').append('<div id="' + id + '" class="tag <%=elementId%>">' + name + '<a href="javascript:fnRemoveUserFromSession(\'' + id + '\');" class="tag-close">x</a></div>');
//            $.post(appPath + 'UserControl/AddUserToSession', { id: id }, function () {
//                $('#div<%=elementId%>').append('<div id="' + id + '" class="tag <%=elementId%>">' + name + '<a href="javascript:fnRemoveUserFromSession(\'' + id + '\');" class="tag-close">x</a></div>');
//            });
        }
    }

    function fnRemoveUserFromSession(id) {
        $('#' + id).remove();
//        $.post(appPath + 'UserControl/RemoveUserFromSession', { id: id }, function () {
//            $('#' + id).remove();
//        });
    }
</script>
<table style="width: 100%;">
    <tr>
        <td valign="top">
            <select multiple="multiple" id="<%=elementId%>" style="height: 150px;">
                <% 
                    foreach (NEXMI.NMCustomersWSI Item in WSIs)
                    {
                %>
                <option onclick="javascript:fnAddUserToSession('<%=Item.Customer.CustomerId%>', '<%=Item.Customer.CompanyNameInVietnamese%>');" ><%=Item.Customer.CompanyNameInVietnamese%></option>
                <%
                    }    
                %>
            </select>
        </td>  
        <td style="width: 90%;" valign="top">
            <div id="div<%=elementId%>">
                <% 
                    if (TempData["CurrentUser"] != null)
                    {
                        List<NEXMI.Customers> Users = (List<NEXMI.Customers>)TempData["CurrentUser"];
                        foreach (NEXMI.Customers Item in Users)
                        {
                %>
                <div id="<%=Item.CustomerId%>" class="tag <%=elementId%>"><%=Item.CompanyNameInVietnamese%><a href="javascript:fnRemoveUserFromSession('<%=Item.CustomerId%>');" class="tag-close">x</a></div>
                <%
                        }
                    }    
                %>
            </div> 
        </td>
    </tr>
</table>
