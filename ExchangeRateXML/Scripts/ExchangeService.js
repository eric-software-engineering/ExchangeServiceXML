// The front end scripting is very straightforward, no framework other than jQuery is really needed
// To see Angular examples, please refer to https://github.com/ericspolidor/iasset-technical-test
var calculate = function () {
  var defaulttext = "Please make a valid selection. Two decimal places max for the amount.";
  var from = $("input[name='from-group']:checked").val();
  var to = $("input[name='to-group']:checked").val();
  var amount = $("#amount").val();
  var regexp = /^\d+(\.\d{1,2})?$/;

  if (from === undefined || to === undefined || !regexp.test(amount)) {
    $("#result").text(defaulttext);
    return;
  }

  $.ajax({
    url: window.location.href + "api/" + from + "/" + to + "/" + amount + "/",
    dataType: 'json',
    success: function (data) {
      if (data.Status === 1)
        $("#result").text(data.Value);
      else
        $("#result").text(defaulttext);
    },
    error: function (data) {
      $("#result").text(defaulttext);
    }
  });
};

$("#amount").keyup(calculate);
$("input[type='radio']").change(calculate);