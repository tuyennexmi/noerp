<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<table style="background-color:Green" width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="4"><b><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %></b></td>
    </tr>
    <tr>
        <td colspan="4"><%=NEXMI.NMCommon.GetCompany().Customer.Address %></td>
    </tr>
    <tr>
        <td>Tel: <%=NEXMI.NMCommon.GetCompany().Customer.Telephone %></td>
        <td>Fax: <%=NEXMI.NMCommon.GetCompany().Customer.FaxNumber %></td>
        <td><%=NEXMI.NMCommon.GetCompany().Customer.EmailAddress %></td>
        <td><%=NEXMI.NMCommon.GetCompany().Customer.Website %></td>
    </tr>
</table>