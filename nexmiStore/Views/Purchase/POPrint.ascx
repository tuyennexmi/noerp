<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMPurchaseOrdersWSI WSI = (NEXMI.NMPurchaseOrdersWSI)ViewData["WSI"];
    String path = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
    NEXMI.NMCustomersWSI CWSI = (NEXMI.NMCustomersWSI)ViewData["Customer"];
%>

<div style="font-size: medium">
    <table width="100%" border=".1">
        <tr>
            <td><img  alt="" src="<%=path%>" width="180px" /></td>
            <td><h1><%=ViewData["Tilte"]%></h1></td>
            <td>
                <table class="tbDetails" width="100%" style="text-align: right;" >
                    <tr>
                        <td >Our ref</td>
                        <td>ICT-TT-10/02</td>
                    </tr>
                    <tr>
                        <td >PO No.</td>
                        <td><%=ViewData["Id"]%></td>
                    </tr>
                    <tr>
                        <td >Date</td>
                        <td><%=WSI.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                    </tr>
                    <tr>
                        <td >Revision</td>
                        <td><%%></td>
                    </tr>
                    <tr>
                        <td >Page</td>
                        <td><%%></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
    <br />
    <table width="100%" border=".1" cellpadding="2" cellspacing="0">
        <tr>
            <td colspan='2' align="center"><h3>SELLER</h3></td>
            <td colspan='2' align="center"><h3>BUYER</h3></td>
        </tr>
        <tr>
            <td width="45%">Company Name</td>
            <td ><%=WSI.Supplier.CompanyNameInVietnamese%></td>
            <td width="45%">Company Name</td>
            <td ><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese%></td>
        </tr>
        <tr>
            <td width="45%">Address</td>
            <td><%=WSI.Supplier.Address%></td>
            <td width="45%">Address</td>
            <td><%=NEXMI.NMCommon.GetCompany().Customer.Address%></td>
        </tr>
        <tr>
            <td width="45%">State</td>
            <td><%=CWSI.AreaWSI.Area.Name%></td>
            <td width="45%">State</td>
            <td><%=NEXMI.NMCommon.GetCompany().AreaWSI.Area.Name%></td>
        </tr>
        <tr>
            <td width="45%">Zip Code</td>
            <td ><%=CWSI.AreaWSI.Area.ZipCode%></td>
            <td width="45%">Zip Code</td>
            <td ><%=NEXMI.NMCommon.GetCompany().AreaWSI.Area.ZipCode%></td>
        </tr>
        <tr>
            <td width="45%">Country</td>
            <td ><%=(CWSI.AreaWSI.Country != null)? CWSI.AreaWSI.Country.Name : "" %></td>
            <td width="45%">Country</td>
            <td><%=(NEXMI.NMCommon.GetCompany().AreaWSI.Country != null)? NEXMI.NMCommon.GetCompany().AreaWSI.Country.Name : ""%></td>
        </tr>
        <tr>
            <td width="45%">Tel No.</td>
            <td><%=WSI.Supplier.Telephone%></td>
            <td width="45%">Tel No.</td>
            <td><%=WSI.Supplier.Telephone%></td>
        </tr>
        <tr>
            <td width="45%">Fax No.</td>
            <td><%=WSI.Supplier.FaxNumber%></td>
            <td width="45%">Fax No.</td>
            <td><%=WSI.Supplier.FaxNumber%></td>
        </tr>
        <tr>
            <td width="45%">Contact Person</td>
            <td><%=ViewData["CustomerAntt"]%></td>
            <td width="45%">Contact Person</td>
            <td><%=WSI.CreatedUser.CompanyNameInVietnamese%></td>
        </tr>
        <tr>
            <td width="45%">Transportation</td>
            <td><%=WSI.Order.Transportation%></td>
            <td width="45%">Transportation</td>
            <td><%=WSI.Order.Transportation%></td>
        </tr>
        <tr>
            <td width="45%">Delivery Term</td>
            <td><%=WSI.Order.Delivered%></td>
            <td width="45%">Delivery Term</td>
            <td><%=WSI.Order.Delivered%></td>
        </tr>
        <tr>
            <td width="45%">Country of Origin</td>
            <td><%=WSI.Order.Transportation%></td>
            <td width="45%"></td>
            <td></td>
        </tr>
        <tr>
            <td width="45%">Port of loading</td>
            <td><%=WSI.Order.Reference%></td>
            <td width="45%">Port of destination</td>
            <td><%=WSI.Order.Reference%></td>
        </tr>
    </table>

    <br />
    
    <table class="tbDetails" border=".1">
        <thead>
            <tr style="background-color:Gray; font-weight: bold">
                <th width="5%">No</th>
                <th >Description</th>
                <th width="12%">Quantity</th>
                <th width="8%">Unit</th>
                <th width="18%">Unit Price </th>
                <th width="18%">Amount</th>
            </tr>
        </thead>
        <tbody>
    <%  int count = 1;        
        foreach (NEXMI.PurchaseOrderDetails Item in WSI.Details)
        {
    %>
            <tr>
                <td><%=count++%></td>
                <td><%= NEXMI.NMCommon.GetName(Item.ProductId, langId) + "<br />" + Item.Description %></td>
                <td align="right"><%=Item.Quantity.ToString("N3") %></td>
                <td ><%=NEXMI.NMCommon.GetProductUnit(Item.ProductId)%></td>
                <td align="right"><%=Item.Price.ToString("N3") %></td>
                <td align="right"><%=Item.Amount.ToString("N3") %></td>
            </tr>
    <% 
        }    
    %>
            <tr>
                <td colspan="5"><b>Total Amount</b></td>
                <td align="right" style="width: 120px;"><label id="lbTotalAmount"> USD <%=WSI.Order.TotalAmount.ToString("N3")%></label></td>
            </tr>
            <tr>
                <td colspan="6"><b>Note: </b><%=WSI.Order.Description%></td>
                
            </tr>
        </tbody>
    </table>
    <table style="width:100%; text-align:center">
        <tr align="center">
            <td style="width:50%; ">For and on Behalf of the Seller</td>
            <td style="width:50%; ">For and on Behalf of the Buyer</td>
        </tr>
        <tr><td> <br /></td><td></td>        </tr>
        <tr align="center">
            <td style="width:50%;"></td>
            <td style="width:50%;"><%=WSI.CreatedUser.CompanyNameInVietnamese%></td>
        </tr>
    </table>

    <h4><i>Thank you for your business!</i></h4>

    <%Html.RenderPartial("ReportFooter"); %>
</div>