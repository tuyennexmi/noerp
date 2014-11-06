<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMDocumentsWSI WSI = (NEXMI.NMDocumentsWSI)ViewData["WSI"];
    string categoryId = "", categoryName = "";
    try
    {
        categoryId = WSI.Category.Id;
        categoryName = WSI.Category.Name;
    }
    catch { }
    string typeId = ViewData["TypeId"].ToString();
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
                    <button onclick="javascript:fnSaveOrUpdateDocument()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                </div>
            </td>
            <td align="right">
                <%//Html.RenderPartial("CustomerSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="windowContent" style="position: static;">
        <script type="text/javascript">
            $(function () {
                $("#tabsDocument").jqxTabs({ theme: theme, keyboardNavigation: false });
                $("#formDocument").jqxValidator({});
                $('#checkAllImages').click(function () {
                    var elms = document.getElementsByName('cbImage');
                    if ($(this).is(":checked")) {
                        for (var x in elms) {
                            elms[x].checked = true;
                        }
                    }
                    else {
                        for (var y in elms) {
                            elms[y].checked = false;
                        }
                    }
                });
            });

            function fnSaveOrUpdateDocument() {
                if ($('#formDocument').jqxValidator('validate')) {
                    var data = {};
                    data.id = $('#txtDocumentId').val();
                    data.typeId = $('#txtTypeId').val();
                    data.owner = $('#txtOwner').val();
                    data.highlight = (document.getElementById('cbHighlight').checked) ? 'true' : 'false';
                    var allLanguageId = '<%=NEXMI.NMCommon.GetAllLanguageId()%>'.split(';');
                    for (var i = 0; i < allLanguageId.length - 1; i++) {
                        var temp = allLanguageId[i];
                        data['name' + temp] = $('#txtDocumentName' + temp).val();
                        data['shortDescription' + temp] = $('#txtDocumentShort' + temp).val();
                        data['description' + temp] = Base64.encode(CKEDITOR.instances['txtDocumentDescription' + temp].getData());
                    }
                    var images = '';
                    var fileName = '';
                    var imageDescription = '';
                    var isDefault = '';
                    var elms = document.getElementsByName('cbImage');
                    for (var x = 0; x < elms.length; x++) {
                        fileName = elms[x].id;
                        imageDescription = '';
                        isDefault = 'false';
                        if ($('input[name=rdIsDefault]:checked').val() == fileName) {
                            isDefault = 'true';
                        }
                        if (images != '') {
                            images += 'ROWS';
                        }
                        images += fileName + 'COLS' + imageDescription + 'COLS' + isDefault;
                    }
                    data.images = images;
                    $.ajax({
                        type: 'POST',
                        url: appPath + 'CMS/SaveOrUpdateDocument',
                        data: data,
                        success: function (data) {
                            if (data.error == '') {
                                if ('<%=typeId%>' == '<%=NEXMI.NMConstant.DocumentTypes.Introduce%>')
                                    LoadContent('', 'CMS/Introduces');
                                else if ('<%=typeId%>' == '<%=NEXMI.NMConstant.DocumentTypes.News%>')
                                    LoadContent('', 'CMS/News');
                                else if ('<%=typeId%>' == '<%=NEXMI.NMConstant.DocumentTypes.Rules%>')
                                    LoadContent('', 'CMS/Rules');
                                else if ('<%=typeId%>' == '<%=NEXMI.NMConstant.DocumentTypes.Supports%>')
                                    LoadContent('', 'CMS/Supports');
                                else if ('<%=typeId%>' == '<%=NEXMI.NMConstant.DocumentTypes.Recruits%>')
                                    LoadContent('', 'CMS/Recruits');
                                else if ('<%=typeId%>' == '<%=NEXMI.NMConstant.DocumentTypes.WorkGuide%>')
                                    LoadContent('', 'CMS/WorkGuides');
                            }
                            else {
                                alert(data.error);
                            }
                        }
                    });
                } else {
                    alert("Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!");
                }
            }

            function fnAddToList(fileName) {
                var row = '';
                row += '<td><input type="checkbox" name="cbImage" id="' + fileName + '" /></td>';
                row += '<td><img alt="" src="<%=Url.Content("~")%>uploads/_thumbs/' + fileName + '" /></td>';
                row += '<td><input type="radio" name="rdIsDefault" value="' + fileName + '" /></td>';
                row += '<td><a href="javascript:fnDeleteItem(\'' + fileName + '\');"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></td>';
                $('#tbImageList tbody').append('<tr id="row' + fileName + '">' + row + '</tr>');
            }

            function fnDeleteItem(fileName) {
                if (confirm("<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %> hình này?")) {
                    fnDelete(fileName);
                }
            }

            function fnDeleteSelectedItems() {
                if (confirm("<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %> hình đã chọn?")) {
                    var elms = document.getElementsByName('cbImage');
                    for (var x = 0; x < elms.length; x++) {
                        if (elms[x].checked) {
                            fnDelete(elms[x].id);
                        }
                    }
                    $('#checkAllImages').attr('checked', false);
                }
            }

            function fnDelete(fileName) {
                $.post(appPath + 'Common/DeleteFile', { fileName: fileName, type: 'Images', owner: '<%=WSI.Document.DocumentId%>' }, function (data) {
                    if (data == '') {
                        var elm = document.getElementById('row' + fileName);
                        elm.parentNode.removeChild(elm);
                    } else {
                        alert(data);
                    }
                });
            }
        </script>
        <div>
            <form id="formDocument" action="" method="post">
                <input type="hidden" id="txtTypeId" value="<%=typeId%>" />
                <input type="hidden" id="txtOwner" value="<%=ViewData["Owner"]%>" />
                <div id="tabsDocument">
                    <ul>
                        <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("CONTENT", langId) %></li>
                        <li><%= NEXMI.NMCommon.GetInterface("PICTURE", langId) %></li>
                    </ul>
                    <div style="padding: 10px;">
                        <table style="width: 100%" class="frmInput">
                            <tr>
                                <td class="lbright">ID</td>
                                <td><input type="text" id="txtDocumentId" value="<%=WSI.Document.DocumentId%>" maxlength="30" readonly="readonly" /></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <% 
                                        string checkHighlight = "";
                                        if (WSI.Document.Highlight != null)
                                            if (WSI.Document.Highlight.Value)
                                                checkHighlight = "checked";
                                    %>
                                    <input type="checkbox" id="cbHighlight" <%=checkHighlight%> /> <label for="cbHighlight">Nổi bật</label>
                                </td>
                            </tr>
                            <%--<% 
                                if (typeId == NEXMI.NMConstant.DocumentTypes.Introduce)
                                {    
                            %>
                            <tr>
                                <td class="lbright">Nhóm bài viết</td>
                                <td>
                                    <% 
                                    string sl1 = "", sl2 = "", sl3 = "";
                                    switch (WSI.Document.Owner)
                                    {
                                        case "AboutUs": sl1 = "selected"; break;
                                        case "NaturesLife": sl2 = "selected"; break;
                                        case "NaturesFood": sl3 = "selected"; break;
                                    }    
                                    %>
                                    <select id="slTypes">
                                        <option value="AboutUs" <%=sl1%>>Về chúng tôi</option>
                                        <option value="NaturesLife" <%=sl2%>>Nature's Life</option>
                                        <option value="NaturesFood" <%=sl3%>>Nature's Food</option>
                                    </select>
                                </td>
                            </tr>
                            <% 
                                }    
                            %>--%>
                            <tr>
                                <td colspan="2">
                                    <%Html.RenderAction("ContentLanguagesForm", "UserControl", new { mode = "Document", data = WSI.Document.DocumentId });%>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="padding: 10px;">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20%" valign="top"><%Html.RenderAction("ImageUploader", "Files");%></td>
                                <td valign="top">
                                    <button type="button" onclick="javascript:fnDeleteSelectedItems()" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %> ảnh đã chọn</button>
                                    <table style="width: 100%" class="list" id="tbImageList">
                                        <thead>
                                            <tr>
                                                <th><input type="checkbox" id="checkAllImages" /></th>
                                                <th></th>
                                                <th>Ảnh chính</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <%
                                                if (ViewData["ImageWSIs"] != null)
                                                {
                                                    List<NEXMI.NMImagesWSI> ImageWSIs = (List<NEXMI.NMImagesWSI>)ViewData["ImageWSIs"];
                                                    string isDefault;
                                                    foreach (NEXMI.NMImagesWSI Item in ImageWSIs)
                                                    {
                                                        isDefault = "";
                                                        if (Item.IsDefault == "True")
                                                        {
                                                            isDefault = "checked";
                                                        }
                                            %>
                                            <tr id="row<%=Item.Name%>">
                                                <td><input type="checkbox" name="cbImage" id="<%=Item.Name%>" /></td>
                                                <td><img alt="" src="<%=Url.Content("~")%>uploads/_thumbs/<%=Item.Name%>" /></td>
                                                <td><input type="radio" name="rdIsDefault" value="<%=Item.Name%>" <%=isDefault%> /></td>
                                                <td class="actionCols"><a href="javascript:fnDeleteItem('<%=Item.Name%>')"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></td>
                                            </tr>
                                            <%
                                                    }
                                                }
                                            %>
                                        </tbody>
                                    </table>            
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>