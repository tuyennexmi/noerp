<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            $("#txtCPName").focus();
            $("#formContactPerson").jqxValidator({
                rules: [
                    { input: '#txtCPName', message: 'Nhập tên người đại diện.', action: 'keyup, blur', rule: 'required' },
                    { input: '#txtCPEmail', message: 'Email không đúng định dạng.', action: 'keyup, blur', rule: 'email' }
                ]
            });
        });
    </script>
    <form id="formContactPerson" action="">
        <table style="width: 100%" class="frmInput">
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NAME", langId) %></td>
                <td>
                    <input type="hidden" id="txtCPId" value="<%=ViewData["id"]%>" />
                    <input type="text" id="txtCPName" value="<%=ViewData["name"]%>" />
                </td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("POSITION", langId) %></td>
                <td><input type="text" id="txtCPPosition" value="<%=ViewData["position"]%>" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></td>
                <td><input type="text" id="txtCPPhoneNumber" value="<%=ViewData["phoneNumber"]%>" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MOBILEPHONE", langId) %></td>
                <td><input type="text" id="txtCPCellphone" value="<%=ViewData["cellphone"]%>" /></td>
            </tr>
            <tr>
                <td class="lbright">Email</td>
                <td><input type="text" id="txtCPEmail" value="<%=ViewData["email"]%>" class="email" /></td>
            </tr>
        </table>
    </form>    
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveOrUpdateContactPerson()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:fnResetFormContactPerson()" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLEAR_ALL", langId) %></button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>