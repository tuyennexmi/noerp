<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div style="font-size: 9pt;">
<% 
    NEXMI.NMExportsWSI WSI = (NEXMI.NMExportsWSI)ViewData["WSI"];
    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
%>
    <table style="width: 100%">
        <tr>
            <td>
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <img height="60px" alt="" src="<%=localpath%>" />
                        </td>
                        <td align="right">&nbsp;</td>
                        <td align="right">MẪU: 02-VT<br />(Ban hành theo QĐ: 48/2006/QĐ- BTC<br />ngày 14/9/2006 của Bộ trưởng BTC)</td>
                    </tr>
                    <%--<tr>
                        <td colspan="2"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>: <%=NEXMI.Constants.companyAddress%></td>
                        <td style="width: 40%;">Đơn vị: <%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese%></td>
                        <td align="right">&nbsp;</td>
                    </tr>--%>
                    <tr>
                        <td></td>
                        <td align="center">
                            <b style="font-size: x-large;">PHIẾU XUẤT KHO</b>                            
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td align="center">
                            Ngày <%=WSI.Export.ExportDate.ToString("dd")%> tháng <%=WSI.Export.ExportDate.ToString("MM")%> năm <%=WSI.Export.ExportDate.ToString("yyyy")%>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td align="center">Số: <%=WSI.Export.ExportId%></td>
                        <td></td>
                    </tr>
                </table>
            </td>                    
        </tr>
        <tr>
            <td align="center">
                <table style="width: 90%;">
                    <tr>
                        <td align="left" colspan="2"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>: <%=(WSI.Customer != null)? WSI.Customer.CompanyNameInVietnamese : ""%></td>
                        <%--<td align="left"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>: <%=WSI.Customer.Address%></td>--%>
                    </tr>
                    <tr>
                        <td align="left">Theo đơn đặt hàng: <%=WSI.Export.Reference%></td>
                        <td align="left"><%= NEXMI.NMCommon.GetInterface("EXPORT_STOCK", langId) %>: <%=(WSI.Stock != null && WSI.Stock.Translation != null) ? WSI.Stock.Translation.Name : ""%> - <%=(WSI.Stock != null && WSI.Stock.Translation != null) ? WSI.Stock.Translation.Address : ""%></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="left"><%= NEXMI.NMCommon.GetInterface("EXPORT_REASON", langId)%>: <%=WSI.Export.DescriptionInVietnamese%></td>
                    </tr>
                </table>
            </td>                    
        </tr>
        <tr>
            <td>
                <table class="tbDetail" style="width: 100%;" border=".1">
                    <tr>
                        <th width="5%">STT</th>
                        <th width="25%">Tên, nhãn hiệu, quy cách, phẩm chất VLSPHH</th>
                        <th ><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></th>
                        <th ><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                        <th ><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %> yêu cầu</th>
                        <th ><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %> thực xuất</th>
                        <th ><%= NEXMI.NMCommon.GetInterface("PRICE", langId)%></th>
                        <th ><%= NEXMI.NMCommon.GetInterface("AMOUNT", langId)%></th>
                    </tr>                    
                    <%
                        int count = 1;
                        NEXMI.NMProductsWSI ProductWSI;
                        double total = 0;
                        foreach (NEXMI.ExportDetails Item in WSI.Details)
                        {
                            ProductWSI = WSI.ProductWSIs.Select(i => i).Where(i => i.Product.ProductId == Item.ProductId).FirstOrDefault();
                            total += Item.TotalAmount;
                    %>
                    <tr>
                        <td><%=count++%></td>
                        <td><%=ProductWSI.Translation.Name%></td>
                        <td><%=ProductWSI.Product.ProductCode%></td>
                        <td><%=ProductWSI.Unit.Name%></td>
                        <td><%=Item.RequiredQuantity.ToString("N3")%></td>
                        <td><%=Item.Quantity.ToString("N3")%></td>
                        <td><%=Item.Price%></td>
                        <td><%=Item.Amount%></td>
                    </tr>
                    <%
                        }
                    %>
                </table>
            </td>                    
        </tr>
        <tr>
            <td align="left">- Tổng số tiền (viết bằng chữ): <%=NEXMI.NMCommon.ReadNum(total) %>.</td>
        </tr>
        <tr>
            <td align="left">- Số chứng từ gốc kèm theo:</td>
        </tr>
        <tr>
            <td align="right">Xuất ngày <%=WSI.Export.ExportDate.ToString("dd")%> tháng <%=WSI.Export.ExportDate.ToString("MM")%> năm <%=WSI.Export.ExportDate.ToString("yyyy")%></td>
        </tr>
        <tr>
            <td>
                <table style="width: 100%">
                    <tr align="center">
                        <td style="width: 25%;"><b>Người lập phiếu</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Người nhận hàng</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Thủ kho</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Kế toán trưởng</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Giám đốc</b><br />(Ký, họ tên)</td>
                    </tr>
                </table>
            </td>                    
        </tr>
    </table>
</div>
