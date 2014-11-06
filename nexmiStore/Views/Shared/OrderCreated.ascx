<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<tr>
    <td class="lbright">Ngày sửa</td>
    <td><%=ViewData["ModifiedDate"]%></td>
    <td class="lbright">Người sửa</td>
    <td><%=ViewData["ModifiedBy"]%></td>
</tr>