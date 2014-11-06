<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    List<NEXMI.NMProductInStocksWSI> lpis = (List<NEXMI.NMProductInStocksWSI>)ViewData["ProductInStocks"];
    List<NEXMI.NMStocksWSI> stock = (List<NEXMI.NMStocksWSI>)ViewData["stock"];
 %>

 <table style="width: 96%; margin:8px;" class="tbDetails">
    <tr>
        <th> Mã Kho</th>
        <th>Tên Kho</th>
        <th>Số dư đầu kỳ</th>
        <th><%= NEXMI.NMCommon.GetInterface("IMPORTED", langId) %></th>
        <th><%= NEXMI.NMCommon.GetInterface("EXPORTED", langId) %></th>
        <th>Tồn kho hiện tại</th>
    </tr>

    <% 
        double balance = 0;
        NEXMI.NMProductInStocksWSI pis;
        foreach (NEXMI.NMStocksWSI st in stock)
        { 
            pis=  lpis.Where(p => p.PIS.StockId == st.Id).FirstOrDefault();
            if (pis != null)
            {
                balance = pis.PIS.BeginQuantity + pis.PIS.ImportQuantity - pis.PIS.ExportQuantity;
    %>
            <tr>
                <td><%=st.Id%></td>
                <td><%= (st.Translation == null) ? "" : st.Translation.Name%></td>
                <td align="right"><%= pis.PIS.BeginQuantity.ToString("N3")%></td>
                <td align="right"><%= pis.PIS.ImportQuantity.ToString("N3")%></td>
                <td align="right"><%= pis.PIS.ExportQuantity.ToString("N3")%></td>
                <td align="right"><%= balance.ToString("N3")%></td>
            </tr>

    <%      }
            else
            {
                %>
            <tr>
                <td><%=st.Id%></td>
                <td><%= (st.Translation == null) ? "" : st.Translation.Name%></td>
                <td colspan='4'>Chưa có số liệu!</td>
            </tr>

    <%      }
        }
    %>
 </table>