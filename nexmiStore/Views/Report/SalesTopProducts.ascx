﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
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
        //var top = $("#top").val();
        LoadContentDynamic('divSalesTopProductsUC', 'Report/SalesTopProductsUC', {
            fromDate: $('#dtFrom').val(),
            toDate: $('#dtTo').val()
        });
    }
</script>

<div class="divActions">
    <%Html.RenderPartial("ToolbarTimeButton"); %>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <table>
                <tr>
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 89px" /></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" style="width: 89px" /></td>
                    <td></td>
                </tr>
            </table>        
        </div>
    </div>
    <div id="divSalesTopProductsUC"></div>
</div>
