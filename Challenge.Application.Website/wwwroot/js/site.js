function showAlert(msg, typeAlert, clickToHide, offsetx, offsety) {
    hideLoading();
    $.notify({
        message: msg
    }, {
        type: typeAlert === undefined ? "danger" : typeAlert,
        z_index: 100000,
        showProgressbar: false,
        mouse_over: "pause",
        offset: {
            x: offsetx === undefined ? 20 : offsetx,
            y: offsety === undefined ? 80 : offsety
        },
        delay: clickToHide ? 0 : 5000,
        animate: {
            enter: "animated fadeInDown",
            exit: "animated fadeOutUp"
        }
    });
}

function hideModalPartial() {
    setTimeout(function () { $("#modal-partial").modal("hide"); }, 300);
}

function hideModalPartialXl() {
    setTimeout(function () { $("#modal-partial-xl").modal("hide"); }, 300);
}

function reload() {
    window.location.reload();
}

function showLoading(msg) {
    return new Promise((resolve, reject) => {
        var message = msg ? msg : "Cargando...";
        $("#modal-loading-msg").html(message);
        $("#modal-loading").modal("show");
        $("#modal-loading-back").show();

        setTimeout(function () {
            resolve();
        }, 650);
    });
}

function hideLoading() {
    $("#modal-loading").modal("hide");
    $("#modal-loading-back").hide();
}

function ShowModalQuestion(title, message, functionYes, functionNo, onlyYesButton, yesButtonText) {
    $("#titleModalQuestion").html(title);
    $("#messageModalQuestion").html(message);

    $("#btnSiModalQuestion").off("click");
    $("#btnNoModalQuestion").off("click");
    $("#btnNoModalQuestion").on("click", function () {
        HideModalQuestion();
    });

    if (typeof (functionYes) !== "undefined") {
        $("#btnSiModalQuestion").off("click");
        $("#btnSiModalQuestion").on("click", functionYes);
    }

    if (typeof (functionNo) !== "undefined") {
        $("#btnNoModalQuestion").off("click");
        $("#btnNoModalQuestion").on("click", functionNo);
    }

    if (yesButtonText !== undefined) {
        $("#btnSiModalQuestion").html(yesButtonText);
    }

    if (onlyYesButton) {
        $("#btnNoModalQuestion").hide();
    }

    $("#backModalQuestion").show();
    $("#modalQuestion").modal("show");
}

function HideModalQuestion() {
    return new Promise((resolve, reject) => {
        $("#backModalQuestion").hide();
        $("#modalQuestion").modal("hide");
        setTimeout(function () {
            resolve();
        },
            650);
    });
}

function CreateAllTooltips() {
    tippy('[data-tippy-content]', {
        arrow: true,
        arrowType: 'round',
        animation: 'scale'
    });
}

function ExecuteAjax(url, type, values, funcionSuccess, parameter) {
    showLoading();
    $.ajaxSetup({ cache: false });

    $.ajax({
        ContentType: "application/json",
        url: url,
        type: type,
        data: values,
        success: function (data) {
            if (funcionSuccess.indexOf(".") >= 0) {
                executeFunction(funcionSuccess, window, data, parameter)
            }
            else {
                window[funcionSuccess](data, parameter);
            }            
            hideLoading();
        },
        error: function (jqXHR, exception) {            
            showAlert("Unexpected error", "warning");
            hideLoading();
        }
    });
}

function showModalResult(data = '', large = false) {
    if (typeof data !== "undefined")
        if (data.message === "Access denied") {
            window.location.href = data.url;
            return;
        }

    var options = { "backdrop": "static", keyboard: true };
    $("#modal-partial").modal(options);
    showModalPartial(large);        
    setNumericInput();    
}

function showModalPartial(large = false) {

    if (large) {
        $("#modal-partial").children().removeClass("modal-lg");
        $("#modal-partial").children().addClass("modal-xl");
    }
    else {
        $("#modal-partial").children().removeClass("modal-xl");
        $("#modal-partial").children().addClass("modal-lg");
    }

    $("#modal-partial").off("shown.bs.modal");
    $("#modal-partial").on("shown.bs.modal", function () {
        $("body").removeClass("modal-open").addClass("modal-open");
    });

    $("#modal-partial").off("hidden.bs.modal");
    $("#modal-partial").on("hidden.bs.modal", function () {
        $("body").removeClass("modal-open");
    });

    $("#modal-partial").modal("show");    

    CreateAllTooltips();
}

function showModalResultXl(data) {
    if (typeof data !== "undefined")
        if (data.message === "Access denied") {
            window.location.href = data.url;
            return;
        }

    var options = { "backdrop": "static", keyboard: true };
    $("#modal-partial-xl").modal(options);
    showModalPartialXl();        
    setNumericInput();    
}

function showModalPartialXl() {

    $("#modal-partial-xl").off("shown.bs.modal");
    $("#modal-partial-xl").on("shown.bs.modal", function () {
        $("body").removeClass("modal-open").addClass("modal-open");
    });

    $("#modal-partial-xl").off("hidden.bs.modal");
    $("#modal-partial-xl").on("hidden.bs.modal", function () {
        $("body").removeClass("modal-open");
    });

    $("#modal-partial-xl").modal("show");
        
    CreateAllTooltips();
}

function getObject(formId) {
    return $("#" + formId).serializeArray();
}

function formValidation(formId, showmessage = true) {
    removeTooltip()

    var validations = [];
    validations.push(requiredFieldValidation(formId));
    validations.push(regularExpValidation(formId));    
    validations.push(zeroValueValidation(formId));
    validations.push(dateRangeValidation(formId));

    showTooltip();
    if ($.inArray(false, validations) !== -1 && showmessage) {
        showAlert("Hay inconsistencias en el formulario, revise los campos demarcados con color rojo.", "warning")        
    }

    return $.inArray(false, validations) === -1;
}

