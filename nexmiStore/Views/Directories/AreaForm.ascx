<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            $('#formArea<%=ViewData["WindowId"]%>').jqxValidator({
                rules: [
                { input: '#txtAreaId<%=ViewData["WindowId"]%>', message: 'Nhập mã khu vực.', action: 'keyup, blur', rule: 'required' },
                { input: '#txtAreaName<%=ViewData["WindowId"]%>', message: 'Nhập tên khu vực.', action: 'keyup, blur', rule: 'required' }
            ]
            });
            $('#txtAreaId<%=ViewData["WindowId"]%>').focus();
        });
    </script>
    <form id="formArea<%=ViewData["WindowId"]%>" action="">
        <% 
            string parentId = ViewData["ParentId"].ToString();
        %>
        <table style="width: 100%" class="frmInput">
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("AREA_CODE", langId) %></td>
                <td><input type="text" id="txtAreaId<%=ViewData["WindowId"]%>" value="<%=ViewData["Id"]%>" maxlength="20" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("AREA", langId) %></td>
                <td><input type="text" id="txtAreaName<%=ViewData["WindowId"]%>" value="<%=ViewData["Name"]%>" /></td>
            </tr>
            <tr>
                <td class="lbright">Zip Code</td>
                <td><input type="text" id="txtZipCode" value="<%=ViewData["ZipCode"]%>" /></td>
            </tr>
            <tr>
                <td class="lbright">Thuộc khu vực</td>
                <td><%Html.RenderAction("cbbAreas", "UserControl", new { areaId = parentId, areaName = ViewData["ParentName"], elementId = ViewData["WindowId"] });%></td>
            </tr>
        </table>  
    </form>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:fnSaveArea("<%=ViewData["WindowId"]%>", "<%=ViewData["ParentElement"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick='javascript:fnResetArea("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLEAR_ALL", langId) %></button>
        <button onclick='javascript:fnCloseAreaForm("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>