<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%

var lstResult = ((IDictionary<string, object>)ViewData["Result"]);

string status = lstResult["status"].ToString();
if (status.CompareTo("success") == 0)
{
    %>
    <table>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %>:</td>
            <td><%=lstResult["status"]%></td>
        </tr>
        <tr>
            <td>Lỗi:</td>
            <td><%=lstResult["error"]%></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>:</td>
            <td><%=lstResult["consumerName"]%></td>
        </tr>
        <tr>
            <td>Ngày sinh:</td>
            <td><%=lstResult["consumerFullBirthday"]%></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:</td>
            <td><%=lstResult["consumerPhone"]%></td>
        </tr>
        <tr>
            <td>Email:</td>
            <td><%=lstResult["consumerEmail"]%></td>
        </tr>
        <tr>
            <td>Giới tính:</td>
            <td><%=lstResult["consumerGender"]%></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:</td>
            <td><%=lstResult["consumerAddress"]%></td>
        </tr>
        <tr>
            <td>tiền tích luỹ:</td>
            <td><%=lstResult["score"]%></td>
        </tr>
        <tr>
            <td>Điểm tích luỹ:</td>
            <td><%=lstResult["amount"]%></td>
        </tr>
        <tr>
            <td>cấp bậc:</td>
            <td><%=lstResult["rank"]%></td>
        </tr>
        <tr>
            <td>Giảm % cho cấp bậc:</td>
            <td><%=lstResult["discountRank"]%></td>
        </tr>
        <tr>
            <td>giảm tiền cho cấp bậc:</td>
            <td><%=lstResult["valueRank"]%></td>
        </tr>
        <tr>
            <td>% giá trị khuyến mãi:</td>
            <td><%=lstResult["discount"]%></td>
        </tr>
        <tr>
            <td>Số tiền khuyến mãi:</td>
            <td><%=lstResult["value"]%></td>
        </tr>
        <tr>
            <td>Ngày hết hạn:</td>
            <td><%=lstResult["expriredDate"]%></td>
        </tr>
        <tr>
            <td>Chủ đề:</td>
            <td><%=lstResult["title"]%></td>
        </tr>
        <%--<tr>
            <td><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %>:</td>
            <td><%=lstResult["description"]%></td>
        </tr>--%>
        <%--<tr>
            <td>Loại khuyến mãi:</td>
            <td><%=lstResult["type"]%></td>
        </tr>--%>
        <tr>
            <td>Trạng thái đã sử dụng:</td>
            <td><%=lstResult["used"]%></td>
        </tr>
        <tr>
            <td>Mã thành viên:</td>
            <td><%=lstResult["memberCode"]%></td>
        </tr>
    </table>
    <%
}
else
{
    %>
    <label>Mã khuyến mại này không đúng hoặc không tồn tại!</label>
<%
}
    %>