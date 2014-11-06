<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.ExportDetails> Details = (List<NEXMI.ExportDetails>)Session["Details"];
    foreach (NEXMI.ExportDetails Item in Details)
    {
%>
        <tr id="<%=Item.ProductId%>">
            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
            <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
            <td><%=Item.RequiredQuantity.ToString("N3")%></td>
            <td><label class="PISQuantity" title="<%=Item.ProductId%>"></label></td>
    <% 
        if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
        {
    %>
            <td><%=Session["SNs" + Item.ProductId]%></td>
    <% 
        }    
    %>
            <td><%=Item.Description %></td>
            <td class="actionCols">
                <button type="button" class="btActions update" onclick="javascript:fnShowExportProductDetail('<%=Item.ProductId%>')"></button>
                <button type="button" class="btActions delete" onclick="javascript:fnRemoveExportProductDetail('<%=Item.ProductId%>')"></button>
            </td>
        </tr>
<% 
    }
%>