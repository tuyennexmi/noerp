<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% 
    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
    
%>

<table style="width:100%">
    <tr>
        <td width="40%">
            <img height="60px" alt="" src="<%=localpath%>" />
        </td>
        <td width="60%">
            <div style="text-align: right;">
                <h1><%=ViewData["Title"]%></h1>
            </div>
        </td>
    </tr>
</table>