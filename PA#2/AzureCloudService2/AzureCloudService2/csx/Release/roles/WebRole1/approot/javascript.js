function getResults() {
    var str = $('#search').val();
    $("#label").html("Showing suggestions for " + str + ":");
    $.ajax({
        type: "POST",
        url: "getQuerySuggestion.asmx/getSuggestions",
        data: JSON.stringify({ search: str }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#suggestions').html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            alert("Search didn't work. Please try again.");
        }
    });
};

// Can be uncommented to build the dictionary when the page is loaded
/*$(function () {
    $.ajax({
        type: "POST",
        url: "getQuerySuggestion.asmx/getWords",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#suggestions").html("success");
        },
        error: function (msg) {
            alert(JSON.stringify(msg));
        }
    });
});*/