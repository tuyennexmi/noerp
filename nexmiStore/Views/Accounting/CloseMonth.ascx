<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/CloseMonth.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {
        check();
        $("#cbType1, #cbType2, #cbType3, #cbType4").click(function () {
            check();
        });
        $("#txtMonthYear").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'mm/yy',
            showOn: "both",
            showButtonPanel: true,
            buttonImage: "/Content/UI_Images/calendar.gif",
            buttonImageOnly: true,
            onClose: function (dateText, inst) {
                var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                $(this).datepicker('setDate', new Date(year, month, 1));
            }
        });
    });
    function check() {

    }
    function fnCalculate() {
        var rows = $("#tbProductInStocks").find("tbody>tr");
        if ($('input[name=rdCalculateType]:checked').val() == "allItem") {
            for (var x = 0; x < rows.length; x++) {
                var stockId = $("#StockId" + x).val();
                var productId = $("#lbProductId" + x).html();
                var beginQuantity = $("#txtBegin" + x).html().replace(/,/g, "");
                var importQuantity = $("#txtImport" + x).html().replace(/,/g, "");
                var exportQuantity = $("#txtExport" + x).html().replace(/,/g, "");
                var endQuantity = $("#txtEnd" + x).html().replace(/,/g, "");
                $.post(appPath + "Stocks/SaveMonthlyInventoryControl", { stockId: stockId, productId: productId, beginQuantity: beginQuantity,
                    importQuantity: importQuantity, exportQuantity: exportQuantity, endQuantity: endQuantity
                },
                function (data) {
                    if (data != "") {
                        alert(data);
                    }
                });
            }
        } else {
            for (var x = 0; x < rows.length; x++) {
                if ($("#cbItem" + x).is(":checked")) {
                    var stockId = $("#StockId" + x).val();
                    var productId = $("#lbProductId" + x).html();
                    var beginQuantity = $("#txtBegin" + x).html().replace(/,/g, "");
                    var importQuantity = $("#txtImport" + x).html().replace(/,/g, "");
                    var exportQuantity = $("#txtExport" + x).html().replace(/,/g, "");
                    var endQuantity = $("#txtEnd" + x).html().replace(/,/g, "");
                    $.post(appPath + "Stocks/SaveMonthlyInventoryControl", { stockId: stockId, productId: productId, beginQuantity: beginQuantity,
                        importQuantity: importQuantity, exportQuantity: exportQuantity, endQuantity: endQuantity
                    },
                    function (data) {
                        if (data != "") {
                            alert(data);
                        }
                    });
                }
            }
        }
    }
</script>
<% 
    string closeType = "", monthYear = DateTime.Today.ToString("MM/yyyy");
    if (ViewData["CloseType"] != null)
    {
        closeType = ViewData["CloseType"].ToString();
    }
    if (ViewData["MonthYear"] != null)
    {
        monthYear = ViewData["MonthYear"].ToString();
    }
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                <button onclick="javascript:fnLoadUpdateStock()" class="button">Đóng sổ</button>
            </td>
            <td></td>
        </tr>
    </table>
