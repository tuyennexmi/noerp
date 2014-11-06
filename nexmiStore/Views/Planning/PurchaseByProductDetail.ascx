<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<table style="width: 100%" class="tbTemplate">    
    <tr>
        <td>
            <div>
                <table style="width: 100%" id="tbUpdateStock" class="tbDetails">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_CODE", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></th>
                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                            <th>Sản lượng</th>
                            <th>Thực hiện</th>
                            <th>%</th>
                        </tr>
                    </thead>
                    <tbody id="rows">
                    <%  string product = ViewData["Product"].ToString();
                        double totalAmount = 0;
                        double totalRs = 0;
                        if (Session["PurchsesPLN"] != null)
                        {
                            List<NEXMI.NMCustomersWSI> Suppliers = ((List<NEXMI.NMCustomersWSI>)ViewData["Partners"]);
                            List<NEXMI.NMMonthlyGeneralJournalsWSI> salesRs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["Purchases"];
                            List<NEXMI.MasterPlanningDetails> List = (List<NEXMI.MasterPlanningDetails>)Session["PurchsesPLN"];
                            int count = 0;
                            NEXMI.NMCustomersWSI cus;
                            totalAmount = List.FindAll(p=>p.ProductId == product).Sum(i=>i.DebitAmount);
                            double rs =0, rate =0;
                            foreach (NEXMI.MasterPlanningDetails Item in List)
                            {
                                if (product != "")
                                    if (Item.ProductId != product)
                                        continue;
                                cus = Suppliers.Where(i => i.Customer.CustomerId == Item.PartnerId).FirstOrDefault();
                                rs = salesRs.Where(c=>c.MGJ.PartnerId == cus.Customer.CustomerId & c.MGJ.ProductId == Item.ProductId).Sum(s=>s.MGJ.ImportQuantity);
                                totalRs += rs;
                    %>
                        <tr id="<%=cus.Customer.CustomerId%>">
                            <td><%=++count %></td>
                            <td><%=cus.Customer.Code%></td>
                            <td ><div > <%=cus.Customer.CompanyNameInVietnamese%></div></td>
                            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                            <td align="right"><%= Item.DebitAmount.ToString("N3")%></td>
                            <td align="right"><%= rs.ToString("N3")%></td>
                            <td align="right"><%= (rs / Item.DebitAmount).ToString("N3") %></td>
                        </tr>
                    <%
                            }
                        }
                    %>
                        <tr>
                            <td colspan='4'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></b></td>                        
                            <td align="right"><%= totalAmount.ToString("N3") %></td> 
                            <td align="right"><%= totalRs.ToString("N3") %></td> 
                            <td align="right"><%= (totalRs/ totalAmount).ToString("N3") %></td> 
                        </tr>
                    </tbody>
                </table>
            </div>
        </td>
    </tr>
</table>