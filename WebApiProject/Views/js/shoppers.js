$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        //var jsonObj = convertQueryStringToJSON(queryString);
        //console.log(jsonObj);
        gridInit(queryString);

    });

});


function gridInit(queryString) {

    //-------------------------
    //*** Grid Binding & Events
    //-------------------------

    var gridDatasource = new kendo.data.DataSource({
        transport: {

            read: function (grid) {

                var xhrPromise = jqXHR('GET-QS', 'shoppers', 'application/json; charset=utf-8', queryString);
                xhrPromise /* promise callbacks are executed in order */
                .always(function (response) {
                    grid.success([]); //** stops the loading indicator regardless success/fail
                })
                .done(function (response) {
                    if (response) {
                        grid.success(response); // bind data to grid
                        if (!response[0]) {
                            //console.log('SERVER RETURNED EMPTY JSON');
                            toastMsg('Message', 'SERVER RETURNED EMPTY JSON', 'warning', 'small');
                        }
                    } else {
                        toastMsg('Message', 'SERVER RETURNED EMPTY JSON', 'warning', 'small');
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