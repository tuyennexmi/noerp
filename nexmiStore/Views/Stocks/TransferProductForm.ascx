<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/TransferProducts.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $("#txtDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $('#tabsTransfer').jqxTabs({ theme: theme, keyboardNavigation: false });
        $('#slStocks').attr('disabled', true);
        $('#slToStock option[value=' + $('#slStocks').val() + ']').remove();
    });
</script>
<div id="divImport">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <button onclick="javascript:fnTransfer()" class="button">Chuyển</button>
                        <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                    </div>
                </td>
                <td align="right"></td>
            </tr>
        </table>
    </div>
    
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons"></div>     
            <div class="divStatusBar"></div>
        </div>
        <div class='divContentDetails'>
        <form id="formTransfer" action="">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%= NEXMI.NMCommon.GetInterface("STOCK_MOVE", langId) %></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_AT", langId) %></td>
                                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("IMPORT_AT", langId) %></td>
                                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slToStock" });%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></td>
                                <td><input type="text"  id="txtDate" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" /></td>
                                <td class="lbright"></td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="tabsTransfer">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table id="tbImportDetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <%--<th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th>Đơn vị tính</th>
                                            <th>Số lượng tốt</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("BAD_QUANTITY", langId) %></th>
                                            <th>Serial numbers</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>--%>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("INVENTORY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails"></tbody>
                                    <tfoot id="formExportDetail" class="form-to-validate">
                                        <%Html.RenderAction("ExportDetailForm", "Stocks");%>
                                    </tfoot>
                                </table>
                            </td>
                        </tr>
                        <%--<tr>
                            <td>
                                <a href="javascript:fnShowImportDetail('')">Thêm chi tiết</a>
                                <input type="hidden" id="txtDetailCount" value="0" />
                            </td>
                        </tr>--%>
                        <tr>
                            <td><textarea id="txtDescription" rows="5" cols="0" style="width: 99%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."><%=ViewData["Description"]%></textarea></td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright">Người vận chuyển</td>
                            <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = ViewData["CarrierId"], customerName = ViewData["CarrierName"], elementId = "" });%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                            <td><input type="text" id="txtTransport" value="<%=ViewData["Transport"]%>" /></td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>  
        </div>
    </div>
</div>