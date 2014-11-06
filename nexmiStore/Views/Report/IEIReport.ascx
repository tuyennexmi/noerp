<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    string functionId = NEXMI.NMConstant.Functions.IEIReport;
    bool viewAll = GetPermissions.GetViewAll((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId);
%>
<script type="text/javascript">
    $(function () {
        
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        
        $('#slStocks').on('change', function () {
            fnReloadData();
        });
        if ('<%=viewAll%>' == 'False') {
            $('#slStocks').attr('disabled', true);
        }
        fnReloadData();
    });

    function fnReloadData() {
        LoadContentDynamic('divIEIReport', 'Report/IEIReportByProducts', {
            stockId: $('#slStocks').val(),
            fromDate: $('#dtFrom').val(),
            toDate: $('#dtTo').val()
        });
    }

    function ChangeCategory() {
        fnReloadData();
    }

    function fnPrint() {
        $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode($('#divPrintIEIReport').html()), mode: '' }, function (data) {
            window.open(appPath + 'UserControl/PrintPage');
        });
    }
</script>

<div class="divActions">
    <table>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td><button class="button" onclick="javascript:fnReloadData()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button></td>
        </tr>
    </table>
</div>
<div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <table>
                    <tr>
                        <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>
                        <td>Từ: <input type="text"  id="dtFrom" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 89px"/></td>
                        <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" style="width: 89px"/></td>
                    </tr>
                </table>
            </div>
        </div>
        
        <table class="frmInput">
            <tr>
                <td valign="top" id="divPrintIEIReport">
                    <h4>BẢNG BÁO CÁO NHẬP - XUẤT - TỒN</h4>
                    <div id="divIEIReport"></div>
                </td>
            </tr>
        </table>
</div>
