$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        var jsonObj = convertQueryStringToJSON(queryString);

        signIn(jsonObj);

    });

});


function signIn(jsonObj) {

    $.toast().reset('all');
    $('.spinner').show();

    var xhrPromise = jqXHR('POST', 'sign-in', 'application/json; charset=utf-8', jsonObj);
    xhrPromise /* promise callbacks are executed in order */
    .always(function (response) {
        $('.spinner').hide();
    })
    .done(function (response) {
        //console.log(response);
        //if (response) {
        if (response[0].Result) {
            if (response[0].Result == 'OK') {
                toastMsg('SIGN-IN SUCCESS', response[0].Ticket, 'success', 'small');
            } else if (response[0].Result == 'BLOCKED') {
                toastMsg('SIGN-IN FAILED', response[0].Msg, 'warning', 'small');
            } else if (response[0].Result == 'ERROR') {
                toastMsg('SERVER ERROR', response[0].ErrMsg, 'error', 'large');
            }
        } else {
            toastMsg('SERVER ERROR', 'INVALID JSON RETURNED', 'error', 'small');
        }
        //} else {
        //    toastMsg('SERVER ERROR', 'NULL JSON RETURNED', 'error', 'small');
        //}
    });
    //.fail(function (response) {
    //    console.log(response);
    //    /* make further ui changes on xhr fail */
    //});

}