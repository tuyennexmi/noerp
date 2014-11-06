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
                        <td align="right">MẪU: 04-VT<br />(Ban hành theo QĐ: 15/2006/QĐ-BTC<br />ngày 20/03/2006 của Bộ trưởng BTC)</td>
                    </tr>
                    <%--<tr>
                        <td colspan="2"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>: <%=NEXMI.NMCommon.GetCompany().Customer.Address%></td>
                        <td colspan="2" align="right">&nbsp;</td>
                        <td colspan="2" align="right">&nbsp;</td>
                    </tr>--%>
                    <tr>
                        <td></td>
                        <td align="center">
                            <b style="font-size: x-large;">PHIẾU XUẤT KHO</b><br />
                        </td>                        
                        <td>Số: <%=WSI.Export.ExportId%></td>
                    </tr>
                    <tr>
                        <td style="width: 20%;"></td>
                        <td align="center">Ngày <%=WSI.Export.ExportDate.ToString("dd")%> tháng <%=WSI.Export.ExportDate.ToString("MM")%> năm <%=WSI.Export.ExportDate.ToString("yyyy")%></td>
                        <td style="width: 20%;"></td>
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
                        <td colspan="2" align="left"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %>: <%=WSI.Export.DescriptionInVietnamese%></td>
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
                        <th><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                    </tr>
                    <%
                        int count = 1;
                        NEXMI.NMProductsWSI ProductWSI;
                        foreach (NEXMI.ExportDetails Item in WSI.Details)
                        {
                            ProductWSI = WSI.ProductWSIs.Select(i => i).Where(i => i.Product.ProductId == Item.ProductId).FirstOrDefault();
                    %>
                    <tr>
                        <td><%=count++%></td>
                        <td><%=ProductWSI.Translation.Name%></td>
                        <td><%=ProductWSI.Product.ProductCode%></td>
                        <td><%=ProductWSI.Unit.Name%></td>
                        <td><%=Item.Quantity.ToString("N3")%></td>
                        <td><%=Item.Description%></td>
                    </tr>
                    <%
                        }
                    %>
                </table>
            </td>                    
        </tr>
        <tr>
            <td align="right">Xuất ngày <%=WSI.Export.ExportDate.ToString("dd")%> tháng <%=WSI.Export.ExportDate.ToString("MM")%> năm <%=WSI.Export.ExportDate.ToString("yyyy")%></td>
        </tr>
        <tr>
            <td>
                <table style="width: 100%">
                    <tr align="center">
                        <td style="width: 25%;"><b>Phụ trách bộ phận sử dụng</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Phụ trách cung tiêu</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b><%= NEXMI.NMCommon.GetInterface("RECIPIENT", langId) %></b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Thủ kho</b><br />(Ký, họ tên)</td>
                    </tr>
                </table>
            </td>                    
        </tr>
    </table>
</div>