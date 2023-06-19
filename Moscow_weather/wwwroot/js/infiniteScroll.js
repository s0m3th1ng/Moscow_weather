function InfiniteScroll(iTable, iAction, iParams) {
    this.table = iTable;
    this.action = iAction;
    this.params = iParams;
    this.loading = false;
    this.AddTableLines = function (firstItem) {
        this.loading = true;
        this.params.firstItem = firstItem;
        $("#loader").css("display", "block");
        $.ajax({
            type: 'POST',
            url: self.action,
            data: self.params,
            dataType: "html"
        })
            .done(function (result) {
                if (result) {
                    $("#" + self.table).append(result);
                    self.loading = false;
                }
            })
            .fail(function (xhr, ajaxOptions, thrownError) {
                console.log("Error in AddTableLines:", thrownError);
            })
            .always(function () {
                $("#loader").css("display", "none");
            });
    }

    var self = this;
    window.onscroll = function (ev) {
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
            if (!self.loading) {
                var itemCount = $('#' + self.table + ' tr').length - 1;
                self.AddTableLines(itemCount);
            }
        }
    };
    this.AddTableLines(0);
}