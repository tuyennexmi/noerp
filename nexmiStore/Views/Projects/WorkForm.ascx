<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
%>
<div class="windowContent" style="position: absolute !important;">
    <script type="text/javascript">
        $(function () {
            $('#form<%=windowId%>').jqxValidator({
                rules: [
                    { input: '#txtName<%=windowId%>', message: 'Không được để trống', action: 'keyup, blur', rule: 'required' }
                ]
            });
            $('#txtTimeSpent<%=windowId%>').autoNumeric('init', { vMin: 0 });
            $('#txtName<%=windowId%>').focus();
        });

        var tempId = 0;

        function fnAddWork(no, saveMode) {
            if ($('#form<%=windowId%>').jqxValidator('validate')) {
                if (no == '')
                    no = tempId;
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Projects/AddWorkToSession',
                    data: {
                        no: no,
                        name: $('#txtName<%=windowId%>').val(),
                        timeSpent: $('#txtTimeSpent<%=windowId%>').val(),
                        startDate: $('#txtStartDate<%=windowId%>').val(),
                        userId: $('#slUsers<%=windowId%>').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == '') {
                            //fnSuccess();
                            //tempId += 1;
                            //fnAppendWorkToList(data.No, $('#txtName<%=windowId%>').val(), $('#txtTimeSpent<%=windowId%>').val(), $('#txtStartDate<%=windowId%>').val(), $('#slUsers<%=windowId%> option:selected').text());
                            fnAppendWorkLine();
                            if (saveMode == 'close') {
                                closeWindow('<%=windowId%>');
                            }
                            else
                                fnResetFormWork();
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
                alert('Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!');
            }
        }

        function fnAppendWorkLine() {
            LoadContentDynamic('TaskItemLine', 'Projects/TaskItemLine');
        }

        function fnResetFormWork() {
            $('#txtName<%=windowId%>').val('');
            $('#txtTimeSpent<%=windowId%>').val('');
            //$('#txtStartDate<%=windowId%>').val('');
            //$('#slUsers<%=windowId%>').val('');
            $('#txtName<%=windowId%>').focus();
        }
    </script>
    <div>
        <form id="form<%=windowId%>" action="">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td class="lbright">Tên công việc</td>
                    <td>
                        <input type="text" id="txtName<%=windowId%>" value="<%=ViewData["Name"]%>" />
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Thời gian</td>
                    <td><input type="text" id="txtTimeSpent<%=windowId%>" value="<%=ViewData["TimeSpent"]%>" style="width: 150px;" /> giờ</td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></td>
                    <td><input type="datetime-local" id="txtStartDate<%=windowId%>" value="<%=ViewData["StartDate"]%>" /></td>
                </tr>
                <tr>
                    <td class="lbright">Người thực hiện</td>
                    <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers" + windowId, userId = ViewData["UserId"] });%></td>
                </tr>
            </table>
        </form>
    </div>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnAddWork('<%=ViewData["Id"]%>', 'close')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:fnAddWork('', '')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium">Thoát</button>
    </div>
</div>