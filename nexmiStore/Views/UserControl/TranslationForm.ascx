<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMTranslationsWSI WSI = (NEXMI.NMTranslationsWSI)ViewData["WSI"];
    if (WSI.Translation == null)
        WSI.Translation = new NEXMI.Translations();
    string mode = ViewData["Mode"].ToString();
    string languageId = ViewData["WindowId"].ToString() + ViewData["LanguageId"].ToString();
    switch (mode)
    {
        case "Document": 
%>
<script type="text/javascript">
    $(function () {
        if (CKEDITOR.instances['txtDocumentDescription' + '<%=languageId%>'])
            delete CKEDITOR.instances['txtDocumentDescription' + '<%=languageId%>'];
        CKEDITOR.replace('txtDocumentDescription' + '<%=languageId%>');
    });
</script>
<table style="width: 100%" class="frmInput">
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
        <td><input type="text" id="txtDocumentName<%=languageId%>" value="<%=WSI.Translation.Name%>" /></td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
        <td><textarea cols="50" rows="4" id="txtDocumentShort<%=languageId%>"><%=WSI.Translation.ShortDescription%></textarea></td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CONTENT", langId) %></td>
        <td>
            <textarea cols="1" rows="1" id="txtDocumentDescription<%=languageId%>" class="jqte"><%=WSI.Translation.Description%></textarea>
        </td>
    </tr>
</table>
            
<%
            break;
        case "Stock": 
%>
<script type="text/javascript">
    $(function () {
        if (CKEDITOR.instances['txtStockDescription' + '<%=languageId%>'])
            delete CKEDITOR.instances['txtStockDescription' + '<%=languageId%>'];
        CKEDITOR.replace('txtStockDescription' + '<%=languageId%>');
    });
</script>
<table style="width: 100%" class="frmInput">
    <tr>
        <td class="lbright">Tên kho</td>
        <td><input type="text" id="txtStockName<%=languageId%>" value="<%=WSI.Translation.Name%>" class="nameRequired" />&nbsp;</td>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %></td>
        <td><input type="text" id="txtStockAddress<%=languageId%>" value="<%=WSI.Translation.Address%>" />&nbsp;</td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SHORT_DESCRIPTION", langId)%></td>
        <td colspan="3"><textarea cols="60" rows="2" id="txtStockShort<%=languageId%>"><%=WSI.Translation.ShortDescription%></textarea>&nbsp;</td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
        <td colspan="3"><textarea cols="30" rows="3" id="txtStockDescription<%=languageId%>"><%=WSI.Translation.Description%></textarea>&nbsp;</td>
    </tr>
</table>
<%       
            break;
            case "Product":
%>
<script type="text/javascript">
    $(function () {
        if (CKEDITOR.instances['txtProductDescription' + '<%=languageId%>'])
            delete CKEDITOR.instances['txtProductDescription' + '<%=languageId%>'];
        CKEDITOR.replace('txtProductDescription' + '<%=languageId%>');
    });
</script>
<table style="width: 100%" class="frmInput">
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
        <td><input type="text" id="txtProductName<%=languageId%>" value="<%=WSI.Translation.Name%>" /></td>
        <td class="lbright">Bảo hành</td>
        <td><input type="text" id="txtWarranty<%=languageId%>" value="<%=WSI.Translation.Warranty%>" /></td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SHORT_DESCRIPTION", langId)%></td>
        <td colspan="3"><textarea id="txtProductShort<%=languageId%>" rows="3" cols="1" style="width: 100%;"><%=WSI.Translation.ShortDescription%></textarea></td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
        <td colspan="3"><textarea id="txtProductDescription<%=languageId%>" rows="10" cols="1" style="width: 100%;"><%=WSI.Translation.Description%></textarea></td>
    </tr>
</table>
<%
            break;
            case "ProductCategory":
%>
<script type="text/javascript">
    $(function () {
        $(function () {
            if (CKEDITOR.instances['txtCategoryDescription' + '<%=languageId%>'])
                delete CKEDITOR.instances['txtCategoryDescription' + '<%=languageId%>'];
            CKEDITOR.replace('txtCategoryDescription' + '<%=languageId%>');
        });
    });
</script>
<table style="width: 100%" class="frmInput">
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PC_NAME", langId) %></td>
        <td><input type="text" id="txtCategoryName<%=languageId%>" name="txtCategoryName<%=languageId%>" value="<%=WSI.Translation.Name%>" /></td>
    </tr>
    <tr>
        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
        <td><textarea id="txtCategoryDescription<%=languageId%>" name="txtCategoryDescription<%=languageId%>" cols="30" rows="5" style="width: 80%;"><%=WSI.Translation.Description%></textarea></td>
    </tr>
</table>
<%
            break;
            
    }
%>