<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMStocksWSI WSI = (NEXMI.NMStocksWSI)ViewData["WSI"];
    string pos = "absolute", windowId = "";
    if (ViewData["WindowId"] == null)
    {
        pos = "static";
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnSaveOrUpdateStock()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("ProductSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
<% 
    }
    else
    {
        windowId = ViewData["WindowId"].ToString();
    } 
%>
    <div class="windowContent" style="position: <%=pos%> !important;">
        <script type="text/javascript">
            $(function () {
                $('#formStock<%=windowId%>').jqxValidator({
                    rules: [
                    { input: '.nameRequired', message: 'Nhập tên kho', action: 'keyup, blur', rule: 'required' }
                ]
                });
                $('#txtName<%=windowId%>').focus();

                $('#btnBrowser<%=windowId%>').on("click", function () {
                    var finder = new CKFinder();
                    finder.selectActionFunction = function (fileUrl, data) {
                        if (appPath.length > 1) {
                            fileUrl = fileUrl.replace(appPath, 'quantri/');
                        } else {
                            fileUrl = fileUrl.replace('//', 'quantri/');
                        }
                        $('#avatarURL<%=windowId%>').val(fileUrl);
                    };
                    finder.popup();
                });
            });

            function fnSaveOrUpdateStock() {
                if ($('#formStock<%=windowId%>').jqxValidator('validate')) {
                    var data = {};
                    data.id = $('#txtId<%=windowId%>').val();
                    data.telephone = $('#txtTelephone<%=windowId%>').val();
                    data.type = $('#slStockTypes').val();
                    data.avatar = $('#avatarURL<%=windowId%>').val();
                    data.insert = (document.getElementById('cbInsert<%=windowId%>').checked) ? 'yes' : '';
                    var highlight = "False";
                    if (document.getElementById('cbHighlight<%=windowId%>').checked) {
                        highlight = "True";
                    }
                    data.highlight = highlight;
                    var allLanguageId = '<%=NEXMI.NMCommon.GetAllLanguageId()%>'.split(';');
                    for (var i = 0; i < allLanguageId.length - 1; i++) {
                        var temp = allLanguageId[i];
                        data['name' + temp] = $('#txtStockName<%=windowId%>' + temp).val();
                        data['address' + temp] = $('#txtStockAddress<%=windowId%>' + temp).val();
                        data['shortDescription' + temp] = $('#txtStockShort<%=windowId%>' + temp).val();
                        data['description' + temp] = Base64.encode(CKEDITOR.instances['txtStockDescription' + temp].getData());
                    }
                    $.ajax({
                        type: "POST",
                        url: appPath + 'Stocks/SaveOrUpdateStock',
                        data: data,
                        beforeSend: function () {
                            fnDoing();
                        },
                        success: function (data) {
                            if (data == "") {
                                fnSuccess();
                                fnResetStockForm();
                            }
                            else {
                                alert(data);
                            }
                        },
                        error: function () {
                            fnError();
                        },
                        complete: function () {
                            fnComplete();
                        }
                    });
                }
                else {
                    alert("Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!");
                }
            }
            function fnResetStockForm() {
                $('#txtId<%=windowId%>').val('');
                $('#txtTelephone<%=windowId%>').val('');
                $('#avatarURL<%=windowId%>').val('');
                document.getElementById('cbHighlight<%=windowId%>').checked = false;
                var allLanguageId = '<%=NEXMI.NMCommon.GetAllLanguageId()%>'.split(';');
                for (var i = 0; i < allLanguageId.length - 1; i++) {
                    $('#txtStockName<%=windowId%>' + allLanguageId[i]).val('');
                    $('#txtStockAddress<%=windowId%>' + allLanguageId[i]).val('');
                    $('#txtStockShort<%=windowId%>' + allLanguageId[i]).val('');
                    CKEDITOR.instances['txtStockDescription' + allLanguageId[i]].setData('');
                }
                $("#txtName<%=windowId%>").focus();
            }
        </script>
        <form id="formStock<%=windowId%>" action="">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td class="lbright">Mã kho</td>
                    <td><input type="text" id="txtId<%=windowId%>" readonly="readonly" value="<%=WSI.Id%>" /></td>
                    <td class="lbright">Loại kho</td>
                    <td><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slStockTypes", objectName = "Stocks", value = WSI.StockType });%></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></td>
                    <td><input type="text" id="txtTelephone<%=windowId%>" value="<%=WSI.Telephone%>" /></td>
                    <td class="lbright">Hình đại diện</td>
                    <td>
                        <input type="text" id="avatarURL<%=windowId%>" value="<%=WSI.Logo%>" />
                        <input class="button" type="button" id="btnBrowser<%=windowId%>" value="Browser..." />
                    </td>
                </tr>                
                <tr>
                    <td class="lbright">Thuộc tính</td>
                    <td colspan="3">
                        <% 
                            string checkHighlight = "";
                            if (WSI.Highlight == "true")
                                checkHighlight = "checked";
                        %>
                        <input type="checkbox" id="cbHighlight<%=windowId%>" <%=checkHighlight%> /> <label for="cbHighlight">Nổi bật</label>
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Hành động kèm theo</td>
                    <td><label for="cbInsert<%=windowId%>"><input type="checkbox" id="cbInsert<%=windowId%>" checked="checked" />Chèn thêm thông tin tất cả sản phẩm cho kho này</label></td>
                </tr>
                <tr>
                    <td colspan="4">
                        <%Html.RenderAction("ContentLanguagesForm", "UserControl", new { mode = "Stock", data = WSI.Id, windowId = windowId });%>
                    </td>
                </tr>
            </table>
        </form>
    </div>
<% 
    if (windowId != "")
    {
%>
    <div class="windowButtons">
        <div class="NMButtons">
            <button onclick="javascript:fnSaveOrUpdateStock()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            <button onclick="javascript:fnResetStockForm()" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLEAR_ALL", langId) %></button>
            <button onclick='javascript:fnCloseFormStock("<%=windowId%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
        </div>
    </div>
<%
    }
%>
</div>