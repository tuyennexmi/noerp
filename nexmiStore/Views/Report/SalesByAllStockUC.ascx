<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {

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

            arr3.push(parseInt(tong[x].value) / 1000000);
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
            "title": "Báo cáo doanh thu tất cả cửa hàng",
            "graphOrientation": "vertical",
            "backgroundType": "gradient",
            "legendBox": false,
            "backgroundGradient2Color": "rgb(113,222,80)",
            "backgroundGradient1Color": "rgb(246,226,135)",
            "colorScheme": "basic",
            "legendBackgroundColor": false,
            "lineType": "spline",
            "smpHairlineColor": "rgb(100,100,100)",
            "xAxisTickColor": "rgb(100,100,100)",
            "smpTitle": "Cửa hàng",
            "showAnimation": true,
            "coordinateLineColor": false,
            "smpLabelFontSize": 10,
            "autoScaleFont": false,
            "maxSmpStringLen": 50
        })
        $("#custom").hide();
    });

    
</script>

<%
    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
    double total = 0;
%>

<table width="100%">
    <tr valign="top">
        <td>
            <canvas id="canvas" width="800px" height="500px"></canvas>
        </td>
        <td style="padding:8px;" width="100%">
            <h3>BẢNG SỐ LIỆU TỔNG HỢP</h3>
            <table class="tbDetails">
            <thead>
                <tr>
                    <th>Cửa hàng</th>
                    <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                    <th>Tổng</th>
                </tr>    
            </thead>
            <tbody>
    <%
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            total += Double.Parse(dt.Rows[i]["tong"].ToString());
    %>
                <tr>            
                    <td align="left"><%=NEXMI.NMCommon.GetName(dt.Rows[i]["Id"].ToString(), langId)%></td>
                    <td align="right"><%=dt.Rows[i]["soluong"].ToString()%></td>
                    <td align="right"><%=Double.Parse(dt.Rows[i]["tong"].ToString()).ToString("N3")%>
                    <input type="hidden" name="thangnam" value="<%=NEXMI.NMCommon.GetName(dt.Rows[i]["Id"].ToString(), langId)%>" />
                    <input type="hidden" name="soluong" value="<%=dt.Rows[i]["soluong"].ToString()%>" />
                    <input type="hidden" name="tong" value="<%=dt.Rows[i]["tong"].ToString()%>" />
                    </td>
                </tr>
    <%
        }    
    %>
            </tbody>
            <tfoot>
                <tr>
                    <th colspan="2">Tổng doanh số</th>
                    <td align="right"><%=total.ToString("N3")%></td>
                </tr>
            </tfoot>
            </table>
            
        </td>
    </tr>
</table>
