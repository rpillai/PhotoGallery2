$(document).ready(function () {
    document.getElementById('launch').onclick = function (event) {
        event.preventDefault();
        var albumID = $('#AlbumID').val();
        var url = $('#SlideShowUrl').val();

        $.ajax({
            url: url,
            datatype: 'json'
        }).done(function (result) {
            var obj = $.parseJSON(result);
            var options = {
                indicatorContainer: 'ol',
                activeIndicatorContainer: 'active',
                thumbnailIndicators: true
            };
            blueimp.Gallery(obj, options);
        });
    };
});

