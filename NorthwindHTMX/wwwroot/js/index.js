"use strict";

/* HTMX test */
(() => {
  (function (lookup, list) {
    var currentFocus = 0;
    htmx.on(lookup, "htmx:configRequest", function (_ref) {
      let {
        detail,
        target
      } = _ref;
      const {
        parameters,
        headers
      } = detail;
      let value = target.value.trim();
      currentFocus = 0;
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
    htmx.on(lookup, "keydown", function (event) {
      /* cleanup before swapping content */
      console.log("keydown");
      //if (event.key === "Escape") {
      //    htmx.addClass(list, "hidden");
      //    return;
      //}
      if (e.keyCode === 40) {
        /*If the arrow DOWN key is pressed,
        increase the currentFocus variable:*/
        currentFocus++;
        /*and and make the current item more visible:*/
        addActive(x);
        return;
      }
      if (e.keyCode === 38) {
        //up
        /*If the arrow UP key is pressed,
        decrease the currentFocus variable:*/
        currentFocus--;
        /*and and make the current item more visible:*/
        addActive(x);
        return;
      }
      if (e.keyCode === 13) {
        /*If the ENTER key is pressed, prevent the form from being submitted,*/
        e.preventDefault();
        if (currentFocus > -1) {
          /*and simulate a click on the "active" item:*/
          if (x) x[currentFocus].click();
        }
        return;
      }
    });
    htmx.on(list, "htmx:load", function (_ref2) {
      let {
        target: list
      } = _ref2;
      console.log("htmx:load");
    });
    htmx.on(list, "htmx:beforeSwap", function (_ref3) {
      let {
        target: list
      } = _ref3;
    } /* cleanup before swapping content */);
    htmx.on(list, "htmx:afterSwap", function (_ref4) {
      let {
        target: list
      } = _ref4;
      /* wireup events and apply attributes and classes as needed */
      htmx.removeClass(list, "hidden");
      htmx.findAll(list, ":scope > li").forEach(item => {
        htmx.on(item, "click", function (_ref5) {
          let {
            target: el
          } = _ref5;
          lookup.value = el.getAttribute("data-name");
          lookup.setAttribute("data-id", el.getAttribute("data-id"));
          htmx.addClass(list, "hidden");
        });
      });
    });
    htmx.on(list, "htmx:beforeSettle", function (_ref6) {
      let {
        target: list
      } = _ref6;
      console.log("htmx:beforeSettle");
    });
    htmx.on(list, "htmx:afterSettle", function (_ref7) {
      let {
        target: list
      } = _ref7;
      console.log("htmx:afterSettle");
    });
    function addActive(list) {
      /*a function to classify an item as "active":*/
      if (!list) return false;
      /*start by removing the "active" class on all items:*/
      removeActive(list);
      if (currentFocus >= list.length) currentFocus = 0;
      if (currentFocus < 0) currentFocus = list.length - 1;
      /*add class "autocomplete-active":*/
      list[currentFocus].classList.add("autocomplete-active");
    }
    function removeActive(list) {
      /*a function to remove the "active" class from all autocomplete items:*/
      list.forEach(item => item.classList.remove("autocomplete-active"));
    }
    function closeAllLists(elmnt) {
      /*close all autocomplete lists in the document,
      except the one passed as an argument:*/
      var x = document.getElementsByClassName("autocomplete-items");
      for (var i = 0; i < x.length; i++) {
        if (elmnt != x[i] && elmnt != inp) {
          x[i].parentNode.removeChild(x[i]);
        }
      }
    }
  })(document.getElementById("product-lookup"), document.getElementById("product-lookup-results"));
})();
