$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        var jsonObj = convertQueryStringToJSON(queryString);

        signIn(jsonObj);

    });

});


function signIn(jsonObj) {

    var xhrPromise = jqXHR('POST', 'sign-in', 'application/json; charset=utf-8', jsonObj);
    xhrPromise /* promise callbacks are executed in order */
    .always(function (response) {
        
    })
    .done(function (response) {
        console.log(response);
        if (response) {
            //if (response[0].Result) {
                if (response.Result == 'OK') {
                    setStorageItem('UserName', response.UserName, 'local');
                    setStorageItem('UserId', response.UserId, 'session');
                    setStorageItem('Ticket', response.Ticket, 'session');
                    toastMsg('SIGN-IN SUCCESS', response.Ticket, 'success', 'small');
                } else if (response.Result == 'BLOCKED') {
                    toastMsg('SIGN-IN FAILED', response.Msg, 'warning', 'small');
                } else if (response.Result == 'ERROR') {
                    toastMsg('SERVER ERROR', response.ErrMsg, 'error', 'large');
                }
            //} else {
            //    toastMsg('SERVER ERROR', 'INVALID JSON RETURNED', 'error', 'small');
            //}
        } else {
            toastMsg('SERVER ERROR', 'NULL JSON RETURNED', 'error', 'small');
        }
    });
    //.fail(function (response) {
    //    console.log(response);
    //    /* make further ui changes on xhr fail */
    //});

}