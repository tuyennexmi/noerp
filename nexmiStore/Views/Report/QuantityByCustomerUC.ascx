<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
    List<NEXMI.NMProductsWSI>  pList = (List<NEXMI.NMProductsWSI>)ViewData["products"];
%>
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
                      "Sản lượng",
                      "Số đơn"
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
                      "Sản lượng"
                    ],
                    "xAxis2": [
                      "Số đơn"
                    ]
                }
            }, {
                "graphType": "BarLine",
                "useFlashIE": true,
                "title": "Báo cáo sản lượng theo khách hàng",
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
                "coordinateLineColor": false,
                "smpLabelFontSize": 10,
                "autoScaleFont": false,
                "maxSmpStringLen": 50
            })
        }
    });
</script>
<input type="hidden" id="count" value="<%=dt.Rows.Count%>" />
<table width="100%">
    <tr valign="top">
        <td>
            <canvas id="canvas" width="800px" height="500px"></canvas>
        </td>
        <td style="padding:8px;">
            <table class="tbDetails" width="100%">
                <tr>
                    <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                    <th>Số đơn</th>
                    <th>Sản lượng</th>
                </tr>    
                <%        
                    for(int i=0; i<dt.Rows.Count; i++)
                    {
                        double amount = 0;
                        double quantity = 0;
                        string pname ="";
                        foreach ( NEXMI.NMProductsWSI Item in pList)
                        {
                            if(Item.Product.ProductId == dt.Rows[i]["id"].ToString())
                            {
                                pname = Item.Translation.Name + " [" + Item.Product.ProductCode + "]";
                                break;
                            }
                        }
                        amount = double.Parse(dt.Rows[i]["tong"].ToString());
                        quantity = double.Parse(dt.Rows[i]["soluong"].ToString());
                        %>
                        <tr>            
                            <td><%=pname%></td>
                            <td><%= quantity%></td>
                            <td align=right><%=amount.ToString("N3")%>
                            <input type="hidden" name="thangnam" value="<%=pname%>" />
                            <input type="hidden" name="soluong" value="<%=quantity %>" />
                            <input type="hidden" name="tong" value="<%=amount %>" />
                            </td>
                        </tr>
                        <%
                    }
                %>
            </table>
        </td>
    </tr>
</table>



