/* HTMX test */
(() => {
    (function (el) {
        el.addEventListener("htmx:configRequest", function ({ detail, target }) {
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
    })(document.getElementById("product-lookup"));

    (function (el) {
        el.addEventListener("htmx:beforeSwap", function (evt) {
            console.log("htmx:beforeSwap", evt);
        });
        el.addEventListener("htmx:afterSwap", function (evt) {
            console.log("htmx:afterSwap", evt);
        });
        el.addEventListener("htmx:beforeSettle", function (evt) {
            console.log("htmx:beforeSettle", evt);
        });
        el.addEventListener("htmx:afterSettle", function (evt) {
            console.log("htmx:afterSettle", evt);
        });
    })(document.getElementById("product-lookup-results"))
})();
