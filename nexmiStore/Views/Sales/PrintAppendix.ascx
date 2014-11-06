<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<div>
    <h4 style="text-align:center"> CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM </h4>
    <h5 style="text-align:center"> Độc lập – Tự do – Hạnh phúc</h5>
    <br />
    <h2 style="text-align:center"><b>PHỤ LỤC HỢP ĐỒNG</b></h2>
    <h5 style="text-align:center">(Số:........../2013/PLHD-..........................)</h5>

    <ul>
        <li>Căn cứ theo hợp đồng số: ....../2013/HD-.............. đã ký ngày ........................</li>
        <li>Căn cứ nhu cầu thực tế của hai bên.</li>
    </ul>
    <br />
    <label>Hôm nay, ngày <%=DateTime.Today.Day %> tháng <%=DateTime.Today.Month %>  năm <%=DateTime.Today.Year %>, chúng tôi gồm:</label>
    <br /><br />	
    <b><label>BÊN A (BÊN MUA): <%=ViewData["CustomerName"]%> </label></b>
    <ul>
    <li><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:  <%=ViewData["CustomerAddress"].ToString() + ", " + ViewData["CustomerArea"].ToString()%> </li>
    <li><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>: <%=ViewData["CustomerTelephone"]%> </li>
    <li>Đại diện:  <%=ViewData["CustomerAntt"]%></li>
    <li>Chức vụ:  <%=ViewData["CustomerAnttTitle"] %></li>
    <li><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %>: <%=ViewData["CustomerTaxCode"] %>  </li>
    </ul>
    <br />
    <b>BÊN B (NHÀ CUNG CẤP):  <%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %></b>
    <ul>
        <li><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:        <%=NEXMI.NMCommon.GetCompany().Customer.Address %></li>
        <li><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %>:     <%=NEXMI.NMCommon.GetCompany().Customer.TaxCode %></li>
        <li>Tài khoản số:   <%%></li>
        <li>Đại diện:       <%=(NEXMI.NMCommon.GetCompany().ManagedBy != null)? NEXMI.NMCommon.GetCompany().ManagedBy.CompanyNameInVietnamese : ""%></li>
        <li>Chức vụ:        <%=(NEXMI.NMCommon.GetCompany().ManagedBy != null)? NEXMI.NMCommon.GetCompany().ManagedBy.JobPosition : ""%></li>
        <li><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:     <%=NEXMI.NMCommon.GetCompany().Customer.Telephone %>	</li>
    </ul>
    <br />
    Hai bên nhất trí cùng nhau ký phụ lục hợp đồng số.........................đối với Hợp đồng đã ký số .......................
    <br /><br />
    <b>ĐIỀU 1: NỘI DUNG VÀ GIÁ CẢ</b>
    <ul>
        <li>Bên A đồng ý chọn Bên B làm nhà cung ứng đồng phục cho cán bộ nhân viên.</li>
        <li>Sau đây là Bảng báo giá may size hai bên đã thống nhất:</li>
    </ul>
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
            ArrayList objs = (ArrayList)ViewData["objs"];
            String path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/_thumbs/";                        
            String localpath = "";
            foreach (ArrayList Item in objs)
            {
        %>
        <tr> 
            <%--<td><%=Item[2]%></td>--%>
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
    Tổng giá trị hợp đồng: <%=ViewData["TotalAmount"]%> VNĐ.
    <br />
    Số tiền bằng chữ: <%= NEXMI.MoneyToString.UppercaseFirst(NEXMI.MoneyToString.SetValue(Decimal.Parse(ViewData["TotalAmount"].ToString()))) %>.
    <ul>
    <%  string []desc = Regex.Split(ViewData["Description"].ToString(), ";");
        foreach (string item in desc)
        {
         %>
        <li><%=item%></li>
        <%} %>
    </ul>
    <br />
    <b>ĐIỀU 2: ĐIỀU KHOẢN CHUNG</b>
    <br />
    <ol>
        <li>Quyền và nghĩa vụ của mỗi bên được qui định tại Hợp đồng số ............................</li>
        <li>Phụ lục hợp đồng được lập thành 04 (bốn) bản, mỗi bên giữa 02 (hai) bản có giá trị pháp lý như nhau.</li>
        <li>Phụ lục này là một phần không thể tách rời của Hợp đồng số ............................. và có giá trị kể từ ngày ký.</li>
    </ol>
    <br /><br />
    <table style="width:100%; text-align:center">
        <tr>
            <td style="width:50%; text-align:center">ĐẠI DIỆN BÊN A</td>
            <td style="width:50%; text-align:center">ĐẠI DIỆN BÊN B</td>
        </tr>
    </table>
</div>