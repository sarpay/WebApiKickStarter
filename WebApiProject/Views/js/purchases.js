﻿var GRID;

$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        var jsonObj = convertQueryStringToJSON(queryString);
        var jsonString = JSON.stringify(jsonObj);
        //console.log(jsonString);

        gridInit(jsonString, jsonObj);

    });

});


function gridInit(jsonString, jsonObj) {

    //-------------------------
    //*** Grid Binding & Events
    //-------------------------

    var gridDatasource = new kendo.data.DataSource({
        transport: {

            read: function (grid) {

                $('#msg').text('');
                $('.spinner').show();

                //XHR(
                //    'POST', //*** method
                //    'purchases', //*** api route
                //    jsonObj //*** data as JSON Object w/ keys & values
                //)
                //.then(
                //    function (response) {
                //        //console.log(response);
                //        if (response[0].Result == 'OK') {
                //            grid.success(response[0].Data); //*** bind data to grid
                //        } else if (response[0].Result == 'ERROR') {
                //            $('#msg').html('<b>Server Error</b><br/>' + response[0].ErrMsg);
                //            grid.error(); //*** notify kendo datasource that the request has failed
                //        }
                //    },
                //    function (response) {
                //        $('#msg').html('<b>XHR Error</b><br/>' + response.Message + '<br/>' + response.MessageDetail);
                //        grid.error(); //*** notify the kendo datasource that the request failed
                //    }
                //)
                //.then(
                //    function () {
                //        $('.spinner').hide();
                //    }
                //);

                var xhrPromise = jqXHR('POST', 'purchases', 'application/json; charset=utf-8', jsonObj);
                xhrPromise /* promise callbacks are executed in order */
                .always(function (response) {
                    $('.spinner').hide();
                    grid.success([]); //** stops the loading effect on empty grid
                })
                .done(function (response) {
                    //console.log(response);
                    if (response) {
                        if (response[0].Result) {
                            if (response[0].Result == 'OK') {
                                grid.success(response[0].Data); // bind data to grid
                            } else if (response[0].Result == 'ERROR') {
                                $('#msg').html('<b>SERVER ERROR</b><br/>' + response[0].ErrMsg);
                            }
                        } else {
                            $('#msg').text('INVALID JSON RETURNED');
                        }
                    } else {
                        $('#msg').text('NULL JSON RETURNED');
                    }
                });
                //.fail(function (response) {
                //    console.log(response);
                //    /* make further ui changes on xhr fail */
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