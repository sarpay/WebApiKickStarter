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
    var params = parseQueryParamsToObject(queryString);
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