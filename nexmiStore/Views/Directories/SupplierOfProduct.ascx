<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%
    List<NEXMI.NMMonthlyGeneralJournalsWSI> sopList = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
    List<NEXMI.NMCustomersWSI> Suppliers = (List<NEXMI.NMCustomersWSI>)ViewData["Suppliers"];
 %>

 <table style="width: 96%; margin:8px;" class="tbDetails">
    <tr>
        <th>Mã Nhà CC</th>
        <th>Tên Nhà CC</th>
        <th>Số lượng đã mua</th>
        <th>Đơn giá trung bình</th>
        <th>Thành tiền</th>
    </tr>

<% 
    double amount = 0;
    double quantity=0;
    double price =0;
    List<NEXMI.NMMonthlyGeneralJournalsWSI> mgjList;
    foreach (NEXMI.NMCustomersWSI sp in Suppliers)
    {
        mgjList = sopList.FindAll(p => p.MGJ.PartnerId == sp.Customer.CustomerId);
        if (mgjList.Count > 0)
        {
            quantity = mgjList.Sum(i=>i.MGJ.ImportQuantity);
            amount = mgjList.Sum(i => i.MGJ.DebitAmount);
            price = amount/quantity;
%>
        <tr>
            <td><%=sp.Customer.CustomerId%></td>
            <td><%=sp.Customer.CompanyNameInVietnamese%></td>
            <td align="right"><%= quantity.ToString("N3")%></td>
            <td align="right"><%= price.ToString("N3")%></td>
            <td align="right"><%= amount.ToString("N3")%></td>
        </tr>

<%      }
    }
%>
 </table>