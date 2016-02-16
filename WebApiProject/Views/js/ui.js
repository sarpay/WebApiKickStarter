$(document).ajaxStart(function () {
    $.toast().reset('all');
    $('.ui-page').hide();
    $('.ui-load-container').show();
});


$(document).ajaxStop(function () {
    $('.ui-load-container').hide();
    $('.ui-page').show();
});