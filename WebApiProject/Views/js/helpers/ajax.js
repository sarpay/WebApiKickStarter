var apiKey = 'AV3xqDcx3txaGAkN';


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

    var apiRoot = '../';
    var url = apiRoot + targetRoute;

    switch (requestMethod) {
        case 'GET-URI':
            requestMethod = 'GET';
            url += (data ? '/' + data : '');
            if (!data) { data = ''; }
            break;
        case 'GET-QS':
            requestMethod = 'GET';
            break;
        case 'POST':
            data = JSON.stringify(data);
            break;
        case 'PUT':
            data = JSON.stringify(data);
            break;
    }

    /* add the API KEY to all requests 
        - works in conjunction with ApiKeyHandler.cs 
    */
    url += '/?key=' + apiKey;

    /* Make the Ajax call */
    return $.ajax({
        type: requestMethod,
        headers: {
            Accept: contentType,
            'Content-Type': contentType
        },
        dataType: 'json',
        timeout: 60000, /*60 seconds*/
        cache: false,
        global: true, /*Set to false to prevent the global handlers like ajaxStart or ajaxStop from being triggered.*/
        xhrFields: {
            withCredentials: false
        },
        abortOnRetry: true,
        url: url,
        data: data,
        async: true,
        beforeSend: function (xhrObj) {
            //console.log(xhrObj);
            xhrObj.setRequestHeader(
                'Authorization',
                'Basic ' + btoa(getStorageItem('UserId', 'session') + ':' + getStorageItem('Ticket', 'session'))
            );
            return true;
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

        if (error.status === 0) { /** happens when beforeSend returns false **/
            msgText += '<br/><br/>Your request has timed out or network connection was lost.';
            toastMsg('XHR ERROR', msgText, 'error', 'small');

        } else if (error.status === 500 && responseJSON.StackTrace) { /* Internal Server Error */
            msgText += '<br/><br/><b>Exception Type: </b>' + responseJSON.ExceptionType;
            msgText += '<br/><br/><b>Exception Message: </b>' + responseJSON.ExceptionMessage;
            msgText += '<br/><br/><b>Stack Trace: </b>' + responseJSON.StackTrace;
            toastMsg('XHR ERROR', msgText, 'error', 'large');

        } else if (error.status === 401) { /* Unauthorized */
            //window.location.href = 'sign-up.html';
            msgText += '<br/><br/><b>Message: </b>' + responseJSON.Message;
            msgText += '<br/><br/><b>Message Detail: </b>' + responseJSON.MessageDetail;
            toastMsg('AUTH ERROR', msgText, 'error', 'small');

        } else if (error.status === 403) { /* Forbidden */
            //window.location.href = 'sign-in.html';
            msgText += '<br/><br/><b>Message: </b>' + responseJSON.Message;
            msgText += '<br/><br/><b>Message Detail: </b>' + responseJSON.MessageDetail;
            toastMsg('AUTH ERROR', msgText, 'error', 'small');

        } else if (error.status === 404) { /* Not Found */
            window.location.href = 'not-found.html';

        } else {
            msgText += '<br/><br/><b>Message: </b>' + responseJSON.Message;
            msgText += '<br/><br/><b>Message Detail: </b>' + responseJSON.MessageDetail;
            toastMsg('XHR ERROR', msgText, 'error', 'small');
        }
    })
    .always(function () { /** should always be at the end **/

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