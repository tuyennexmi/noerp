<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = "", parentElement = "";
    try {
        parentElement = ViewData["ParentElement"].ToString();
    }
    catch { }
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
                    <button onclick="javascript:fnSaveOrUpdatePC('', '<%=parentElement%>')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
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
                $('#uploadFileImage<%=windowId%>').change(function () {
                    loadImage(document.getElementById('uploadFileImage<%=windowId%>'), 'avatar<%=windowId%>');
                });
            });

            function fnSaveOrUpdatePC(windowId, parentElement) {
                //if ($('#formPC' + windowId).jqxValidator('validate')) {
                    var data = {};
                    var parentId = "";
                    try {
                        parentId = $('#cbbCategories' + windowId).jqxComboBox('getSelectedItem').value;
                    } catch (err) { }
                    data.id = $('#txtCategoryId' + windowId).val();
                    data.code = $('#txtCategoryCode' + windowId).val();
                    data.objectName = $('#txtObjectName' + windowId).val();
                    data.parentId = parentId;
                    data.fileName = $('#uploadFileImage' + windowId).val().split('\\').pop();
                    data.imageUrl = $('#avatar' + windowId).attr('src');
                    var allLanguageId = '<%=NEXMI.NMCommon.GetAllLanguageId()%>'.split(';');
                    for (var i = 0; i < allLanguageId.length - 1; i++) {
                        var temp = allLanguageId[i];
                        data['name' + temp] = $('#txtCategoryName<%=windowId%>' + temp).val();
                        data['description' + temp] = Base64.encode(CKEDITOR.instances['txtCategoryDescription<%=windowId%>' + temp].getData());
                    }
                    //data.windowId = '<%=windowId%>';
                    $.ajax({
                        type: 'POST',
                        url: appPath + 'Directories/SaveOrUpdateProductCategory',
                        data: data,
                        beforeSend: function () {
                            fnDoing();
                        },
                        success: function (data) {
                            if (data.error == "") {
                                if (parentElement == '') {
                                    fnResetFormPC('<%=windowId%>');
                                    history.back();
                                } else {
                                    fnSuccess();
                                    fnSetItemForCombobox(parentElement, data.id, Base64.encode(name));
                                    closeWindow('<%=windowId%>');
                                    //window.history.go(-1);
                                    history.back();
                                    //$(window).hashchange();
                                }
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
    //            }
    //            else {
    //                alert("Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!");
    //            }
            }

            function fnResetFormPC(windowId) {
                $('#txtCategoryId' + windowId).val('');
                $('#avatar' + windowId).attr('src', appPath + 'Content/UI_Images/noimage.png');
                var allLanguageId = '<%=NEXMI.NMCommon.GetAllLanguageId()%>'.split(';');
                for (var i = 0; i < allLanguageId.length - 1; i++) {
                    $('#txtCategoryName' + windowId + allLanguageId[i]).val('');
                    $('#txtCategoryDescription' + windowId + allLanguageId[i]).val('');
                }
            }
        </script>
        <form id="formPC<%=windowId%>" action="">
            <input type="hidden" id="txtObjectName<%=windowId%>" value="<%=ViewData["ObjectName"]%>" />
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td rowspan="3" align="center" valign="top" style="width: 200px;">
                        <input type="file" id="uploadFileImage<%=windowId%>" style="display: none;" />
                        <img id="avatar<%=windowId%>" alt="" src="<%=ViewData["avatar"]%>" style="width: 100px; height: 100px;" /><br />
                        <a href='javascript:$("#uploadFileImage<%=windowId%>").click();'>Thay đổi</a>
                    </td>
                    <td class="lbright">ID</td>
                    <td><input type="text" id="txtCategoryId<%=windowId%>" name="txtCategoryId<%=windowId%>" readonly="readonly" value="<%=ViewData["Id"]%>" />&nbsp;</td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PC_CODE", langId) %></td>
                    <td valign="top">
                        <input type="text" id="txtCategoryCode<%=windowId%>" name="txtCategoryCode<%=windowId%>" value="<%=ViewData["Code"]%>" />&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("KIND", langId) %></td>
                    <td valign="top">
                        <%Html.RenderAction("cbbCategories", "UserControl", new { currentId = ViewData["CategoryId"], currentName = ViewData["CategoryName"], elementId = windowId, objectName = ViewData["ObjectName"] });%>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <%Html.RenderAction("ContentLanguagesForm", "UserControl", new { mode = "ProductCategory", data = ViewData["Id"], windowId = windowId });%>
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
            <button onclick='javascript:fnSaveOrUpdatePC("<%=windowId%>", "<%=parentElement%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            <button onclick='javascript:fnResetFormPC("<%=windowId%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLEAR_ALL", langId) %></button>
            <button onclick='javascript:fnCloseFormPC("<%=windowId%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
        </div>
    </div>
<%
    }
%>
</div>