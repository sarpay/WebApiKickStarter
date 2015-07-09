$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        var jsonObj = convertQueryStringToJSON(queryString);
        var jsonData = JSON.stringify(jsonObj);

        postData(jsonObj, jsonData);
        
    });

    $('#raw-json-submitBtn').bind('click', function () {
        submitRawJsonData();
    });

});


function submitRawJsonData() {
    
    event.preventDefault();

    var jsonText = document.getElementById('raw-json').value;
    var jsonObj = JSON.parse(jsonText);
    var jsonData = JSON.stringify(jsonObj);

    postData(jsonObj, jsonData);
    
}


function writeMsg($obj, text) {
    
    $obj.html(text);
    $('h3').show();

}


function postData(jsonObj, jsonData) {

    writeMsg($('#response'), 'Waiting response...');
    writeMsg($('#json'), JSON.stringify(jsonObj, null, 4));

    var ajaxCall = makeAjaxCall('POST', 'new-shopper', jsonObj);

    ajaxCall.success(function (response) {

        console.log(response);

        if (response[0].Result) {
            var result = response[0].Result;
            var msg;
            if (result == 'OK') {
                msg = 'Success :: New Account ID : ' + response[0].NewID;
            } else if (result == 'ERROR') {
                msg = response[0].ErrMsg;
            }
            writeMsg($('#response'), msg);
        }
        
        //var dataLength = response.length; //*** 1-based indexing
        //if (dataLength > 0) {
        //    //var dataArray = response[0];
        //    //if (dataArray.Error) {
        //    //    writeMsg($('#response'), dataArray.Error);
        //    //} else {
        //        writeMsg($('#response'), 'Success!');// New row id: ' + dataArray.SignID);
        //    //}
        //} else {
        //    //*** ajax call returned nothing
        //}
    });

}