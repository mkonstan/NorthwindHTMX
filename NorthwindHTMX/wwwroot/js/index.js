"use strict";

/* HTMX test */
(() => {
  (function (el) {
    el.addEventListener("htmx:configRequest", function (_ref) {
      let {
        detail,
        target
      } = _ref;
      console.log("htmx:configRequest", {
        detail,
        target
      });
      const {
        parameters,
        headers
      } = detail;
      let value = target.value.trim();
      if (value !== "") {
        value = "%".concat(value, "%");
      }
      headers["x-htmx-template"] = "product-lookup";
      parameters["fields"] = ["ProductID", "ProductName", "UnitPrice"];
      parameters["limit"] = 100;
      parameters["orderby"] = [{
        "field": "ProductName",
        "dir": "asc"
      }];
      parameters["where"] = {
        "operator": "or",
        expressions: [{
          "field": "ProductName",
          "operator": "like",
          "value": value
        }]
      };
    });
  })(document.getElementById("product-lookup"));
  (function (el) {
    el.addEventListener("htmx:beforeSwap", function (evt) {
      console.log("htmx:beforeSwap", evt);
    });
    el.addEventListener("htmx:afterSwap", function (evt) {
      console.log("htmx:afterSwap", evt);
    });
  })(document.getElementById("product-lookup-results"));
})();
