function getSearchResults() {
    var str = $('#search').val();
    $.ajax({
        type: "POST",
        url: "Admin.asmx/getSearchResults",
        data: JSON.stringify({ search: str }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#searchResults').html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            // alert("Search didn't work. Please try again and type slower... (and don't use 'a', 'and', and 'the' in the search");
        }
    });
    $.ajax({
        url: 'http://ec2-54-187-135-48.us-west-2.compute.amazonaws.com/info344NBAstats/function.php',
        data: { name: str },
        dataType: 'jsonp',
        //jsonp: 'callback',
        //jsonpCallback: 'jsonpCallback',
        success: function (data) {
            $('#playerStatsLabel').html("[Name, GP, FGP, TTP, FTP, PPG]");
            $('#playerStats').text(data);
        }
    });
};

function getResults() {
    var str = $('#search').val();
    if (str !== "")
    {
        $("#label").html("Showing suggestions for " + str + ":");
        $.ajax({
            type: "POST",
            url: "Admin.asmx/getSuggestions",
            data: JSON.stringify({ search: str }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $('#suggestions').html(JSON.stringify(msg.d));
                getSearchResults();
            },
            error: function (msg) {
                alert("Search didn't work. Please try again.");
            }
        });
        //$.ajax({
        //    url: 'http://ec2-54-187-135-48.us-west-2.compute.amazonaws.com/info344NBAstats/function.php',
        //    data: { name: str },
        //    dataType: 'jsonp',
        //    //jsonp: 'callback',
        //    //jsonpCallback: 'jsonpCallback',
        //    success: function (data) {
        //        $('#playerStatsLabel').html("[Name, GP, FGP, TTP, FTP, PPG]");
        //        $('#playerStats').text(data);
        //    }
        //});
    }
    else {
        $('#label').html("");
        $('#suggestions').html("");
        $('#searchResults').html("");
        $('#playerStats').html("");
        $('#playerStatsLabel').html("");
        str = "";
    }
};

//function jsonpCallback(data) {
//    $('#playerStatsLabel').html("[Name, GP, FGP, TTP, FTP, PPG]");
//    $('#playerStats').text(data);
//};