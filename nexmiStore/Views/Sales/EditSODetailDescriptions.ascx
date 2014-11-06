<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#txtContent").jqte();
    });

    function fnOK() {
        $('#txtDescriptionDetail').val($('#txtContent').val());
        fnSaveSalesOrderDetail();
        $("input[name$='cbbProducts']").val("");
        closeWindow('<%=ViewData["WindowId"]%>');
    }
</script>

<%
    //NEXMI.NMProductsWSI WSI = (NEXMI.NMProductsWSI)ViewData["Product"];    
 %>
<div>
    <textarea id="txtContent" name="txtContent" rows="1" cols="1" style="width: 100%; height: 100%;"><%=ViewData["Description"]%></textarea>
</div>

<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:fnOK()' class="button medium">OK</button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>