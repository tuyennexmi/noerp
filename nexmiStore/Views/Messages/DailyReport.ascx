<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMDailyReportsWSI WSI = (NEXMI.NMDailyReportsWSI)ViewData["WSI"];
    
     %>


<script type="text/javascript">
    $(function () {
        $("#formDailyReport").jqxValidator({
            rules: [
                        { input: '#txtTitle', message: 'Bạn chưa nhập tiêu đề.', action: 'keyup, blur', rule: function (input, commit) {
                            var txt = $('#txtTitle').val();
                            if (txt == '')
                                return false;
                            return true;
                        }
                        },
                        { input: '#areaContent', message: 'Bạn chưa nhập nội dung.', action: 'keyup, blur', rule: function (input, commit) {
                            var txt = $('#areaContent').val();
                            if (txt == '')
                                return false;
                            return true;
                        }
                        }
                    ]
        });
    });

    function fnSaveDailyReport(saveMode) {
        if ($("#formDailyReport").jqxValidator('validate')) {            
            $.ajax({
                type: "POST",
                url: appPath + 'Messages/SaveOrUpdateDailyReport',
                data: {
                    id: $("#txtDRId").val(),
                    title: $("#txtTitle").val(),
                    advantage: $("#areaAdvantages").val(),
                    content: $("#areaContent").val(),
                    hard: $("#areaHard").val(),
                    promote: $("#areaPromote").val(),
                    taskId: $('#txtTaskId').val()
                },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    if (data.error == "") {
                        fnSuccess();
                        if ('<%=ViewData["WindowId"]%>' == '') {
                            if (saveMode == '1')
                                fnResetForm();
                            else
                                fnLoadDailyReportDetail(data.id);
                        } else {
                            closeWindow('<%=ViewData["WindowId"]%>');
                            $(window).hashchange();
                        }
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
            alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!");
        }
    }

    function fnResetForm() {
        $("#txtTitle").val('')
        $("#areaAdvantages").val('')
        $("#areaContent").val('')
        $("#areaHard").val('')
        $("#areaPromote").val('')
    }

</script>

<form id="formDailyReport" action="">
    <input type="hidden" id="txtTaskId" value="<%=ViewData["TaskId"] %>" />
    <table class="frmInput">
        <tr>
            <td class="lbright">Số báo cáo</td>
            <td><input type="text" id="txtDRId" name="txtDRId" disabled="disabled" value="<%=WSI.DailyReport.Id%>" /></td>
        </tr>
        <tr>
            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
            <td><input type="text" id="txtTitle" name="txtTitle" value="<%=WSI.DailyReport.Title%>" style="width: 600px"/></td>
        </tr>
        <tr>
            <td class="lbright">Tóm tắc</td>
            <td><textarea id="areaContent" name="areaContent" cols="100" rows="2" style="width: 600px"><%=WSI.DailyReport.Contents%></textarea></td>
        </tr>
        <tr>
            <td class="lbright">Thuận lợi</td>
            <td><textarea id="areaAdvantages" name="areaAdvantages" cols="100" rows="3" style="width: 600px"><%=WSI.DailyReport.Advantages%></textarea></td>
        </tr>
        <tr>
            <td class="lbright">Khó khăn</td>
            <td><textarea id="areaHard" name="areaHard" cols="100" rows="3" style="width: 600px"><%=WSI.DailyReport.Hards%></textarea></td>
        </tr>
        <tr>
            <td class="lbright">Đề xuất</td>
            <td><textarea id="areaPromote" name="areaPromote" cols="100" rows="3" style="width: 600px"><%=WSI.DailyReport.Promotes%></textarea></td>
        </tr>                    
    </table>
</form>

<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveDailyReport('')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>