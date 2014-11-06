<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $('#dtFrom, #dtTo').on('change', function () {
            GetContent();
        });
    });

    function fnSetDate(fromDate, toDate) {
        $('#dtFrom').val(fromDate);
        $('#dtTo').val(toDate);
        GetContent();
    }

    function GetContent() {
        var userId = '';
        try {
            userId = $('#cbbCustomers').jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        LoadContentDynamic('divSalesTopProductsUC', 'Report/SalesBySalerUC', {
            user: userId,
            fromDate: $('#dtFrom').val(),
            toDate: $('#dtTo').val()
        });
    }
</script>
       
<%
    DateTime thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    DateTime lastMonth = thisMonth.AddMonths(-1);
    string today = DateTime.Today.ToString("yyyy-MM-dd"),
        yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
        firstDateOfThisMonth = thisMonth.ToString("yyyy-MM-dd"),
        lastDateOfThisMonth = thisMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"),
        firstDateOfLastMonth = lastMonth.ToString("yyyy-MM-dd"),
        lastDateOfLastMonth = lastMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
%>
<%
    double total = 0;
    int hournow = 0;
    //System.Data.DataTable dt = (System.Data.DataTable)ViewData["dt"];
    //if (dt.Rows.Count > 0)
    //{
    //    hournow = Int32.Parse(dt.Rows[0]["thang"].ToString());
    //}
    
    List<NEXMI.NMCustomersWSI> cus = (List<NEXMI.NMCustomersWSI>)ViewData["User"];
    //int frommonth = Int32.Parse(ViewData["frommonth"].ToString());
    //int fromyear = Int32.Parse(ViewData["fromyear"].ToString());
    //int tomonth = Int32.Parse(ViewData["tomonth"].ToString());
    //int toyear = Int32.Parse(ViewData["toyear"].ToString());

%>
 
<div class="divActions">
    <%--<input type="hidden" id="hournows" value="<%=hournow%>" />--%>
        <%--<table style="padding:10px"> 
        <tr>
            <td>Từ tháng:</td>
            <td><input type="number" id="frommonth" name="quantity" min="1" max="12" value="<%=frommonth%>" /></td>
            <td><input type="number" id="fromyear" name="quantity" min="1970" max="9999" value="<%=fromyear%>" /></td>
        
            <td>Đến tháng:</td>
            <td><input type="number" id="tomonth" name="quantity" min="1" max="12" value="<%=tomonth%>" /></td>
            <td><input type="number" id="toyear" name="quantity" min="1970" max="9999" value="<%=toyear%>" /></td>
        
            <td>Nhân viên:</td>
            <td>
            <select id="user">
                <%
                    foreach (NEXMI.NMCustomersWSI item in cus)
                    { 
                %>
                        <option <%if(item.Customer.CustomerId==ViewData["selected"].ToString()){%>selected<%}%> value="<%=item.Customer.CustomerId%>"><%=item.Customer.CompanyNameInVietnamese%></option>        
                <%
                    }    
                %>
            </select>            
            </td>            
            <td><input type="submit" onclick="changedate()" value="Xem" /></td>
        </tr>
    </table>--%>
    
    <%Html.RenderPartial("ToolbarTimeButton"); %>
</div>
<div class="divContent">
    
<div class="divStatus">
    <div class="divButtons">
        <table>
            <tr>
                <td> <%Html.RenderAction("cbbCustomers", "UserControl", new { cgroupId = NEXMI.NMConstant.CustomerGroups.User, customerId = "", customerName = "" });%></td>
                <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="<%=today%>" style="width: 89px"/></td>
                <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" style="width: 89px"/></td>
                <td></td>
            </tr>
        </table>        
    </div>
    </div>
    <div id="divSalesTopProductsUC"></div>        
</div>
