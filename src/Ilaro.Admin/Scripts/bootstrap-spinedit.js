/**
* Release:
*  2014-01-16: Version 1.0.0
*  
*
* https://github.com/scyv/bootstrap-spinedit
*
*
* Author: 
*   - geersch@gmail.com
*
* Contributors:
*   - ivoras@gmail.com
*   - yves.schubert@ksitec.de
*/

jQuery.fn.mousehold = function (f) {
    var timeout = 100;
    if (f && typeof f == 'function') {
        var intervalId = 0;
        var firstStep = false;
        var clearMousehold = undefined;
        return this.each(function () {
            $(this).mousedown(function () {
                firstStep = true;
                var ctr = 0;
                var t = this;
                intervalId = setInterval(function () {
                    ctr++;
                    f.call(t, ctr);
                    firstStep = false;
                }, timeout);
            });

            clearMousehold = function () {
                clearInterval(intervalId);
                if (firstStep) f.call(this, 1);
                firstStep = false;
            };

            $(this).mouseout(clearMousehold);
            $(this).mouseup(clearMousehold);
        });
    }
};

!function ($) {

    var SpinEdit = function (element, options) {
        this.element = $(element);
        this.element.addClass("noSelect");

        this.intervalId = undefined;

        this.dataToOptions(options);

        var hasOptions = typeof options == 'object';

        this.minimum = $.fn.spinedit.defaults.minimum;
        if (hasOptions && typeof options.minimum == 'number') {
            this.setMinimum(options.minimum);
        }

        this.maximum = $.fn.spinedit.defaults.maximum;
        if (hasOptions && typeof options.maximum == 'number') {
            this.setMaximum(options.maximum);
        }

        this.numberOfDecimals = $.fn.spinedit.defaults.numberOfDecimals;
        if (hasOptions && typeof options.numberOfDecimals == 'number') {
            this.setNumberOfDecimals(options.numberOfDecimals);
        }

        var value = $.fn.spinedit.defaults.value;
        if (this.element.val()) {
            var initialValue = parseFloat(this.element.val());
            if (!isNaN(initialValue)) value = initialValue.toFixed(this.numberOfDecimals);
        }
        else if (hasOptions && (options.value == '' || typeof options.value == 'number')) {
            value = options.value;
        }
        this.setValue(value);

        this.step = $.fn.spinedit.defaults.step;
        if (hasOptions && typeof options.step == 'number') {
            this.setStep(options.step);
        }

        var template = $(DRPGlobal.template);
        if (this.element.next().hasClass('input-group-addon')) {
            this.element.next().after(template);
        } else {
            this.element.after(template);
        }
        $(template).each(function (i, x) {
            $(x).bind('selectstart click mousedown', function () { return false; });
        });

        template.find('.glyphicon-chevron-up').mousehold($.proxy(this.increase, this));
        template.find('.glyphicon-chevron-down').mousehold($.proxy(this.decrease, this));

        this.element.on('keydown', $.proxy(this._keydown, this));
        this.element.on('keypress', $.proxy(this._keypress, this));
        this.element.on('blur', $.proxy(this._checkConstraints, this));
    };

    SpinEdit.prototype = {
        constructor: SpinEdit,

        setMinimum: function (value) {
            this.minimum = parseFloat(value);
        },

        setMaximum: function (value) {
            this.maximum = parseFloat(value);
        },

        setStep: function (value) {
            this.step = parseFloat(value);
        },

        setNumberOfDecimals: function (value) {
            this.numberOfDecimals = parseInt(value);
        },

        setValue: function (value) {
            if (value != '') {
                value = parseFloat(value);
                if (isNaN(value))
                    value = this.minimum;
                if (this.value == value)
                    return;
                if (this.minimum && value < this.minimum)
                    value = this.minimum;
                if (this.maximum && value > this.maximum)
                    value = this.maximum;
                this.value = value;
                this.element.val(this.value.toFixed(this.numberOfDecimals));
                this.element.change();

                this.element.trigger({
                    type: "valueChanged",
                    value: parseFloat(this.value.toFixed(this.numberOfDecimals))
                });
            }
            else {
                this.value = value;
                this.element.val(this.value);
                this.element.change();

                this.element.trigger({
                    type: "valueChanged",
                    value: this.value
                });
            }
        },

        increase: function () {
            // the preceeding set is needed to allow clicking on chevron up
            // while the cursor is within the textfield (the potentionally new 
            // value has not been validated and set at this time yet)
            this.setValue(this.element.val());

            if (!this.value) {
                this.value = 0;
            }

            var newValue = this.value + this.step;
            this.setValue(newValue);
        },

        decrease: function () {
            // the preceeding set is needed to allow clicking on chevron down
            // while the cursor is within the textfield (the potentionally new 
            // value has not been validated and set at this time yet)
            this.setValue(this.element.val());

            if (!this.value) {
                this.value = 0;
            }

            var newValue = this.value - this.step;
            this.setValue(newValue);
        },

        _keydown: function (event) {
            var pressedKey = event.keyCode || event.which;
            if (pressedKey === 38) {
                this.increase();
            }
            else if (pressedKey === 40) {
                this.decrease();
            }
        },

        _keypress: function (event) {

            //var pressedKey = event.keyCode || event.which;
            var pressedKey = event.keyCode || event.charCode;

            // Allow: -
            if (pressedKey == 45) {
                return;
            }
            // Allow decimal separator (.)
            if (this.numberOfDecimals > 0 && pressedKey == 46) {
                return;
            }

            // Ensure that it is a number and stop the keypress if not
            var a = [];
            for (var i = 48; i < 58; i++) {
                a.push(i);
            }

            // for FF: allow left, right and backspace
            a.push(37);
            a.push(39);
            a.push(8);

            if (!(a.indexOf(pressedKey) >= 0)) {
                event.preventDefault();
            }
        },

        _checkConstraints: function (e) {
            var target = $(e.target);
            this.setValue(target.val());
        },

        dataToOptions: function (options) {
            var eData = this.element.data();

            if (eData.numberValue !== undefined) options.value = eData.numberValue;
            if (eData.numberMinimum !== undefined) options.minimum = eData.numberMinimum;
            if (eData.numberMaximum !== undefined) options.maximum = eData.numberMaximum;
            if (eData.numberStep !== undefined) options.step = eData.numberStep;
            if (eData.numberNumberOfDecimals !== undefined) options.numberOfDecimals = eData.numberNumberOfDecimals;
        }
    };

    $.fn.spinedit = function (option) {
        var args = Array.apply(null, arguments);
        args.shift();
        return this.each(function () {
            var $this = $(this),
                data = $this.data('spinedit'),
                options = typeof option == 'object' && option;

            if (!data) {
                $this.data('spinedit', new SpinEdit(this, $.extend({}, $.fn.spinedit().defaults, options)));
            }
            if (typeof option == 'string' && typeof data[option] == 'function') {
                data[option].apply(data, args);
            }
        });
    };

    $.fn.spinedit.defaults = {
        value: 0,
        minimum: null,
        maximum: null,
        step: 1,
        numberOfDecimals: 0
    };

    $.fn.spinedit.Constructor = SpinEdit;

    var DRPGlobal = {};

    DRPGlobal.template =
    '<div class="input-group-addon spinedit">' +
    '<span class="glyphicon glyphicon-chevron-up"></span>' +
    '<span class="glyphicon glyphicon-chevron-down"></span>' +
    '</div>';

}(window.jQuery);