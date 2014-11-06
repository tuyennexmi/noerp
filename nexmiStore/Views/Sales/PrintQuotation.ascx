<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMSalesOrdersWSI SOWSI = (NEXMI.NMSalesOrdersWSI)ViewData["SOWSI"];
    string lang = ViewData["Lang"].ToString();
    NEXMI.NMCustomersWSI CustomerAntt = (NEXMI.NMCustomersWSI)ViewData["CustomerAntt"];
    List<NEXMI.NMProductsWSI> ProductList = (List < NEXMI.NMProductsWSI > )ViewData["ProductList"];
    
    NEXMI.NMCustomersBL CBL = new NEXMI.NMCustomersBL();
    NEXMI.NMCustomersWSI CWSI = new NEXMI.NMCustomersWSI();
    CWSI.Mode = "SEL_OBJ";
    CWSI.Customer = new NEXMI.Customers();
    CWSI.Customer.CustomerId = SOWSI.Order.CustomerId;
    CWSI = CBL.callSingleBL(CWSI);

    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
%>


<div style="font-size: medium">
    <table style="width:100%">
        <tr>
            <td width="40%">
                <img height="60px" alt="" src="<%=localpath%>" />
            </td>
            <td width="60%">
                <div style="text-align: right;">
                    <h1><%=ViewData["Title"]%></h1>
                </div>
            </td>
        </tr>
    </table>

    <table style="width:100%">
        <tr>
            <td width="12%" valign="top">
                <label style="text-align:right"><%= NEXMI.NMCommon.GetInterface("TO", lang) %>: </label>
            </td>
            <td>
                <table width="100%" style="text-align: right;" >
                <%if (CWSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                { %>
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>:</td>--%>
                        <td style="font-weight: bold"><%=CWSI.ContactPersons.Count > 0? CWSI.ContactPersons[0].CompanyNameInVietnamese : "" %></td>
                    </tr>
                <%} %>
                    <tr>
                        <%--<td >Công ty:</td>--%>
                        <td style="font-weight: bold"><%=SOWSI.Customer.CompanyNameInVietnamese%></td>
                    </tr>                    
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:</td>--%>
                        <td><%=SOWSI.Customer.Address%></td>
                    </tr>
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("AREA", langId) %>:</td>--%>
                        <td><%=CWSI.AreaWSI != null? CWSI.AreaWSI.FullName : ""%></td>
                    </tr>   
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:</td>--%>
                        <td><%=SOWSI.Customer.Telephone%></td>
                    </tr>
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("CUSTOMER_ID", langId) %>:</td>--%>
                        <td><%= NEXMI.NMCommon.GetInterface("CUSTOMER_ID", lang)%> <%=SOWSI.Customer.CustomerId%></td>
                    </tr>   
                </table>
            </td>
            <td valign="top">
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td ><%= NEXMI.NMCommon.GetInterface("ID", lang)%>:</td>
                        <td><%=SOWSI.Order.OrderId%></td>
                        </tr>
                    <tr>
                        <td ><%= NEXMI.NMCommon.GetInterface("DATE", lang)%>:</td>
                        <td><%=SOWSI.Order.OrderDate%></td>
                    </tr>
                    <tr>
                        <td ><%= NEXMI.NMCommon.GetInterface("REF_DOC", lang)%>:</td>
                        <td><%=SOWSI.Order.Reference%></td>
                    </tr>
                    <%--<tr>
                        <td >Thời hạn báo giá:</td>
                        <td><%=ViewData["ExpirationDate"]%></td>
                    </tr>--%>   
                </table>
            </td>
        </tr>
    </table>
    
    <br />
    <table width="100%" border="1">
        <tr>
            <td colspan='5'>
                <p><%= NEXMI.NMCommon.GetInterface("GREETINGS", lang)%></p>
            </td>
        </tr>
    </table>
    
    <br />
    <table class="tbDetails" border=".1">
        <thead>
            <tr style="background-color:Gray; font-weight: bold">
                <%--<th >Mã sản phẩm</th>--%>
                <th width="5%"><%= NEXMI.NMCommon.GetInterface("NO", lang)%></th>
                <th width="45%"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", lang)%></th>
                <%--<th ><%= NEXMI.NMCommon.GetInterface("PICTURE", lang)%></th>
                <th ><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", lang)%></th>--%>
                <th width="8%"><%= NEXMI.NMCommon.GetInterface("UNIT", lang)%></th>
                <th width="8%"><%= NEXMI.NMCommon.GetInterface("QUANTITY", lang)%></th>
                <th ><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", lang)%></th>
                <%--<th ><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %></th>--%>
                <th ><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", lang)%></th>
            </tr>
        </thead>
        <tbody>
    <%  
        String path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/_thumbs/";        
        int count = 1;        
        foreach (NEXMI.SalesOrderDetails Item in SOWSI.Details)
        {
            NEXMI.NMProductsWSI product = ProductList.Find(i => i.Product.ProductId == Item.ProductId);
    %>
            <tr> 
                <%--<td><%=Item[2]%></td>--%>
                <td><%=count++%></td>
                <td>
                    <%=product.Translation.Name%> <br />
                    <%=product.Translation.Description %>
                </td>
                <td><%=product.Unit.Name%></td>
                <td align="right"><%=Item.Quantity.ToString("N3")%></td>
                <td align="right"><%=Item.Price.ToString("N3")%></td>
                <%--<td><%=Item[8]%></td>--%>
                <td align="right"><%=Item.Amount.ToString("N3")%></td>
            </tr>   
    <% 
        }    
    %>
            <tr>
                <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", lang)%>: </b></td>
                <td align="right" style="width: 120px;"><label id="lbTotalAmount"><%=SOWSI.Order.Amount.ToString("N3")%></label></td>
            </tr>
        <%if (SOWSI.Order.Discount > 0)
        { %>
            <tr>
                <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", lang)%>: </b></td>
                <td align="right" style="width: 120px;"><label id="Label1"><%=SOWSI.Order.Discount.ToString("N3")%></label></td>
            </tr>
        <%} %>
            <tr>
                <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT", lang)%>: </b></td>
                <td align="right" style="width: 120px;"><label id="Label2"><%=SOWSI.Order.Tax.ToString("N3")%></label></td>
            </tr>
            <tr>
                <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", lang)%>: </b></td>
                <td align="right"><label id="lbInvoiceAmount"><%=SOWSI.Order.TotalAmount.ToString("N3")%></label></td>
            </tr>
        </tbody>
    </table>
            
    <table>
        <tr>
            <td width="10%" valign="top"><h4><%= NEXMI.NMCommon.GetInterface("NOTE", lang)%>:</h4></td>
            <td align="left">
                <%= SOWSI.Order.Description%>
                <%= SOWSI.Order.PaymentTerm%>
            </td>
        </tr>
    </table>
            
    <table style="width:100%; text-align:center" align="center">
        <tr align="center">
            <td style="width:50%; "><%= NEXMI.NMCommon.GetInterface("CONFIRM", lang)%>:</td>
            <td style="width:50%; "><%= NEXMI.NMCommon.GetInterface("CREATED_BY", lang)%>:</td>
        </tr>
        <tr><td> <br /></td><td></td>        </tr>
        <tr align="center">
            <td style="width:50%;"></td>
            <td style="width:50%;"><%=SOWSI.CreatedBy.CompanyNameInVietnamese%></td>
        </tr>
    </table>
    <h4><i><%= NEXMI.NMCommon.GetInterface("THANKS", lang)%></i></h4>

    <%Html.RenderPartial("ReportFooter"); %>
</div>