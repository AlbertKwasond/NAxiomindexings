$(function () {
    $("#inputSearch").autocomplete({
        source: function (request, response) {
            var category = $("#category").val();
            if (category === "Journals" || category === "Author") {
                $.ajax({
                    url: "/Home/GetAutoCompleteData",
                    type: "GET",
                    dataType: "json",
                    data: {
                        term: request.term,
                        category: category
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            } else {
                response([]); // Handle the case where the category is not recognized
            }
        },
        minLength: 1 // Minimum characters before triggering autocomplete
    });
});