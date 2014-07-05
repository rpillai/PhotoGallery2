$(function () {
    var selected= [];
    $('#btnShowMoveDialog').click(function() {
        selected = GetSelectedID('chkDeleteID');
        if (selected.length > 0) {
            $('#moveModal .modal-title').text('Moving ' + selected.length + ' selected photo(s) to... ');
            $('#moveModal').modal();
        }
        
    });

    $('#btnMove').click(function() {
        var albumId = $('#Albums').val();
        $.ajax({
            url: 'MovePhotos',
            type: 'POST',
            contentType: 'application/json',
            data : JSON.stringify({
                selectedPhotos: selected,
                albumID: albumId
            }),
            success: function (result) {
                if (result) {
                    selected.length = 0;
                    $('#moveModal'), modal('hide');
                }
            },
            error: function(error) {
                console.log(error);
            }
        });
    });
});