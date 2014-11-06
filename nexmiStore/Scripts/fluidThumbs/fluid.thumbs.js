/*

	Fluid Thumbnail Bar
		
	Author	: Sam Dunn
	Source	: Build Internet (www.buildinternet.com)
	Company : One Mighty Roar (www.onemightyroar.com)
	License : MIT License / GPL License

*/

jQuery(function ($) {

    /* Variables
    ----------------------------*/
    var thumbTray = '#thumb-tray',
		thumbList = '#thumb-list',
		thumbNext = '#thumb-next',
		thumbPrev = '#thumb-prev',
		thumbInterval,
		thumbPage = 0;


    /* Setup
    ----------------------------*/
    // Adjust to true width of thumb markers
    $(thumbList).width($('> li', thumbList).length * $('> li', thumbList).outerWidth(true));

    // Hide thumb arrows if not needed
    if ($(thumbList).width() <= $(thumbTray).width()) $(thumbPrev + "," + thumbNext).fadeOut(0);

    // Thumb Intervals
    thumbInterval = Math.floor($(thumbTray).width() / $('> li', thumbList).outerWidth(true)) * $('> li', thumbList).outerWidth(true);

    /* Thumb Navigation
    ----------------------------*/
    $(thumbNext).click(function () {
        if (thumbPage - thumbInterval <= -$(thumbList).width()) {
            thumbPage = 0;
            $(thumbList).stop().animate({ 'left': thumbPage }, { duration: 500, easing: 'easeOutExpo' });
        } else {
            thumbPage = thumbPage - thumbInterval;
            $(thumbList).stop().animate({ 'left': thumbPage }, { duration: 500, easing: 'easeOutExpo' });
        }
    });

    $(thumbPrev).click(function () {
        if (thumbPage + thumbInterval > 0) {
            thumbPage = Math.floor($(thumbList).width() / thumbInterval) * -thumbInterval;
            if ($(thumbList).width() <= -thumbPage) thumbPage = thumbPage + thumbInterval;
            $(thumbList).stop().animate({ 'left': thumbPage }, { duration: 500, easing: 'easeOutExpo' });
        } else {
            thumbPage = thumbPage + thumbInterval;
            $(thumbList).stop().animate({ 'left': thumbPage }, { duration: 500, easing: 'easeOutExpo' });
        }
    });

    /* Window Resize
    ----------------------------*/
    $(window).resize(function () {

        // Update Thumb Interval & Page
        thumbPage = 0;
        thumbInterval = Math.floor($(thumbTray).width() / $('> li', thumbList).outerWidth(true)) * $('> li', thumbList).outerWidth(true);

        // Adjust thumbnail markers
        if ($(thumbList).width() > $(thumbTray).width()) {
            $(thumbPrev + "," + thumbNext).fadeIn('fast');
            $(thumbList).stop().animate({ 'left': 0 }, 200);
        } else {
            $(thumbPrev + "," + thumbNext).fadeOut('fast');
        }
    });
});
