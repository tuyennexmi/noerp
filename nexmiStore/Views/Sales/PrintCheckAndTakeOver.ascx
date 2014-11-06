<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

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
                        <td >Số</td>
                        <td><%=SOWSI.Order.OrderId%></td>
                    </tr>
                    <tr>
                        <td >Rev.</td>
                        <td><%=SOWSI.Order.Reference%></td>
                    </tr>
                    <tr>
                        <td >Ngày</td>
                        <td><%=SOWSI.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                    </tr>
                    <tr>
                        <td >Trang</td>
                        <td></td>
                    </tr>   
                </table>
            </td>
        </tr>
    </table>
    
    <h2 style="width: 100%" align="center">SITE DELIVERY NOTE                    </h2>
    <h4 style="width: 100%" align="center">BIÊN BẢN NGHIỆM THU HÀNG HOÁ TẠI CHÂN CÔNG TRƯỜNG</h4>
    
    <table width="100%" border="1">
        <tr>
            <td width="22%" valign="top" style="font-weight: bold">Project/ Công trình:</td>
            <td></td>
        </tr>
        <tr>
            <td style="font-weight: bold" width="22%">Location/ Địa điểm:</td>
            <td></td>
        </tr>
        <tr>
            <td style="font-weight: bold" width="22%">Work/Hạn mục:</td>
            <td></td>
        </tr>
    </table>
    <label style="width: 100%"><b>Inspection document/ Chứng từ kiểm tra</b><//label>
    <table>
        <tr>
            <td>Contract No./ HĐ số:</td>
            <td>CO/ Chứng chỉ XX:</td>
        </tr>
        <tr>
            <td>BOL/ Vận đơn:</td>
            <td>Others/ Khác:</td>
        </tr>
    </table>
    
    <table class="tbDetails" border=".1">
        <thead>
            <tr>
                <td colspan='5' align="center"><label style="width: 100%"><b>Goods to be inspected and hand-over/ Chi tiết hàng hoá kiểm tra và bàn giao</b></label></td>
            </tr>
            <tr style="background-color:Gray; font-weight: bold">
                <th width="5%">Stt<br />No.</th>
                <th width="45%">Products - Sản phẩm</th>
                <th>Maker <br /> Nhà sản xuất</th>
                <th width="8%">ĐVT<br />Unit</th>
                <th width="8%">Số lượng<br />Quantity</th>
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
                <td><%=(product.Manufacture != null)? product.Manufacture.CompanyNameInVietnamese : "" %></td>
                <td><%=product.Unit.Name%></td>
                <td align="right"></td>
            </tr>   
    <% 
        }
    %>      
        </tbody>
    </table>
            
    <table>
        <tr>
            <td>Delivery date/ Ngày giao:..........................................................</td>
            <td>Time/ giờ:............................................................</td>
        </tr>
        <tr>
            <td>Status of goods/ Tình trạng hàng hóa:</td>
            <td>: □  New/ mới and/ và □  Good / Tốt</td>
        </tr>
        <tr>
            <td>Storage location/ Nơi lưu hàng</td>
            <td>: □ Warehouse/ Trong kho. □  Outside/ ngòai trời</td>
        </tr>
    </table>
    <label><b>Inspection parties / Thành phần nghiệm thu</b></label>
    <table>
        <tr>
            <td style="width:50%;">Owner/ Chủ đầu tư:</td>
            <td>Inspector/K. tra bởi:</td>
        </tr>
        <tr>
            <td><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese%></td>
            <td>Inspector/K. tra bởi:</td>
        </tr>
    </table>
    <h4>Result/ Kết qủa:.............................................................................................................................</h4>
    <h4>Note/ Ghi chú:...............................................................................................................................</h4>

    <table width="100%">
        <tr>
            <td align="center">Owner/ Chủ đầu tư:</td>
            <td align="center">Contractor/Thầu Chính:</td>
        </tr>
    </table>
    <br /><br /><br /><br />
    <label><b>Hand-over document/ Chứng từ đã bàn giao</b></label>
    <table>
        <tr>
            <td>□ Packing List/ qui cách đóng gói</td>
            <td>□ Certificated of Original/ Chứng từ xuất xứ</td>
        </tr>
        <tr>
            <td>□ BOL/AWB/ Vận đơn.</td>
            <td>□ Certificate of Quality/ Chứng chỉ chất lượng</td>
        </tr>
        <tr>
            <td>□  Invoice/ Hóa đơn</td>
            <td>□ </td>
        </tr>
    </table>

    <%Html.RenderPartial("ReportFooter"); %>

</div>