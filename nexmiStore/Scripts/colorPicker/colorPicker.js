(function ($) {
    $.fn.extend({
        colorPicker: function (options) {

            var defaults = {

            };
            var options = $.extend(defaults, options);

            return this.each(function () {
                var opts = options;

                var validHex = /^#[0-9A-F]{6}$/i;
                var obj = $(this);
                var value = '#' + obj.html();
                obj.addClass('colorPicker-container');
                var elmId = obj.attr('id');
                obj.removeAttr('id');

                obj.html('#<input id="' + elmId + '" class="colorPicker-input" maxlength="6" value="" />' +
                '<a id="button-' + elmId + '" class="colorPicker-button" />' +
                '<input type="color" id="colorPicker-' + elmId + '" name="colorPicker-' + elmId + '" value="" class="colorPicker" />');

                var clp = $('#colorPicker-' + elmId);
                clp.on('change', function () {
                    obj.css({ 'background-color': $(this).val() });
                    clpInput.val($(this).val().substring(1));
                });

                var clpInput = $('#' + elmId);
                if (validHex.test(value)) {
                    obj.css({ 'background-color': value });
                    clpInput.val(value.substring(1));
                } else {
                    obj.css({ 'background-color': '' });
                }
                clpInput.on('keyup', function () {
                    var hexColor = '#' + $(this).val();
                    if (validHex.test(hexColor))
                        obj.css({ 'background-color': hexColor });
                    else
                        obj.css({ 'background-color': '' });
                });
                clpInput.on('change', function () {
                    var hexColor = '#' + $(this).val();
                    if (!validHex.test(hexColor)) {
                        obj.css({ 'background-color': '' });
                        $(this).val('');
                    }
                });

                var clpButton = $('#button-' + elmId);
                clpButton.click(function () {
                    clp[0].click();
                });
            });
        }
    });
})(jQuery);