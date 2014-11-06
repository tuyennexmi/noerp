/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.enterMode = CKEDITOR.ENTER_BR;
    //config.baseFloatZIndex = ;
    config.extraPlugins = 'mediaembed';
    config.toolbar = 'DefaultToolbar';
    config.toolbar_DefaultToolbar =
	[
        { name: 'document', items: ['Source', 'Templates'] },
		{ name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
		{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'Scayt'] },
		{ name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe', 'MediaEmbed'] },
        { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
        '/',
		{ name: 'styles', items: ['Font', 'FontSize', 'Styles', 'Format'] },
        { name: 'colors', items: ['TextColor', 'BGColor'] },
		{ name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', '-', 'RemoveFormat'] },
		{ name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
		{ name: 'tools', items: ['Maximize', '-', ] }
	];
};
