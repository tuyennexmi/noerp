<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
  
  <% string langId = Session["Lang"].ToString(); %>

       <script type="text/javascript">
           $(document).ready(function () {
               var arr1 = new Array();
               var arr2 = new Array();
               var arr3 = new Array();
               count = $("#hournows").val();

               for (var x = 1; x <= count; x++) {
                   var gio = parseInt($("#n" + (x)).val());
                   var soluong = parseInt($("#s" + (x)).val());
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
               if (count == 0) {
                   arr1.push(0);
                   arr2.push(0);
                   arr3.push(0);
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
                   "title": "Báo cáo doanh thu theo tháng",
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
                   "smpTitle": "Năm",
                   "showAnimation": true,
                   "coordinateLineColor": false

               })
               var years = new Date();
               document.getElementById("year").value = years.getFullYear();
           });
           function changedate() {
               var str = $("#year").val();
               if (str != "") {
                   var year = str.split("-");
                   $.post(appPath + "Report/SalesByMonthOfYearReport", { nam: year[0] }, function (data) {
                       $("#pagemonth").html(data);
                   });
               }
           }

           $("#thisyear").click(function () {
               var years = new Date();
               $.post(appPath + "Report/SalesByMonthOfYearReport", { nam: year[0] }, function (data) {
                   $("#pagemonth").html(data);
               });
           });

           $("#lastyear").click(function () {
               var years = new Date();
               $.post(appPath + "Report/SalesByMonthOfYearReport", { nam: year[0]-1 }, function (data) {
                   $("#pagemonth").html(data);
               });
           });

    </script> 

<%
    double total = 0; double tong = 0;
    int hournow = 0;
    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
    if (dt.Rows.Count > 0)
    {
        hournow = Int32.Parse(dt.Rows[0]["thang"].ToString());
    }
%>
<div id="pagemonth" >
<div class="divActions">
    <input type="hidden" id="hournows" value="<%=hournow%>" />
    <div style="padding:20px">
        <a id="thisyear">Năm nay</a> | <a id="lastyear">Năm ngoái</a> | <%--<a id="A2">12 tháng qua</a> |--%>
        <label>Tùy chọn năm khác: </label><input type="number" id="year" name="quantity" min="2010" max="2020" value="">
        <input type="submit" onclick="changedate()" value="Xem" />
    </div>
</div>

<div class="divContent">
    <div style="padding:10px">
        <canvas id="canvas" width="800px" height="500px"></canvas>         
    </div>
    <div style="padding:15px">
    <table class="tbDetails" width=26%>
        <tr><td colspan=3>BẢNG SỐ LIỆU TỔNG HỢP</td></tr>
        <tr>            
            <th>Tháng</th>
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
                if (j == Int32.Parse(dt.Rows[i]["thang"].ToString()))
                {
                    tong = double.Parse(dt.Rows[i]["tong"].ToString());
                %>
                <tr>
                    <td><%=dt.Rows[i]["thang"]%>/<%=dt.Rows[i]["nam"]%></td>
                    <td align=right><%=dt.Rows[i]["soluong"]%>
                        <input type="hidden" id="n<%=dt.Rows[i]["thang"]%>" value="<%=dt.Rows[i]["thang"]%>" />
                        <input type="hidden" id="s<%=dt.Rows[i]["thang"]%>" value="<%=dt.Rows[i]["soluong"]%>" />
                    </td>
                    <td align=right><%=tong.ToString("N3")%><input type="hidden" id="t<%=dt.Rows[i]["thang"]%>" value="<%=tong%>" /></td>
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
                    <td><%=j%>/<%=dt.Rows[0]["nam"]%></td>
                    <td align=right>0</td>
                    <td align=right>0</td>
                </tr>
            <%
            }
        }
        %>
        <tr>
            <td colspan=2 ><b>TỔNG DOANH SỐ</b></td>
            <td align=right><%=total.ToString("N3")%></td>
        </tr>
    </table>    
</div>
</div>
</div>