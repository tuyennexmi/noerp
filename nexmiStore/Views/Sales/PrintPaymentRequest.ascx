<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<div >
    <div style="text-align: right;">
        <h1>ĐỀ NGHỊ THANH TOÁN</h1>
        <label>Ngày <%=DateTime.Today.Day %> tháng <%=DateTime.Today.Month %>  năm <%=DateTime.Today.Year %>.</label>
    </div>
    
    <table style="width:100%">
        <tr>
            <td width="12%" valign="top">
                <h4 style="text-align:right"><%= NEXMI.NMCommon.GetInterface("TO", langId) %>: </h4>
            </td>
            <td>
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td style="font-weight: bold"><%=ViewData["CustomerAntt"]%></td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold"><%=ViewData["CustomerName"]%></td>
                    </tr>                    
                    <tr>
                        <td><%=ViewData["CustomerAddress"]%></td>
                    </tr>
                    <tr>
                        <td><%=ViewData["CustomerArea"]%></td>
                    </tr>   
                    <tr>
                        <td><%=ViewData["CustomerTelephone"]%></td>
                    </tr>
                    <tr>
                        <td><%= NEXMI.NMCommon.GetInterface("CUSTOMER_ID", langId) %>: <%=ViewData["CustomerId"]%></td>
                    </tr>   
                </table>
            </td>
            <td valign="top">
                <%--<table width="100%" style="text-align: right;" >
                    <tr>
                        <td >Số:</td>
                        <td><%=ViewData["Id"]%></td>
                        </tr>
                    <tr>
                        <td >Ngày báo giá:</td>
                        <td><%=ViewData["OrderDate"]%></td>
                    </tr>
                    <tr>
                        <td ><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %>:</td>
                        <td><%=ViewData["Reference"]%></td>
                        </tr>
                    <%--<tr>
                        <td >Thời hạn báo giá:</td>
                        <td><%=ViewData["ExpirationDate"]%></td>
                    </tr>
                </table>--%>
            </td>
        </tr>
    </table>
    
    <br />
<table border="1" width="100%">
    <tr style="background-color:Gray; font-weight: bold">
        <%--<td style="width:5%">STT</td>--%>
        <td style="width:20%"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
        <td style="width: 20%;"><%= NEXMI.NMCommon.GetInterface("PICTURE", langId) %></td>
        <td style="width: 25%;"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
        <td style="width:10%"><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></td>
        <td style="width:10%"><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></td>
        <%--<td ><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %></td>--%>
        <td style="width:10%"><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></td>
    </tr>
    <% 
        ArrayList objs = (ArrayList)ViewData["objs"];
        String path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/_thumbs/";                        
        String localpath = "";
        int count = 0;
        foreach (ArrayList Item in objs)
        {
            count += 1;
    %>
    <tr> 
        <%--<td><%=count%></td>--%>
        <td><%=Item[3]%></td>
        <td>
        <%  localpath = "~/uploads/_thumbs/" + Item[4];
            localpath = Server.MapPath(@localpath);
            if (System.IO.File.Exists(localpath))
            {%>
            <img alt="" src="<%=path + Item[4]%>" />
            <%} %>
        </td>
        <td ><%=Item[5]%></td>
        <td align="right"><%=Item[6]%></td>
        <td align="right"><%=Item[7]%></td>
        <%--<td><%=Item[8]%></td>--%>
        <td align="right"><%=Item[10]%></td>
    </tr>   
    <% 
        }    
    %>
    <tr>
        <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
        <td align="right" style="width: 120px;"><label id="lbTotalAmount"><%=ViewData["Amount"]%></label></td>
    </tr>

    <tr>
        <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>: </b></td>
        <td align="right"><label id="lbInvoiceAmount"><%=ViewData["TotalAmount"]%></label></td>
    </tr>
</table>
    <p><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %>: bằng chuyển khoản, theo thông tin như sau:</p>
       <ul>
            <li><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %></li>
            <li>Số tài khoản:</li>            
       </ul> 

    <p>Mọi thắc mắc có liên quan, xin vui lòng liên hệ với chúng tôi qua số máy: 08. 381 34 893.</p>
    <p>Trong tình hình kinh tế khó khăn hiện nay, rất mong quý công ty xem xét xác nhận và thanh toán số tiền ở trên cho công ty chúng tôi để vượt qua khó khăn và đảm bảo quyền lợi của quý công ty. Trân trọng cảm ơn sự hợp tác và giúp đỡ của quý công ty.</p>

    <br />            
    <table style="width:100%; text-align:center">
        <tr>
            <td style="width:30%; ">Người yêu cầu:</td>
            <td style="width:30%; ">Kế toán:</td>
            <td style="width:30%; ">Giám đốc:</td>
        </tr>
        
        <tr><td><br /> </td><td></td><td></td>        </tr>
        <tr><td> </td><td></td><td></td>        </tr>
        <tr>
            <td style="width:30%;"><br /><%=ViewData["slUsers"].ToString()%></td>
            <td style="width:30%;"><br /></td>
            <td style="width:30%;"><br /><%=NEXMI.NMCommon.GetCompany().ManagedBy.CompanyNameInVietnamese%></td>
        </tr>
    </table>
    <h4><i>Trân trọng cảm ơn quý khách đã ủng hộ công ty chúng tôi!</i></h4>

    <%Html.RenderPartial("ReportFooter"); %>
</div>