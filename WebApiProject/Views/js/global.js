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
    .success(function () {
        toastMsg('XHR SUCCESS', 'Successfully resolved request', 'success', 'small');
    })
    .fail(function (error) {
        //console.log(error);
        var responseJSON = error.responseJSON;
        var msgText = error.status + ' : ' + error.statusText;
        if (error.status === 0) {
            msgText += '<br/><br/>Your request has timed out or network connection was lost.';
            toastMsg('XHR ERROR', msgText, 'error', 'small');
        }
        else if (error.status === 500 && responseJSON.StackTrace) {
            msgText += '<br/><br/><b>Exception Type: </b>' + responseJSON.ExceptionType;
            msgText += '<br/><br/><b>Exception Message: </b>' + responseJSON.ExceptionMessage;
            msgText += '<br/><br/><b>Stack Trace: </b>' + responseJSON.StackTrace;
            toastMsg('XHR ERROR', msgText, 'error', 'large');
        }
        else {
            msgText += '<br/><br/><b>Message: </b>' + responseJSON.Message;
            msgText += '<br/><br/><b>Message Detail: </b>' + responseJSON.MessageDetail;
            toastMsg('XHR ERROR', msgText, 'error', 'small');
        }
    });

}


/*** REQUIRES ES6 HARMONY ***/
/*
Expects a request method type (GET, POST, PUT, DELETE..)
Expects a target route to the web api method (a url)
Expects an optional data as JSON object. (not string)
*/
function XHR(requestMethod, targetRoute, requestType, data) {

    var apiRoot = '../api/';

    //** Return a new promise.
    return new Promise(function (resolve, reject) {

        //** Do the usual XHR stuff
        var xr = new XMLHttpRequest();

        xr.open(requestMethod, apiRoot + targetRoute, true, null, null);

        xr.setRequestHeader('Accept', requestType);
        xr.setRequestHeader('Content-Type', requestType);
        xr.responseType = "json"; //** return a JavaScript object, parsed from a JSON string returned by the server
        xr.timeout = 60000; //** 60 seconds
        xr.withCredentials = false; //** CORS using credentials such as cookies or authorization headers

        xr.onload = function () {
            if (xr.status == 200 && xr.statusText == 'OK') {
                toastMsg('XHR SUCCESS', 'Successfully resolved request', 'success', 'small');
                resolve(xr.response); //** Resolve the promise with the response
            } else { //** Otherwise reject
                //console.log(xr);
                var msgText = xr.status + ' : ' + xr.statusText;
                if (xr.status === 500 && xr.response.StackTrace) {
                    msgText += '<br/><br/><b>Exception Type: </b>' + xr.response.ExceptionType;
                    msgText += '<br/><br/><b>Exception Message: </b>' + xr.response.ExceptionMessage;
                    msgText += '<br/><br/><b>Stack Trace: </b>' + xr.response.StackTrace;
                    toastMsg('XHR ERROR', msgText, 'error', 'large');
                } else {
                    msgText += '<br/><br/><b>Message: </b>' + xr.response.Message;
                    msgText += '<br/><br/><b>Message Detail: </b>' + xr.response.MessageDetail;
                    toastMsg('XHR ERROR', msgText, 'error', 'small');
                }
                reject(xr.response); //** Reject the promise with the response
            }
        };

        //** Handle network errors?
        xr.onerror = function () {
            msgText = xr.response;
            toastMsg('XHR ERROR', msgText, 'error', 'small');
            reject(xr.response);
        };

        //** Handle timeout errors
        xr.ontimeout = function () {
            msgText = 'Your request has timed out.';
            toastMsg('XHR ERROR', msgText, 'error', 'small');
            reject(xr.response);
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


function toastMsg(title, text, icon, boxSize) {

    var hideAfter = (icon == 'error' ? false : 3600);

    $.toast({
        text: text, // Text that is to be shown in the toast
        heading: title, // Optional heading to be shown on the toast
        showHideTransition: 'fade', // fade, slide, plain
        allowToastClose: true, // Boolean value true, false
        hideAfter: hideAfter, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
        stack: 3, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
        position: 'bottom-left', // bottom-left, bottom-right, bottom-center, top-left, top-right, top-center, mid-center, an object representing the left, right, top, bottom values
        //bgColor: '#b91713',  // Background color of the toast
        //textColor: '#f7f7f7',  // Text color of the toast
        textAlign: 'left',  // Text alignment i.e. left, right, center
        icon: icon //error, info, warning, success
        //beforeShow: function () {}, // will be triggered before the toast is shown
        //afterShown: function () {}, // will be triggered after the toat has been shown
        //beforeHide: function () {}, // will be triggered before the toast gets hidden
        //afterHidden: function () {}  // will be triggered after the toast has been hidden
    });

    var boxSize = (boxSize == 'large' ? boxSize = '800px' : '300px');
    $('.jq-toast-wrap').css('width', boxSize);

}