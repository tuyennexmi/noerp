<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Permissions.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        if ('<%=Request.Params["UserId"]%>' == "") {
            fnChangeUser();
        }
        $("#slUsers").change(function () {
            fnChangeUser();
        });
    });
    function fnCheckAll(functionId) {
        var obj = document.getElementById("allF" + functionId);
        var objs = document.getElementsByName("F" + functionId);
        if (obj.checked) {
            for (var x in objs) {
                objs[x].checked = true;
            }
        } else {
            for (var x in objs) {
                objs[x].checked = false;
            }
        }
        fnChangeStatus(functionId);
    }
    function fnCheckOne(functionId) {
        var isChecked = true;
        var objs = document.getElementsByName("F" + functionId);
        for (var x in objs) {
            if (objs[x].checked == false) {
                isChecked = false;
                break;
            }
        }
        var obj = document.getElementById("allF" + functionId);
        if (isChecked) {
            obj.checked = true;
        }
        else {
            obj.checked = false;
        }
        fnChangeStatus(functionId);
    }

    function fnChangeStatus(functionId) {
        var userId = $("#slUsers").val();
        if (userId != "") {
            var id = $("#id" + functionId).val();
            var pSelect = "N"; if (document.getElementById("select" + functionId).checked) pSelect = "Y";
            var pInsert = "N"; if (document.getElementById("insert" + functionId).checked) pInsert = "Y";
            var pUpdate = "N"; if (document.getElementById("update" + functionId).checked) pUpdate = "Y";
            var reconcile = "N"; if (document.getElementById("reconcile" + functionId).checked) reconcile = "Y";
            var pDelete = "N"; if (document.getElementById("delete" + functionId).checked) pDelete = "Y";

            var pViewPrice = "N"; if (document.getElementById("viewPrice" + functionId).checked) pViewPrice = "Y";
            var pPrint = "N"; if (document.getElementById("print" + functionId).checked) pPrint = "Y";
            var pExport = "N"; if (document.getElementById("export" + functionId).checked) pExport = "Y";
            var pDuplicate = "N"; if (document.getElementById("duplicate" + functionId).checked) pDuplicate = "Y";

            var approval = "N"; if (document.getElementById("approval" + functionId).checked) approval = "Y";
            var viewAll = "N"; if (document.getElementById("viewall" + functionId).checked) viewAll = "Y";
            var calculate = "N"; if (document.getElementById("calculate" + functionId).checked) calculate = "Y";
            var history = "N"; if (document.getElementById("history" + functionId).checked) history = "Y";
            $.post(appPath + "Managements/ChangePermissions", {id: id, userId: userId, functionId: functionId, pSelect: pSelect, pInsert: pInsert,
                pUpdate: pUpdate, pDelete: pDelete, approval: approval, viewAll: viewAll, calculate: calculate, history: history,
                pViewPrice: pViewPrice, pPrint: pPrint, pExport: pExport, pDuplicate: pDuplicate, reconcile: reconcile
            },
            function (data) {
                if (data == "") {

                }
                else {
                    alert(data);
                }
            });
        } else {
            alert("Chọn người dùng để phân quyền!");
        }
    }

    function fnChangeUser() {
        var userId = $('#slUsers').val();
        LoadContent('', 'Managements/Permissions?userId=' + userId);
    }
