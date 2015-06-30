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

    var ajaxCall = makeAjaxCall(FUNC_NEW_SIGNATURE, jsonData);
    ajaxCall.success(function (responseJsonData) {
        //console.log(responseJsonData);
        var dataLength = responseJsonData.d.length; //*** 1-based indexing
        if (dataLength > 0) {
            var dataArray = responseJsonData.d[0];
            //console.log(dataArray);
            if (dataArray.Error) {
                writeMsg($('#response'), dataArray.Error);
            } else {
                writeMsg($('#response'), 'Success! New row id: ' + dataArray.SignID);
            }
        } else {
            //*** ajax call returned nothing
        }
    });

}