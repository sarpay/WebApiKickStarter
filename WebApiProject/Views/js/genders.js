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

            read: function (grid) {

                $('#msg').text('');
                $('.spinner').show();

                var xhrPromise = jqXHR('GET', 'genders', 'application/json; charset=utf-8', null);
                xhrPromise /* promise callbacks are executed in order */
                .always(function () {
                    $('.spinner').hide();
                    grid.success([]); //** stops the loading indicator regardless success/fail
                })
                .done(function (response) {
                    //console.log(response);
                    if (response) {
                        grid.success(response); // bind data to grid
                        if (!response[0]) {
                            $('#msg').text('SERVER RETURNED EMPTY JSON');
                        } else {
                            $.each(response[0], function (key, item) {
                                $('<li>', { text: item }).appendTo($('ul#genders'));
                            });
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
                    //{
                    //    field: 'IX',
                    //    operator: 'contains',
                    //    value: searchText
                    //},
                    {
                        field: 'Text',
                        operator: 'startswith',
                        value: searchText
                    }
                ]
            });

        }

    });
    
}