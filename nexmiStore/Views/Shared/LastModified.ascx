<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<tr>
    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %></td>
    <td><%= NEXMI.NMCommon.GetCustomerName(ViewData["CreatedBy"].ToString())%></td>
    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CREATED_DATE", langId) %></td>
    <td><%=DateTime.Parse(ViewData["CreatedDate"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt") %></td>
</tr>

<tr>
    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MODIFIED_BY", langId) %></td>
    <td><%= (ViewData["ModifiedBy"] == null) ? "" : NEXMI.NMCommon.GetCustomerName(ViewData["ModifiedBy"].ToString())%></td>
    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MODIFIED_DATE", langId) %></td>
    <td><%=DateTime.Parse(ViewData["ModifiedDate"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt")%></td>
</tr>