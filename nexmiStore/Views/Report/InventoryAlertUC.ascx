<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<h3>Những sản phẩm còn ít nhất trong kho</h3>

<table class="tbDetails" style="width: 100%" >
    <tr>
        <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
        <th>Nhóm sản phẩm</th>
        <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>
        <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
        <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
        <th>Đơn vị</th>
        <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
    </tr>
<%
    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
    for (int i = 0; i < dt.Rows.Count; i++)
    {
%>
        <tr>
            <td><%=NEXMI.NMCommon.GetName(dt.Rows[i]["ProductId"].ToString(), langId)%></td>
            <td><%=NEXMI.NMCommon.GetName(dt.Rows[i]["CategoryId"].ToString(), langId)%></td>
            <td><%=NEXMI.NMCommon.GetName(dt.Rows[i]["StockId"].ToString(), langId)%></td>
            <td><%=dt.Rows[i]["soluong"]%></td>
            <td align="right">
                <% 
        try
        {    
                %>
                <%=Double.Parse(dt.Rows[i]["gia"].ToString()).ToString("N3")%>
                <% 
        }
        catch { }    
                %>
            </td>
            <td><%=dt.Rows[i]["donvi"]%></td>
            <td><%if (dt.Rows[i]["trangthai"].ToString() == "False")
                  {%>Đang bán<%}
                  else
                  {%>Ngưng bán<%} %></td>
        </tr>
<%
    }
%>
</table>