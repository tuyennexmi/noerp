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

    function GetReportByHour(fromDate, toDate) {
        $('#dtFrom').val(fromDate);
        $('#dtTo').val(toDate);
        LoadContentDynamic('ReportContent', 'Report/SalesByHoursOfDayReport', {            
            fromDate: $('#dtFrom').val(),
            toDate: $('#dtTo').val()
        });
    }

    function fnReloadCustomers(customerId) {
        GetContent();
    }
    
</script>

<%
    DateTime thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    DateTime lastMonth = thisMonth.AddMonths(-1);
    string today = DateTime.Today.ToString("yyyy-MM-dd"),
        yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
        firstDateOfThisMonth = thisMonth.ToString("yyyy-MM-dd"),
        lastDateOfThisMonth = thisMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"),
        firstDateOfLastMonth = lastMonth.ToString("yyyy-MM-dd"),
        lastDateOfLastMonth = lastMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");    
    
%>
<div class="divActions">    
    <table style="width: 100%;">            
        <tr>
            <td>
                <button onclick="javascript:GetReportByHour('<%=today%>', '')" class="button"> Hôm nay </button>                    
                <button onclick="javascript:GetReportByHour('<%=yesterday%>', '<%=yesterday%>')" class="button"> Hôm qua </button>                    
                <button onclick="javascript:fnSetDate('<%=firstDateOfThisMonth%>', '<%=lastDateOfThisMonth%>')" class="button"> Tháng này </button>
                <button onclick="javascript:fnSetDate('<%=firstDateOfLastMonth%>', '<%=lastDateOfLastMonth%>')" class="button"> Tháng trước </button>
                <button onclick="javascript:GetContent()" class="button"> Theo ngày chọn </button>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
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
    <div id="ReportContent">
    </div>
</div>
