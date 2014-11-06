<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%
    DateTime thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    DateTime lastMonth = thisMonth.AddMonths(-1);
    string today = DateTime.Today.ToString("yyyy-MM-dd"),
        yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
        firstDateOfThisMonth = thisMonth.ToString("yyyy-MM-dd"),
        lastDateOfThisMonth = thisMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"),
        firstDateOfLastMonth = lastMonth.ToString("yyyy-MM-dd"),
        lastDateOfLastMonth = lastMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"),
        start_thisquarter = thisMonth.AddMonths(-2).ToString("yyyy-MM-dd"),
        end_thisquarter = lastDateOfThisMonth,
        start_lastquarter = thisMonth.AddMonths(-5).ToString("yyyy-MM-dd"),
        end_lastquarter = thisMonth.AddMonths(-2).AddDays(-1).ToString("yyyy-MM-dd");
%>

<table>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td>
            <button class="button" onclick="javascript:fnSetDate('<%=today%>', '')"> Hôm nay </button>                
            <button class="button" onclick="javascript:fnSetDate('<%=yesterday%>', '<%=yesterday%>')"> Hôm qua </button>
            <button class="button" onclick="javascript:fnSetDate('<%=firstDateOfThisMonth%>', '<%=lastDateOfThisMonth%>')"> Tháng này </button>                
            <button class="button" onclick="javascript:fnSetDate('<%=firstDateOfLastMonth%>', '<%=lastDateOfLastMonth%>')"> Tháng trước </button>
            <button class="button" onclick="javascript:fnSetDate('<%=start_thisquarter %>', '<%=end_thisquarter%>')"> Quý này </button>
            <button class="button" onclick="javascript:fnSetDate('<%=start_lastquarter%>', '<%=end_lastquarter%>')"> Quý trước </button>
            <button onclick="javascript:GetContent()" class="button"> Theo ngày chọn </button>
        </td>
    </tr>
</table>