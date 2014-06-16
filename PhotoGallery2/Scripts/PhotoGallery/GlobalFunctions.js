$(function () {
    $('#btnDelete').click(function (e) {
        e.preventDefault();
        var selected = new Array();
        $("input[id='chkDeleteID'][type='checkbox']:checked").each(function () {
            selected.push($(this).val());
        });

        if (selected.length) {
            callDeleteAjax(selected);
        }
    });

    $('#chkAll').change(function () {
        var checkedVal = this.checked;

        $("input[id='chkDeleteID'][type='checkbox']").each(function (index, context) {
            this.checked = checkedVal;
        });
    });
});

function callDeleteAjax(selected) {
    $.ajax({
        type: 'POST',
        data: JSON.stringify(selected),
        url: $('#DeleteUrl').val(),
        contentType: 'application/json',
        success: function (result) {
            //code to reload the lists again.
            $('#EntityList').load({
                 
            })
        }
    });
}
