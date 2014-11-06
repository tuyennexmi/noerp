<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<ul class="breadcrumbs-onePOS">
    <li><a href="javascript:fnLoadCategories('')"><img style="width: 24px;" alt="" src="<%=Url.Content("~")%>Content/UI_Images/home.png" /></a></li>
    <% 
        List<NEXMI.NMCategoriesWSI> WSIs = (List<NEXMI.NMCategoriesWSI>)ViewData["WSIs"];
        foreach (NEXMI.NMCategoriesWSI Item in WSIs)
        { 
    %>
    <li><a href="javascript:fnLoadCategories('<%=Item.Category.Id%>')"><%=(Item.Translation == null) ? "" : Item.Translation.Name%></a></li>
    <%
        }
    %>
</ul>