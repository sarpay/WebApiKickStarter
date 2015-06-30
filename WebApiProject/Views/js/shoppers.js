var GRID;

$(document).ready(function () {

    gridInit();

});


function gridInit() {

    //-------------------------
    //*** Grid Binding & Events
    //-------------------------

    var gridDatasource = new kendo.data.DataSource({
        transport: {

            read: function (request) {

                $('#msg').text('Loading..');

                var ajaxCall = makeAjaxCall('GET', 'shoppers', ''); //*** does not expect any params.
                ajaxCall.done(function (data) {
                    //$.each(data, function (key, item) {
                    //    $('<li>', { text: formatItem(item) }).appendTo($('ul#genders'));
                    //});
                    if (data) {
                        request.success(data); // bind data to grid
                        if (!data[0]) {
                            //console.log('SERVER RETURNED EMPTY JSON');
                            $('#msg').text('SERVER RETURNED EMPTY JSON');
                        } else {
                            $('#msg').text('');
                        }
                    } else {
                        $('#msg').text('SERVER RETURNED EMPTY JSON');
                    }
                });
                ajaxCall.error(function (data) {
                    request.error(data); //*** notify the kendo datasource that the request failed
                });

                //PARAMS = {};
                ////PARAMS.Ticket = TICKET;
                //PARAMS_JSON = JSON.stringify(PARAMS);

                //var ajaxCall = makeAjaxCall('GET', GET_SHOPPERS, PARAMS_JSON);
                //ajaxCall.success(function (responseJsonData) {
                //    //console.log(responseJsonData);
                //    request.success(responseJsonData); // notify the kendo datasource that the request succeeded
                //    if (responseJsonData[0]) {
                //        var response = responseJsonData[0];
                //        if (response.ID == 0) { //*** if response is NOT undefined
                //            console.log(response.Email); //*** used for debugging and authentication failure action
                //        } else {

                //        }
                //    }
                //});
                //ajaxCall.error(function (responseJsonData) {
                //    request.error(responseJsonData); //*** notify the kendo datasource that the request failed
                //});

            } //*** read ends

        }, //*** transport ends
        batch: false, //*** cannot be true in this setup.
        autoSync: false,
        pageSize: 20,
        schema: {
            //data: 'd',
            total: function (result) { //*** without it pager does not initiate on page load.
                var data = this.data(result);
                return data ? data.length : 0;
            }
        }

    }); //*** dataSource ends


    $('#grid').kendoGrid({
        dataSource: gridDatasource,
        pageable: {
            refresh: true,
            info: true,
            buttonCount: 5
        },
        sortable: false, //*** messes up things when performed while adding a new row.
        reorderable: false,
        selectable: 'row', //*** necessary to send params from editables
        scrollable: false,
        toolbar: kendo.template($('#toolbar_template').html())
    });


    $('.k-grid-ClearSearch', '#grid').bind('click', function (e) {

        //*** clear search filter and elements
        var datasource = $('#grid').data('kendoGrid').dataSource;
        datasource.filter({});

        $('#searchbox').val('').focus();
        $('.k-grid-ClearSearch', '#grid').css('visibility', 'hidden');

    });


    $('#searchbox').keyup(function (e) {

        var searchText = $('#searchbox').val();
        var clearSearchBtn = $('.k-grid-ClearSearch', '#grid');
        var datasource = $('#grid').data('kendoGrid').dataSource;

        if (searchText.length == 0) {

            clearSearchBtn.css('visibility', 'hidden');
            datasource.filter({});

        } else {

            clearSearchBtn.css('visibility', 'visible');
            datasource.filter({
                logic: 'or',
                filters: [
                    {
                        field: 'Email',
                        operator: 'contains',
                        value: searchText
                    },
                    {
                        field: 'Name',
                        operator: 'contains',
                        value: searchText
                    }
                ]
            });

        }

    });
    
}