<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    
    function fnSetDate(fromDate, toDate) {
        $('#dtFrom').val(fromDate);
        $('#dtTo').val(toDate);
        var cusId = document.getElementsByName('cbbCustomers')[0].value;
        if (cusId == '') {
            alert('Vui lòng chọn khách hàng!');
        } else {
            GetContent(cusId);
        }
    }

    function GetContent(customerId) {
        LoadContentDynamic('QuantityByCustomerContent', 'Report/QuantityByCustomerUC', {
            customerId: customerId,
            fromDate: $('#dtFrom').val(),
            toDate: $('#dtTo').val()
        });
    }

    function cbbCustomerChanged(customerId) {
        var cusId = document.getElementsByName('cbbCustomers')[0].value;        
        GetContent(cusId);
    }
    
</script>


<div class="divActions">
    <%Html.RenderPartial("ToolbarTimeButton"); %>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <table>
                <tr>
                    <td>Chọn khách hàng:</td>
                    <td> <%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = "", customerName = "" });%></td>
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 89px"/></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" style="width: 89px"/></td>
                    <td></td>
                </tr>
            </table>
        </div>
    </div>
    <div id="QuantityByCustomerContent"></div>
</div>