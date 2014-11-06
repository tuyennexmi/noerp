<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        var arr1 = new Array();
        var arr2 = new Array();
        var arr3 = new Array();
        count = $("#hournows").val();

        for (var x = 0; x <= count; x++) {
            var gio = parseInt($("#n" + (x)).val());
            var soluong = parseInt($("#s" + x).val());
            var tong = parseInt($("#t" + (x)).val());
            while (tong > 1000) {
                tong = tong / 1000;
            }
            if (x == gio) {
                arr2.push(tong);
                arr3.push(soluong);
            }
            else {
                arr2.push(0);
                arr3.push(0);
            }
            arr1.push(x);
        }
        new CanvasXpress("canvas", {
            "y": {
                "vars": [
                    "<%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>",
                    "<%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %>"
                ],
                "smps": arr1,
                "desc": [
                    "triệu đồng",
                    "đơn vị"
                ],
                "data": [arr2, arr3]
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
            "title": "Biểu đồ phân tích doanh thu và sản lượng theo giờ trong ngày",
            "graphOrientation": "vertical",
            "backgroundType": "gradient",
            "legendBox": false,
            "backgroundGradient2Color": "rgb(112,222,79)",
            "backgroundGradient1Color": "rgb(248,226,136)",
            "colorScheme": "basic",
            "legendBackgroundColor": false,
            "lineType": "spline",
            "smpHairlineColor": "rgb(100,100,100)",
            "xAxisTickColor": "rgb(100,100,100)",
            "smpTitle": "Giờ",
            "showAnimation": true,
            "coordinateLineColor": false
        })
        var now = new Date();
        document.getElementById("ngay").value = now.toDateString();
    });
    
</script> 
<%
    double total = 0; double tong = 0;
    int hournow = 0;
    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
    if (dt.Rows.Count > 0)
    {
        hournow = Int32.Parse(dt.Rows[0]["gio"].ToString());
    }
%>
    
<div style="padding:10px">
             
</div>

<table class="tbDetails" width=26%>
    <tr><td colspan = 4>BẢNG SỐ LIỆU TỔNG HỢP</td></tr>

    <tr>
        <td>
            <canvas id="canvas" width="800px" height="500px"></canvas>
        </td>
        <td>
            <table>
                <tr>
                    <th>Giờ</th>
                    <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
                    <th>Số Lượng</th>
                    <th>Tổng Tiền</th>
                </tr>
    <%
    bool flag = false;
    for (int j = hournow; j >= 0; j--)
    {
        flag = false;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (j == Int32.Parse(dt.Rows[i]["gio"].ToString()))
            {
                tong = double.Parse(dt.Rows[i]["tong"].ToString());
                %>

                <tr>                                
                    <td><%=dt.Rows[i]["gio"]%></td>
                    <td><%=dt.Rows[i]["ngay"]%>/<%=dt.Rows[i]["thang"]%>/<%=dt.Rows[i]["nam"]%></td>
                    <td align=right><%=dt.Rows[i]["soluong"]%>
                        <input type="hidden" id="n<%=dt.Rows[i]["gio"]%>" value="<%=dt.Rows[i]["gio"]%>" />
                        <input type="hidden" id="s<%=dt.Rows[i]["gio"]%>" value="<%=dt.Rows[i]["soluong"]%>" />
                    </td>
                    <td align=right><%=tong.ToString("N3")%><input type="hidden" id="t<%=dt.Rows[i]["gio"]%>" value="<%=tong%>" /></td>
                </tr>
                <%
                total += tong;
                flag = true;
                break;
            }
        }
        if (flag == false && dt.Rows.Count > 0)
        {
            %>
                <tr>
                    <td><%=j%></td>
                    <td><%=dt.Rows[0]["ngay"]%>/<%=dt.Rows[0]["thang"]%>/<%=dt.Rows[0]["nam"]%></td>
                    <td align=right>0</td>
                    <td align=right>0</td>
                </tr>
            <%
        }

    }%>
                <tr>
                    <td colspan=3><b>TỔNG DOANH THU</b></td>
                    <td align=right><%=total.ToString("N3")%></td>
                </tr>
            </table>
        </td>
            
    </tr>
</table>