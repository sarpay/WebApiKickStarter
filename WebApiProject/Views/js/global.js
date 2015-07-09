function makeAjaxCall(type, uri, data) {

    var routeRoot = '../api/';

    switch (type) {

        case 'GET':
            return $.getJSON(routeRoot + uri + '/' + data)

        case 'POST':
            return $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                timeout: 60000, //*** 60 seconds
                cache: false,
                abortOnRetry: true,
                url: routeRoot + uri,
                data: JSON.stringify(data),
                async: true
            });

    }

}


/*** REQUIRES jQuery 1.8 and up ***/
/*
Expects a request method type (GET, POST, PUT, DELETE..)
Expects a target route to the web api method (a url)
Expects an optional data as JSON object or a FORM. (not string)
*/
function jqXHR(requestMethod, targetRoute, contentType, data) {

    var apiRoot = '../api/';
    var url = apiRoot + targetRoute;

    switch (requestMethod) {
        case 'GET':
            url += (data ? '/' + data : '');
            if (!data) { data = ''; }
            break;
        case 'POST':
            data = JSON.stringify(data);
            break;
    }

    return $.ajax({
        type: requestMethod,
        headers: {
            Accept: contentType,
            'Content-Type': contentType
        },
        dataType: 'json',
        timeout: 60000, //*** 60 seconds
        cache: false,
        xhrFields: {
            withCredentials: false
        },
        abortOnRetry: true,
        url: url,
        data: data,
        async: true,
        beforeSend: function (xhrObj) {
            //console.log(xhrObj);
        }
        //contentType: 'application/x-www-form-urlencoded; charset=UTF-8' //default
        //contentType: 'application/atom+xml' //Atom
        //contentType: 'text/css' //CSS
        //contentType: 'text/javascript' //JavaScript
        //contentType: 'image/jpeg' //JPEG Image
        //contentType: 'application/json' //JSON
        //contentType: 'application/pdf' //PDF
        //contentType: 'application/rss+xml; charset=ISO-8859-1' //RSS
        //contentType: 'text/plain' //Text (Plain)
        //contentType: 'text/xml' //XML
    })
    .fail(function (error) {
        //console.log(error);
        $('#msg').html('<b>XHR ERROR</b>');
        if (error.status === 0) {
            $('#msg').append('<br/>Your request has timed out or network connection was lost.');
        }
        else if (error.status == 500) {
            $('#msg').append('<br/>Internal Server Error [500]');
            $('#msg').append('<br/><br/><b>Exception Type: </b>' + error.responseJSON.ExceptionType);
            $('#msg').append('<br/><br/><b>Exception Message: </b>' + error.responseJSON.ExceptionMessage);
            $('#msg').append('<br/><br/><b>Stack Trace: </b>' + error.responseJSON.StackTrace);
        }
        else {
            $('#msg').append('<br/>' + error.statusText);
            $('#msg').append('<br/><br/><b>Message: </b>' + error.responseJSON.Message);
            $('#msg').append('<br/><br/><b>Message Detail: </b>' + error.responseJSON.MessageDetail);
        }
    });

}


/*** REQUIRES ES6 HARMONY ***/
/*
Expects a request method type (GET, POST, PUT, DELETE..)
Expects a target route to the web api method (a url)
Expects an optional data as JSON object. (not string)
*/
function XHR(requestMethod, targetRoute, data) {

    var apiRoot = '../api/';

    //** Return a new promise.
    return new Promise(function (resolve, reject) {

        //** Do the usual XHR stuff
        var xr = new XMLHttpRequest();

        xr.open(requestMethod, apiRoot + targetRoute, true);

        xr.setRequestHeader('Accept', 'application/json');
        xr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        xr.responseType = "json"; //** return a JavaScript object, parsed from a JSON string returned by the server
        xr.timeout = 60000; //** 60 seconds
        xr.withCredentials = false; //** CORS using credentials such as cookies or authorization headers

        xr.onload = function () {
            //** This is called even on 404 etc.. so check the status
            if (xr.status == 200) {
                // Resolve the promise with the response
                //resolve(JSON.parse(xr.response)); //** use if responseType is not set
                resolve(xr.response);
            }
            else {
                //** Otherwise reject with the status text
                //reject(JSON.parse(xr.responseText)); //** use if responseType is not set
                reject(xr.response);
            }
        };

        //** Handle network errors
        xr.onerror = function () {
            reject(Error("Network Error"));
        };

        //** Make the request
        xr.send(JSON.stringify(data));

    });

}


function convertQueryStringToJSON(qs) {

    var pairs = qs.split('&');

    var result = {};
    pairs.forEach(function (pair) {
        pair = pair.split('=');
        result[pair[0]] = decodeURIComponent(pair[1] || '');
    });

    return JSON.parse(JSON.stringify(result));

}


function checkZipCodeValidation(zip) {

    //console.log(zip);
    var res = /^\d{5}(?:[-\s]\d{4})?$/.test(zip);
    return res;

}


function checkEmailValidation(email) {

    //console.log(email);
    var res = /^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$/.test(email);
    return res;

}