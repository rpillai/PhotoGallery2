$(function () {
    $('#btnSubmit').click(function (e) {
        e.preventDefault();
        $.ajax({
            type: 'POST',
            data: {
                'PhotoID': $('#PhotoID').val(),
                'Comment': $('#Description').val(),
                '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            url: '@Url.Action("UpdateComment", "Comment")',
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
                '/Comment/ListComments',
                              { 'PhotoID': photoId }, function () {
                              });
    }
});