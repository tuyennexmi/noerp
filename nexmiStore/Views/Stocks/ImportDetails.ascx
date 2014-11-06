<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.ImportDetails> Details = (List<NEXMI.ImportDetails>)Session["Details"];
    foreach (NEXMI.ImportDetails Item in Details)
    {
%>
        <tr id="<%=Item.ProductId%>">
            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
            <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
            <td><%=Item.GoodQuantity.ToString("N3")%></td>
            <td><%=Item.BadQuantity.ToString("N3")%></td>
    <% 
        if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
        {
    %>
            <td><%=Session["SNs" + Item.ProductId]%></td>
    <% 
        }    
    %>
            <td><%=Item.Description%></td>
            <td class="actionCols">
                <button type="button" class="btActions update" onclick="javascript:fnShowImportDetail('<%=Item.ProductId%>')"></button>
                <button type="button" class="btActions delete" onclick="javascript:fnRemoveImportDetail('<%=Item.ProductId%>')"></button>
            </td>
        </tr>
<% 
    }    
%>