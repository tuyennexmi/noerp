<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/AccountPayable.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {
        check();
        $("#cbType1, #cbType2, #cbType3").click(function () {
            check();
        });
        $(".txtDateTime").datepicker({
            changeMonth: true,
            changeYear: true,
            showOn: "both",
            buttonImage: "/Content/UI_Images/calendar.gif",
            buttonImageOnly: true
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
        $("#divDate1").hide();
        $("#divDate2").hide();
        switch ($('input[name=cbType]:checked').val()) {
            case "1":
                $("#divDate1").hide();
                $("#divDate2").hide();
                $("#btCalculate").hide();
                break;
            case "2":
                $("#divDate1").show();
                $("#divDate2").hide();
                $("#btCalculate").hide();
                break;
            case "3":
                $("#divDate1").hide();
                $("#divDate2").show();
                $("#btCalculate").show();
                break;
        }
    }
    function fnCalculate() {
        var rows = $("#tbAccountPayable").find("tbody>tr");
        if ($('input[name=rdCalculateType]:checked').val() == "allItem") {
            for (var x = 0; x < rows.length - 1; x++) {
                var supplierId = $("#lbSupplierId" + x).html();
                var beginAmount = $("#txtBegin" + x).html().replace(/,/g, "");
                var purchaseAmount = $("#txtPurchase" + x).html().replace(/,/g, "");
                var paidAmount = $("#txtPaid" + x).html().replace(/,/g, "");
                var endAmount = $("#txtEnd" + x).html().replace(/,/g, "");
                $.post(appPath + "Purchase/SaveMonthlyAccountPayable", { supplierId: supplierId, beginAmount: beginAmount,
                    purchaseAmount: purchaseAmount, paidAmount: paidAmount, endAmount: endAmount
                },
                function (data) {
                    if (data != "") {
                        alert(data);
                    }
                });
            }
        } else {
            for (var x = 0; x < rows.length - 1; x++) {
                if ($("#cbItem" + x).is(":checked")) {
                    var supplierId = $("#lbSupplierId" + x).html();
                    var beginAmount = $("#txtBegin" + x).html().replace(/,/g, "");
                    var purchaseAmount = $("#txtPurchase" + x).html().replace(/,/g, "");
                    var paidAmount = $("#txtPaid" + x).html().replace(/,/g, "");
                    var endAmount = $("#txtEnd" + x).html().replace(/,/g, "");
                    $.post(appPath + "Purchase/SaveMonthlyAccountPayable", { supplierId: supplierId, beginAmount: beginAmount,
                        purchaseAmount: purchaseAmount, paidAmount: paidAmount, endAmount: endAmount
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
    string monthYear = "", fromDate = "", toDate = "";
    if (ViewData["MonthYear"] != null)
    {
        monthYear = ViewData["MonthYear"].ToString();
    }
    if (ViewData["FromDate"] != null)
    {
        fromDate = ViewData["FromDate"].ToString();
    }
    if (ViewData["ToDate"] != null)
    {
        toDate = ViewData["ToDate"].ToString();
    }
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="center"></td>
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
                            string strcheck1 = "", strcheck2 = "", strcheck3 = "";
                            if (monthYear == "" && fromDate == "")
                            {
                                strcheck1 = "checked";
                            }
                            else
                            {
                                if (fromDate != "")
                                {
                                    strcheck3 = "checked";
                                }
                                strcheck2 = "checked";
                            }
                        %>
                        <input type="radio" id="cbType1" name="cbType" value="1" <%=strcheck1%> /> Tháng hiện hành
                        <input type="radio" id="cbType2" name="cbType" value="2" <%=strcheck2%> /> Tháng trong quá khứ
                        <input type="radio" id="cbType3" name="cbType" value="3" <%=strcheck3%> /> Chọn ngày
                    </td>
                </tr>
            </table>   
        </td>
    </tr>
    <tr>
        <td>
            <table style="width: 100%">
                <tr>
                    <td>
                        <div id="divDate">
                            <div id="divDate1">
                                Chọn tháng: <input type="text" id="txtMonthYear" value="<%=monthYear%>" style="width: 150px;" />
                            </div>
                            <div id="divDate2">
                                <%= NEXMI.NMCommon.GetInterface("FROM", langId) %> ngày: <input type="text" id="txtFromDate" class="txtDateTime" value="<%=fromDate%>" style="width: 150px;" />
                                <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %> ngày: <input type="text" id="txtToDate" class="txtDateTime" value="<%=toDate%>" style="width: 150px;" />
                                <input type="radio" name="rdCalculateType" value="allItem" checked="checked" /> Tất cả
                                <input type="radio" name="rdCalculateType" value="selectedItem" /> Đã chọn
                                <button onclick="javascript:fnCalculate()" class="button" id="btCalculate">
                                <img alt="" title="" src="<%= Url.Content("~")%>Content/UI_Images/16Ok-apply-icon.png" /><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                            </div>                            
                        </div>
                    </td>
                </tr>
            </table>
            <table class="dataTable" id="tbAccountPayable">
                <thead>
                    <tr>
                        <th>#</th>
                        <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_CODE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_NAME", langId) %></th>
                        <th style="width: 100px;">Nợ đầu kỳ</th>
                        <th style="width: 100px;">Mua</th>
                        <th style="width: 100px;">Trả</th>
                        <th style="width: 100px;">Nợ cuối kỳ</th>
                        <th style="width: 100px;"<%= NEXMI.NMCommon.GetInterface("MAX_DEBT", langId) %>/th>
                    </tr>
                </thead>
                <tbody>
                <%
                    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
                    if (dt != null)
                    {
                        string supplierId = "", supplierName = "";
                        string strBegin = "0", strPurchase = "0", strPaid = "0", strEnd = "0", strMaxDebit = "0";
                        double begin = 0, purchase = 0, paid = 0, end = 0;
                        double totalBegin = 0, totalPurchase = 0, totalPaid = 0, totalEnd = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            supplierId = dt.Rows[i]["SupplierId"].ToString();
                            supplierName = dt.Rows[i]["SupplierName"].ToString();
                            strBegin = "0"; strPurchase = "0"; strPaid = "0"; strEnd = "0";
                            try
                            {
                                begin = double.Parse(dt.Rows[i]["BeginAmount"].ToString());
                                totalBegin += begin;
                                strBegin = begin.ToString("N3");
                            }
                            catch { }
                            try
                            {
                                purchase = double.Parse(dt.Rows[i]["PurchaseAmount"].ToString());
                                totalPurchase += purchase;
                                strPurchase = purchase.ToString("N3");
                            }
                            catch { }
                            try
                            {
                                paid = double.Parse(dt.Rows[i]["PaidAmount"].ToString());
                                totalPaid += paid;
                                strPaid = paid.ToString("N3");
                            }
                            catch { }
                            end = begin + purchase - paid;
                            totalEnd += end;
                            strEnd = end.ToString("N3");
                            try
                            {
                                strMaxDebit = (double.Parse(dt.Rows[i]["MaxDebitAmount"].ToString())).ToString("N3");
                            }
                            catch { }
                %>
                    <tr>
                        <td><input type="checkbox" id="cbItem<%=i%>" /></td>
                        <td><label id="lbSupplierId<%=i%>"><%=supplierId%></label></td>
                        <td><%=supplierName%></td>
                        <td align="right"><label id="txtBegin<%=i%>"><%=strBegin%></label></td>
                        <td align="right"><label id="txtPurchase<%=i%>"><%=strPurchase%></label></td>
                        <td align="right"><label id="txtPaid<%=i%>"><%=strPaid%></label></td>
                        <td align="right"><label id="txtEnd<%=i%>"><%=strEnd%></label></td>
                        <td align="right"><label><%=strMaxDebit%></label></td>
                    </tr>
                <%
                        }
                %>
                    <tr>
                        <td>ZZZZ</td>
                        <td></td>
                        <td></td>
                        <td align="right"><%=totalBegin.ToString("N3")%></td>
                        <td align="right"><%=totalPurchase.ToString("N3")%></td>
                        <td align="right"><%=totalPaid.ToString("N3")%></td>
                        <td align="right"><%=totalEnd.ToString("N3")%></td>
                        <td></td>
                    </tr>
                <% 
                    }    
                %>
                </tbody>
            </table>
        </td>
    </tr>
</table>