<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">

    function fnPopupBankDialog(id) {
        openWindow('Thêm/<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> thông tin ngân hàng', 'Directories/BankForm', { id: id }, 500, 400);
    }

    function fnDeleteBank(id) {
        var answer = confirm("Bạn muốn xóa đơn vị này?");
        if (answer) {
            OpenProcessing();
            $.post(appPath + "Directories/DeleteBank", { id: id },
        function (data) {
            CloseProcessing();
            if (data == "") {
                fnLoadContent();
            }
            else {
                alert(data);
            }
        });
        }
    }

    function fnSaveOrUpdateBank() {
        var name = $("#txtName").val();

        if (name != '') {
            $.ajax({
                type: "POST",
                url: appPath + 'Directories/SaveOrUpdateBank',
                data: {
                    id: $("#txtId").val(), name: $("#txtName").val(),
                    code: $("#txtCode").val(),
                    add: $("#txtAdd").val()
                },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    if (data == "") {
                        fnSuccess();
                        fnResetBankForm();
                    }
                    else {
                        alert(data);
                    }
                },
                error: function () {
                    fnError();
                },
                complete: function () {
                    fnComplete();
                }
            });            
        } else {
            alert("Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!");
            $("#txtName").focus();
        }
    }

    function fnLoadContent() {
        fnLoad("Directories/Banks");
    }

    function fnResetBankForm() {
        $("#txtId").val("");
        $("#txtCode").val("");
        $("#txtName").val("");
        $("#txtAdd").val("");
        $("#txtCode").focus();        
    }

</script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Units;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                <button onclick="javascript:fnPopupBankDialog('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                %>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
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
                    <table style="width: 100%" id="tbUnits" class="tbDetails">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>ID</th>
                                <th><%= NEXMI.NMCommon.GetInterface("BANK_CODE", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("BANK_NAME", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        <% 
                                      
                            List<NEXMI.NMBanksWSI> WSIs = (List<NEXMI.NMBanksWSI>)ViewData["WSIs"];
                            int i = 1;
                            if (WSIs != null)
                            {
                                foreach (NEXMI.NMBanksWSI Item in WSIs)
                                {                        
                        %>
                            <tr>
                                <td><%=i++%></td>
                                <td><%=Item.Bank.Id%></td>
                                <td><%=Item.Bank.Code%></td>
                                <td><%=Item.Bank.Name%></td>
                                <td><%=Item.Bank.Address%></td>
                                <td >
                        <% 
                                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                                {    
                        %>
                                    <a href="javascript:fnPopupBankDialog('<%=Item.Bank.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
                        <% 
                                }
                        %>
                        <% 
                                if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Delete"))
                                {
                        %>
                                    <a href="javascript:fnDeleteBank('<%=Item.Bank.Id%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                        <% 
                                }
                        %>
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