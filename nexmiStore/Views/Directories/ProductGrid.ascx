<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%--<script type="text/javascript">
    $(function () {
        $('.button-group').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadProductGrid(page);
            }
        });
        var t;
        $("#txtKeywordProduct").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnLoadProductGrid();
            }, 1000);
        });
        $("#txtKeywordProduct").focus();
    });

    function ChangeCategory() {
        fnLoadProductGrid();
    }
</script>

<div class="divActions">
    <table>
        <tr>
            <td></td>
            <td align="right"><input id="txtKeywordProduct" name="txtKeywordProduct" value="<%=ViewData["Keyword"]%>" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Products;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                    {
                %>
                        <button onclick='javascript:fnShowProductForm("", "<%=ViewData["GroupId"]%>")' class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                    else
                    {
                %>
                        <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <%
                    }    
                %>
                <button class="button" onclick="javascript:$(window).hashchange();"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                <button onclick="javascript:fnLoadImportFromExcel('Products')" class="button">Nhập từ Excel</button>
                <button onclick="javascript:fnExport2Excel()" class="button">Xuất ra Excel</button>
            </td>
            <td style="float: right;">
                <%Html.RenderAction("Pagination", "UserControl", new { currentPage = ViewData["Page"], totalPage = totalPage });%>
                &nbsp;
                <%Html.RenderPartial("ProductSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <table>
                <tr>
                    <td><%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", currentId = ViewData["CategoryId"], objectName = "Products" });%></td>
                </tr>
            </table>
        </div>
    </div>
    
</div>--%>

<%
    List<NEXMI.NMProductsWSI> WSIs = (List<NEXMI.NMProductsWSI>)ViewData["WSIs"];
    int totalPage = 1;
    try
    {
        int totalRows = WSIs[0].TotalRows;
        if (totalRows % NEXMI.NMCommon.PageSize() == 0)
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize();
        }
        else
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
        }
    }
    catch { }
%>
<input type="hidden" value='<%= ViewData["ViewType"] %>' id ='productViewType' />
<table style="width: 100%">
    <tr>
        <td>
            <ul class="ulList">
            <%
                string avatar = "";
                foreach(NEXMI.NMProductsWSI Item in WSIs)
                {
                    avatar = "Content/UI_Images/noimage.png";
                    if (Item.Images.Count > 0)
                    {
                        var ImageDefault = Item.Images.Where(i => i.IsDefault == true).FirstOrDefault();
                        if (ImageDefault == null)
                        {
                            ImageDefault = Item.Images[0];
                        }
                        avatar = "uploads/_thumbs/" + ImageDefault.Name;
                    }
            %>
                <li class="allCorners small-item">
                    <table style="width: 100%;" cellpadding="0" cellspacing="1">
                        <tr>
                            <td valign="top" style="width: 80px">
                                <img alt="" class="avatar" src="<%=Url.Content("~")%><%=avatar%>" onclick="javascript:fnLoadProductDetail('<%=Item.Product.ProductId%>')" />
                            </td>
                            <td valign="top">
                                <table style="width: 100%;">
                                    <tr>
                                        <td colspan="2"><a href="javascript:fnLoadProductDetail('<%=Item.Product.ProductId%>')"><b><%=(Item.Translation.Name == "") ? "[Không tên]" : Item.Translation.Name%>&nbsp;</b></a></td>
                                    </tr>
                                    <tbody>
                                    <tr><td>Mã sản phẩm: </td><td><%=Item.Product.ProductCode%></td></tr>
                                    <%--<tr><td><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %> tồn: </td><td><%=String.Format("{0:0,0}", NEXMI.NMCommon.GetProductQuantity(Item.Product.ProductId, ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId))%></td></tr>--%>
                                    <tr><td>Giá bán: </td><td><%=(Item.PriceForSales == null) ? "0" : String.Format("{0:0,0}", Item.PriceForSales.Price)%></td></tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <div style="padding: 6px;">
                                    <a title="Lịch sử thay đổi giá bán" href="javascript:fnShowPriceForSales('<%=Item.Product.ProductId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/16Chart-icon.png" /></a>
                                    <%--&nbsp;<a title="Lịch sử thay đổi giá nhập" href="javascript:()"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/16Chart-add-icon.png" /></a>--%>
                                    &nbsp;<a title="Chỉnh sửa" href="javascript:fnShowProductForm('<%=Item.Product.ProductId%>', '')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                                </div>
                            </td>
                        </tr>
                    </table>
                </li>
            <%
                } 
            %>
            </ul>
        </td>
    </tr>
</table>

<script type="text/javascript">
    $(function () {
        $('#pagination-Product').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>