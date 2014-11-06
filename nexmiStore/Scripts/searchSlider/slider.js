/*  js function for Toggle Slide  */
/*  by- Roshan Rajopadhyaya       */
/*  www.rosanko.blogspot.com      */
$(document).ready(function () {
    $('div.topBlock').click(function () {
		$('.dropList').slideToggle(400);
		$('div.topBlock').css('visibility','hidden');
    });
	$('.close').click(function () {
		$('.dropList').slideToggle(400);
		$('div.topBlock').css('visibility','visible');
    });
	
});