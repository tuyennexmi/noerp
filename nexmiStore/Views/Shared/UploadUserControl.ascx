<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    var queueSize = 0;
    $(document).ready(function () {
        $("#fileInput").uploadify({
            'swf': appPath + "Scripts/uploadify/uploadify.swf",
            'uploader': appPath + "Common/UploadFile",
            'buttonText': "Select Image",
            'multi': true,
            'auto': false,
            'fileTypeDesc': 'Image Files',
            'fileTypeExts': '*.gif; *.jpg; *.png',
            'fileSizeLimit': 2000,
            'removeTimeout': 1,
            'onUploadSuccess': function (file, data, response) {
                fnAddToList(data);
            }
        });
    });

    function fnUpload() {
        $("#fileInput").uploadify('upload', '*');
    }

    function fnCancel() {
        $("#fileInput").uploadify('cancel', '*');
    }
</script>
<input type="file" id="fileInput" name="fileInput" />
<button type="button" onclick="javascript:fnUpload()" class="button">Upload</button>
<button type="button" onclick="javascript:fnCancel()" class="button">Cancel</button>