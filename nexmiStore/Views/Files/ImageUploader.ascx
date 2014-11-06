<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(function () {
        $('#imageUploader').uploadify({
            'swf': appPath + 'Scripts/uploadify/uploadify.swf',
            'uploader': appPath + 'Files/UploadImage',
            'buttonText': 'Select Images',
            'multi': true,
            'auto': false,
            'fileTypeDesc': 'Image Files',
            'fileTypeExts': '*.gif; *.jpg; *.png',
            'fileSizeLimit': '50MB',
            'removeTimeout': 1,
            'onUploadSuccess': function (file, data, response) {
                fnAddToList(data);
            }
        });
    });
</script>
<input type="file" id="imageUploader" name="imageUploader" />
<button type="button" onclick="javascript:$('#imageUploader').uploadify('upload', '*');" class="button">Upload</button>
<button type="button" onclick="javascript:$('#imageUploader').uploadify('cancel', '*');" class="button">Cancel</button>