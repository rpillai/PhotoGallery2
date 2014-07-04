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

    $(this).on('change', '#chkAll', function() {
        var checkedVal = this.checked;

        $("input[id='chkDeleteID'][type='checkbox']").each(function (index, context) {
            this.checked = checkedVal;
        });
    });

    $(this).on('click', '.btn.btn-link', function() {
        $.ajax({
            url: 'GetNextResult',
            data: {
                currentPageNumber: $(this).text().trim()
            },
            error: function (error) {

            },
            success: function (result) {
                $('#EntityList').html(result);
            }
        });
    });

    function GetSelectedID(checkboxName) {
        var selected = [];
        $("input[id='" + checkboxName + "'][type='checkbox']:checked").each(function () {
            selected.push($(this).val());
        });
        return selected;
    }

    function callDeleteAjax(selected) {
        $.ajax({
            type: 'POST',
            data: JSON.stringify(selected),
            url: $('#DeleteUrl').val(),
            contentType: 'application/json',
            success: function (result) {
                //code to reload the lists again.
                $('#EntityList').load($('#ReloadUrl'));
            }
        });
    }
})();


