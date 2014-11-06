<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMSalesOrdersWSI SOWSI = (NEXMI.NMSalesOrdersWSI)ViewData["SOWSI"];
    string lang = ViewData["Lang"].ToString();
    NEXMI.NMCustomersWSI CustomerAntt = (NEXMI.NMCustomersWSI)ViewData["CustomerAntt"];
    List<NEXMI.NMProductsWSI> ProductList = (List<NEXMI.NMProductsWSI>)ViewData["ProductList"];
    
    String path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/_thumbs/";
    String localpath = "";
if (ViewData["Lang"].ToString() == "vi" | Session["Lang"].ToString() == "en")
{
%>
    <div id='DAIA'>

    <h2 style="text-align:center"><b>HỢP ĐỒNG THUÊ MUA MÁY LỌC NƯỚC</b></h2>

    <h4 align="right">Ngày: <%=DateTime.Today.Day %> <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> <%=DateTime.Today.Month %>  năm <%=DateTime.Today.Year %></h4>
    <h4 align="right">Số: <%=SOWSI.Order.OrderId %></h4>

    <b>BÊN A:  <%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %> (Bên cho thuê)</b>
    <ul>
        <li><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %>:     <%=NEXMI.NMCommon.GetCompany().Customer.TaxCode %></li>
        <li><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:        <%=NEXMI.NMCommon.GetCompany().Customer.Address %></li>
        <li><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:     <%=NEXMI.NMCommon.GetCompany().Customer.Telephone %>	</li>
        <li>Tài khoản số:   <% %></li>
    </ul>

    <b>BÊN B: <%=ViewData["CustomerName"]%> (Bên thuê máy)</b>
    <ul>
        <li><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %>: <%=SOWSI.Customer.TaxCode %>  </li>
        <li><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:  <%=SOWSI.Customer.Address %> </li>
        <li><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>: <%=SOWSI.Customer.Telephone %> </li>
        <li>Đại diện:  <%=ViewData["CustomerAntt"]%></li>
    </ul>
    <br />

    <table border="1" width="100%" style="font-size: medium"> 
        <tr style="background-color:Gray; font-weight: bold">
            <td ><%= NEXMI.NMCommon.GetInterface("PRODUCT_CODE", langId) %></td>
            <td ><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></td>
            <td >Phí thuê (bảo dưỡng)/<%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> (VNĐ)</td>
            <td >Phí lắp đặt</td>
            <td ><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></td>
        </tr>
    <%  double count = 0;
        foreach (var Item in SOWSI.Details)
        {
            count += Item.Quantity;        
    %>
        <tr>
            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
            <td align="right"><%=Item.Quantity %></td>
            <td align="right"><%=Item.Price %></td>
            <td align="right"></td>
            <td align="right"><%=Item.TotalAmount %></td>
        </tr>   
    <% 
        }    
    %>
        <tr>
            <td align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
            <td align="right" style="width: 120px;"><label id="Label3"><%=count %></label></td>
            <td colspan='2'></td>
            <td align="right" style="width: 120px;"><label id="Label1"><%=ViewData["Amount"]%></label></td>
        </tr>
    </table>

    <br />
    <table border="1" width="100%" style="font-size: medium"> 
    <tr>
        <td width="10%">Điều<br /> kiện <br />hợp <br />đồng:</td>
        <td width="30%">
            <table border='0'>
                <tr>
                    <td>Số lần tham gia:</td>
                    <td>48 	lần</td>
                </tr>
                <tr>
                    <td>Số lần phải sử dụng:</td>
                    <td>48 	lần	</td>
                </tr>
                <tr>
                    <td>Ngày ký hợp đồng:</td>
                    <td></td>
                </tr>
            </table>
        </td>
        <td width="10%">Điều<br /> khoản <br />đặc <br />biệt:</td>
        <td>
            1.	 Phí trên không bao gồm VAT. Tỷ giá ngoại tệ Ngân Hàng CP Ngoại Thương Việt Nam- Vietcombank TP.HCM tại thời điểm lập hóa đơn. Số tiền này có thể chênh  lệch +-5% nếu tỷ giá ngân hàng thay đổi.
            <br />    2.	Thời hạn HĐ là 48 tháng. Sau khi hết thời hạn HĐ này qúy khách được sở hữu máy lọc nước đã thuê của công ty chúng tôi.
            <br />            3.	Phí thuê hàng tháng phải được thanh toán sau 15 ngày kể từ ngày xuất hóa đơn.
            <br />      4.	Khi hết thời hạn HĐ, công ty chúng tôi sẽ trả lại tiền đặt cọc (nếu có).
            <br />5.	Nếu quý khách hủy HĐ trước 24 tháng thì công ty chúng tôi sẽ không trả lại tiền đặt cọc (nếu có) và chịu hoàn toàn chi phí vận chuyển.
        
        </td>
    </tr>
    </table>
    <br />
    <table border="1" width="100%" style="font-size: medium">
    <tr>
        <td>Điều kiện hợp đồng:</td>
        <td>Chu kỳ thay lọc</td>
    </tr>
    <tr >
        <td width="70%">
            1.	Hàng tháng công ty cung cấp máy lọc nước có nghĩa vụ cho nhân viên đến kiểm tra, vệ sinh máy 01 lần/ tháng.
            <br />2.	Khi có hư hỏng trong vòng 02 ngày phải cho người kiểm tra và khắc phục sớm.
            <br />3.	Thay lọc miễn phí suốt thời gian thuê, theo định kỳ và tuổi thọ của từng lõi lọc.
            <br />4.	Bên cho thuê sẽ duy trì nguồn nước uống theo tiêu chuẩn nước uống của Pasteur Việt Nam.
            <br />5.	Sau 48 tháng nếu Bên Thuê Máy có nhu cầu bảo dưỡng, Bên cho thuê sẽ tiếp tục ký hợp đồng bảo dưỡng máy.
        </td>
        <td width="30%">
            	Chu kỳ thay lõi lọc máy lọc nước:
            <br />	SED: 04 tháng
            <br />	PRE: 08 tháng
            <br />	UF: 12 tháng
            <br />	POST: 12 tháng
        </td>
    </tr>
    </table>
    <br />
    <table style="width:100%; text-align:center">
        <tr>
            <td style="width:50%; text-align:center">ĐẠI DIỆN BÊN A</td>
            <td style="width:50%; text-align:center">ĐẠI DIỆN BÊN B</td>
        </tr>
    </table>
    <br /><br /><br />
    <h3 align="center" style="text-decoration: underline">ĐIỀU KHOẢN HỢP ĐỒNG</h3>
    <br />
    <table style="font-size: small">
    <tr>
        <td>
            Điều 1: Mục đích Hợp đồng
            <br />Là ghi những điều khoản quy định về trách  nhiệm, quyền hạn và nghĩa vụ của bên cho thuê (gọi là Bên A) và bên thuê (gọi là Bên B) trong hợp đồng cho thuê máy lọc nước.
            <br />Điều 2: Hiệu lực hợp đồng
            <br />Khi Bên B ghi đủ thông tin trong bảng hợp đồng mà Bên A giao và Bên B ký tên hoặc đóng dấu thì có phát sinh hiệu lực hợp đồng.
            <br />Điều 3: Thời hạn hợp đồng cho thuê
            <br />①	Từ ngày Bên A giao và lắp ráp máy lọc nước cho Bên B, được tính là ngày bắt đầu thời hạn hợp đồng cho thuê.
            <br />②	Tổng thời gian hợp đồng cho thuê là 48 tháng. Quý khách sẽ được sở hữu máy lọc nước đã thuê của Bên A sau 04 năm từ ngày ký hợp đồng. Nhưng nếu Bên B có nợ với Bên A thì sau khi trả hết toàn bộ nợ cho Bên A mới được sở hữu máy lọc nước đã thuê của Bên A và sẽ nhận lại được tiền đặt cọc.
            <br />③	Bên B phải sử dụng trong 48 tháng, trước 48 tháng không được hủy hợp đồng trừ khi có lý do hợp lý.
            <br />Điều 4: Lắp ráp và giao hàng.
            <br />①	Trong vòng 10 ngày kể từ ngày hợp đồng có hiệu lực, Bên A lắp ráp nơi Bên B yêu cầu để Bên B sử dụng được bình thường.
            <br />②	Sau khi hết hợp đồng cho thuê, chi phí thu lại máy thì Bên A chịu (Trừ chuyển quyền sở hữu). Nhưng nếu do Bên B hủy hợp đồng đồng trước khi hết hợp đồng thì Bên B chịu toàn bộ chi phí thu lại máy lọc nước.
            <br />Điều 5: Thanh toán tiền thuê
            <br />Phí thuê hàng tháng phải được thanh toán sau 15 ngày kể từ ngày xuất hóa đơn.
            <br />Điều 6: Thanh toán tiền thuê trả chậm
            <br />Bên A có quyền yêu cầu lãi suất 02% hàng tháng với Bên B khi Bên B không thanh toán theo hợp đồng cho thuê này.
            <br />Điều 7: Hủy hợp đồng và quyền trả lại
            <br />①	Bên B có thể trả lại trong vòng 07 ngày kể từ ngày lắp ráp bằng văn bản. Nhưng nếu lỗi do Bên B làm mất mát hoặc hư hỏng thì Bên B không được quyền trả lại hoặc hủy hợp đồng này.
            <br />②	Bên A phải trả lại tiền thuê đã nhận được với Bên B trong vòng 03 ngày làm việc kể từ ngày thu lại máy lọc nước. Nhưng Đặt điểm của sản phẩm cho thuê là sau khi lắp ráp thì trở thành sản phẩm cũ do bình lọc tiêu dụng, không được bán sản phẩm mới nên Bên A có quyền không trả lại tiền đặt cọc và chi phí lắp ráp Bên B đã nộp.
            <br />③	Những điều khác về hiệu lực, phương thức, điều kiện của quyền trả lại thì sẽ theo quyết định của pháp luật liên quan.
            <br />Điều 8: Mất quyền lợi ích của thời hạn HĐ
            <br />①	Khi Bên B không nộp tiền thuê theo hợp đồng này sẽ mất quyền lợi ích của thời hạn và phải trả toàn nợ cho Bên A và Bên A có quyền hủy hợp đồng đơn phương.
            <br />②	Nhờ Điều khoản trên nếu hủy hợp đồng thì Bên A có thể thu lại máy lọc nước mà không cần sự đồng ý của Bên B và Bên A dừng lại toàn bộ dịch vụ cho Bên B.
            <br />Điều 9: Bồi thường về hủy hợp đồng
            <br />Trước khi hết hợp đồng, Bên B hủy hợp đồng thì không được yêu cầu trả lại tiền đặt cọc (nhưng sau 24 tháng kể từ ngày ký hợp đồng thì được) và không trả lại máy lọc nước đã thuê của Bên A thì Bên B phải một lúc trả tiền thuê còn lại thời hạn hợp đồng.

        </td>
        <td>
            Điều 10: Sử dụng, bảo quản và quản lý duy trì sản phẩm cho thuê
            <br />①	Bên B sử dụng máy lọc nước đúng mục đích của nó và phải làm nghĩa vụ tư cách người quản lý tốt. Nếu lỗi do Bên B làm hư hỏng thì phải trả chi phí sửa chữa.
            <br />②	Bên B không được chuyển nhượng, cho thuê lại và bán lại cho người thứ ba khi không có sự đồng ý bằng văn bản của Bên A. Nếu Bên B sửa đổi máy lọc nước thì phải có sự đồng ý của Bên A và trong khi sửa đổi có phát sinh vấn đề thì Bên A không có trách nhiệm điều đó. Bên B sẽ chịu tất cả chi phí sửa đổi.
            <br />③	Nếu Bên B cần chuyển địa chỉ hoặc chuyển nơi lắp ráp máy lọc nước lại trong TP. HCM thì phải báo cho Bên A và Bên A phải tiến hành chuyển nơi lắp ráp miễn phí cho Bên B. Nhưng lần thứ hai thì Bên B phải trả chi phí lắp ráp là 700,000VNĐ.
            <br />④	Bên A sẽ làm vệ sinh máy lọc nước mỗi tháng 1 lần cho Bên B để giữ vệ sinh và sử dụng tốt.
            <br />⑤	Bên B không được lấy ra hoặc tháo gỡ tem nhãn quyền sở hữu và tem nhãn khác.
            <br />Điều 11: Bảo hành bảo trì và thay bình lọc (Filter)
            <br />①	Trong thời hạn hợp đồng, nếu máy lọc nước bị hư thì Bên B yêu cầu Bên A sửa chữa và thay phụ tùng miễn phí. Bên B có quyền yêu cầu bồi thường thiệt hại. Nhưng nếu lỗi do Bên B làm hư hỏng thì Bên B phải trả chi phí sửa chữa và thay phụ tùng.
            <br />②	Nếu trong nước mà máy lọc nước đã thuê của Bên A ra, có vật chất lạ hoặc không được bình thường thì Bên B có quyền yêu cầu thay máy lọc nước khác hoặc hủy hợp đồng.
            <br />③	Sau thời gian hợp đồng 48 tháng được thay bình lọc (Filter) miễn phí, Bên B được sở hữu máy lọc nước đã thuê của Bên A và phải trả chi phí thay bình lọc (Filter) của máy lọc nước.
            <br />④	Bên A thay bình lọc (Filter) cho Bên B trước hoặc sau so với ngày định kỳ
            <br />Điều 12: Sử dụng quyền hạn và đồng ý.
            <br />Bên B đã biết rằng quyền sở hữu máy lọc nước đã thuê là của Bên A (Cty TNHH MTV Đại Á bán máy lọc nước Seoul Aqua).
            <br />Điều 13: Chuyển nhượng quyền hạn
            <br />①	Bên A có quyền chuyển nhượng toàn bộ hoặc một phần của quyền hạn trong hợp đồng này cho người thứ ba mà không cần báo cho Bên B biết.
            <br />②	Bên B không được quyền chuyển nhượng toàn bộ hoặc một phần của quyền hạn trong hợp đồng này cho người thứ ba mà Không báo cho Bên A biết. Nếu muốn chuyển nhượng thì phải có sự đồng ý của Bên A bằng văn bản.
            <br />Điều 14: Sử dụng và đăng ký thông tin cá nhân
            <br />Bên B đồng ý về đăng ký thông tin cá nhân vào kinh doanh liên quan và sử dụng một cách chính đáng. Nhưng nếu không có sự đồng ý của Bên B thì không được giao cho người thứ ba không liên quan bất cứ lý do nào.
            <br />Điều 15: Điều khoản chung
            <br />①	Điều chưa quyết định trong hợp đồng này thì Bên A và Bên B thảo luận và bàn bạc một cách thân thiện. Nhưng nếu không được thống nhất thì phải tuân theo sự giải quyết của pháp luật.
            <br />Hợp đồng này được lập thành 02 bản, mỗi bên giữ 01 bản và có giá trị pháp lý như nhau.
            <%
                String logopath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/";
                 %>
            <table>
                <tr>
                    <td width="35%"><img alt="logo" src="<%=logopath%>Logo.png" width="100px" /></td>
                    <td><h5 align="right">CTY TNHH MTV TM & DV ĐẠI Á <br />
                            Trung tâm bảo trì:  08 - 6678 3824
                        </h5>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    </table>
    
    </div>
<%
}
else if (ViewData["Lang"].ToString() == "kr")
{ %>
    <div id='kr'>

    <h2 style="text-align:center"><b>임대 약정서 </b></h2>

    <h4 align="right">설치일자: <%=DateTime.Today.Day %> <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> <%=DateTime.Today.Month %>  năm <%=DateTime.Today.Year %></h4>
    <h4 align="right">No.: <%=SOWSI.Order.OrderId %></h4>

    <b>회자:  <%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %> (임대상품)</b>
    <ul>
        <li>Tax code:     <%=NEXMI.NMCommon.GetCompany().Customer.TaxCode %></li>
        <li>Address:        <%=NEXMI.NMCommon.GetCompany().Customer.Address %></li>
        <li>Tel:     <%=NEXMI.NMCommon.GetCompany().Customer.Telephone %>	</li>
        <li>Account No.:   <% %></li>
    </ul>

    <b><label>성 명: <%=ViewData["CustomerName"]%> </label>(구 매 자) </b>
    <ul>
        <li>Tax code: <%=ViewData["CustomerTaxCode"] %>  </li>
        <li>설치주소:  <%=ViewData["CustomerAddress"].ToString() + ", " + ViewData["CustomerArea"].ToString()%> </li>
        <li>휴 대 폰: <%=ViewData["CustomerTelephone"]%> </li>
        <li>Đại diện:  <%=ViewData["CustomerAntt"]%></li>
    </ul>
    <br />

    <table border="1" width="100%" style="font-size: medium"> 
        <tr style="background-color:Gray; font-weight: bold">
            <td >모 델 명</td>
            <td >수량</td>
            <td >설 치 비 (USD)</td>
            <td >보 증 금 (USD)</td>
            <td >월임대료 (USD)</td>
        </tr>
    <%  double count = 0;
        foreach (var Item in SOWSI.Details)
        {
            count += Item.Quantity;        
    %>
        <tr>
            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
            <td align="right"><%=Item.Quantity %></td>
            <td align="right"><%=Item.Price %></td>
            <td align="right"></td>
            <td align="right"><%=Item.TotalAmount %></td>
        </tr>   
    <% 
        }    
    %>
        <tr>
            <td align="right"><b>Total: </b></td>
            <td align="right" style="width: 120px;"><label id="Label2"><%=count %></label></td>
            <td colspan='2'></td>
            <td align="right" style="width: 120px;"><label id="Label4"><%=ViewData["Amount"]%></label></td>
        </tr>
    </table>

    <br />
    <table border="1" width="100%" style="font-size: medium"> 
    <tr>
        <td width="10%">계<br /> 약<br />조<br />건:</td>
        <td width="30%">
            <table border='0'>
                <tr>
                    <td>불 입 회 수</td>
                    <td>48 	회</td>
                </tr>
                <tr>
                    <td>의무사용기간</td>
                    <td>48 	개월	</td>
                </tr>
                <tr>
                    <td>계 약 일 자</td>
                    <td></td>
                </tr>
            </table>
        </td>
        <td width="10%">특<br /> 기<br />사<br />항</td>
        <td>
            1. 의무사용기간은 48개월이며, 48개월 사용 후에 소유권이 이전됩니다.
            <br />2. 임대료는 선납이며, 설치 후 매달 설치 일에 자동으로 청구됩니다.
            <br />3. 의무사용기간이 종료되는 날에 보증금을 반환합니다. 
            <br />4. 24개월 이전에 계약을 해지 할 경우에는 보증금 반환이 안됩니다.
        </td>
    </tr>
    </table>
    <br />
    <table border="1" width="100%" style="font-size: medium">
    <tr>
        <td width="8%">회사 의 무 사 항</td>
        <td>필 터 교 환 주 기</td>
    </tr>
    <tr >
        <td width="60%">
            1.	월1회무상 점검 및 스팀 청소를 의무화 한다.
            <br />2.	고장시 2일이내 수리를 완료 한다.
            <br />3.	필터는 필터 교환 주기에 따라 무료로 교환한다.
            <br />4.	수질은 베트남 공식 인정기관인 파스터의 기준에 적합하게 유지한다.
            <br />5.	48개월후 별도의 협의에따라 유지보수를 제계약 한다. 
            <br />6.	정수기에는 관리 책임자 및 관리 점검표를 부착한다.
        </td>
        <td width="32%">
            정수기 필터 교환 주기
            <br />세디먼트 :3개월
            <br />프리카본 : 6개월
            <br />UF 필터: 8개월
            <br />Post 카본: 12개월
        </td>
    </tr>
    </table>
    (뒷면 약정조항 참조)
    <br />           			           

    <table style="width:100%; text-align:center">
        <tr>
            <td style="width:50%; text-align:center">임대주는 대표자 <br />(서명)</td>
            <td style="width:50%; text-align:center">임대 대표자<br />(서명)</td>
        </tr>
    </table>
    <br /><br /><br />
    <h3 align="center" style="text-decoration: underline">약 정 조 항</h3>
    <br />
    <table style="font-size: small">
    <tr>
        <td>
                제1조: 계약목적
            <br />본 약관은 정수기 임대인(이하 “갑”)과 임대정수기 임차인(이하 “을”) 사이에 체결된 임대정수기 임대차 계약상의 권리, 의무, 책임 등에 관한 전반적인 사항을 규정함을 목적으로 한다.
            <br />제2조: 계약의 효력
            <br />본 계약은 “갑”이 제공하는 본 약정서에 모든 사항을 기재하고, “을”이 서명 또는 날인함으로써 그 효력이 발생한다.
            <br />제3조: 임대차 계약기간
            <br />①	“갑”이“을”에게 임대정수기를 인도, 설치한 일자로부터 임대차기간이 개시된다.
            <br />②	본 임대차 계약의 총 계약기간은 48개월이며, 4년 사용 후에는 “을”에게 그 소유권이 이전 된다. 단, 월불입금이 연체되거나, 기타 채무금이 남아 있는 경우에는 잔존금이 모두 납입되는 시점에 소유권이 이전되며 보증금을 반환 받는다.
            <br />③	“을”의 의무사용기간은 48개월이고, 의무사용기간 동안에는 부득이한 경우를 제외하고 계약을 해지할 수 없다.
            <br />제4조: 임대제품의 인도 및 설치
            <br />①	“갑”은 계약 효력일로부터 14일 이내 “을”이 요청하는 장소에 임대제품을 정상적인 사용이 가능하도록 설치한다.
            <br />②	소요되는 운송 및 설치에 따른 비용은 “갑”의 부담으로 한다.
            <br />③	임대차 계약이 종료된 후 임대제품의 철거비용은 “갑”의 부담으로(소유권이전은 제외) 한다. 다만, “을”의 사정으로 임대차 계약이 종료된 경우에는 “을”의 부담으로 한다.
            <br />제5조: 제품대금지불
            <br />본 약정서 앞면에 기재된 “을”은 공급받은 제품의 대금을 선납으로 불입한다.
            <br />제6조: 연체료
            <br />“갑”은 “을”이 약정서에 기재된 내용대로 불입금을 납부하지 않을 경우 월 2%의 연체료를 부과한다.
            <br />제7조: 계약해지 및 철회권
            <br />①	“을”은 임대제품을 설치한 날로부터 7일 이내에 서면으로 계약해지 또는 약정철회를 할 수 있다. 다만, “을”에게 책임 있는 사유로 임대제품이 분실 또는 훼손된 경우에는 계약해지 또는 약정철회를 할 수 없다.
            <br />②	“갑”은 계약해지로 인하여 철거된 임대제품의 수령일로부터 영업일 3일 이내에 이미 지급 받은 임대료를 “을”에게 환불한다. 다만, 임대제품의 특성상 필터 등의 소모성 부품이 포함되어 있는 “설치상품”에 해당되어 일단 설치가 되고 나면 철거 후 신규판매가 불가능하므로 보증금에 대해서는 반환할 의무가 없다. 또한, “을”이 이미 지불한 설치비는 환불 받지 못한다.
            <br />③	“을”은 계약해지 후, 계약 또는 설치 시에 “갑”으로부터 제공받은 일체의 사은품을 “갑”에게 반납하여야 한다.
            <br />④	철회권 행사의 요건, 방법 및 효력에 관한 기타 사항은 관련 법률의 각 규정에 의한다.
            <br />제8조: 기한 이익 상실
            <br />①	“을”이 2개월 이상 월 불입금 연체 시 기한의 이익을 상실하고 계약기간의 잔여 불입금을 일시 납입하여야 하며, “갑”이 임의로 본 약정을 해지할 수 있다.
            <br />②	전 항에 의해 본 약정을 해지할 경우, “갑”은 “을”에게 통보 없이 임의로 제품을 수거하며, “갑”이 제품을 수거 후에는 “을”에게 제공하던 일체의 서비스를 종료한다.
            <br />제9조: 약정 해지에 따른 배상
            <br />“을” 제품사용 중 의무사용기간 내에 약정을 해지하고 제품을 반환할 경우 보증금 반환 요구를 할 수 없으며 (단, 24개월 사용 후에는 가능) 제품을 반환하지 않을 경우에는 남은 기간의 총 

        </td>
        <td>
              불입금을 일시에 불입하여야 한다.   
            <br />제10조: 임대제품의 사용, 보관 및 유지 관리
            <br />①	“갑”은 계약기간 중 임대제품이 적정한 상태를 유지하기 위하여 필터 교환 등, “을”이 임대제품을 사용하기 위하여 필요한 기본적인 상태를 유지시켜야 한다.
            <br />②	“을”은 임대제품을 통상적인 용도에 따라 사용하고, 선량한 관리자로서의 의무를 다 하여야 한다. “을”의 부주의로 인한 제품의 결함 발생시 “을”은 “갑”에게 실비를 지불한다.
            <br />③	“을”은 “갑”의 승인 없이 임대제품을 타인에게 양도, 대여 또는 전매할 수 없다. 또한, 개조 시 반드시 “갑”의 사전동의를 얻어야 하며, 개조 시 발생한 문제에 관해서는 “갑”은 책임이 없다. 개조에 따른 재 비용은 “을”이 부담한다.
            <br />④	“을”은 주소변경이나 제품 재설치가 필요한 경우 “갑”에게 즉시 통보하여야 하며, “갑”은 호치민시 지역 내에서 이전 재 설치에 대해 별도의 청구 없이 이행한다. 두 번째 이전 재설치 시 설치비 35달러(USD)를 청구 할 수 있다.
            <br />⑤	“갑”은 임대정수기의 청결 및 유지를 위하여 월 1회 점검을 하여야 한다.
            <br />⑥	“을”은 임대제품에 부착된 “갑”의 소유권표시, 기타 표지 등을 제거하거나 손상시키지 아니한다.
            <br />제11조: A/S보증 및 필터교환
            <br />①	계약기간 중 임대제품이 고장, 훼손된 경우에는 “을”은 “갑”에게 무상으로 수리 및 부품교환을 요청할 수 있으며 발생한 손해에 대한 배상을 청구할 수 있다. 다만, “을”의 잘못으로 인하여 임대제품이 고장, 훼손된 경우에는 “을”은 자신의 비용으로 “갑”에게 수리 및 부품 교환을 요구할 수 있다.
            <br />②	A/S관리 소홀로 인하여 정수기로부터 유출되는 물에 이물질 혼입이 발생하거나 수질 이상 시 “을”은 제품교환  요구할 수 있다.
            <br />③	계약 총 48개월 간 필터교환의 무상 기간이 끝난 후 소유권이 이전된 이후의 필터교환 비용은 “갑”의 교정에 따라 “을”이 지불해야 한다.
            <br />④	필터교환은 필터교환주기에 따라, 해당 필터의 예정된 교환일자를 전후하여 14일 이내에 한다.
            <br />제12조: 권리의 이용 및 승낙
            <br />“을”은 약정에 의한 제품의 소유권은 서울 아쿠아 정수기 임대 및 판매 업체에 있음을 숙지한다.
            <br />제13조: 권리의 양도
            <br />①	“갑”은 본 약정에 의하여 발생한 권리의 전부나 일부를 별도의 통보 없이 제3자에게 양도 또는 담보로 제공할 수 있다. 
            <br />②	“을”은 본 약정에 의하여 발생한 권리의 전부나 일부를 별도의 통보 없이 제3자에게 양도 또는 담보로 제공할 수 없으며, 만약 “을”이 양도 시에는 반드시 “갑”에게 통보하여 문서로 작성된 승낙서를 취득하여야 한다.
            <br />제14조: 신용정보의 등록 이용
            <br />“을”의 신용정보를 관련업계에 등록하고 정당하게 사용하는 것을 허용한다. 단, 어떠한 이유라도 “을”의 동의 없이 타인에게 제공하지 않는다.
            <br />제15조: 기타
            <br />①	이 약정조항에 규정되지 않은 사항은 “신의 성실의 원칙”에 따라 “갑”과 “을”이 합의하여 결정하되 합의되지 아니한 사항은 관련 법율에 따른다.
            <br /> 본 약정서는 2부를 작성하여 “갑”과 “을”이 각각 1부씩 보관한다.
            <%
                String logopath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/";
                 %>
            <table>
                <tr>
                    <td width="35%"><img alt="logo" src="<%=logopath%>Logo.png" width="100px" /></td>
                    <td><h5 align="right">DAI A TRADING - SERVICE CO.,LTD <br />
                            고객센터:  08 – 6678 - 3824
                        </h5>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    </table>
    
    </div>
<%
} %>
<%if(false)
  {
      string[] desc = Regex.Split(ViewData["Description"].ToString(), ";");
      string[] payterm = Regex.Split(ViewData["PaymentTerm"].ToString(), ";");
      ArrayList objs = (ArrayList)ViewData["objs"];
%>

<div id='VIKOR'>

<h4 style="text-align:center"> CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM </h4>
<h5 style="text-align:center"> Độc lập – Tự do – Hạnh phúc</h5>
<br />
<h2 style="text-align:center"><b>HỢP ĐỒNG KINH TẾ</b></h2>
<h5 style="text-align:center">(Số:........../2013/HD-..........................)</h5>

<ul>
    <li>Căn cứ Luật Thương mại và Luật Dân sự được Quốc Hội nước CHXHCN VN thông qua ngày 14/6/2005. </li>
    <li>Căn cứ vào khả năng cung cấp và nhu cầu tiêu thụ của các bên.</li>
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
<li>Tài khoản số:   <% %></li>
<li>Đại diện:       <%=(NEXMI.NMCommon.GetCompany().ManagedBy != null)? NEXMI.NMCommon.GetCompany().ManagedBy.CompanyNameInVietnamese : ""%></li>
<li>Chức vụ:        <%=(NEXMI.NMCommon.GetCompany().ManagedBy != null)? NEXMI.NMCommon.GetCompany().ManagedBy.JobPosition : ""%></li>
<li><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %>:     <%=NEXMI.NMCommon.GetCompany().Customer.Telephone %>	</li>
</ul>
<br />
    Hai bên nhất trí cùng nhau ký kết hợp đồng dịch vụ về việc cung ứng đồng phục với các điều khoản sau:
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
        <td colspan="5" align="right"><b><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %>: </b></td>
        <td align="right"><label id="lbInvoiceAmount"><%=ViewData["TotalAmount"]%></label></td>
    </tr>
</table>
Tổng giá trị hợp đồng: <%=ViewData["TotalAmount"]%> VNĐ.
<br />
Số tiền bằng chữ: <%= NEXMI.MoneyToString.UppercaseFirst(NEXMI.MoneyToString.SetValue(Decimal.Parse(ViewData["TotalAmount"].ToString()))) %>.
<ul>
<%  
    foreach (string item in desc)
    {
     %>
    <li><%=item%></li>
    <%} %>
</ul>
<br />
<b>ĐIỀU 2: NGUỒN VẢI VÀ CHẤT LIỆU VẢI</b>
<br />
<label> Bên B cam kết đảm bảo nguồn vải đúng theo mẫu đã được Bên A chấp thuận. Bảo đảm cung cấp số lượng kịp thời và đầy đủ trong suốt thời gian thực hiện Hợp đồng. </label>
<br /><br />
<b>ĐIỀU 3: CHẤT LƯỢNG – QUY CÁCH – KIỂU DÁNG </b>
<ul>
    <li>Bên B cam kết may theo mẫu thiết kế của Bên A.</li>
    <li>Mẫu thiết kế, mẫu vải, mẫu may thực tế (nếu có) và bảng thông số đã thống nhất giữa 2 bên là những phụ lục liên quan đến hợp đồng và không thề tách rời.</li>
    <li>Bên B cam kết sử dụng các phụ liệu như: chỉ Phong Phú, dây kéo HKK, nút Lý Minh, băng dán loại 1.</li>
    <li>Bên B cam kết về chất lượng kỹ thuật may, về độ sắc nét của đường kim mũi chỉ (5 mũi kim/cm) trên toàn bộ sản phẩm, đảm bảo đúng mẫu chuẩn.</li>
    <li>Quy cách đóng gói: Đồng phục của mỗi nhân viên Bên A sẽ được đóng gói trong một bao bì có đính kèm tên nhân viên, mã số nhân viên, tên đơn vị/ bộ phận mà nhân viên đang công tác.(nếu có)</li>
</ul>
<br />
<b>ĐIỀU 4: GIAO HÀNG – SỬA HÀNG - BẢO HÀNH </b>
<br />
a. CÔNG TÁC GIAO HÀNG:
<ul>
    <li>Bên B sẽ giao hàng cho Bên A trong vòng <%=ViewData["DeliveryDate"]%> ngày (trừ ngày tết, lễ, chủ nhật) kể từ ngày đã thực hiện đầy đủ các công việc sau:</li>
    
    <ol style="padding:20px">
        <li>    - Ký kết Hợp đồng.</li>
        <li>    - Bên B nhận được tạm ứng của Bên A.</li>
        <li>    - Bên A ký xác nhận trên tất cả các mẫu gồm: bảng thông số thành phẩm, mẫu thiết kế, mẫu vải, logo và mẫu may thực tế (nếu có).</li>
    </ol>
    <li>Địa điểm giao hàng: Tại Bên A.</li>
    <li>Quý Công ty vui lòng cung cấp thông tin người liên hệ tại mỗi chi nhánh để tiện cho việc giao nhận hàng hóa (đặc biệt đối với chi nhánh tỉnh). Việc giao nhận hàng được xác nhận bằng biên bản bàn giao.</li>
    <li><b><i> Đối với chi nhánh TPHCM</i></b>: Nhân viên giao nhận của chúng tôi sẽ chuyển hàng đến địa chỉ  của Công ty và kiểm hàng tại chỗ cùng với anh chị phụ trách.</li>
    <li><b><i>Đối với chi nhánh tỉnh</i></b>: Chúng tôi sẽ chuyển hàng thông qua công ty chuyển phát, hàng sẽ được đóng thùng trước khi chuyển đến đơn vị. Anh chị phụ trách vui lòng kiểm hàng cẩn thận trước khi kí xác nhận vào biên lai nhận hàng của công ty chuyển phát và phát cho chúng tôi biên bản bàn giao (gửi kèm trong thùng hàng) theo đường bưu điện hoặc theo số fax công ty: <%=NEXMI.NMCommon.GetCompany().Customer.FaxNumber %>. Đây là cơ sở để 2 bên cùng nghiệm thu – thanh toán.</li>
</ul>
b. CÔNG TÁC SỬA HÀNG VÀ BẢO HÀNH:
<ul>
    <li>Trường hợp vải bị rách hoặc hư do lỗi của Bên B thì Bên B sẽ may lại cho Bên A.</li>
    <li>Thời hạn sửa hàng: Trong vòng <%=ViewData["RepairDate"]%> ngày, kể từ ngày Bên B giao hàng, Bên A tập hợp danh sách nhân viên chỉnh sửa đồng phục và thông báo cho Bên B được biết. Bên B có trách nhiệm cử nhân viên đến địa điểm giao hàng để lấy thông tin chỉnh sửa. Sau <%=ViewData["RepairDate"]%> ngày, nếu nhân viên nào không có tên trong danh sách sửa hàng Bên B sẽ không chịu trách nhiệm về việc hàng bị lỗi. Nếu hàng sửa lần 1 vẫn chưa đạt yêu cầu thì các lần sửa tiếp theo cũng thực hiện theo thời gian quy định như lần sửa thứ nhất.</li>
</ul>
<br />
<b>ĐIỀU 5: THỜI HẠN VÀ PHƯƠNG THỨC THANH TOÁN</b>
<ul>
    <%  
        foreach (string item in payterm)
    {
     %>
    <li><%=item%></li>
    <%} %>
    <li><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %>: Chuyển khoản.</li>
</ul>
<br />
<b>ĐIỀU 6: TRÁCH NHIỆM CỦA HAI BÊN</b>
<br />
a. Trách nhiệm của Bên A:
<ul>
    <li>Bên A có trách nhiệm thanh toán đúng thời hạn theo Điều 5 của Hợp đồng này. Nếu thanh toán chậm quá 07 (bảy) ngày thì Bên A phải trả thêm tiền lãi (căn cứ theo tỷ giá của Ngân hàng Ngoại Thương) trên tổng số tiền chưa thanh toán.</li>
    <li>Trong quá trình thực hiện hợp đồng nếu bên A đơn phương chấm dứt hợp đồng thì sẽ phải bồi thường cho Bên B toàn bộ số tiền đã tạm ứng (thanh toán) cho Bên B.</li>
</ul>
b. Trách nhiệm của Bên B:
<ul>
    <li>Bên B có trách nhiệm giao hàng đúng qui định tại Điều 1, Điều 2, Điều 3, Điều 4 và Điều 5 của hợp đồng này. Nếu giao hàng chậm thì Bên B phải trả tiền lãi (căn cứ theo tỷ giá của Ngân hàng Ngoại thương) trên tổng giá trị hàng chưa giao. </li>
    <li>Trong quá trình thực hiện hợp đồng nếu Bên B đơn phương chấm dứt hợp đồng thì phải bồi thường gấp 2 lần số tiền Bên A đã tạm ứng cho Bên B.</li>
</ul>
<br />
<b>ĐIỀU 7: ĐIỀU KHOẢN CHUNG</b>
<ul>
    <li>Hai bên cam kết thực hiện đúng, đầy đủ các điều khoản đã thỏa thuận trong hợp đồng. mọi sự thay đổi bổ sung sau này phải được sự đồng ý của cả hai bên và được thực hiện bằng văn bản.</li>
    <li>Trong quá trình thực hiện hợp đồng, nếu có vấn đề phát sinh, hai bên sẽ cùng bàn bạc giải quyết trên tinh thần hợp tác và tôn trọng lợi ích của cả hai bên. Nếu không giải quyết được thì phán quyết của tòa kinh tế toà án nhân dân sẽ là quyết định cuối cùng ràng buộc hai bên phải thực hiện. Các chi phí về kiểm tra, xác minh và lệ phí do bên thua kiện chịu.</li>
    <li>Trong trường hợp tổn thất hàng hóa hay tiến độ giao hàng chậm vì thiên tai, địch họa hay những trở lực khách quan không thể lường trước được, Bên B sẽ không chịu trách nhiệm khi đã thi hành mọi biện pháp cần thiết.</li>
    <li>Hợp đồng có hiệu lực kể từ ngày ký đến hết ngày 31/12/2013.</li>
    <li>Các Bên có trách nhiệm thực hiện đúng các điều khoản đã ghi trong Hợp đồng.</li>
    <li>Hợp đồng gồm 03 (ba) trang được lập thành 04 (bốn) bản. Mỗi Bên giữ 02 (hai) bản có giá trị pháp lý như nhau và có hiệu lực kể từ ngày ký.</li>
</ul>
<br />           			           

<table style="width:100%; text-align:center">
    <tr>
        <td style="width:50%; text-align:center">ĐẠI DIỆN BÊN A</td>
        <td style="width:50%; text-align:center">ĐẠI DIỆN BÊN B</td>
    </tr>
</table>

</div>

<%} %>
