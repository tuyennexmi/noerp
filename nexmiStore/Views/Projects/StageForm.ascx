<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = "";
    string parentId = ViewData["ParentId"].ToString();
    NEXMI.NMStagesWSI WSI=new NEXMI.NMStagesWSI();
    if (ViewData["WSI"] != null)
    {
        WSI = (NEXMI.NMStagesWSI)ViewData["WSI"];
    }
    if (WSI.Stage == null)
        WSI.Stage = new NEXMI.Stages();
    if (ViewData["WindowId"] == null)
    {
        pos = "static";
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button">Thoát</button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("ProductSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
<% 
    }
    else
    {
        windowId = ViewData["WindowId"].ToString();
    } 
%>
<div class="windowContent" style="position: <%=pos%> !important;">
    <script type="text/javascript">
        $(function () {
            $('#slStatuses<%=windowId%>').prepend('<option value=""></option>');
            $('#form<%=windowId%>').jqxValidator({
                rules: [
                    { input: '#txtName<%=windowId%>', message: 'Không được để trống', action: 'keyup, blur', rule: 'required' }
                ]
            });
            $('#txtName<%=windowId%>').focus();
        });

        function fnSaveOrUpdateStage(windowMode, saveMode) {
            if ($('#form<%=windowId%>').jqxValidator('validate')) {
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Projects/SaveOrUpdateStage',
                    data: {
                        id: $('#txtId<%=windowId%>').val(),
                        name: $('#txtName<%=windowId%>').val(),
                        relatedStatus: $('#slStatuses<%=windowId%>').val(),
                        sequence: $('#txtSequence<%=windowId%>').val(),
                        isCommon: (document.getElementById('cbCommon<%=windowId%>').checked) ? 'true' : 'false',
                        folded: (document.getElementById('cbFold<%=windowId%>').checked) ? 'true' : 'false',
                        description: $('#txtDescription<%=windowId%>').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            fnSuccess();
                            var parentId = '<%=parentId%>';
                            if (parentId != '') {
                                $('#' + parentId).append('<option value="' + data.id + '">' + data.name + '</option>');
                                try {
                                    $('#tbody' + parentId).append('<tr id="' + data.id + '"><td>' + data.name + '</td><td></td><td><a href="javascript:fnRemoveStageFromSession(\'' + data.id + '\');" style="font-weight: bold;">x</a></td></tr>');
                                } catch (err) { }
                            }
                            if (saveMode == 'close') {
                                if (windowMode == 'popup')
                                    closeWindow('<%=windowId%>');
                                //else
                                try {
                                    fnReloadTask();
                                } catch (err) { }
                            }
                            else
                                fnResetFormStage();
                        }
                        else {
                            alert(data.error);
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
                alert('Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!');
            }
        }

        function fnResetFormStage() {
            $('#txtId<%=windowId%>').val('');
            $('#txtName<%=windowId%>').val('');
            $('#slStatuses<%=windowId%>').val('');
            $('#txtSequence<%=windowId%>').val('');
            document.getElementById('cbCommon<%=windowId%>').checked = false;
            document.getElementById('cbFold<%=windowId%>').checked = false;
            $('#txtDescription<%=windowId%>').val('');
        }
    </script>
    <div>
        <form id="form<%=windowId%>" action="">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td class="lbright">Tên giai đoạn</td>
                    <td>
                        <input type="hidden" id="txtId<%=windowId%>" value="<%=WSI.Stage.StageId%>" />
                        <input type="text" id="txtName<%=windowId%>" value="<%=WSI.Stage.StageName%>" />
                    </td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></td>
                    <td><%Html.RenderAction("slStatuses", "UserControl", new { elementId = "slStatuses" + windowId, objectName = "Stages", current = WSI.Stage.RelatedStatus });%></td>
                </tr>
                <tr>
                    <td class="lbright">Thứ tự</td>
                    <td><input type="number" min="0" id="txtSequence<%=windowId%>" value="<%=WSI.Stage.Sequence%>" /></td>
                </tr>
                <tr>
                    <td class="lbright">Mặc định cho dự án mới</td>
                    <td><input type="checkbox" id="cbCommon<%=windowId%>" <%=(WSI.Stage.IsCommon) ? "checked" : ""%> /></td>
                </tr>
                <tr>
                    <td class="lbright">Mặc định thu gọn</td>
                    <td><input type="checkbox" id="cbFold<%=windowId%>" <%=(WSI.Stage.Folded) ? "checked" : ""%> /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <textarea style="width: 99%;" cols="1" rows="6" placeholder="<%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %>..." id="txtDescription<%=windowId%>"><%=WSI.Stage.Description%></textarea>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</div>
<% 
    if (windowId != "")
    {
%>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveOrUpdateStage('popup', 'close')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:fnSaveOrUpdateStage('popup', '')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium">Thoát</button>
    </div>
</div>
<% 
    }    
%>
</div>