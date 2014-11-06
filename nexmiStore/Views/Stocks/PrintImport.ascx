<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div style="font-size: 9pt;">
<%
    NEXMI.NMImportsWSI WSI = (NEXMI.NMImportsWSI)ViewData["WSI"];
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
                        <td align="right">MẪU: 03-VT<br />(Ban hành theo QĐ: 15/2006/QĐ-BTC<br />ngày 20/03/2006 của Bộ trưởng BTC)</td>
                    </tr>
                    <%--<tr>
                        <td colspan="2"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>: <%=NEXMI.NMCommon.GetCompany().Customer.Address%></td>
                        <td colspan="2" align="right">&nbsp;</td>
                        <td colspan="2" align="right">&nbsp;</td>
                    </tr>--%>
                    <tr>
                        <td></td>
                        <td align="center">
                            <b style="font-size: x-large;">PHIẾU NHẬP KHO</b><br />
                        </td>
                        <td>Số: <%=WSI.Import.ImportId%></td>
                    </tr>
                    <tr>
                        <td style="width: 20%;"></td>
                        <td align="center">Ngày <%=WSI.Import.ImportDate.ToString("dd")%> <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> <%=WSI.Import.ImportDate.ToString("MM")%> năm <%=WSI.Import.ImportDate.ToString("yyyy")%></td>
                        <td style="width: 20%;"></td>
                    </tr>
                </table>
            </td>                    
        </tr>
        <tr>
            <td align="center">
                <table style="width: 90%;">
                    <tr>
                        <td align="left" colspan="2"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %>: <%=(WSI.Supplier == null) ? "" : WSI.Supplier.CompanyNameInVietnamese%></td>
                        <%--<td align="left"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>: <%=(WSI.Supplier == null) ? "" : WSI.Supplier.Address%></td>--%>
                    </tr>
                    <tr>
                        <td align="left">Theo đơn đặt hàng: <%=WSI.Import.Reference%></td>
                        <td align="left">Nhập tại kho: <%=(WSI.Stock != null && WSI.Stock.Translation != null) ? WSI.Stock.Translation.Name : ""%> - <%=(WSI.Stock != null && WSI.Stock.Translation != null) ? WSI.Stock.Translation.Address : ""%></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="left"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %>: <%=WSI.Import.DescriptionInVietnamese%></td>
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
                        <th>P/N</th>
                        <th><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                        <%--<th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %><br />yêu cầu</th>--%>
                        <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                <% 
                    if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
                    {
                %>
                        <th><%= NEXMI.NMCommon.GetInterface("SERIAL", langId) %></th>
                <% 
                    }    
                %>
                        <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                    </tr>
                <%
                    int count = 1;
                    NEXMI.NMProductsWSI ProductWSI;
                    foreach (NEXMI.ImportDetails Item in WSI.Details)
                    {
                        ProductWSI = WSI.ProductWSIs.Select(i => i).Where(i => i.Product.ProductId == Item.ProductId).FirstOrDefault();
                %>
                    <tr>
                        <td><%=count++%></td>
                        <td><%=ProductWSI.Translation.Name%></td>
                        <td><%=ProductWSI.Product.BarCode%></td>
                        <td><%=ProductWSI.Product.ProductCode%></td>
                        <td><%=ProductWSI.Unit.Name%></td>
                        <%--<td><%=Item.RequiredQuantity.ToString("N3")%></td>--%>
                        <td><%=Item.GoodQuantity.ToString("N3")%></td>
                    <% 
                        if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
                        {
                    %>
                        <td><%=string.Join(",", (WSI.ProductSNs.Select(i => i).Where(i => i.ProductId == Item.ProductId)).Select(i => i.SerialNumber))%></td>
                    <% 
                        }
                    %>
                        <td><%=Item.Description%></td>
                    </tr>
                <%
                    }
                %>
                </table>
            </td>                    
        </tr>
        <tr>
            <td align="right">Nhập ngày <%=WSI.Import.ImportDate.ToString("dd")%> tháng <%=WSI.Import.ImportDate.ToString("MM")%> năm <%=WSI.Import.ImportDate.ToString("yyyy")%></td>
        </tr>
        <tr>
            <td>
                <table style="width: 100%">
                    <tr align="center">
                        <td style="width: 25%;"><b>Phụ trách bộ phận sử dụng</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Phụ trách cung tiêu</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Người giao hàng</b><br />(Ký, họ tên)</td>
                        <td style="width: 25%;"><b>Thủ kho</b><br />(Ký, họ tên)</td>
                    </tr>
                </table>
            </td>                    
        </tr>
    </table>
</div>