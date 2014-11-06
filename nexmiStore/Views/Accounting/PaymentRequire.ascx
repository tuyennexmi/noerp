<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMCustomersWSI Customer = (NEXMI.NMCustomersWSI)ViewData["Customers"];
    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["MGJs"];

    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;

 %>
<div >
    <table style="width:100%">
        <tr>
            <td width="40%">
                <img height="60px" alt="" src="<%=localpath%>" />
            </td>
            <td width="60%">
                <div style="text-align: right;">
                    <h3>THÔNG BÁO XÁC NHẬN SỐ DƯ NỢ</h3>
                    <label><%= NEXMI.NMCommon.GetInterface("DATE", langId) %><%=DateTime.Today.Day %> <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> <%=DateTime.Today.Month %>  năm <%=DateTime.Today.Year %>.</label>
                </div>
            </td>
        </tr>
    </table>
        
    <table style="width:100%">
        <tr>
            <td width="12%" valign="top">
                <h4 style="text-align:right"><%= NEXMI.NMCommon.GetInterface("TO", langId)%>: </h4>
            </td>
            <td>
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td style="font-weight: bold"><%=Customer.Customer.CompanyNameInVietnamese%></td>
                    </tr>                    
                    <tr>
                        <td><%=Customer.Customer.Address%></td>
                    </tr>
                    <tr>
                        <td><%=Customer.AreaWSI.FullName%></td>
                    </tr>   
                    <tr>
                        <td><%=Customer.Customer.Telephone%></td>
                    </tr>
                    <tr>
                        <td><%= NEXMI.NMCommon.GetInterface("CUSTOMER_ID", langId) %>: <%=Customer.Customer.CustomerId%></td>
                    </tr>   
                </table>
            </td>
            <td valign="top">                
            </td>
        </tr>
    </table>
    
    <br />
    <p>Xin trân trọng thông báo đến quý khách hàng tình hình phát sinh và số dư nợ của quý khách hàng đến ngày <%=DateTime.Today.ToString("dd-MM-yyyy") %>.</p>
    <br />
<table class="tbDetails" border=".1">
    <thead>
        <tr>          
            <th rowspan='2' align="center">NỢ ĐẦU KỲ</th>
            <th rowspan='2' align="center">PHÁT SINH TĂNG</th>
            <th colspan='2' align="center">PHÁT SINH GIẢM</th>            
            <th rowspan='2' align="center">NỢ CUỐI KỲ</th>
        </tr>
        <tr>
            <th align="center">TRỪ CHIẾT KHẤU</th>
            <th align="center">ĐÃ TRẢ</th>            
        </tr>
    </thead>
    <tbody>
        <%  double totalCredit = 0, totalDebit = 0, begin = 0;
            if (WSIs.Count > 0)
            {   
                begin = WSIs.Where(b => b.MGJ.IsBegin == true).Sum(b => b.MGJ.DebitAmount);
                totalCredit = WSIs.Sum(a => a.MGJ.CreditAmount);
                totalDebit += WSIs.Where(c => c.MGJ.IsBegin == false).Sum(a => a.MGJ.DebitAmount);
        %>
        <tr>            
            <td width="18%" align="right"><%=begin.ToString("N3")%></td>
            <td width="30%" align="right"><%=totalDebit.ToString("N3")%></td>
            <td></td>
            <td align="right"><%=totalCredit.ToString("N3")%></td>
            <td align="right"><%=(begin + totalDebit - totalCredit).ToString("N3")%></td>
        </tr>
        <%} %>
    </tbody>
</table>
    <p>Số tiền còn nợ đến hết ngày <%=DateTime.Today.ToString("dd-MM-yyyy") %> là: <%=(begin + totalDebit - totalCredit).ToString("N3")%> đ.</p>
    <p>Số tiền bằng chữ: <%=NEXMI.NMCommon.ReadNum(begin + totalDebit - totalCredit)%>.</p>
    <p><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %>: bằng chuyển khoản, theo thông tin như sau:</p>
       <ul>
            <li><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %></li>
            <li>Số tài khoản: <% %></li>
       </ul> 

    <p>Mọi thắc mắc có liên quan, xin vui lòng liên hệ với chúng tôi qua số máy: <%=NEXMI.NMCommon.GetCompany().Customer.Telephone %>.</p>
    <p>Lưu ý: Nhận được giấy báo, rất mong quý khách hàng xem xét ký xác nhận, gửi lại cho chúng tôi để tiện việc theo dõi đồng thời thanh toán số tiền ở trên cho công ty chúng tôi để đảm bảo quyền lợi của quý công ty. Nếu có sai lệch xin quý khách hàng đối chiếu với kế toán trong vòng 3 ngày. Trân trọng cảm ơn sự hợp tác và 
        hỗ trợ của quý khách hàng.</p>

    <br />
    <table style="width:100%; text-align:center">
        <tr>
            <td align="center"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %>.... <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> .... năm <%=DateTime.Today.Year %></td>
            <td align="center"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %>.... <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> .... năm <%=DateTime.Today.Year %></td>
        </tr>
        <tr>            
            <td style="width:30%; " align="center">KHÁCH HÀNG KÝ NHẬN</td>
            <td style="width:30%; " align="center">GIÁM ĐỐC</td>
        </tr>
        
        <tr><td><br /> </td><td></td>
        </tr>
        <tr><td> </td><td></td>
        </tr>
        <tr>            
            <td style="width:30%;"><br /></td>
            <td style="width:30%;"><br /></td>
        </tr>
    </table>
    <h4><i>Trân trọng cảm ơn quý khách đã ủng hộ công ty chúng tôi!</i></h4>

    <%Html.RenderPartial("ReportFooter"); %>
</div>