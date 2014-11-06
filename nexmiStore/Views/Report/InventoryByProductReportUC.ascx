<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("BEGIN_AMOUNT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("IMPORTED", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("EXPORTED", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("END_AMOUNT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("VALUE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("ABOVE_LIMIT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("LOWER_LIMIT", langId) %></th>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMProductInStocksWSI> PISs = (List<NEXMI.NMProductInStocksWSI>)ViewData["PIS"];
        double totalBegin = 0, totalImport = 0, totalExport = 0, totalBalance = 0, totalAmount = 0;
        if (PISs.Count > 0)
        {
            int count = 1;
            double totalQuantity = 0, amount = 0;
            string status = "";
            
            foreach (NEXMI.NMProductInStocksWSI Item in PISs)
            {
                if (Item.PIS.BeginQuantity == 0 & Item.PIS.ImportQuantity == 0 & Item.PIS.ExportQuantity == 0)
                    continue;
                totalQuantity = Item.PIS.BeginQuantity + Item.PIS.ImportQuantity - Item.PIS.ExportQuantity;
                totalBegin += Item.PIS.BeginQuantity;
                totalImport += Item.PIS.ImportQuantity;
                totalExport += Item.PIS.ExportQuantity;
                totalBalance += totalQuantity;
                amount = totalQuantity * Item.PIS.CostPrice;
                totalAmount += amount;
                status = "";
                if (totalQuantity > Item.PIS.MaxQuantity)
                {
                    status = "#CC3300";
                }
                else if (totalQuantity < Item.PIS.MinQuantity)
                {
                    status = "#0033CC";
                }
            %>
            <tr style='color:<%=status %>'>
                <td width="3%"><%=count++%></td>
                <td><%= NEXMI.NMCommon.GetName(Item.PIS.StockId, langId)%></td>
                <td align="right"><%= Item.PIS.BeginQuantity.ToString("N3")%></td>
                <td align="right"><%= Item.PIS.ImportQuantity.ToString("N3")%></td>
                <td align="right"><%= Item.PIS.ExportQuantity.ToString("N3")%></td>
                <td align="right"><%= totalQuantity.ToString("N3")%></td>
                <td align="right"><%= amount.ToString("N3")%></td>
                <td align="right"><%= Item.PIS.MaxQuantity.ToString("N3")%></td>
                <td align="right"><%= Item.PIS.MinQuantity.ToString("N3")%></td>
            </tr>
    <%
            }
            %>
    <%
        }
    %>
            <tr>
                <td width="3%"></td>
                <td>Tổng cộng:</td>
                <td align="right"><%= totalBegin.ToString("N3")%></td>
                <td align="right"><%= totalImport.ToString("N3")%></td>
                <td align="right"><%= totalExport.ToString("N3")%></td>
                <td align="right"><%= totalBalance.ToString("N3")%></td>
                <td align="right"><%= totalAmount.ToString("N3")%></td>
                <td align="right"></td>
                <td align="right"></td>
            </tr>
    </tbody>
    <tfoot>
    
    </tfoot>
</table>