<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        var count = $("#count").val();
        if (count > 0) {
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
                "title": "Báo cáo doanh thu theo khách hàng",
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
                "smpTitle": "<%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>",
                "showAnimation": true,
                "coordinateLineColor": false,
                "smpLabelFontSize": 10,
                "autoScaleFont": false,
                "maxSmpStringLen": 50
            })
        }
        //$("#custom").hide();
    });
    function change(mon) {
        var fromdate = $("#fromdate").val();
        var todate = $("#todate").val();
        var stockid = $("#slStocks").val();
        $.post(appPath + "Report/SalesByCustomer", { stockid: stockid, fromdate: fromdate, todate: todate }, function (data) {
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
            <canvas id="canvas" width="800px" height="680px"></canvas>
        </td>
        <td style="padding:8px;">        
        <table class="tbDetails" width="100%">
            <tr>
                <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
                <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %> đơn hàng</th>
                <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
            </tr>    
            <%
                bool flg = true;
                double tong_sl =0;
                double tong_tien =0;
                double tong, sl;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tong = double.Parse(dt.Rows[i]["tong"].ToString());
                    if (tong != 0)
                    {
                        sl =double.Parse(dt.Rows[i]["soluong"].ToString());
                        tong_tien += tong;
                        tong_sl += sl;
                        flg = false;
                    %>
                    <tr>            
                        <td><%=dt.Rows[i]["CompanyNameInVietnamese"].ToString()%></td>
                        <td><%=dt.Rows[i]["soluong"].ToString()%></td>
                        <td align=right><%= tong.ToString("N3")%>
                        <input type="hidden" name="thangnam" value="<%=dt.Rows[i]["CompanyNameInVietnamese"].ToString()%>" />
                        <input type="hidden" name="soluong" value="<%=dt.Rows[i]["soluong"].ToString()%>" />
                        <input type="hidden" name="tong" value="<%=dt.Rows[i]["tong"].ToString()%>" />
                        </td>
                    </tr>                    
                <%
                    }
                }                
                if(flg)
                {
                %>
                    <input type="hidden" id="count" value="0" />
                <%
                }
                else
                {
                    %>
                    <input type="hidden" id="count" value="1" />
              <%
                }%>
                    <tr>
                        <td>Tổng cộng</td>
                        <td><%=tong_sl.ToString() %></td>
                        <td><%=tong_tien.ToString("N3") %></td>
                    </tr>
        </table>
    </td>
    </tr>
</table>
