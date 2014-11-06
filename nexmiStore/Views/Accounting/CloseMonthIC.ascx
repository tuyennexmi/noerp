<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table class="frmInput" style="width: 100%;">
    <tr>
        <td>

            <table id="tbReceipts" class="tbDetails" border=".1" style="width: 100%;">
                <thead>
                    <tr>
                        <th>#</th>
                        <th><%= NEXMI.NMCommon.GetInterface("STORE", langId) %> hàng</th>
                        <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("BEGIN_AMOUNT", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("IMPORTED", langId) %></th>            
                        <th><%= NEXMI.NMCommon.GetInterface("EXPORTED", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("END_AMOUNT", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("VALUE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("ABOVE_LIMIT", langId) %></th>
                        <%  //if (ViewData["Mode"].ToString() != "Print")
                          {%>
                        <th><%= NEXMI.NMCommon.GetInterface("LOWER_LIMIT", langId) %></th>
                        <%--<th>Trạng thái</th>--%>
                        <%} %>
                    </tr>
                </thead>
                <tbody>
                <%
                    NEXMI.NMCloseMonthsWSI WSIs = (NEXMI.NMCloseMonthsWSI)ViewData["WSIs"];
        
                    if (WSIs.PIS.Count > 0)
                    {
                        int count = 1;
                        double totalQuantity = 0;
                        string status = "";
            
                        foreach (var Item in WSIs.PIS)
                        {
                            totalQuantity = Item.BeginQuantity+Item.ImportQuantity-Item.ExportQuantity;
                            status = "";
                            if (totalQuantity > Item.MaxQuantity)
                            {
                                status = "#CC3300";
                            }
                            else if (totalQuantity < Item.MinQuantity)
                            {
                                status = "#0033CC";
                            }
                        %>
                        <tr style='color:<%=status %>'>
                            <td width="3%"><%=count++%></td>
                            <td><%= NEXMI.NMCommon.GetName(Item.StockId, langId) %></td>                
                            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%= NEXMI.NMCommon.GetName(Item.ProductId, langId) %></td>
                            <td><%= NEXMI.NMCommon.GetProductUnitName(Item.ProductId) %></td>
                            <td align="right"><%= Item.BeginQuantity.ToString("N3")%></td>
                            <td align="right"><%= Item.ImportQuantity.ToString("N3")%></td>
                            <td align="right"><%= Item.ExportQuantity.ToString("N3")%></td>
                            <td align="right"><%=totalQuantity.ToString("N3")%></td>
                            <td align="right"><%=(totalQuantity*Item.CostPrice).ToString("N3")%></td>
                            <%
                            //if (ViewData["Mode"].ToString() != "Print")
                              {%>
                            <td align="right"><%=Item.MaxQuantity.ToString("N3")%></td>
                            <td align="right"><%=Item.MinQuantity.ToString("N3")%></td>
                            <%--<td ><%= status  %></td>--%>
                            <%} %>
                        </tr>
                <%
                        }
                        %>
                <%
                    }
                %>
                </tbody>
    
            </table>

        </td>
    </tr>
</table>