</div>
<table style="width: 100%" class="tbTemplate">
    <tr>
        <td>
            <table style="width: 100%">
                <tr>
                    <td>
                        <% 
                            string strcheck1 = "", strcheck2 = "", strcheck3 = "", strcheck4 = "";
                            switch (closeType)
                            {
                                case "1": strcheck1 = "checked"; break;
                                case "2": strcheck2 = "checked"; break;
                                case "3": strcheck3 = "checked"; break;
                                case "4": strcheck4 = "checked"; break;
                                default: strcheck1 = "checked"; break;
                            }
                        %>
                        Chọn tháng: <input type="text" id="txtMonthYear" value="<%=monthYear%>" style="width: 50px;" readonly="readonly" />
                        <input type="radio" id="cbType1" name="cbType" value="1" <%=strcheck1%> /> Số dư
                        <input type="radio" id="cbType2" name="cbType" value="2" <%=strcheck2%> /> Thu
                        <input type="radio" id="cbType3" name="cbType" value="3" <%=strcheck3%> /> Chi
                        <input type="radio" id="cbType4" name="cbType" value="4" <%=strcheck4%> /> Xuất - nhập - tồn
                    </td>
                    <td align="right" valign="bottom">
                        
                    </td>
                </tr>
            </table>
            <% 
                monthYear = "09/2012";
                switch (closeType)
                {
                    case "1":
                        ArrayList cols = new ArrayList();
                        cols.Add("IssueId");
                        cols.Add("IssueDate");
                        cols.Add("Description");
                        cols.Add("ReceiptAmount");
                        cols.Add("PaymentAmount");
                        cols.Add("BalanceAmount");
                        Dictionary<String, object> criterias = new Dictionary<String, object>();

                        NEXMI.NMDataTableBL BL = new NEXMI.NMDataTableBL();

                        NEXMI.NMDataTableWSI WSI = new NEXMI.NMDataTableWSI();
                        WSI.QueryString = "SELECT P.PaymentId, P.PaymentDate, P.DescriptionInVietnamese, 0 AS ReceiptAmount, P.PaymentAmount, 0 AS BalanceAmount "
                        + "FROM Payments P WHERE (RIGHT('0' + RTRIM(MONTH(P.PaymentDate)), 2) + '/' + RIGHT('0' + RTRIM(YEAR(P.PaymentDate)), 4)) = :monthYear";
                        criterias.Add("monthYear", monthYear);
                        WSI.Columns = cols;
                        WSI.Criterias = criterias;
                        WSI = BL.GetData(WSI);
                        System.Data.DataTable dt = WSI.Data;
                        WSI = new NEXMI.NMDataTableWSI();
                        WSI.QueryString = "SELECT R.ReceiptId, R.ReceiptDate, R.DescriptionInVietnamese, R.ReceiptAmount, 0 AS PaymentAmount, 0 AS BalanceAmount "
                        + "FROM Receipts R WHERE (RIGHT('0' + RTRIM(MONTH(R.ReceiptDate)), 2) + '/' + RIGHT('0' + RTRIM(YEAR(R.ReceiptDate)), 4)) = :monthYear";
                        WSI.Columns = cols;
                        WSI.Criterias = criterias;
                        WSI = BL.GetData(WSI);
                        System.Data.DataTable dt2 = WSI.Data;
            %>
            <table id="tbView">
                <thead>
                    <tr>
                        <th>Số chứng từ</th>
                        <th><%= NEXMI.NMCommon.GetInterface("RECORD_DATE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                        <th>Tiền thu</th>
                        <th>Tiền chi</th>
                        <th>Tồn quỹ</th>
                    </tr>
                </thead>
                <tbody>
            <%
                        for (int i = 0; i < dt.Rows.Count; i++)
                        { 
            %>
                    <tr>
                        <td><%=dt.Rows[i]["IssueId"].ToString()%></td>
                        <td><%=dt.Rows[i]["IssueDate"].ToString()%></td>
                        <td><%=dt.Rows[i]["Description"].ToString()%></td>
                        <td align="right"><%=(double.Parse(dt.Rows[i]["ReceiptAmount"].ToString())).ToString("N3")%></td>
                        <td align="right"><%=(double.Parse(dt.Rows[i]["PaymentAmount"].ToString())).ToString("N3")%></td>
                        <td align="right"><%=(double.Parse(dt.Rows[i]["BalanceAmount"].ToString())).ToString("N3")%></td>
                    </tr>
            <%
                        }
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        { 
            %>
                    <tr>
                        <td><%=dt2.Rows[i]["IssueId"].ToString()%></td>
                        <td><%=dt2.Rows[i]["IssueDate"].ToString()%></td>
                        <td><%=dt2.Rows[i]["Description"].ToString()%></td>
                        <td align="right"><%=(double.Parse(dt2.Rows[i]["ReceiptAmount"].ToString())).ToString("N3")%></td>
                        <td align="right"><%=(double.Parse(dt2.Rows[i]["PaymentAmount"].ToString())).ToString("N3")%></td>
                        <td align="right"><%=(double.Parse(dt2.Rows[i]["BalanceAmount"].ToString())).ToString("N3")%></td>
                    </tr>
            <%
                        }
            %>
                </tbody>
            </table>
            <%
                        break;
                    case "2":

                        break;
                    case "3":

                        break;
                    case "4":

                        break;
                }
            %>
        </td>
    </tr>
</table>