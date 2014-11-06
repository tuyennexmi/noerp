<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
var lstResult = ((IDictionary<string, object>)ViewData["Result"]);

if (lstResult["status"] == "success")
{
    %>
    <table>
        <tr>
            <td>Giảm tiền cho cấp bậc:</td>
            <td><%=lstResult["valueRank"]%></td>
        </tr>
        <%--<tr>
            <td>Lỗi:</td>
            <td><%=lstResult["error"]%></td>
        </tr>--%>
        <tr>
            <td>Mã người dùng:</td>
            <td><%=lstResult["usercode"]%></td>
        </tr>
        <%--<tr>
            <td>Trạng thái:</td>
            <td><%=lstResult["status"]%></td>
        </tr>--%>
        <tr>
            <td>tiền tích luỹ:</td>
            <td><%=lstResult["score"]%></td>
        </tr>
        <tr>
            <td>Email:</td>
            <td><%=lstResult["consumerEmail"]%></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>:</td>
            <td><%=lstResult["consumerName"]%></td>
        </tr>
        <tr>
            <td>Điểm tích luỹ:</td>
            <td><%=lstResult["amount"]%></td>
        </tr>
        <tr>
            <td>Ngày sinh:</td>
            <td><%=lstResult["consumerFullBirthday"]%></td>
        </tr>
        <tr>
            <td>cấp bậc:</td>
            <td><%=lstResult["rank"]%></td>
        </tr>
        <tr>
            <td><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:</td>
            <td><%=lstResult["consumerPhone"]%></td>
        </tr>
        <tr>
            <td>Giới tính:</td>
            <td><%=lstResult["consumerGender"]%></td>
        </tr>
        <tr>
            <td>Mã cấp bậc:</td>
            <td><%=lstResult["rankCode"]%></td>
        </tr>
        <tr>
            <td>Giảm % cho cấp bậc:</td>
            <td><%=lstResult["discountRank"]%></td>
        </tr>
    </table>

<%
}
else
{
    %>
    <label>Mã khách hàng không đúng hoặc thành viên này không tồn tại!</label>
<%
}
    %>