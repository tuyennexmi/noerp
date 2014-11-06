<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
    string status = NEXMI.NMConstant.JobsStatus.Draft;
    NEXMI.NMJobsWSI WSI = new NEXMI.NMJobsWSI();
    if (ViewData["WSI"] != null)
    {
        WSI = (NEXMI.NMJobsWSI)ViewData["WSI"];
        status = WSI.Job.Status;
    }

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

        function fnAddJob(id, saveMode) {
            if ($('#form<%=windowId%>').jqxValidator('validate')) {
                if (id == '')
                    id = tempId;
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Projects/SaveOrUpdateJob',
                    data: {
                        id: id,
                        name: $('#txtName<%=windowId%>').val(),
                        timeSpent: $('#txtTimeSpent<%=windowId%>').val(),
                        purpose: $('#txtPurpose<%=windowId%>').val(),
                        criteria: $('#txtCriteria<%=windowId%>').val(),
                        workSummary: $('#txtWorkSummary<%=windowId%>').val(),
                        tags: $('#txtTags<%=windowId%>').val(),
                        workGuids: $('#txtWorkGuids<%=windowId%>').val(),
                        status: '<%=status %>'
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data == '') {
                            //fnSuccess();
                            //tempId += 1;
                            //fnAppendWorkToList(id, $('#txtName<%=windowId%>').val(), $('#txtTimeSpent<%=windowId%>').val(), $('#txtStartDate<%=windowId%>').val(), $('#slUsers<%=windowId%> option:selected').text());
                            if (saveMode == 'close') {
                                closeWindow('<%=windowId%>');
                                $(window).hashchange();
                            }
                            else
                                fnResetFormJob();
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

        function fnResetFormJob() {
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
                        <input type="text" id="txtName<%=windowId%>" value="<%=WSI.Job.Name%>" />
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Mục đích</td>
                    <td>
                        <input type="text" id="txtPurpose<%=windowId%>" value="<%=WSI.Job.Purpose %>" />
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Tiêu chí</td>
                    <td>
                        <input type="text" id="txtCriteria<%=windowId%>" value="<%=WSI.Job.Criteria %>" />
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Tóm tắt</td>
                    <td>
                        <input type="text" id="txtWorkSummary<%=windowId%>" value="<%=WSI.Job.WorkSummary %>" />
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Thời gian thực hiện</td>
                    <td><input type="text" id="txtTimeSpent<%=windowId%>" value="<%=WSI.Job.TimeSpent %>" style="width: 150px;" /> giờ</td>
                </tr>
                <tr>
                    <td class="lbright">Các đánh dấu</td>
                    <td>
                        <input type="text" id="txtTags<%=windowId%>" value="<%=WSI.Job.Tags %>" />
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Hướng dẫn</td>
                    <td>
                        <textarea id="txtWorkGuids<%=windowId%>" cols="1" rows="5" style="width: 99%;" ><%=WSI.Job.WorkGuids %></textarea>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnAddJob('<%=WSI.Job.Id%>', 'close')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:fnAddJob('', '')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium">Thoát</button>
    </div>
</div>