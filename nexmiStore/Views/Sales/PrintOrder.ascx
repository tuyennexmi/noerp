<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div style="font-size: 9pt;">
    <div style="text-align: center;">
        <h1><%= NEXMI.NMCommon.GetInterface("SO_TITLE", langId) %></h1>
        <h4>Số <%=ViewData["Id"]%></h4>
    </div>
    <br /><br />
    <h4>Thông tin báo giá</h4>
    <table width="100%">
        <tr>
            <td style="font-weight: bold"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>:</td>
            <td><%=ViewData["CustomerName"]%></td>
            <td style="font-weight: bold"><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %>:</td>
            <td><%=ViewData["OrderDate"]%></td>
        </tr>
        <tr>
            <td style="font-weight: bold"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %>:</td>
            <td><%=ViewData["Reference"]%></td>
            <td style="font-weight: bold"><%= NEXMI.NMCommon.GetInterface("PAYMENT_TIME", langId)%>:</td>
            <td><%=ViewData["PaymentTerm"]%></td>
        </tr>   
    </table>
    <br /><br />
    <h4><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></h4>
    <table border="1" width="100%">
        <tr>
            <td style="font-weight: bold">Mã sản phẩm</td>
            <td style="font-weight: bold"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
            <td style="font-weight: bold"><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></td>
            <td style="font-weight: bold"><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></td>
        </tr>
        <% 
            ArrayList objs = (ArrayList)ViewData["objs"];
            foreach (ArrayList Item in objs)
            {
        %>
        <tr> 
            <td><%=Item[2]%></td>
            <td><%=Item[3]%></td>
            <td><%=Item[4]%></td>
            <td><%=Item[5]%></td>
        </tr>   
        <% 
            }    
        %>
    </table>
</div>