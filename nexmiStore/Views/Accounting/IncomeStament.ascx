﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });
    function fnReloadIS() {
        LoadContentDynamic('IncomeStamentUC', 'Accounting/IncomeStamentUC', {
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
        });
    }
</script>



<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <button onclick="javascript:fnReloadIS()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>                
            </td>
            <td></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">            
            <table>
                <tr>
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="" style="width: 89px"/></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd") %>" style="width: 89px"/></td>
                </tr>
            </table>            
        </div>
    </div>
    <div class='divContentDetails'>
        <div id="IncomeStamentUC">

        </div>
    </div>
</div>
