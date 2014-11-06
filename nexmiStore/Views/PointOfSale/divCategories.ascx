<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script type="text/javascript">
    $(function () {
        $('img').on('error', function () {
            $(this).attr('src', appPath + 'Content/UI_Images/noimage.png');
        });
    });
</script>

    <% 
        List<NEXMI.NMCategoriesWSI> WSIs = (List<NEXMI.NMCategoriesWSI>)ViewData["WSIs"];
        foreach (NEXMI.NMCategoriesWSI Item in WSIs)
        {
            if (Item.Translation == null)
                Item.Translation = new NEXMI.Translations();
    %>
    <div class="live-tile category-itemPOS" onclick="javascript:fnLoadCategories('<%=Item.Category.Id%>')">
        <div>
            <img class="full" src="<%=Url.Content("~")%>Content/avatars/<%=Item.Category.Image%>" alt="" />
            <span class="tile-title" style="background-color: rgba(0,0,0,0.5); font-size: 110%;"><%=Item.Translation.Name%></span>
        </div>
    </div>    
    <%
        }
    %>
