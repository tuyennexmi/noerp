<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table class="frmInput">
    <tr>
        <td>
            <table class="tbDetails">
                <tr>
                    <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("KIND", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %> hàng</th>
                </tr>
                <tbody>
                <%
                    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
                    if (dt.Rows.Count > 0)
                    {
                        double total1 = 0, total2 = 0, total3 = 0, total4 = 0, total5 = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            total1 += double.Parse(dt.Rows[i]["Quantity"].ToString());
                            total2 += double.Parse(dt.Rows[i]["DiscountAmount"].ToString());
                            total3 += double.Parse(dt.Rows[i]["TaxAmount"].ToString());
                            total4 += double.Parse(dt.Rows[i]["Amount"].ToString());
                            total5 += double.Parse(dt.Rows[i]["TotalAmount"].ToString());
                %>
                <tr>
                    <td><%=dt.Rows[i]["ProductName"].ToString()%></td>
                    <td align="right"><%=double.Parse(dt.Rows[i]["Quantity"].ToString()).ToString("N3")%></td>
                    <td align="right"><%=double.Parse(dt.Rows[i]["DiscountAmount"].ToString()).ToString("N3")%></td>
                    <td align="right"><%=double.Parse(dt.Rows[i]["TaxAmount"].ToString()).ToString("N3")%></td>
                    <td align="right"><%=double.Parse(dt.Rows[i]["Amount"].ToString()).ToString("N3")%></td>
                    <td align="right"><%=double.Parse(dt.Rows[i]["TotalAmount"].ToString()).ToString("N3")%></td>
                    <td><%=dt.Rows[i]["CategoryName"].ToString()%></td>
                    <td><%=dt.Rows[i]["StockName"].ToString()%></td>
                </tr>
                <%
                        }
                %>
                <tr style="font-weight: bold;">
                    <td></td>
                    <td align="right"><%=total1.ToString("N3")%></td>
                    <td align="right"><%=total2.ToString("N3")%></td>
                    <td align="right"><%=total3.ToString("N3")%></td>
                    <td align="right"><%=total4.ToString("N3")%></td>
                    <td align="right"><%=total5.ToString("N3")%></td>
                    <td></td>
                    <td></td>
                </tr>
                <% 
                    }
                    else
                    { 
                %>
                </tbody>
                <tr><td colspan="8" align="center"><h4>Không có dữ liệu.</h4></td></tr>
                <%
                    }
                %>
            </table>
        </td>
    </tr>
</table>