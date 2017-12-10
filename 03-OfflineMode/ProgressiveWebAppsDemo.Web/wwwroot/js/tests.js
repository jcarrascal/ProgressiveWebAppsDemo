QUnit.test( "hello test", function( assert ) {
    let cfg = { 
        amount: $("<input/>").text("999"),
        reason: $("<input/>").text("should be clear"),
        balance: $("<span/>").text("999"),
        movements: $("<table><tbody/></table>")
    };
    var expenses = $("<form/>").expenses(cfg);

    expenses._processResponse({
        balance: 500,
        movement: {
            category: { name: "Default" },
            amount: 500,
            createdOn: "2017-12-10T10:00:00",
            reason: "test reason"
        }
    });

    assert.equal(cfg.amount.val(), "0");
    assert.equal(cfg.reason.val(), "");
    assert.equal(cfg.balance.text(), "500");
    assert.equal($("tr", cfg.movements).length, 1, cfg.movements);
});
