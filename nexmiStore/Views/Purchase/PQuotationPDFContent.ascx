<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMPurchaseOrdersWSI WSI = (NEXMI.NMPurchaseOrdersWSI)ViewData["WSI"];
    string tittle = "";
    if (WSI.Order.OrderTypeId == NEXMI.NMConstant.POType.Quotation)
        tittle = "Yêu cầu báo giá";
    else
        tittle = NEXMI.NMCommon.GetInterface("SO_TITLE", langId);
%>
<div>
    <div style="text-align: right;">
        <h1><%=tittle%></h1>
    </div>
    <table style="width:100%">
        <tr>
            <td width="12%" valign="top">
                <h4 style="text-align:right"><%= NEXMI.NMCommon.GetInterface("TO", langId) %>: </h4>
            </td>
            <td>
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>:</td>--%>
                        <td style="font-weight: bold"><%%></td>
                    </tr>
                    <tr>
                        <%--<td >Công ty:</td>--%>
                        <td style="font-weight: bold"><%=WSI.Supplier.CompanyNameInVietnamese%></td>
                    </tr>                    
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:</td>--%>
                        <td><%=WSI.Supplier.Address%></td>
                    </tr>
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("AREA", langId) %>:</td>--%>
                        <td><%=WSI.Supplier.AreaId%></td>
                    </tr>   
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:</td>--%>
                        <td><%=WSI.Supplier.Telephone%></td>
                    </tr>
                    <tr>
                        <%--<td ><%= NEXMI.NMCommon.GetInterface("CUSTOMER_ID", langId) %>:</td>--%>
                        <td>ID nhà cung cấp: <%=WSI.Supplier.CustomerId%></td>
                    </tr>   
                </table>
            </td>
            <td valign="top">
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td ><%= NEXMI.NMCommon.GetInterface("ID", langId) %>:</td>
                        <td><%=WSI.Order.Id%></td>
                        </tr>
                    <tr>
                        <td >Ngày báo giá:</td>
                        <td><%=WSI.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                    </tr>
                    <tr>
                        <td ><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %>:</td>
                        <td><%=WSI.Order.Reference%></td>
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
    <%--<h4><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></h4>--%>
    <br />
    <table border="1" width="100%"> 
    <tr style="background-color:Gray; font-weight: bold">
        <%--<td >Mã sản phẩm</td>--%>
        <td ><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
        <td style="width: 250px;"><%= NEXMI.NMCommon.GetInterface("PICTURE", langId) %></td>
        <td style="width: 250px;"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
        <td ><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></td>
        <td ><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></td>
        <%--<td ><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %></td>--%>
        <td ><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></td>
    </tr>
    <% 
        String path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/_thumbs/";                        
        String localpath = "";
        foreach (NEXMI.PurchaseOrderDetails Item in WSI.Details)
        {
    %>
    <tr> 
        <%--<td><%=Item[2]%></td>--%>
        <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
        <td>
        <%--<%  localpath = "~/uploads/_thumbs/" + Item[4];
            localpath = Server.MapPath(@localpath);
            if (System.IO.File.Exists(localpath))
            {%>
            <img alt="" src="<%=path + Item[4]%>" />
            <%} %>--%>
        </td>
        <td ><%=Item.Description%></td>
        <td align="right"><%=Item.Quantity%></td>
        <td align="right"><%--<%=Item.Price%>--%></td>
        <td align="right"><%=Item.Amount%></td>
    </tr>   
    <% 
        }    
    %>
    <tr>
        <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
        <td align="right" style="width: 120px;"><label id="lbTotalAmount"><%=WSI.Order.Amount%></label></td>
    </tr>

    <tr>
        <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %>: </b></td>
        <td align="right"><label id="lbInvoiceAmount"><%=WSI.Order.TotalAmount%></label></td>
    </tr>
</table>
    
            
    <table>
        <tr>
            <td width="10%" valign="top"><h4><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>:</h4></td>
            <td>
                <ul>
                    <%--VIKOR<li>Giá trên chưa bao gồm <%= NEXMI.NMCommon.GetInterface("VAT", langId) %> 10%.</li>
                    <li>Giá trên chưa bao gồm in và thêu logo.</li>
                    <li><b>Giá trên đảm bảo nhuộm đúng màu khách hàng yêu cầu.</b></li>
                    <li>Giá trên  bao gồm giao hàng tại địa điểm.</li>
                    <li><%= NEXMI.NMCommon.GetInterface("DELIVERY_DATE", langId) %>: từ 10 ngày - 20 ngày cho một đơn hàng.</li>
                    <li>Đóng gói: mỗi sản phẩm được cắt chỉ thừa sạch sẽ & đóng trong một bao nylon có keo dán, với tên khách hàng, mã số, bộ phận. Tất cả đồng phục sẽ được đóng gói và chuyển đến địa chỉ chỉ định của <%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>.</li>
                    <li>Bảng báo giá trên có giá trị trong vòng 15 ngày.</li>
                    <li><b>Qui trình làm viêc:</b><br />
                        1.	Duyệt chất liệu+ Bảng báo giá <br />
                        2.	May mẫu<br />
                        3.	Ký hợp đồng<br />
                        4.	Thanh toán 50% tạm ứng<br />
                        5.	Sản xuất<br />
                        6.	Giao hàng+Thanh lý hợp đồng+Thanh toán 50%<br />
                    </li>
                    <li>Giá trên đã bao gồm VAT 0%</li>                    --%>
                    <li><%=WSI.Order.Description%></li>
                </ul>
            </td>
        </tr>
    </table>
            
    <table style="width:100%; text-align:center">
        <tr>
            <td style="width:50%; ">Xác nhận của nhà cung cấp:</td>
            <td style="width:50%; "><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %>:</td>
        </tr>
        <tr><td> <br /></td><td></td>        </tr>
        <tr>
            <td style="width:50%;"><br /></td>
            <td style="width:50%;"><br /><%=WSI.CreatedUser.CompanyNameInVietnamese%></td>
        </tr>
    </table>
    <h4><i>Trân trọng cảm ơn quý đối tác đã giúp đỡ công ty chúng tôi!</i></h4>

    <%Html.RenderPartial("ReportFooter"); %>
</div>