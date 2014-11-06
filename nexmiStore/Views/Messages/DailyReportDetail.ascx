<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>


<%
    NEXMI.NMDailyReportsWSI WSI = (NEXMI.NMDailyReportsWSI)ViewData["WSI"];
    
     %>

<table class="frmInput">
    <tr>
        <td class="lbright">Số báo cáo</td>
        <td><%=WSI.DailyReport.Id%></td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
        <td><%=WSI.DailyReport.Title%> </td>
    </tr>
    <tr>
        <td class="lbright">Tóm tắc</td>
        <td><%=WSI.DailyReport.Contents%></td>
    </tr>
    <tr>
        <td class="lbright">Thuận lợi</td>
        <td><%=WSI.DailyReport.Advantages%></td>
    </tr>
    <tr>
        <td class="lbright">Khó khăn</td>
        <td><%=WSI.DailyReport.Hards%></td>
    </tr>
    <tr>
        <td class="lbright">Đề xuất</td>
        <td><%=WSI.DailyReport.Promotes%></td>
    </tr>                    
</table>