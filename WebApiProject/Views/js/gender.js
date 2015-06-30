var GRID;

$(document).ready(function () {

    $('form').submit(function (event) {

        event.preventDefault();

        var queryString = $(this).serialize(); //*** serialize all form values
        var jsonObj = convertQueryStringToJSON(queryString);
        //var jsonData = JSON.stringify(jsonObj);

        //console.log(jsonObj.gender_ix);
        gridInit(jsonObj.gender_ix);

    });

});


function gridInit(gender_ix) {

    //-------------------------
    //*** Grid Binding & Events
    //-------------------------

    var gridDatasource = new kendo.data.DataSource({
        transport: {

            read: function (request) {

                $('#msg').text('Loading..');

                var ajaxCall = makeAjaxCall('GET', 'gender', gender_ix);
                ajaxCall.done(function (data) {
                    if (data) {
                        request.success([data]); // bind data to grid
                        if (!data[0]) {
                            $('#msg').text('SERVER RETURNS ONLY 1 RECORD');
                        }
                    } else {
                        request.success([]);
                        $('#msg').text('SERVER RETURNED EMPTY JSON');
                    }
                });
                ajaxCall.error(function (data) {
                    request.error([data]); //*** notify the kendo datasource that the request failed
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