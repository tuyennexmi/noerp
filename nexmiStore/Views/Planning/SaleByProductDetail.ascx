<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<table style="width: 100%" class="tbTemplate">    
    <tr>
        <td>
            <div>
                <table style="width: 100%" id="Table1" class="tbDetails">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER_CODE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                            <th>Sản lượng</th>
                            <th>Tỷ lệ</th>
                            <th>Thực hiện</th>
                            <th>% thực hiện</th>
                        </tr>
                    </thead>
                    <tbody id="Tbody1">
                    <%  double plAmount = 0;// double.Parse(ViewData["Amount"].ToString());
                        string product = ViewData["Product"].ToString();
                        double totalAmount = 0;
                        double totalRs = 0;
                        totalRs = 0; totalAmount = 0;
                        if (Session["SalesPLN"] != null)
                        {
                            List<NEXMI.NMCustomersWSI> Customers = ((List<NEXMI.NMCustomersWSI>)ViewData["Partners"]);
                            List<NEXMI.NMMonthlyGeneralJournalsWSI> purchaseRs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["Sales"];
                            List<NEXMI.MasterPlanningDetails> List = (List<NEXMI.MasterPlanningDetails>)Session["SalesPLN"];
                            int count = 0;
                            totalAmount = List.FindAll(p => p.ProductId == product).Sum(i => i.CreditAmount);
                            NEXMI.NMCustomersWSI cus;
                            double rs = 0, rate = 0;
                            foreach (NEXMI.MasterPlanningDetails Item in List)
                            {
                                if (product != "")
                                    if (Item.ProductId != product)
                                        continue;
                                plAmount = List.Where(s => s.ProductId == Item.ProductId).Sum(i => i.CreditAmount);
                                cus = Customers.Where(i => i.Customer.CustomerId == Item.PartnerId).FirstOrDefault();
                                rs = purchaseRs.Where(c => c.MGJ.PartnerId == cus.Customer.CustomerId & c.MGJ.ProductId == Item.ProductId).Sum(s => s.MGJ.ImportQuantity);
                                totalRs += rs;
                    %>
                        <tr id="Tr1">
                            <td><%=++count %></td>
                            <td><%=cus.Customer.Code%></td>
                            <td ><%=cus.Customer.CompanyNameInVietnamese%></td>
                            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                            <td align="right"><%= Item.CreditAmount.ToString("N3")%></td>
                            <td align="right"><%= (Item.CreditAmount / plAmount).ToString("N3")%></td> 
                            <td align="right"><%= rs.ToString("N3")%></td>
                            <td align="right"><%= (rs / Item.CreditAmount).ToString("N3") %></td>
                        </tr>
                    <%
                            }
                        }
                    %>
                        <tr>
                            <td colspan='4'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></b></td>                        
                            <td align="right"><%= totalAmount.ToString("N3") %></td> 
                            <td ></td>
                            <td align="right"><%= totalRs.ToString("N3") %></td> 
                            <td align="right"><%= (totalRs/ totalAmount).ToString("N3") %></td> 
                        </tr>
                    </tbody>
                </table>
            </div>
        </td>
    </tr>
</table>