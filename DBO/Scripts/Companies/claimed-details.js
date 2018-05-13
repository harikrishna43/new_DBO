$("#revision-btn").on("click", function () {
    var companyId = $(this).attr("data-companyId");
    $.ajax({
        url: "/Companies/RegisterClaimedCompany",
        type: "POST",
        data: { id: companyId },
        success: function (data) {
            $("#main-claimed-info").html(data);
        }
    });
});