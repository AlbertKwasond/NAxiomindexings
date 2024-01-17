// pagination.js

$(document).ready(function () {
    var totalPages = parseInt($('#pagination').data('total-pages')); // Total number of pages from your ViewModel
    var currentPage = parseInt($('#pagination').data('current-page')); // Current page from your ViewModel

    $('#pagination').twbsPagination({
        totalPages: totalPages,
        startPage: currentPage,
        visiblePages: 3,
        onPageClick: function (event, page) {
            loadJournalData(page);
        }
    });

    function loadJournalData(page) {
        $.ajax({
            url: '/Home/FindJournalsAsync', // Replace 'YourController' with the actual controller name
            type: 'GET',
            data: { page: page },
            success: function (data) {
                $('#journalList').html(data);
            },
            error: function () {
                console.log('Error loading data.');
            }
        });
    }
});