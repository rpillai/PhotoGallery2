$(function () {
    $('#btnSubmit').click(function (e) {
        e.preventDefault();
        if ($('#updateCommentForm').valid() == false) return;

        $.ajax({
            type: 'POST',
            data: {
                PhotoID: $('#PhotoID').val(),
                Description: $('#Description').val()
            },
            url: $('#updateCommentForm').attr('action'),
            success: function (result) {
                $('#Description').val('');
                loadCommentData($('#PhotoID').val());
            },
            error: function (error) {

            }
        });
    });

    $('#prevButton').click(function (e) {
        e.preventDefault();
        getPhotoData('P');
    });

    $('#nextButton').click(function (e) {
        e.preventDefault();
        getPhotoData('N');
    });

    function getPhotoData(next) {
        $.getJSON('/Photo/GetNext', {
            'PhotoID': $('#PhotoID').val(),
            'AlbumID': $('#AlbumID').val(),
            'Flag': next == 'N' ? 'N' : 'P'
        }).done(function (result) {
            $('#detailsView img').attr('src', result['PhotoPath']);
            $('#detailsView h4').text(result['Title']);
            $('#detailsView p').text(result['Description']);
            $('#PhotoID').val(result['PhotoID']);
            loadCommentData(result['PhotoID']);
        });
    };

    function loadCommentData(photoId) {
        $('#commentsList').load(
                $('#CommentListUrl').val(),
                              { 'PhotoID': photoId }, function () {
                              });
    }
});