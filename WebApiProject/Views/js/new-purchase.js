﻿$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        var jsonObj = convertQueryStringToJSON(queryString);

        postData(jsonObj);
        
    });

    $('#raw-json-submitBtn').bind('click', function () {
        submitRawJsonData();
    });

});


function submitRawJsonData() {
    
    event.preventDefault();

    var jsonText = document.getElementById('raw-json').value;
    var jsonObj = JSON.parse(jsonText);

    postData(jsonObj);
    
}


function writeMsg($obj, text) {
    
    $obj.html(text);
    $('h3').show();

}


function postData(jsonObj) {

    writeMsg($('#json'), JSON.stringify(jsonObj, null, 4));

    $.toast().reset('all');
    $('.spinner').show();
    
    var xhrPromise = jqXHR('PUT', 'new-purchase', 'application/json; charset=utf-8', jsonObj);
    xhrPromise /* promise callbacks are executed in order */
    .always(function (response) {
        $('.spinner').hide();
    })
    .done(function (response) {
        //console.log(response);
        if (response) {
            if (response[0].Result) {
                if (response[0].Result == 'OK') {
                    toastMsg('SUCCESS', 'New Purchase ID : ' + response[0].NewID, 'success', 'small');
                } else if (response[0].Result == 'ERROR') {
                    toastMsg('SERVER ERROR', response[0].ErrMsg, 'error', 'large');
                }
            } else {
                toastMsg('MESSAGE', 'INVALID JSON RETURNED', 'error', 'small');
            }
        } else {
            toastMsg('MESSAGE', 'NULL JSON RETURNED', 'error', 'small');
        }
    });
    //.fail(function (response) {
    //    console.log(response);
    //    /* make further ui changes on xhr fail */
    //});

}