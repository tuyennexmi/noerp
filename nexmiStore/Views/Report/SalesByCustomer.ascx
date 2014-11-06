<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type ="text/javascript">

    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    function fnSetDate(fromDate, toDate) {
        $('#dtFrom').val(fromDate);
        $('#dtTo').val(toDate);
        GetContent();
    }

    function GetContent() {
        var area = document.getElementsByName('cbbAreas')[0].value;
        LoadContentDynamic('SalesByCustomerContent', 'Report/SalesByCustomerUC', {
            //stockid: $("#slStocks").val(),
            area: area,
            fromDate: $('#dtFrom').val(),
            toDate: $('#dtTo').val()
        });
    }

    function fnReloadCustomers(customerId) {
        //var cusId = document.getElementsByName('cbbCustomers')[0].value;
        GetContent();
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
                    <%--<td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>--%>                
                    <td><%= NEXMI.NMCommon.GetInterface("AREA", langId) %>:</td>
                    <td> <%Html.RenderAction("cbbAreas", "UserControl", new { areaId = "", areaName = "", elementId = "", holderText = NEXMI.NMCommon.GetInterface("SELECT_AREA", langId) });%></td>
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 89px"/></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" style="width: 89px"/></td>
                </tr>
            </table>
        </div>
    </div>
    <div id="SalesByCustomerContent">
    </div>
</div>
