<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string functionProduct = NEXMI.NMConstant.Functions.Products;
    string productId = ViewData["ProductId"].ToString();
    string previousId = NEXMI.NMCommon.PreviousId(productId, "ProductId", "Products", "");
    string nextId = NEXMI.NMCommon.NextId(productId, "ProductId", "Products", "");
%>

<script type="text/javascript">
    $(function () {
        $("#productTabs").jqxTabs({ theme: theme, keyboardNavigation: false });

        $('#productTabs').bind('selected', function (event) {
            var item = event.args.item;
            switch (item) {
//                case 2:
//                    LoadContentDynamic('BoMs', 'Directories/BoMs', { productId: '<%=productId %>' });
                case 5:
                    LoadContentDynamic('ProductInStocks', 'Directories/GetProductInStocks', { productId: '<%=productId %>' });
                    break;
                case 6:
                    LoadContentDynamic('SupplierOfProduct', 'Directories/SupplierOfProduct', { productId: '<%=productId %>' });
                    break;
                case 7:
                    LoadContentDynamic('CustomersBuyProduct', 'Directories/CustomersBuyProduct', { productId: '<%=productId %>' });
                    break;
            }
        });
    });
    
</script>

<div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <%
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionProduct, "Insert"))
                        {
                    %>
                    <button onclick='javascript:fnShowProductForm("", "<%=ViewData["TypeId"]%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                        //Kiểm tra quyền Update
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionProduct, "Update"))
                        {
                    %>
                    <button onclick="javascript:fnShowProductForm('<%=productId%>', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                    <% 
                        }
                        //Kiểm tra quyền Delete
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionProduct, "Delete"))
                        {
                    %>
                    <button onclick="javascript:fnDeleteProduct('<%=productId%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                    <% 
                        }
                    %>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <input id="btNext" type="hidden" value="<%=nextId%>" />
                                <ul class="button-group">
	                                <li title="Trước"><button class="button" onclick="javascript:fnLoadProductDetail('<%=previousId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                    <li title="Sau"><button class="button" onclick="javascript:fnLoadProductDetail('<%=nextId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                                </ul>
                            </td>
                            <td>   
                                <%Html.RenderPartial("ProductSV"); %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
</div>
<div class="divContent">
    <div class="divStatus">
    </div>
    <div class='divContentDetails'>
        <table style="width: 100%" class="frmInput">
            <tr>
                <td>
                    <table style="width: 100%" class="frmInput">
                        <tr>
                            <td style="width: 120px;" align="center" rowspan="3">
                                <img id="avatar" alt="" src="<%=Url.Content("~")%><%=ViewData["Avatar"]%>" style="width: 100px; height: 100px;" />
                            </td>
                            <td valign="top" style="font-weight: bold">
                                <h1><%=ViewData["ProductName"]%> (<%=ViewData["ProductCode"]%>)</h1>
                                <h2><%=ViewData["CategoryName"]%></h2>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div id="productTabs">
            <ul>
                <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("COMMON_INFO", langId)%></li>
                <li><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></li>
                <li><%= NEXMI.NMCommon.GetInterface("BOM", langId)%></li>
                <li><%= NEXMI.NMCommon.GetInterface("PICTURE", langId) %></li>
                <li><%= NEXMI.NMCommon.GetInterface("DOCUMENTS", langId) %></li>
                <li><%= NEXMI.NMCommon.GetInterface("INVENTORY", langId)%></li>
                <li><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId)%></li>
                <li><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId)%></li>
            </ul>
            <div style="padding: 10px;">
                <table style="width: 100%" class="frmInput">
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("UNIT", langId)%></td>
                        <td><%=ViewData["slProductUnits"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("VAT", langId) %></td>
                        <td><%=ViewData["VAT"]%></td>
                    </tr>
                    <%if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.PRICE_BY_STORE))
                      { %>
                    <tr>
                        <td class="lbright">Giá gốc</td>
                        <td><%=ViewData["CostPrice"]%></td>
                        <td class="lbright">Giá bán</td>
                        <td><%=ViewData["Price"]%></td>
                    </tr>
                    <%} %>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MAINTAIN", langId)%></td>
                        <td><%=ViewData["Warranty"]%></td>
                        <td class="lbright">Barcode</td>
                        <td><%=ViewData["BarCode"]%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MANUFACTURE", langId)%></td>
                        <td><%=ViewData["CustomerName"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("KIND", langId) %></td>
                        <td><%=GlobalValues.GetACCType(ViewData["TypeId"].ToString()).AccountNumber.NameInVietnamese%></td>
                    </tr>
                    <%--<tr>
                        <td class="lbright">Ngưng bán</td>
                        <td><input type="checkbox" id="cbDiscontinued" <%=ViewData["Discontinued"]%> disabled="disabled" /></td>
                        <td class="lbright">Nhóm sản phẩm</td>
                        <td><%=NEXMI.NMCommon.GetGroupName(ViewData["GroupId"].ToString())%></td>
                    </tr>--%>
                </table>
            </div>
            <div style="padding: 10px;"><%=ViewData["Description"]%></div>
            <div style="padding: 10px;" id='BoMs'>
                <%Html.RenderAction("BoMs", new { productId = productId, objs = ViewData["BoMs"] });%>
            </div>
            <div style="padding: 10px;">
                <%Html.RenderAction("DynamicUploader", "Files", new { elementId = "img_uploader", owner = productId, type = NEXMI.NMConstant.FileTypes.Images });%>
            </div>
            <div style="padding: 10px;">
                <%Html.RenderAction("DynamicUploader", "Files", new { elementId = "docs_uploader", owner = productId, type = NEXMI.NMConstant.FileTypes.Docs });%>
            </div>
            <%--tình hình tồn kho--%>
            <div id="ProductInStocks"></div>
            <div id="SupplierOfProduct"></div>
            <div id="CustomersBuyProduct"></div>
        </div>
    
        <%Html.RenderAction("Logs", "Messages", new { ownerId = productId, sendTo = ViewData["SendTo"] });%>
    </div>
</div>