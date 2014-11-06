<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<ul class="button-group" id="pagination-<%=ViewData["Id"]%>">
	<li title="Đầu"><a class="button first" data-action="first">&laquo;</a></li>
    <li title="Trước"><a class="button previous" data-action="previous">&lsaquo;</a></li>
    <li><input readonly="readonly" style="width: 90px; height: 21px; text-align: center; font-weight: bold; border: 1px solid #ccc;" /></li>
    <li title="Sau"><a class="button next" data-action="next">&rsaquo;</a></li>
    <li title="Cuối"><a class="button last" data-action="last">&raquo;</a></li>
</ul>
