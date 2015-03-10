function getStart() {
    alert("Are you sure? All existing information will be cleared and will have to be crawled again. If you want to continue, the crawler should be stopped first.");
    $.ajax({
        type: "POST",
        url: "Admin.asmx/Start",
        success: function (msg) {
            $("#response").html("The crawler has been started!");
            $('#status').html("Crawling in progress...");
        },
        error: function (msg) {
            alert("Uh oh, the crawler start command didn't go through");
        }
    });
};

function getStop() {
    alert("Are you sure? URL queue will be deleted and will have to be started again.");
    $.ajax({
        type: "POST",
        url: "Admin.asmx/Stop",
        success: function (msg) {
            $("#response").html("The crawler has been stopped!");
            $('#status').html("Idle");
        },
        error: function (msg) {
            alert("Uh oh, the crawler stop command didn't go through");
        }
    });
};

function getDeleteTable() {
    alert("Are you sure? All existing information will be cleared and will have to be crawled again. You may want to start crawling as well.");
    $.ajax({
        type: "POST",
        url: "Admin.asmx/DeleteTable",
        success: function (msg) {
            $("#response").html("The index table has been deleted.  Please wait a bit before crawling again!");
        },
        error: function (msg) {
            alert("Uh oh, the delete table command didn't go through");
        }
    });
};

$(document).ready(function () {
    console.log("ready");
    $.ajax({
        type: "POST",
        url: "Admin.asmx/cpuUsage",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#cpu").html(JSON.stringify(msg.d) + "%");
            $("#errors").html("0");
        },
        error: function (msg) {
            $("#cpu").html("Unavailable");
        }
    });

    $.ajax({
        type: "POST",
        url: "Admin.asmx/cpuUsage",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#ram").html(JSON.stringify(msg.d) + "%");
        },
        error: function (msg) {
            $("#ram").html("Unavailable");
        }
    });

    $.ajax({
        type: "POST",
        url: "Admin.asmx/getTogo",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#togo").html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            $("#togo").html("Unavailable");
        }
    });

    $.ajax({
        type: "POST",
        url: "Admin.asmx/getIndex",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#crawled").html(JSON.stringify(msg.d));
            $("#indexed").html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            $("#crawled").html("Unavailable");
            $("#indexed").html(JSON.stringify(msg.d));
        }
    });

    $.ajax({
        type: "POST",
        url: "Admin.asmx/dictionaryCount",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#numTitles").html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            $("#numTitles").html("Unavailable");
        }
    });

    $.ajax({
        type: "POST",
        url: "Admin.asmx/returnLastTitle",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#lastTitle").html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            $("#lastTitle").html("Unavailable");
        }
    });

    $.ajax({
        type: "POST",
        url: "Admin.asmx/lastCrawled",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#tenCrawled").html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            $("#tenCrawled").html("Unavailable");
        }
    });
});


function getTitle() {
    var str = $('#text').val();
    $.ajax({
        type: "POST",
        url: "Admin.asmx/getTitle",
        data: JSON.stringify({ url: str }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#result").html(JSON.stringify(msg.d));
        },
        error: function (msg) {
            alert("result failed");
        }
    });
};