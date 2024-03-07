"use strict";

/* HTMX test */
(() => {
  (function (productLookup, productList) {
    productLookup.addEventListener("htmx:configRequest", function (_ref) {
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
    productList.addEventListener("htmx:load", function (evt) {
      console.log("htmx:load", evt);
    });
    productList.addEventListener("htmx:beforeSwap", function (_ref2) {
      let {
        target
      } = _ref2;
      console.log("htmx:beforeSwap");
      htmx.findAll(target, ":scope > li").forEach(el => {
        htmx.off(el, "click");
      });
    });
    productList.addEventListener("htmx:afterSwap", function (_ref3) {
      let {
        target
      } = _ref3;
      console.log("htmx:afterSwap");
      htmx.findAll(target, ":scope > li").forEach(el => {
        htmx.on(el, "click", function (_ref4) {
          let {
            target
          } = _ref4;
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
