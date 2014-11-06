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

    String path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/_thumbs/";
    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/logo_quote.png";

    if(NEXMI.NMCommon.GetSetting("USE_MULTI_LANG_QUOTE") == false)
    {
        %>

        <div style="font-size: medium">
            <table style="width:100%">
                <tr>
                    <td width="80%">
                        <img width="440px" alt="" src="<%=localpath%>" />
                    </td>
                    <td width="20%">
                        <table border="1" style="font-size: 6px;">
                            <tr>
                                <td width="40%" ><%= NEXMI.NMCommon.GetInterface("FORM", lang)%>:</td>
                                <td >ICT - TT - 07/06</td>
                            </tr>
                            <tr>
                                <td ><%= NEXMI.NMCommon.GetInterface("QT_NO", lang)%>:</td>
                                <td><%=SOWSI.Order.OrderId%></td>
                            </tr>
                            <tr>
                                <td ><%= NEXMI.NMCommon.GetInterface("REVERSION", lang)%>:</td>
                                <td><%=SOWSI.Order.Reference%></td>
                            </tr>
                            <tr>
                                <td ><%= NEXMI.NMCommon.GetInterface("DATE", lang)%>:</td>
                                <td><%=SOWSI.Order.OrderDate.ToShortDateString()%></td>
                            </tr>
                            <tr>
                                <td ><%= NEXMI.NMCommon.GetInterface("PAGE", lang)%>:</td>
                                <td></td>
                            </tr>   
                        </table>
                    </td>
                </tr>
            </table>
    
            <h2 style="width: 100%" align="center"><%=NEXMI.NMCommon.GetInterface("BUILD_QUOTE", lang)%></h2>
    
            <table width="100%" border="1">
                <tr>
                    <td width="15%" valign="top">
                        <label style="text-align:right"><%= NEXMI.NMCommon.GetInterface("TO", lang) %>: </label>
                    </td>
                <%if (CWSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                  { %>
                    <td width="50%"  style="font-weight: bold"><%=CWSI.ContactPersons.Count > 0 ? CWSI.ContactPersons[0].CompanyNameInVietnamese : ""%></td>
                <%}
                  else
                  { %>
                    <td width="50%" ></td>
                <%} %>
                    <td width="15%"><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:</td>
                    <td><%=SOWSI.Customer.Telephone%></td>
                </tr>
                <tr>
                    <td width="15%"><%= NEXMI.NMCommon.GetInterface("COMPANY", lang)%>:</td>
                    <td style="font-weight: bold"><%=SOWSI.Customer.CompanyNameInVietnamese%></td>
                    <td width="15%">Fax:</td>
                    <td width="20%"><%=SOWSI.Customer.FaxNumber%></td>
                </tr>
            </table>

            <table>
                <tr>
                    <td>
                        <%= NEXMI.NMCommon.GetInterface("GREETINGS", lang)%>
                    </td>
                </tr>
            </table>
    
            <table class="tbDetails" border=".1">
                <thead>
                    <tr style="background-color:Gray; font-weight: bold">
                        <th width="5%"><%= NEXMI.NMCommon.GetInterface("NO", lang)%></th>
                        <th width="45%"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", lang)%></th>
                        <th width="8%"><%= NEXMI.NMCommon.GetInterface("UNIT", lang)%></th>
                        <th width="8%"><%= NEXMI.NMCommon.GetInterface("QUANTITY", lang)%></th>
                        <th ><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", lang)%></th>
                        <th ><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", lang)%></th>
                    </tr>
                </thead>
                <tbody>
            <%  
                int count = 1;        
                foreach (NEXMI.SalesOrderDetails Item in SOWSI.Details)
                {
                    NEXMI.NMProductsWSI product = ProductList.Find(i => i.Product.ProductId == Item.ProductId);
            %>
                    <tr>
                        <td><%=count++%></td>
                        <td>
                            <%=product.Translation.Name%> <br />
                            <%=Item.Description %>
                        </td>
                        <td><%=product.Unit.Name%></td>
                        <td align="right"><%=Item.Quantity.ToString("N3")%></td>
                        <td align="right"><%=Item.Price.ToString("N3")%></td>
                        <td align="right"><%=Item.Amount.ToString("N3")%></td>
                    </tr>   
            <% 
                }
            %>
                    <tr>
                        <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", lang)%>: </b></td>
                        <td align="right"><label id="lbInvoiceAmount"><%=SOWSI.Order.Amount.ToString("N3")%></label></td>
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
            <h4><i><%= NEXMI.NMCommon.GetInterface("THANKS", lang)%></i></h4>        
            <table style="width:100%; text-align:center" align="center">
                <tr align="center">
                    <td style="width:50%;"><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese%></td>
                    <td style="width:50%;"></td>
                </tr>
            </table>

        </div>

<%  }
    else 
    { 
        %>

        <div style="font-size: medium">
            <table style="width:100%">
                <tr>
                    <td width="80%">
                        <img width="440px" alt="" src="<%=localpath%>" />
                    </td>
                    <td width="20%">
                        <table border="1" style="font-size: 6px;">
                            <tr>
                                <td width="40%" >BM</td>
                                <td >ICT - TT - 07/06</td>
                            </tr>
                            <tr>
                                <td >Số BG</td>
                                <td><%=SOWSI.Order.OrderId%></td>
                            </tr>
                            <tr>
                                <td >Số đời</td>
                                <td><%=SOWSI.Order.Reference%></td>
                            </tr>
                            <tr>
                                <td >Ngày</td>
                                <td><%=SOWSI.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                            </tr>
                            <tr>
                                <td >Số trang</td>
                                <td></td>
                            </tr>   
                        </table>
                    </td>
                </tr>
            </table>
    
            <h2 style="width: 100%" align="center">BẢNG CHÀO GIÁ - QUOTATION</h2>
    
            <table width="100%" border="1">
                <tr>
                    <td width="26%" valign="top">
                        <label style="text-align:right">Kính gửi/ To:</label>
                    </td>
                <%if (CWSI.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                  { %>
                    <td style="font-weight: bold"><%=CWSI.ContactPersons.Count > 0 ? CWSI.ContactPersons[0].CompanyNameInVietnamese : ""%></td>
                <%}
                  else
                  { %>
                    <td ></td>
                <%} %>
                    <td width="10%">Tel:</td>
                    <td width="20%"><%=SOWSI.Customer.Telephone%></td>
                </tr>
                <tr>
                    <td width="26%">Công ty/ Company:</td>
                    <td style="font-weight: bold"><%=SOWSI.Customer.CompanyNameInVietnamese%></td>
                    <td width="10%">Fax:</td>
                    <td width="20%"><%=SOWSI.Customer.FaxNumber%></td>
                </tr>
            </table>

            <table>
                <tr>
                    <td>
                        Cảm ơn Qúy khách hàng đã quan tâm đến sản phẩm và dịch vụ của chúng tôi. IC&T luôn tạo mối quan hệ lâu dài với Qúy khách hàng. Chúng tôi kính gửi đến Qúy khách hàng bảng chào giá như sau: <br />
                        Thanks for your interesting in our products and services. IC&T is always looking forward to long-term business relationship. We would like to offer you a best quotation as bellow.
                    </td>
                </tr>
            </table>
    
            <table class="tbDetails" border=".1">
                <thead>
                    <tr style="background-color:Gray; font-weight: bold">
                        <th width="5%">STT<br />NO</th>
                        <th width="45%">PRODUCTS - SẢN PHẨM</th>
                        <th width="8%">ĐVT<br />Unit</th>
                        <th width="8%">Số lượng<br />Quantity</th>
                        <th >Đơn giá (…)<br />Unit price (…)</th>
                        <th >Thành tiền (…)<br />Amount (…)</th>
                    </tr>
                </thead>
                <tbody>
            <%  
                int count = 1;        
                foreach (NEXMI.SalesOrderDetails Item in SOWSI.Details)
                {
                    NEXMI.NMProductsWSI product = ProductList.Find(i => i.Product.ProductId == Item.ProductId);
            %>
                    <tr>
                        <td><%=count++%></td>
                        <td>
                            <%=product.Translation.Name%> <br />
                            <%=Item.Description %>
                        </td>
                        <td><%=product.Unit.Name%></td>
                        <td align="right"><%=Item.Quantity.ToString("N3")%></td>
                        <td align="right"><%=Item.Price.ToString("N3")%></td>
                        <td align="right"><%=Item.Amount.ToString("N3")%></td>
                    </tr>   
            <% 
                }
            %>
                    <tr>
                        <td colspan="5" align="right"><b>TỔNG CỘNG/SUMMARY </b></td>
                        <td align="right"><label id="Label1"><%=SOWSI.Order.Amount.ToString("N3")%></label></td>
                        <td colspan="5" align="right"><b>THUẾ VAT/VAT TAX </b></td>
                        <td align="right"><label id="Label2"><%=SOWSI.Order.Tax.ToString("N3")%></label></td>
                        <td colspan="5" align="right"><b>TỔNG THÀNH TIỀN/TOTAL AMOUNT (…) </b></td>
                        <td align="right"><label id="Label3"><%=SOWSI.Order.TotalAmount.ToString("N3")%></label></td>
                    </tr>
                </tbody>
            </table>
            
            <table>
                <tr>
                    <td width="20%" valign="top"><h4>Ghi chú/ Remark:</h4></td>
                    <td align="left">
                        <%= SOWSI.Order.Description%>
                        <%= SOWSI.Order.PaymentTerm%>
                    </td>
                </tr>
            </table>
            <h4><i>Chân thành cảm ơn và mong nhận được phản hồi từ Qúy khách hàng.<br />Hope to received your information.</i></h4>
            <table style="width:100%; text-align:center" align="center">
                <tr align="center">
                    <td style="width:50%;"><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese%></td>
                    <td style="width:50%;"></td>
                </tr>
            </table>

        </div>

<%  } %>