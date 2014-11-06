<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#fromdate, #todate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        var thang = document.getElementsByName("thangnam");
        var soluong = document.getElementsByName("soluong");
        var tong = document.getElementsByName("tong");
        var arr1 = new Array();
        var arr2 = new Array();
        var arr3 = new Array();

        for (var x = 0; x < thang.length; x++) {
            arr1.push(thang[x].value);
        }
        for (var x in soluong) {
            arr2.push(soluong[x].value);
        }
        for (var x in tong) {

            arr3.push(tong[x].value);
        }

        new CanvasXpress("canvas", {
            "y": {
                "vars": [
                      "<%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>",
                      "<%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %>"
                    ],
                "smps": arr1,
                "desc": [
                      "Đơn vị triệu đồng",
                      "Đơn vi đếm"
                    ],
                "data": [arr3, arr2]
            },
            "a": {
                "xAxis": [
                      "<%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>"
                    ],
                "xAxis2": [
                      "<%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %>"
                    ]
            }
        }, {
            "graphType": "BarLine",
            "useFlashIE": true,
            "title": "Báo cáo doanh thu của tất cả nhân viên",
            "graphOrientation": "vertical",
            "backgroundType": "gradient",
            "legendBox": false,
            "backgroundGradient2Color": "rgb(112,179,222)",
            "backgroundGradient1Color": "rgb(226,236,248)",
            "colorScheme": "basic",
            "legendBackgroundColor": false,
            "lineType": "spline",
            "backgroundGradient2Color": "rgb(112,222,79)",
            "backgroundGradient1Color": "rgb(248,226,136)",
            "smpTitle": "Tên nhân viên",
            "showAnimation": true,
            "coordinateLineColor": false,
            "smpLabelFontSize": 10,
            "autoScaleFont": false,
            "maxSmpStringLen": 50
        })
        $("#custom").hide();
    });
    function change(mon) {
        var dates = new Date();
        var year = dates.getFullYear();
        if (dates.getMonth() == 12) {
            year = dates.getFullYear() - 1;
        }
        var endmonth = 28;
        if (dates.getMonth() == "1" || dates.getMonth() == "3" || dates.getMonth() == "5" || dates.getMonth() == "7" || dates.getMonth() == "8" || dates.getMonth() == "10" || dates.getMonth() == "12") {
            endmonth = 31;
        }
        if (dates.getMonth() == "4" || dates.getMonth() == "6" || dates.getMonth() == "9" || dates.getMonth() == "11") {
            endmonth = 30;
        }
        if (dates.getMonth() == "2") {
            if (Int32.Parse(year) % 100 != 0 && Int32.Parse(year) % 4 == 0) {
                endmonth = 29;
            } else {
                endmonth = 28;
            }
        }
        if (mon != "" && mon != null && mon != undefined) {
            if (mon == "previous") {
                var fromdate = year + "-" + dates.getMonth() + "-1";
                var todate = year + "-" + dates.getMonth() + "-" + endmonth;
            }
            if (mon == "30") {

                var monthprevious = endmonth - (30 - dates.getDate());
                var fromdate = year + "-" + dates.getMonth() + "-" + monthprevious;
                var todate = dates.getFullYear() + "-" + (dates.getMonth() + 1) + "-" + dates.getDate();

            }
        }
        else {
            var fromdate = $("#fromdate").val();
            var todate = $("#todate").val();
        }
        var fromtime = $("#fromtime").val();
        var totime = $("#totime").val();

        $.post(appPath + "Report/SalesByAllStaff", { fromtime: fromtime, fromdate: fromdate, totime: totime, todate: todate }, function (data) {
            $("#pagetop").html(data);
        });
    }
</script>

<%
    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
%>
<table width="100%">
    <tr valign="top">
        <td>
            <canvas id="canvas" width="800px" height="500px"></canvas>
        </td>
        <td style="padding:8px;">
            <table class="tbDetails" width=26%>
            <tr>
                <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                <th>Tổng</th>
            </tr>    
            <%
                for (int i = 0; i < dt.Rows.Count; i++)
                {         
                    %>
                    <tr>            
                        <td><%=dt.Rows[i]["CompanyNameInVietnamese"].ToString()%></td>
                        <td><%=dt.Rows[i]["soluong"].ToString()%></td>
                        <td align=right><%= double.Parse(dt.Rows[i]["tong"].ToString()).ToString("N3")%>
                        <input type="hidden" name="thangnam" value="<%=dt.Rows[i]["CompanyNameInVietnamese"].ToString()%>" />
                        <input type="hidden" name="soluong" value="<%=dt.Rows[i]["soluong"].ToString()%>" />
                        <input type="hidden" name="tong" value="<%=dt.Rows[i]["tong"].ToString()%>" />
                        </td>
                    </tr>
                    <%
                }    
            %>
            </table>
        </td>
    </tr>
</table>
