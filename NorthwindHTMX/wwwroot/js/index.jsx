/* HTMX test */
(() => {
    (function (productLookup, productList) {
        productLookup.addEventListener("htmx:configRequest", function ({ detail, target }) {
            console.log("htmx:configRequest", { detail, target });
            const { parameters, headers } = detail;
            let value = target.value.trim();
            if (value !== "") {
                value = `%${value}%`;
            }
            headers["x-htmx-template"] = "product-lookup";
            parameters["fields"] = ["ProductID", "ProductName", "UnitPrice"];
            parameters["limit"] = 100;
            parameters["orderby"] = [{ "field": "ProductName", "dir": "asc" }];
            parameters["where"] = { "operator": "or", expressions: [{ "field": "ProductName", "operator": "like", "value": value }] };
        });
        productList.addEventListener("htmx:load", function (evt) {
            console.log("htmx:load", evt);
        });

        productList.addEventListener("htmx:beforeSwap", function ({ target }) {
            console.log("htmx:beforeSwap");
            htmx.findAll(target, ":scope > li").forEach(el => {
                htmx.off(el, "click");
            });
        });
        productList.addEventListener("htmx:afterSwap", function ({ target }) {
            console.log("htmx:afterSwap");
            htmx.findAll(target, ":scope > li").forEach(el => {
                htmx.on(el, "click", function ({ target }) {
                    const value = target.getAttribute("data-name");
                    productLookup.value = value;
                });
            });
        });
        productList.addEventListener("htmx:beforeSettle", function (evt) {
            console.log("htmx:beforeSettle", evt);
        });
        productList.addEventListener("htmx:afterSettle", function (evt) {
            console.log("htmx:afterSettle", evt);
        });
    })(document.getElementById("product-lookup"), document.getElementById("product-lookup-results"));
})();
