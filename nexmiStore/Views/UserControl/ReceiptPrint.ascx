<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
NEXMI.NMReceiptsWSI WSI = (NEXMI.NMReceiptsWSI)ViewData["WSI"];

if (ViewData["Lang"].ToString() == "vi" | ViewData["Lang"].ToString() == "en")
{
    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
%>

<div style="font-size: medium">
    <table style="width:100%">
        <tr>
            <td>
                <img height="60px" alt="" src="<%=localpath%>" />
            </td>
            <td width="70%">
                <div style="text-align: right;">
                    <table width="50%" style="text-align: right;" cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan='2'><h1>PHIẾU THU</h1></td>
                        </tr>
                        <tr>
                            <td width="40%">Số:</td>
                            <td width="60%"><%=WSI.Receipt.ReceiptId %></td>
                        </tr>
                        <tr>
                            <td width="40%">Ngày thu:</td>
                            <td width="60%"><%=WSI.Receipt.ReceiptDate.ToString("dd-MM-yyyy")%></td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    
    <table width="100%">
        <tr>
            <td width="40%">Họ và tên người nộp tiền:</td>
            <td ><%=WSI.Customer.CompanyNameInVietnamese %></td>
        </tr>
        <tr>
            <td width="40%"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:</td>
            <td><%=WSI.Customer.Address + ", " + ViewData["CustomerArea"]%></td>
        </tr>
        <tr>
            <td width="40%">Lý do nộp:</td>
            <td><%=WSI.Receipt.DescriptionInVietnamese %></td>
        </tr>
        <tr>
            <td width="40%">Số tiền:</td>
            <td><%=WSI.Receipt.ReceiptAmount.ToString("N3") + ". Bằng chữ: " + NEXMI.NMCommon.ReadNum(WSI.Receipt.ReceiptAmount) %>.</td>
        </tr>
        <tr>
            <td width="40%">Kèm theo chứng từ gốc:</td>
            <td><%=WSI.Receipt.InvoiceId %>.</td>
        </tr>   
    </table>
    <div style="text-align: right;">
        <label>Ngày <%=DateTime.Today.Day %> tháng <%=DateTime.Today.Month %> năm <%=DateTime.Today.Year %>.</label>
    </div>
    <table width="100%" >
        <tr align="center">
            <th >Giám đốc <br /> (Ký, họ tên, đóng dấu)</th>
            <th>Kế toán trưởng <br /> (Ký, họ tên)</th>
            <th>Người nộp tiền <br /> (Ký, họ tên)</th>
            <th>Người lập phiếu <br /> (Ký, họ tên)</th>
            <th>Thủ quỹ <br /> (Ký, họ tên)</th>
        </tr>
        <tr>
            <td><br /><br /></td>
            <td><br /><br /></td>
            <td><br /><br /></td>
            <td><br /><br /></td>
            <td><br /><br /></td>
        </tr>
        <tr align="center">
            <td></td>
            <td></td>
            <td><%=WSI.Customer.CompanyNameInVietnamese %></td>
            <td><%= NEXMI.NMCommon.GetCustomerName(WSI.Receipt.CreatedBy) %></td>
            <td></td>
        </tr>
    </table>
    <%Html.RenderPartial("ReportFooter"); %>
</div>

<%}
else if (ViewData["Lang"].ToString() == "kr")
{
    %>

    <div style="font-size: medium">
    <div style="text-align: right;">
        <h1>영수증</h1>
        <table width="40%" style="text-align: right;" cellpadding="0" cellspacing="0">
            <tr>
                <td >설치일자:</td>
                <td><%=WSI.Receipt.ReceiptId %></td>
            </tr>
            <tr>
                <td>방문일자:</td>
                <td><%=WSI.Receipt.ReceiptDate.ToString("dd-MM-yyyy")%></td>
            </tr>
        </table>
    </div>
    
    <table width="100%">
        <tr>
            <td width="40%">성    명:</td>
            <td ><%=WSI.Customer.CompanyNameInVietnamese %></td>
        </tr>
        <tr>
            <td width="40%">주    소:</td>
            <td><%=WSI.Customer.Address + ", " + ViewData["CustomerArea"]%></td>
        </tr>
        <tr>
            <td width="40%">이유:</td>
            <td><%=WSI.Receipt.DescriptionInVietnamese %></td>
        </tr>
        <tr>
            <td width="40%">금 액:</td>
            <td><%=WSI.Receipt.ReceiptAmount.ToString("N3") + ". Bằng chữ: " + NEXMI.NMCommon.ReadNum(WSI.Receipt.ReceiptAmount) %>.</td>
        </tr>
        <tr>
            <td width="40%">원본 문서에 첨부:</td>
            <td><%=WSI.Receipt.InvoiceId %>.</td>
        </tr>   
    </table>
    <div style="text-align: right;">
        <label>Ngày <%=DateTime.Today.Day %> tháng <%=DateTime.Today.Month %> năm <%=DateTime.Today.Year %>.</label>
    </div>
    <table width="100%" >
        <tr align="center">
            <th >Giám đốc <br /> (Ký, họ tên, đóng dấu)</th>
            <th>Kế toán trưởng <br /> (Ký, họ tên)</th>
            <th>고 객 확 인 <br /> (Ký, họ tên)</th>
            <th>Người lập phiếu <br /> (Ký, họ tên)</th>
            <th>Thủ quỹ <br /> (Ký, họ tên)</th>
        </tr>
        <tr>
            <td><br /><br /></td>
            <td><br /><br /></td>
            <td><br /><br /></td>
            <td><br /><br /></td>
            <td><br /><br /></td>
        </tr>
        <tr align="center">
            <td></td>
            <td></td>
            <td><%=WSI.Customer.CompanyNameInVietnamese %></td>
            <td><%= NEXMI.NMCommon.GetCustomerName(WSI.Receipt.CreatedBy) %></td>
            <td></td>
        </tr>
    </table>
    <%Html.RenderPartial("ReportFooter"); %>
</div>

<%} %>