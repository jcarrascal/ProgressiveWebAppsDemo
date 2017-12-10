(function ($) {

    $.fn.expenses = function (cfg) {
        let self = this;
        self._processResponse = function (json) {
            cfg.amount.val("0");
            cfg.reason.val("");
            cfg.balance.text(json.balance);
            let tbody = $("tbody", cfg.movements);
            let row = $("<tr/>");
            row.append($("<td/>").text(json.movement.amount));
            row.append($("<td/>").text(new Date(Date.parse(json.movement.createdOn)).toLocaleString().replace(",", "")));
            row.append($("<td/>").text(json.movement.category.name));
            row.append($("<td/>").text(json.movement.reason));
            tbody.prepend(row);
            if (tbody.children().length > 10) {
                tbody.children(":last").remove();
            }
        };

        self._formSubmit = function (e) {
            e.preventDefault();
            let formData = new FormData(this);
            let button = e.originalEvent.explicitOriginalTarget;
            formData.append(button.name, button.value);
            $.ajax({
                url: cfg.url,
                data: formData,
                type: "POST",
                processData: false,
                contentType: false,
                success: self._processResponse
            });

        };
        this.on("submit", self._formSubmit);

        return this;
    };

})(jQuery);
