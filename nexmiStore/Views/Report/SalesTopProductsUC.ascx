<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
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
            "title": "TOP sản phẩm bán chạy",
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
            "smpTitle": "<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %>",
            "showAnimation": true,
            "coordinateLineColor": false
        })
    });
</script>
<table width="100%">
    <tr valign="top">
        <td>
            <canvas id="canvas" width="800px" height="500px"></canvas>
        </td>
        <td style="padding:8px;">
            <table class="tbDetails" style="width: 100%">
                <tr>
                    <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                    <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                </tr>    
            <%
                System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
                for (int i = 0; i < dt.Rows.Count; i++)
                {         
                    %>
                    <tr>            
                        <td>[<%=dt.Rows[i]["ProductId"].ToString() %>] <%=NEXMI.NMCommon.GetName(dt.Rows[i]["ProductId"].ToString(), langId)%></td>
                        <td><%=dt.Rows[i]["soluong"].ToString()%></td>
                        <td align="right"><%= double.Parse(dt.Rows[i]["tong"].ToString()).ToString("N0")%>
                            <input type="hidden" name="thangnam" value="<%=NEXMI.NMCommon.GetName(dt.Rows[i]["ProductId"].ToString(), langId)%>" />
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

