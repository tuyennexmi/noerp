<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
    string orderId = ViewData["OrderId"].ToString();
%>
<script type="text/javascript">
    $(function () {
        $('#maintainDate').datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    function fnCheck() {
        $('.sl').hide();
        $('.' + $('#sl<%=windowId%>').val()).show();
    }

    function fnSaveMaintainDateChange() {
        $.ajax({
            type: 'POST',
            url: appPath + 'Sales/SaveMaintainDateChange',
            data: {
                orderId: '<%=orderId%>'
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data.error == '') {
                    $(window).hashchange();
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
    }
</script>
<div class="windowContent" style="padding-left: 30px">
    <input type='text' id='maintainDate' value='' style="z-index: 2000;" />
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <span class="sl sl1">
            <button onclick="javascript:fnSaveMaintainDateChange()" class="color red button medium">Lưu thay đổi</button>
            
        </span>
        
        <button onclick='javascript:closeWindow("<%=windowId%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>