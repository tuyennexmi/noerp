<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PrintPage</title>
    <script src="<%=Url.Content("~")%>Scripts/PDFPlugin/pdfobject.js" type="text/javascript"></script>
    <script src="<%=Url.Content("~")%>Scripts/PDFPlugin/jspdf.js" type="text/javascript"></script>
</head>
<body>
    <div>
        <iframe class="preview-pane" width="100%" height="650" frameborder="0" src="data:application/pdf;base64,<%=Session["DataUrl"]%>"></iframe>
    </div>
</body>
</html>
