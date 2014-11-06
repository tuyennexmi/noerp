<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(function () {
        try {
            $('.subMenu').find('.subCurrent').removeClass('subCurrent');
            $('#' + fnGetHashParameter('functionId')).addClass('subCurrent');
        } catch (err) { }
    });
</script>
<div class="subMenu">
    <ul>
        <li style="padding:0px;">
            <ul>
            <% 
                string name = "";
                List<NEXMI.NMFunctionsWSI> FunctionWSIs = (List<NEXMI.NMFunctionsWSI>)ViewData["WSIs"];
                foreach (NEXMI.NMFunctionsWSI FunctionItem in FunctionWSIs)
                {
                    if (Session["Lang"].ToString() == "vi")
                        name = FunctionItem.Name;
                    else if (Session["Lang"].ToString() == "kr")
                        name = FunctionItem.NameInKorea;
                    else if (Session["Lang"].ToString() == "en")
                        name = FunctionItem.NameInEnglish;
                    
                    if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], FunctionItem.Id, ""))
                    { 
            %>
                <li id="<%=FunctionItem.Id%>">
                    <a title="<%=FunctionItem.Description%>" href="javascript:fnChangeHash('<%=FunctionItem.ModuleId%>', '<%=FunctionItem.Id%>', '<%=FunctionItem.Action%>')">
                        <img style='vertical-align:middle' src= '<%=Url.Content("~")%>Content/UI_Images/<%=FunctionItem.Icon%>'/>
                        <%=name %>
                    </a>
                </li>
            <% 
                    }
                }
            %>
            </ul>
        </li>
    </ul>
</div>