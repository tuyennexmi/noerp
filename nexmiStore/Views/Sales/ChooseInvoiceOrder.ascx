<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
    string orderId = ViewData["OrderId"].ToString();
%>
<script type="text/javascript">
    $(function () {
        fnCheck();
    });

    function fnCheck() {
        $('.sl').hide();
        $('.' + $('#sl<%=windowId%>').val()).show();
    }

    function fnCreateInvoiceFromOrder(mode) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Sales/CreateInvoiceFromOrder',
            data: {
                orderId: '<%=orderId%>'
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data.error == '') {
                    if (mode == '0') {
                        closeWindow('<%=windowId%>');
                        LoadContent('', 'Sales/SalesInvoice?invoiceId=' + data.invoiceId);
                    }
                    else
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

    function fnShowInvoiceFromOrder() {
        LoadContent('', 'Sales/SalesInvoiceForm?orderId=' + '<%=orderId%>');
        closeWindow('<%=windowId%>');
    }
</script>
<div class="windowContent">
    <table style="width: 100%;" cellpadding="3">
        <tr>
            <td colspan="2">Chọn cách bạn muốn tạo hóa đơn cho đơn hàng này. Việc này sẽ tạo ra một bản nháp có thể được sửa đổi trước khi xác nhận.</td>
        </tr>
        <tr>
            <td><b>Bạn muốn tạo hóa đơn theo cách nào?</b></td>
            <td>    
                <select id="sl<%=windowId%>" onchange="javascript:fnCheck();">
                    <option value="sl1">Tạo hóa đơn cho toàn bộ đơn hàng</option>
                    <option value="sl2">Tạo hóa đơn theo chi tiết của đơn hàng</option>
                </select>
            </td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <span class="sl sl1">
            <button onclick="javascript:fnCreateInvoiceFromOrder('0')" class="color red button medium">Tạo & xem hóa đơn</button>
            <button onclick="javascript:fnCreateInvoiceFromOrder('1')" class="color red button medium">Tạo hóa đơn</button>
            
        </span>
        <span class="sl sl2">
            <button onclick="javascript:fnShowInvoiceFromOrder()" class="color red button medium">Hiển thị các chi tiết</button>
        </span>
        <button onclick='javascript:closeWindow("<%=windowId%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>