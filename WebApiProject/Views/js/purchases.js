var GRID;

$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        var jsonObj = convertQueryStringToJSON(queryString);
        var jsonData = JSON.stringify(jsonObj);
        console.log(jsonData);
        gridInit(jsonData);

    });

});


function gridInit(jsonData) {

    //-------------------------
    //*** Grid Binding & Events
    //-------------------------

    var gridDatasource = new kendo.data.DataSource({
        transport: {

            read: function (request) {

                $('#msg').text('Loading..');

                var ajaxCall = makeAjaxCall('POST', 'purchases', jsonData);
                ajaxCall.done(function (data) {
                    //console.log(data);
                    if (data) {
                        if (data[0].Result) {
                            if (data[0].Result == 'OK') {
                                $('#msg').text('');
                                request.success(data[0].Data); // bind data to grid
                            } else if (data[0].Result == 'ERROR') {
                                $('#msg').text(data[0].ErrMsg);
                                request.error(); //*** notify kendo datasource that the request has failed
                            }
                        } else {
                            $('#msg').text('INVALID JSON RETURNED - 2');
                        }
                    } else {
                        $('#msg').text('INVALID JSON RETURNED - 1');
                    }
                });
                ajaxCall.error(function (data) {
                    request.error(data);
                });

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
        toolbar: kendo.template($('#toolbar_template').html()),
        columns: [
            {
                field: 'ID', //*** required for "Add new record" to work properly
                filterable: false,
                hidden: false
            },
            {
                field: 'ShopperEmail',
                title: 'Email',
                encoded: false,
                //width: '350px',
                filterable: false
            },
            {
                field: 'ShopperName',
                title: 'Name',
                //width: '200px',
                filterable: false
            },
            {
                field: 'ShopperGender',
                title: 'Gender',
                encoded: false,
                //width: '200px',
                filterable: false,
                attributes: { style: 'text-align: center;' }
                //*** column header title text is centered using css in html file
            },
            {
                field: 'TotalPurchase',
                title: 'Total Purchase',
                //width: '100px',
                filterable: false,
                attributes: { style: 'text-align: right;' }
                //*** column header title text is centered using css in html file
            },
            {
                field: 'RegDate',
                title: 'Registered On',
                //width: '100px',
                filterable: false,
                attributes: { style: 'text-align: center;' }
                //*** column header title text is centered using css in html file
            }
        ]
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
                        field: 'ShopperName',
                        operator: 'contains',
                        value: searchText
                    },
                    {
                        field: 'ShopperEmail',
                        operator: 'contains',
                        value: searchText
                    }
                ]
            });

        }

    });
    
}