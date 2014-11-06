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
            "title": "Báo cáo doanh thu theo từng nhân viên",
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
            "smpTitle": "Tháng",
            "showAnimation": true,
            "coordinateLineColor": false

        })
    });
    function changedate() {
        var frommonth = $("#frommonth").val();
        var tomonth = $("#tomonth").val();
        var fromyear = $("#fromyear").val();
        var toyear = $("#toyear").val();
        var user = $("#user").val();
        $.post(appPath + "Report/SalesBySaler", { frommonth: frommonth, tomonth: tomonth, fromyear: fromyear, toyear: toyear, user: user }, function (data) {
            $("#pagemonth").html(data);
        });
    }
    </script>

<%
    double total = 0;
    int hournow = 0;
    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
    if (dt.Rows.Count > 0)
    {
        hournow = Int32.Parse(dt.Rows[0]["thang"].ToString());
    }
    
    //List<NEXMI.NMCustomersWSI> cus = (List<NEXMI.NMCustomersWSI>)ViewData["User"];
    int frommonth = Int32.Parse(ViewData["frommonth"].ToString());
    int fromyear = Int32.Parse(ViewData["fromyear"].ToString());
    int tomonth = Int32.Parse(ViewData["tomonth"].ToString());
    int toyear = Int32.Parse(ViewData["toyear"].ToString());
    DateTime fromDate = DateTime.Parse(ViewData["fromDate"].ToString());
    DateTime toDate = DateTime.Parse(ViewData["toDate"].ToString());
%>


<table width="100%">
    <tr valign="top">
        <td width="100%">
            <canvas id="canvas" width="1000px" height="500px"></canvas>
        </td>
    </tr>
    <tr>
        <td>
            <table class="tbDetails">
                <tr>
                    <th>Tháng</th>
                    <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                    <th>Tổng số</th>
                </tr>
            <%   
        int month = frommonth;
        int year = fromyear;                
        int temp=12;                
        while (toDate > fromDate)
        {
            bool flag = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["thang"].ToString() == month.ToString()&&dt.Rows[i]["nam"].ToString() == year.ToString())
                {                            
                    flag = true;
                    total += Double.Parse(dt.Rows[i]["tong"].ToString());
                    %>
                    <tr>
                        <td><%=dt.Rows[i]["thang"]%>/<%=dt.Rows[i]["nam"]%></td>
                        <td><%=dt.Rows[i]["soluong"]%></td>
                        <td align=right><%= double.Parse( dt.Rows[i]["tong"].ToString()).ToString("N3")%>
                        <input type="hidden" name="thangnam" value="<%=dt.Rows[i]["thang"]%>/<%=dt.Rows[i]["nam"]%>" />
                        <input type="hidden" name="soluong" value="<%=dt.Rows[i]["soluong"]%>" />
                        <input type="hidden" name="tong" value="<%=dt.Rows[i]["tong"]%>" />
                        </td>
                    </tr>
                    <%
                }                        
            }
            if (flag == false)
            {
                %>
                <tr>
                    <td><%=month%>/<%=year%></td>
                    <td>0</td>
                    <td>0
                    <input type="hidden" name="thangnam" value="<%=month%>/<%=year%>" />
                    <input type="hidden" name="soluong" value="0" />
                    <input type="hidden" name="tong" value="0" />
                    </td>
                </tr>
                <%
            }
            if (month == 12)
            {
                month = 0;
                ++year;
            }
            if (month == tomonth && year == toyear)
            {
                break;
            }
            ++month;

            fromDate = fromDate.AddMonths(1);
        }           
            %>
                <tr>            
                    <td colspan=2>Tổng doanh số</td>
                    <td align=right><%=total.ToString("N3")%></td>
                </tr>
            </table>
        </td>
    </tr>
</table>
        
    