</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <button onclick="javascript:$(window).hashchange();" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td></td>
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
            <table style="width: 100%">
                <tr>
                    <td class="lbright" style="width: 200px">Người dùng</td>
                    <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers", userId = ViewData["slUsers"] }); %></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></td>
                    <td><input type="checkbox" id="cbStatus" <%=ViewData["checkStatus"]%> disabled="disabled" /> Kích hoạt</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table style="width:100%" class="tbDetails">
                <tr>
                    <th style="width:10%">Phân hệ</th>
                    <th style="width:15%">Chức năng</th>
                    <th>Xem</th>
                    <th><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId)%></th>
                    <th><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></th>
                    <th>Hiệu chỉnh</th>
                    <th><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></th>

                    <th>Xem giá</th>                    
                    <th><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></th>
                    <th>Xuất dữ liệu</th>
                    <th><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></th>

                    <th>Kiểm duyệt</th>
                    <th>Xem tất cả</th>
                    <th>Tính toán</th>
                    <th>Lịch sử</th>
                    <th>Tất cả</th>
                </tr>
                <%
                    if (ViewData["UserId"] != null)
                    {
                        string userId = ViewData["UserId"].ToString();
                        List<NEXMI.NMModulesWSI> ModuleWSIs = (List<NEXMI.NMModulesWSI>)ViewData["ModuleWSIs"];
                        int count = 0;
                        String strSelect = "", strInsert = "", strUpdate = "", strDelete = "", strAll = "",
                            strViewPrice = "", strExport = "", strPrint = "", strDuplicate = "",
                            strApproval = "", strViewAll = "", strCalculate = "", strHistory = "", strReconcile = "";
                        for (int i = 0; i < ModuleWSIs.Count; i++)
                        {
                            if (ModuleWSIs[i].Active != null && ModuleWSIs[i].Active.ToLower() == "true")
                            {
                                List<NEXMI.Functions> Functions = ModuleWSIs[i].Module.FunctionsList.Cast<NEXMI.Functions>().Select(x => x).Where(x => x.Active == true).ToList();
                                count = Functions.Count;
                %>
                <tr>
                    <td rowspan="<%=count %>" valign="top">
                        <b><%=ModuleWSIs[i].Module.Name%></b>
                    </td>
                <%
                    NEXMI.NMPermissionsBL PermissionBL = new NEXMI.NMPermissionsBL();
                    NEXMI.NMPermissionsWSI PermissionWSI;
                    for (int j = 0; j < Functions.Count; j++)
                    {
                            strSelect = ""; strInsert = ""; strUpdate = ""; strDelete = ""; strAll = "";
                            strApproval = ""; strViewAll = ""; strCalculate = ""; strHistory = "";
                            strViewPrice = ""; strExport = ""; strPrint = ""; strDuplicate = ""; strReconcile = "";
                            PermissionWSI = new NEXMI.NMPermissionsWSI();
                            PermissionWSI.Mode = "SEL_OBJ";
                            PermissionWSI.UserId = userId;
                            PermissionWSI.FunctionId = Functions[j].Id;
                            PermissionWSI = PermissionBL.callSingleBL(PermissionWSI);
                            if (PermissionWSI.PSelect == "Y") strSelect = "checked";
                            if (PermissionWSI.PInsert == "Y") strInsert = "checked";
                            if (PermissionWSI.PUpdate == "Y") strUpdate = "checked";
                            if (PermissionWSI.Reconcile == "Y") strReconcile = "checked";
                            if (PermissionWSI.PDelete == "Y") strDelete = "checked";
                                    
                            if (PermissionWSI.ViewPrice == "Y") strViewPrice = "checked";
                            if (PermissionWSI.PPrint == "Y") strPrint = "checked";
                            if (PermissionWSI.Export == "Y") strExport = "checked";
                            if (PermissionWSI.Duplicate == "Y") strDuplicate = "checked";
                                    
                            if (PermissionWSI.Approval == "Y") strApproval = "checked";
                            if (PermissionWSI.ViewAll == "Y") strViewAll = "checked";
                            if (PermissionWSI.Calculate == "Y") strCalculate = "checked";
                            if (PermissionWSI.History == "Y") strHistory = "checked";
                            if (strSelect != "" && strInsert != "" && strUpdate != "" && strDelete != "" && strApproval != "" 
                                && strViewAll != "" && strCalculate != "" && strHistory != "" && strViewPrice != "" && strPrint !="" && strExport != "" && strDuplicate != "" && strReconcile != "")
                            {
                                strAll = "checked";
                            }
                %>  
                    <td><%=Functions[j].Name%><input type="hidden" id="id<%=Functions[j].Id%>" value="<%=PermissionWSI.Id %>" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strSelect%> id="select<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strInsert%> id="insert<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strUpdate%> id="update<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strReconcile%> id="reconcile<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strDelete%> id="delete<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>

                    <td style="width: 5.7%"><input type="checkbox" <%=strViewPrice%> id="viewPrice<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strPrint%> id="print<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strExport%> id="export<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strDuplicate%> id="duplicate<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>

                    <td style="width: 5.7%"><input type="checkbox" <%=strApproval%> id="approval<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strViewAll%> id="viewall<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strCalculate%> id="calculate<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strHistory%> id="history<%=Functions[j].Id%>" name="F<%=Functions[j].Id%>" onclick="javascript:fnCheckOne('<%=Functions[j].Id%>')" /></td>
                    <td style="width: 5.7%"><input type="checkbox" <%=strAll%> id="allF<%=Functions[j].Id%>" onclick="javascript:fnCheckAll('<%=Functions[j].Id%>')" /></td>
                </tr>
                <%
                                }
                            }
                        }
                    }
                %>          
            </table>    
        </td>
    </tr>
</table>
</div>
</div>