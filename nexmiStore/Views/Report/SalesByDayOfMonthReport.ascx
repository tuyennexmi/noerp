<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        var arr1 = new Array();
        var arr3 = new Array();
        arr3[0] = new Array();
        arr3[1] = new Array();

        var count = $("#datenows").val();
        if (count == 0) {
            alert('Không có dữ liệu trong thời gian này!');
            return;
        }
        
        for (var x = 0; x < count; x++) {
            var ngay = parseInt($("#d" + (x + 1)).val());
            var soluong = parseInt($("#n" + (x + 1)).val());
            var tong = parseInt($("#t" + (x + 1)).val());
            while (tong > 1000) {
                tong = tong / 1000;
            }
            arr1[x] = parseInt(x + 1);
            arr3[0].push(soluong);
            arr3[1].push(tong);
        }

        new CanvasXpress("canvas", {
            "y": {
                "vars": ["<%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %>", "<%= NEXMI.NMCommon.GetInterface("VALUE", langId) %>"],
                "smps": arr1,
                "desc": [
                      "triệu đồng",
                      "đơn vị"
                    ],
                "data": arr3
            },
            "a": {
                "xAxis": [
                      "<%= NEXMI.NMCommon.GetInterface("VALUE", langId) %>"
                    ],
                "xAxis2": [
                      "<%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %>"
                    ]
            }

        }, {
            "graphType": "BarLine",
            "useFlashIE": true,
            "title": "Biểu đồ phân tích doanh thu và sản lượng theo ngày trong tháng",
            "graphOrientation": "vertical",
            "backgroundType": "gradient",
            "legendBox": false,
            "backgroundGradient2Color": "rgb(112,222,79)",
            "backgroundGradient1Color": "rgb(248,226,136)",
            "colorScheme": "basic",
            //"oddColor": "rgb(255,255,255)",
            //"evenColor": "rgba(226,236,248,0.9)",
            "legendBackgroundColor": false,
            "lineType": "spline",
            "smpHairlineColor": "rgb(100,100,100)",
            "xAxisTickColor": "rgb(100,100,100)",
            "smpTitle": "Ngày",
            "showAnimation": true,
            "coordinateLineColor": false
        })
    });

    $("#thismonth").click(function () {
        var now = new Date();
        $.post(appPath + "Report/SalesByDayOfMonthReport", { thang: now.getMonth() + 1 }, function (data) {
            $("#pagemonth").html(data);
        });
    });

    $("#lastmonth").click(function () {
        var now = new Date();
        $.post(appPath + "Report/SalesByDayOfMonthReport", { thang: now.getMonth() }, function (data) {
            $("#pagemonth").html(data);
        });
    });

    function changedate() {
        var str = $("#month").val();
        if (str != "") {
            var year = str.split("-");
            $.post(appPath + "Report/SalesByDayOfMonthReport", { thang: year[1], nam: year[0] }, function (data) {
                $("#pagemonth").html(data);
            });
        }
    }

</script>
<%    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
      int datenow = 0;
      if (dt.Rows.Count > 0)
      {
          datenow = Int32.Parse(dt.Rows[0]["ngay"].ToString());
      }
      String IsDisp = "";
      if (ViewData["IsTable"].ToString() != "")
          IsDisp = "style=\"display:" + ViewData["IsTable"].ToString() + "\"";
     %>

<div id="pagemonth">
<div class="divActions">
    <input type="hidden" id="datenows" value="<%=datenow %>" />
    <div style="margin:20px;" >
        <a id="thismonth">Tháng này</a> | <a id="lastmonth">Tháng trước</a> | <%--<a id="A2">24 giờ qua</a> |--%>
        Tùy chọn khác: <input type="month" id="month" value="" />
        <input type="submit" onclick="changedate()" value="Xem" />
    </div>
</div>
<div class="divContent">
<div style="margin:10px; float:left">
    <canvas id="canvas" width="800px" height="500px"></canvas>
    <%--<canvas id="canvas" width="<%=ViewData["width"]%>" height="<%=ViewData["height"]%>"></canvas>--%>
</div>
<div style="margin:15px;">
    <table class="tbDetails" <%=IsDisp%> width=25%>
        <tr><td colspan = 3>BẢNG SỐ LIỆU TỔNG HỢP</td></tr>
        <tr>
            <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
            <th>Số Lượng</th>
            <th>Tổng Tiền</th>
        </tr>
        
            <%
                double total = 0; double tong = 0.0;
                bool flag = false;
                String sl = "";
                for (int j = datenow; j >= 1; j--)
                {   
                    flag = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (j == Int32.Parse(dt.Rows[i]["ngay"].ToString()))
                        {
                            tong = double.Parse(dt.Rows[i]["tong"].ToString());
                            sl = dt.Rows[i]["soluong"].ToString();                            
                            %>
                            <tr>                                
                                <td><%=dt.Rows[i]["ngay"]%>/<%=dt.Rows[i]["thang"]%>/<%=dt.Rows[i]["nam"]%>
                                    <input type="hidden" id="d<%=dt.Rows[i]["ngay"]%>" value="<%=dt.Rows[i]["ngay"]%>" />
                                </td>
                                <td align=right><%=dt.Rows[i]["soluong"]%>
                                    <input type="hidden" id="n<%=dt.Rows[i]["ngay"]%>" value="<%=dt.Rows[i]["soluong"]%>" />
                                </td>
                                <td align=right><%=tong.ToString("N3")%>
                                    <input type="hidden" id="t<%=dt.Rows[i]["ngay"]%>" value="<%=tong%>" />
                                </td>
                            </tr>
                            <%                            
                            total += tong;
                            flag = true;
                            break;
                        }
                    }
                    if (flag == false)
                    {
                        %>
                            <tr>                                
                                <td><%=j%>/<%=dt.Rows[0]["thang"]%>/<%=dt.Rows[0]["nam"]%>
                                    <input type="hidden" id="d<%=j%>" value="0" />
                                </td>
                                <td align=right>0
                                    <input type="hidden" id="n<%=j%>" value="0" />
                                </td>
                                <td align=right>0
                                    <input type="hidden" id="t<%=j%>" value="0" />
                                </td>
                            </tr>
                        <%
                    }
                }
            %>            
            <tr>
                <td colspan=2><b> TỔNG DOANH THU</b></td>
                <td align=right><%=total.ToString("N3")%></td>
            </tr>
    </table>
</div>
</div>
</div>