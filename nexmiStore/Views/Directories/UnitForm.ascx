<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            $("#txtName").focus();
            validateForm("formUnit");
        });

        function fnCloseUnitForm() {
            closeWindow('<%=ViewData["WindowId"]%>');
            fnLoadContent();
        }
    </script>
    <form id="formUnit" action="">
        <table style="width: 100%" class="frmInput">
            <tr><td colspan="2">&nbsp;<label class="summaryError validation-summary-errors"></label></td></tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("UNIT_ID", langId) %></td>
                <td><input type="text" id="txtId" value="<%=ViewData["Id"]%>" readonly="readonly" />&nbsp;</td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("UNIT_NAME", langId) %></td>
                <td><input type="text" id="txtName" value="<%=ViewData["Name"]%>" class="required" />&nbsp;</td>
            </tr>
        </table>
    </form>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:fnSaveOrUpdateUnit()' class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick='javascript:fnCloseUnitForm()' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>