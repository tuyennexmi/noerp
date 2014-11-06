<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();    
%>
<div class="windowContent">
    <form id="form<%=windowId%>" method="post" action="">
        <table class="frmInput">
            <tr>
                <td class="lbright">Ngày thực hiện</td>
                <td><input type="text"  id="txtInvoiceDate<%=windowId%>" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" /></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                <td><input type="text" id="txtReference<%=windowId%>" value="" /></td>
            </tr>
            <%--<tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALER", langId) %></td>
                <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers" + windowId, userId = Page.User.Identity.Name });%></td>
            </tr>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SALE_AT", langId) %></td>
                <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks" + windowId, stockId = "" });%></td>
            </tr>--%>
            <tr>
                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                <td colspan="3"><textarea id="txtDescription<%=windowId%>" rows="5" cols="0" style="width: 95%"></textarea></td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        $(function () {
            $("#txtInvoiceDate<%=windowId%>").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
            $('#form<%=windowId%>').jqxValidator({
                rules: [
                    { input: '#txtInvoiceDate<%=windowId%>', message: 'Không được để trống.', action: 'keyup, blur', rule:
                        function (input, commit) {
                            if ($(input).val() == '')
                                return false;
                            return true;
                        } 
                    }
               ]
            });
            //$('#slStocks, #slUsers').prop('disabled', true);
        });

        function fnSaveInvoice() {
            if ($('#form<%=windowId%>').jqxValidator('validate')) {
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Sales/SaveOrderAfterInventory',
                    data: {
                        invoiceDate: $('#txtInvoiceDate<%=windowId%>').val(),
                        reference: $('#txtReference<%=windowId%>').val(),
                        description: $('#txtDescription<%=windowId%>').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            //fnSuccess();
                            closeWindow('<%=windowId%>');
                            LoadContent('', 'Sales/SalesInvoice?invoiceId=' + data.id);
                        }
                        else {
                            alert(data.error);
                        }
                    },
                    error: function () {
                        fnError();
                    },
                    complete: function () {
                        fnComplete();
                    }
                });
            } else {
                alert('Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!');
            }
        }
    </script>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveInvoice()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>