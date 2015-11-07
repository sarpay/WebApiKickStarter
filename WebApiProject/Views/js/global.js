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


function convertQueryStringToJSON(qs) {

    var pairs = qs.split('&');

    var result = {};
    pairs.forEach(function (pair) {
        pair = pair.split('=');
        result[pair[0]] = decodeURIComponent(pair[1].replace(/\+/g, ' ') || ''); /* replace + with space ' ' */
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


function getQueryParamValByName(name) {

    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return 0;
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));

}


function parseQueryParamsToObject(query) {

    /*
    ------
    USAGE:
    ------
    var params = parseQuery(queryString);
    if (typeof params.src != 'undefined') {
        if (params.src.length > 0) {
            document.write('<iframe id="' + params.id + '" src="' + params.src + '" width="' + params.width + 
                '" height="' + params.height + '" scrolling="no" frameborder="0" class="noScrolling"></iframe>');
        }
    }
    */
    var params = new Object();
    if (!query) return params; // return empty object

    var pairs = query.split(/[;&]/);
    for (var i = 0; i < pairs.length; i++) {
        var keyVal = pairs[i].split('=');
        if (!keyVal || keyVal.length != 2) continue;
        var key = unescape(keyVal[0]);
        var val = unescape(keyVal[1]);
        val = val.replace(/\+/g, ' ');
        params[key] = val;
    }

    return params;

}


function setStylesheet(styleName) {

    if (document.createStyleSheet) {
        document.createStyleSheet('styles/' + styleName);
    }
    else {
        $("head").append($("<link rel=\"stylesheet\" href=\"styles/" + styleName + "\" type=\"text/css\" media=\"screen\" />"));
    }

}


var storageKeyPrefix = 'WebApiKickStarter_';

function setStorageItem(key, value, type) {

    if (typeof (Storage) !== 'undefined') {
        if (window[type + 'Storage']) {
            var storage = window[type + 'Storage'];
            storage.setItem(storageKeyPrefix + key, value);
            //console.log(storage.getItem(storageKeyPrefix + key));
        }
    } else {
        if (type == 'local') {
            $.cookie(storageKeyPrefix + key, value, { expires: 3650 }); /*expires in 10 years*/
        } else {
            $.cookie(storageKeyPrefix + key, value); /*expires at the end of session - time is not specified*/
        }
    }

}


function getStorageItem(key, type) {

    var item = null;

    if (typeof (Storage) !== 'undefined') {
        if (window[type + 'Storage']) {
            var storage = window[type + 'Storage'];
            item = storage.getItem(storageKeyPrefix + key);
            //console.log(item);
        }
    } else {
        item = $.cookie(storageKeyPrefix + key);
    }

    return item;

}


function removeStorageItem(key, type) {

    if (typeof (Storage) !== 'undefined') {
        if (window[type + 'Storage']) {
            var storage = window[type + 'Storage'];
            storage.removeItem(storageKeyPrefix + key);
        }
    } else {
        $.cookie(storageKeyPrefix + key, null);
    }

}


function clearStorageItems() {

    //removeStorageItem('username', 'local');
    removeStorageItem('Ticket', 'session');

}


Array.prototype.remove_by_value = function () {

    //*** remove an item from an array with the value of 'xxx'
    //*** usage: names.remove_by_value('xxx');
    var what, a = arguments, m = a.length, ax;

    while (m && this.length) {
        what = a[--m];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }

    return this;

};