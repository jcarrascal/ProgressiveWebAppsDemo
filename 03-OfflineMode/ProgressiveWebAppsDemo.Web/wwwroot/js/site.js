(function ($) {

    $.fn.expenses = function (cfg) {
        let self = this;
        self._processResponse = function (json) {
            cfg.amount.val("0");
            cfg.reason.val("");
            cfg.balance.text(json.balance);
            let tbody = $("tbody", cfg.movements);
            let row = $("<tr/>");
            row.append($("<td/>").addClass("text-right").text(json.movement.amount));
            row.append($("<td/>").text(new Date(Date.parse(json.movement.createdOn)).toLocaleString().replace(",", "")));
            row.append($("<td/>").text(json.movement.category.name));
            row.append($("<td/>").text(json.movement.reason));
            tbody.prepend(row);
            if (tbody.children().length > 10) {
                tbody.children(":last").remove();
            }
        };

        self._pending = [];
        self._processPending = function () {
            let formData = self._pending[0];
            $.ajax({
                url: cfg.url,
                data: formData,
                type: "POST",
                processData: false,
                contentType: false,
                error: function () {
                    errorReceived = true;
                },
                success: function (data) {
                    console.log("Movement " + data.movement.amount + " stored on server.")
                    self._pending.shift();
                    cfg.balance.text(data.balance);
                    if (self._pending.length > 0) {
                        self._processPending();
                    }
                }
            });
        }
        self._simulateResponse = function (formData) {
            let createdOn = new Date().toISOString();
            formData.append("createdOn", createdOn);
            self._pending.push(formData);

            let balance = parseFloat(cfg.balance.text());
            let amount = parseFloat(formData.get("amount")) *
                (formData.get("type") == "expense" ? -1 : 1);
            balance += amount;
            let movement = {
                amount: amount,
                createdOn: createdOn,
                reason: formData.get("reason"),
                hasPicture: formData.get("picture") != null,
                category: {
                    id: formData.get("categoryId"),
                    name: $("option:selected", cfg.category).text()
                }
            };
            let json = { balance: balance, movement: movement };
            self._processResponse(json);
            console.log("Movement " + movement.amount + " stored on client for later.")
        }

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
                error: function () {
                    self._simulateResponse(formData);
                },
                success: function (data) {
                    self._processResponse(data);
                    self._processPending();
                }
            });

        };
        this.on("submit", self._formSubmit);

        return this;
    };

    if ('serviceWorker' in navigator) {
        window.addEventListener('load', function () {
            navigator.serviceWorker.register('/sw.js').then(function (registration) {
                console.log('ServiceWorker registration successful with scope: ', registration.scope);
            }, function (err) {
                console.log('ServiceWorker registration failed: ', err);
            });
        });
    }

})(jQuery);
