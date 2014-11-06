<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#txtName").focus();
        validateForm("formShift");
    });
</script>
<form id="formShift" action="">
<table style="width: 100%" class="frmInput">
    <tr><td colspan="2">&nbsp;<label class="summaryError validation-summary-errors"></label></td></tr>
    <tr>
        <td class="lbright">Mã</td>
        <td><input type="text" id="txtId" value="<%=ViewData["id"]%>" readonly="readonly" />&nbsp;</td>
    </tr>
    <tr>
        <td class="lbright">Tên ca</td>
        <td><input type="text" id="txtName" value="<%=ViewData["name"]%>" class="required" />&nbsp;</td>
    </tr>
    <tr>
        <td class="lbright">Bắt đầu</td>
        <td><input type="text" id="txtstart" value="<%=ViewData["start"]%>" class="required" />&nbsp;</td>
    </tr>
    <tr>
        <td class="lbright">Kết thúc</td>
        <td><input type="text" id="txtfinish" value="<%=ViewData["finish"]%>" class="required" /></td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
        <td><input type="text" id="txtdes" value="<%=ViewData["des"]%>" /></td>
    </tr>    
</table>
</form>