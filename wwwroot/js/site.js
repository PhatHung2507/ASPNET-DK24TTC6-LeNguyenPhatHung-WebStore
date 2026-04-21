$(window).on('scroll', function () {
    if ($(window).scrollTop() > 50) {
        $('.main-header').addClass('fixed-nav');
    } else {
        $('.main-header').removeClass('fixed-nav');
    }
});