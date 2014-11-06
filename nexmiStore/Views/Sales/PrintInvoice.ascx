<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMSalesInvoicesWSI WSI = (NEXMI.NMSalesInvoicesWSI)ViewData["WSI"];
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
                    <h1>HÓA ĐƠN BÁN HÀNG</h1>
                </div>
            </td>
        </tr>
    </table>
    
    <table style="width:100%">
        <tr>
            <td width="60%">
                <table class="tbDetails">
                    <tr>
                        <td width="20%"><%= NEXMI.NMCommon.GetInterface("BUYER", langId) %>:</td>
                        <td style="font-weight: bold"><%=ViewData["CustomerAntt"]%></td>
                    </tr>
                    <tr>
                        <td width="20%">Đơn vị:</td>
                        <td style="font-weight: bold"><%=WSI.Customer.CompanyNameInVietnamese%></td>
                    </tr>                    
                    <tr>
                        <td width="20%"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:</td>
                        <td><%=WSI.Customer.Address + ", " + ViewData["CustomerArea"]%></td>
                    </tr>
                    <tr>
                        <td width="20%"><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:</td>
                        <td><%=WSI.Customer.Telephone%></td>
                    </tr>
                    <tr>
                        <td width="20%"><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %>:</td>
                        <td><%=WSI.Customer.TaxCode%></td>
                    </tr>   
                </table>
            </td>
            <td valign="top">
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td >Số:</td>
                        <td><%=WSI.Invoice.InvoiceId%></td>
                    </tr>
                    <tr>
                        <td ><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %>:</td>
                        <td><%=WSI.Invoice.InvoiceDate.ToString("dd-MM-yyyy")%></td>
                    </tr>
                    <tr>
                        <td >Đơn hàng/Hợp đồng:</td>
                        <td><%=WSI.Invoice.Reference%></td>
                    </tr>   
                </table>
            </td>
        </tr>
    </table>
    
    <br />
    <table width="100%" border=".1">
        <tr style="font-weight:bold; background-color:Gray; vertical-align:top;">
            <th ><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></th>
            <%--<th>Vị trí</th>--%>
            <th><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("INCOTERMS", langId) %></th>
            <%--<th><%= NEXMI.NMCommon.GetInterface("DELIVERY_DATE", langId) %></th>--%>
            <%--<th><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></th>--%>
            <%--<th><%= NEXMI.NMCommon.GetInterface("PAYMENT_TIME", langId) %></th>--%>
        </tr>
        <tr>
            <td><%=WSI.SalesPerson.CompanyNameInVietnamese%></td>
            <%--<td><%=ViewData["PaymentTerm"]%></td>--%>
            <td><%=NEXMI.NMCommon.GetParameterName(WSI.Invoice.PaymentMethod)%></td>
            <td></td>
            <%--<td><%=ViewData["PaymentMethod"]%></td>--%>
            <%--<td><%=ViewData["PaymentTerm"]%></td>--%>
        </tr>
    </table>
    
    <br />
    <table class="tbDetails" border=".1">
        <thead>
            <tr style="background-color:Gray; font-weight: bold">
                <th width="5%">STT</th>
                <th width="45%"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                <th width="6%"><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                <th width="9%"><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                <th ><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                <th ><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
            </tr>
        </thead>
        <tbody>
    <%  int count = 1;
        double price = 0, amount = 0, subTotal = 0 ;
        foreach (NEXMI.SalesInvoiceDetails Item in WSI.Details)
        {
            price = Item.Price * (1 - Item.Discount / 100);
            amount = Item.Quantity * price;
            subTotal += amount;
    %>
            <tr>
                <td><%=count++%></td>
                <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
                <td align="right"><%=Item.Quantity.ToString("N3")%></td>
                <td align="right"><%=price.ToString("N3")%></td>
                <td align="right"><%=amount.ToString("N3")%></td>
            </tr>   
            <% 
                }    
            %>
            <tr>
                <td colspan="5"><b>Cộng tiền bán hàng hóa/dịch vụ: </b></td>
                <td align="right" style="width: 120px;"><label id="lbTotalAmount"><%=subTotal.ToString("N3")%></label></td>
            </tr>
            <tr>
                <td colspan="5"><b><%= NEXMI.NMCommon.GetInterface("VAT", langId) %>: </b></td>
                <td align="right" style="width: 120px;"><label id="Label2"><%=WSI.Invoice.Tax.ToString("N3")%></label></td>
            </tr>
            <tr>
                <td colspan="5"><b>Tổng cộng tiền thanh toán: </b></td>
                <td align="right"><label id="lbInvoiceAmount"><%=WSI.Invoice.TotalAmount.ToString("N3")%></label></td>
            </tr>
            <tr>
                <td colspan="6"><b>Số tiền viết bằng chữ: </b> <label><%=NEXMI.NMCommon.ReadNum( WSI.Invoice.TotalAmount)%>.</label> </td>
            </tr>
        </tbody>
    </table>
    
    <table style="width:100%; ">
        <tr>
            <td style="width:50%; " align="center">Người mua hàng <br />(ký, ghi rõ họ tên)</td>
            <td style="width:50%; " align="center"><%= NEXMI.NMCommon.GetInterface("SALER", langId) %> <br />(ký, đóng dấu, ghi rõ họ tên)</td>
        </tr>
        <tr>
            <td> <br /><br /></td>
            <td> </td>        
        </tr>
        <tr>
            <td style="width:50%;" align="center"><%=ViewData["CustomerAntt"] %></td>
            <td style="width:50%;" align="center"><%=WSI.CreatedBy.CompanyNameInVietnamese%></td>
        </tr>
    </table>
    <h4><i>Trân trọng cảm ơn quý khách đã ủng hộ công ty chúng tôi!</i></h4>

    <%Html.RenderPartial("ReportFooter"); %>
</div>