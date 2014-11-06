<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(function () {
        $("#fileUpload").change(function () {
            loadFile(document.getElementById("fileUpload"));
        });
    });
    function fileUploadClick() {
        $("#fileUpload").click();
    }

    function loadFile(evt) {
        var file = evt.files[0];
        var reader = new FileReader();
        reader.onload = appendToList;
        reader.readAsDataURL(file);
    }

    function appendToList(evt) {
        var file = $("#fileUpload")[0].files[0];
        var fileName = file.name;
        var size = file.size;
        var data = evt.target.result.split(',')[1];
        var id = fnRandomString();
        var row = "";
        row += "<td>" + fileName + "<input type='hidden' name='attachFiles' value='" + data + "' title='" + fileName + "' /></td>";
        row += "<td>" + size + " bytes</td>";
        row += "<td><a href='javascript:removeFromList(\"" + id + "\")'>X</a></td>";
        $("#tbFiles tbody").append("<tr id='" + id + "'>" + row + "</tr>");
    }

    function removeFromList(id) {
        $("#" + id).remove();
    }
</script>
<table style="width: 100%">
    <tr>
        <td valign="top">
            <input type="file" id="fileUpload" style="display: none;" />
            <img id="avatar" alt="" onclick="javascript:fileUploadClick()" src="<%=Url.Content("~")%>Content/UI_Images/24Attach-icon.png" style="cursor: pointer;" />
        </td>
        <td>
            <table style="width: 100%" id="tbFiles" class="tbDetails">
                <tbody>
            <% 
                if (ViewData["Attachment"] != null)
                {
                    string attachmentName = ViewData["AttachmentName"].ToString();
                    string attachmentSize = ViewData["AttachmentSize"].ToString();
                    string attachment = ViewData["Attachment"].ToString();
            %>
                    <tr id="defaultFile">
                        <td><%=attachmentName%><input type="hidden" name="attachFiles" value="<%=attachment%>" title="<%=attachmentName%>" /></td>
                        <td><%=attachmentSize%></td>
                        <td><a href="javascript:removeFromList('defaultFile')">X</a></td>
                    </tr>
            <%
                }
                if (Session["ImageWSIs"] != null)
                {
                    List<NEXMI.NMImagesWSI> ImageWSIs = (List<NEXMI.NMImagesWSI>)Session["ImageWSIs"];
                    foreach (NEXMI.NMImagesWSI Item in ImageWSIs)
                    {
            %>
                        <tr id="<%=Item.Id %>">
                        <td><%=Item.Name%><input type="hidden" name="readyFiles" value="" title="" /></td>
                        <td></td>
                        <td><a href="javascript:removeFromList('<%=Item.Id %>')">X</a></td>
                    </tr>
            <%      }
                } %>
                </tbody>
            </table>
        </td>
    </tr>
</table>