function showTooltip() {
    $(".errorValidate").mouseover(function () {
        if ($(this).attr("data-errormessage") !== undefined) {
            if ($(this).attr("data-errormessage").length > 0) {
                $(this).parent().append("<div class='tooltipError'>" + $(this).attr("data-errormessage") + "</div>");
                if ($(this).parent().prop("tagName") == "TD") {
                    $('.tooltipError').css('right', 'auto');
                    $('.tooltipError').css('top', 'auto');
                }
            }
        }
    });
    $(".errorValidate").mouseout(function () {
        $(this).parent().find(".tooltipError").remove();
    });
}
function removeTooltip() {
    $(".errorValidate").off("mouseover");
    $(".errorValidate").off("mouseout");
}

function requiredFieldValidation(formId) {
    var requested = $("#" + formId).find(".required");
    var ok = true;    
    $("#" + formId + " .errorValidate").removeClass("errorValidate");
    $.each(requested, function (index, value) {
        var group = $(this)[0].tagName;
        var tipo = $(this)[0].type;
        switch (group) {
            case "INPUT":
                switch (tipo) {
                    case "text":
                        if ($(this).val() === "") {
                            $(this).attr("data-errormessage", "Este campo es obligatorio");
                            $(this).addClass("errorValidate");
                            ok = false;
                        }
                        break;
                    case "number":
                        if ($(this).val() === "") {
                            $(this).attr("data-errormessage", "Este campo es obligatorio");
                            $(this).addClass("errorValidate");
                            ok = false;
                        }
                        break;
                }
                break;
            case "TEXTAREA":
                if ($(this).val() === "") {
                    $(this).attr("data-errormessage", "Este campo es obligatorio");
                    $(this).addClass("errorValidate");
                    ok = false;
                }
                break;
        }
    });
    return ok;
}
function regularExpValidation(formId) {    
    var _listEmail = $("#" + formId).find(".email");
    var ok = true;

    $.each(_listEmail, function (i, item) {

        $(this).removeClass("errorValidate");
        var group = $(this)[0].tagName;
        var type = $(this)[0].type;

        switch (group) {
            case "INPUT":
                switch (type) {
                    case "text":
                        if ($(this).val().length > 0) {                            
                            if (!validateEmail($(this).val())) {
                                $(this).attr("data-errormessage", "El Email es invalido");
                                $(this).addClass("errorValidate");
                                ok = false;
                            }
                        } else {
                            if (this.classList.contains("required")) {

                                $(this).attr("data-errormessage", "Este campo es obligatorio");
                                $(this).addClass("errorValidate");
                                ok = false;
                            }
                        }
                        break;
                }
                break;
        }
    });    
    return ok;
}

function setNumericInput() {    
    $(".inputPositiveInput2d").numeric({ negative: false, decimalPlaces: 2, altDecimal: '.', decimal: ',' });    
}

function replaceAll(text, findValue, replaceText) {
    return text.replace(new RegExp(regularExpReplace(findValue), 'g'), replaceText);
}
function regularExpReplace(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function executeFunction(functionName, context /*, args */) {

    try {
        var args = Array.prototype.slice.call(arguments, 2);
        var namespaces = functionName.split(".");
        var func = namespaces.pop();
        for (var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }
        return context[func].apply(context, args);
    } catch {
        hideLoading();
        showAlert("No fue posible ejecutar la función " + functionName, 'danger');        
    }
}

function validateEmail  (email) {
    expr = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return expr.test(email);
}
function zeroValueValidation(formId) {
    var ok = true;
    var rangos = $("#" + formId).find(".zerovalue");
    $.each(rangos, function (index, value) {
        var valueToValidate = $(value).val().replace(",", "");
        valueToValidate = valueToValidate.replace(".", "");
        if (parseFloat(valueToValidate) <= 0) {
            $(this).attr("data-errormessage", $(this).attr("data-message"));
            $(this).addClass("errorValidate");
            ok = false;
        }
    });
    return ok;
}
function dateRangeValidation(formId) {
    var ok = true;
    var ranges = $("#" + formId).find(".daterange");
    $.each(ranges, function (index, value) {
        if ($(value).data("datevalue") === "minDate") {

            var beginDate = $(value).val();
            var endDate = $("#" + $(value).data("compare")).val();

            var beginDateVal = new Date(beginDate.replace(/(\d+[/])(\d+[/])/, '$2$1'));
            var endDateVal = new Date(endDate.replace(/(\d+[/])(\d+[/])/, '$2$1'));

            if ((beginDate != '' && endDate != '') && beginDateVal > endDateVal) {
                $(this).attr("data-errormessage", $(this).attr("data-message"));
                $(this).addClass("errorValidate");
                ok = false;
            }            
        }
        else if ($(value).data("datevalue") === "maxDate") {
            var beginDate = $("#" + $(value).data("compare")).val();
            var endDate = $(value).val(); 

            var beginDateVal = new Date(beginDate.replace(/(\d+[/])(\d+[/])/, '$2$1'));
            var endDateVal = new Date(endDate.replace(/(\d+[/])(\d+[/])/, '$2$1'));

            if ((beginDate != '' && endDate != '') && endDateVal < beginDateVal ) {
                $(this).attr("data-errormessage", $(this).attr("data-message"));
                $(this).addClass("errorValidate");
                ok = false;
            } 
        }
    });
    return ok;
}
function escapeRegExp(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}