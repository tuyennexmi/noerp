<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<style type="text/css">
  #gridCollections { list-style-type: none; margin: 5px; padding: 0; width: 100%; }
  #gridCollections li { margin: 3px 3px 3px 0; padding: 5px; float: left; width: 120px; height: 120px; text-align: center; border: 1px solid #ccc; }
</style>
<script type="text/javascript">
    $(function () {
        $("#gridCollections").sortable();
        $("#gridCollections").disableSelection();
    });

    function fnSaveOrder() {
        $.post(appPath + 'UserControl/SaveOrderObjects', { data: $("#gridCollections").sortable("toArray").toString() }, function (data) {
            if (data == '')
                alert('Đã lưu!');
            else
                alert(data);
        });
    }
</script>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                    <button class="button" onclick="javascript:fnSaveOrder()"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                </td>
                <td></td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <ul id="gridCollections">
        <% 
            List<NEXMI.NMCategoriesWSI> WSIs = (List<NEXMI.NMCategoriesWSI>)ViewData["WSIs"];
            foreach (NEXMI.NMCategoriesWSI Item in WSIs)
            {
                if (Item.Translation == null)
                    Item.Translation = new NEXMI.Translations();
        %>
            <li id="<%=Item.Category.Id%>">
                <table style="width: 100%; height: 100%;">
                    <tr>
                        <td style="height: 20px; font-weight: bold;"><%=Item.Translation.Name%></td>
                    </tr>
                </table>
            </li>
        <%
            }
        %>
        </ul>
    </div>
</div>