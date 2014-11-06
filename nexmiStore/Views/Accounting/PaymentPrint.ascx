<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMPaymentsWSI WSI = (NEXMI.NMPaymentsWSI)ViewData["WSI"];
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
                    <h1>PHIẾU CHI</h1>
                    <table width="50%" style="text-align: right;" cellpadding="0" cellspacing="0">
                        <tr>
                            <td width="40%">Số:</td>
                            <td width="60%"><%=WSI.Payment.PaymentId %></td>
                        </tr>
                        <tr>
                            <td width="40%">Ngày chi:</td>
                            <td width="60%"><%=WSI.Payment.PaymentDate.ToString("dd-MM-yyyy")%></td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    
    
    <table width="100%">
        <tr>
            <td width="40%">Họ và tên người nhận tiền:</td>
            <td ><%=WSI.Customer.CompanyNameInVietnamese %></td>
        </tr>
        <tr>
            <td width="40%"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>:</td>
            <td><%=WSI.Customer.Address + ", " + ViewData["CustomerArea"]%></td>
        </tr>
        <tr>
            <td width="40%">Lý do chi:</td>
            <td><%=WSI.Payment.DescriptionInVietnamese %></td>
        </tr>
        <tr>
            <td width="40%">Số tiền:</td>
            <td><%=WSI.Payment.PaymentAmount.ToString("N3") + ". Bằng chữ: " + NEXMI.NMCommon.ReadNum(WSI.Payment.PaymentAmount) %>.</td>
        </tr>
        <tr>
            <td width="40%">Kèm theo chứng từ gốc:</td>
            <td><%=WSI.Payment.InvoiceId %></td>
        </tr>   
    </table>
    <div style="text-align: right;">
        <label><%= NEXMI.NMCommon.GetInterface("DATE", langId) %><%=DateTime.Today.Day %> tháng <%=DateTime.Today.Month %> năm <%=DateTime.Today.Year %>.</label>
    </div>
    <table width="100%" >
        <tr align="center">
            <th >Giám đốc <br /> (Ký, họ tên, đóng dấu)</th>
            <th>Kế toán trưởng <br /> (Ký, họ tên)</th>
            <th>Người nhận tiền <br /> (Ký, họ tên)</th>
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
            <td><%= NEXMI.NMCommon.GetCustomerName(WSI.Payment.CreatedBy) %></td>
            <td></td>
        </tr>
    </table>
    <%Html.RenderPartial("ReportFooter"); %>
</div>