function Joystick(options) {

    var defaults = {
        onStateChange: $.noop,
        onButtonChange: $.noop,
        container: 'body'
    };

    var opts = $.extend(defaults, options);

    var me = this;

    this.lastState = {
        right: false,
        up: false,
        left: false,
        down: false
    };

    this.onGamepadTouchStart = function(e) {
        e.preventDefault();
        e.stopPropagation();

        console.log('start', $(this).data('gamepad'));

        opts.onButtonChange({
            button: $(this).data('gamepad'),
            pressed: true
        });
    };

    this.onGamepadTouchEnd = function (e) {
        e.preventDefault();
        e.stopPropagation();

        console.log('end', $(this).data('gamepad'));

        opts.onButtonChange({
            button: $(this).data('gamepad'),
            pressed: false
        });
    };

    this.start = function () {
        this.containerEl = document.getElementById(opts.container);
        this.vjoystick = new VirtualJoystick({
            container: this.containerEl,
            mouseSupport: true
        });

        $(this.containerEl).find('[data-gamepad]')
            .on('touchstart mousedown', this.onGamepadTouchStart)
            .on('touchend mouseup', this.onGamepadTouchEnd);

        setInterval(function() {
            me.updateState();
        }, 10);
    };

    this.updateState = function () {
        var changed = false,
            newDirections = {
                right: this.vjoystick.right(),
                up: this.vjoystick.up(),
                left: this.vjoystick.left(),
                down: this.vjoystick.down()
            };

        for (var prop in this.lastState) {
            if (this.lastState.hasOwnProperty(prop)) {
                if (this.lastState[prop] !== newDirections[prop]) {
                    changed = true;
                    break;
                }
            }
        }

        if (changed) {
            this.lastState = newDirections;
            opts.onStateChange(newDirections);
        }
    };
}

(function ($) {

    var isFullscreen = false;

    $(function () {

        var connection = new WebSocket('ws://' + window.location.hostname + ':8181');

        var joystick = new Joystick({
            container: 'container',
            onStateChange: function(newState) {
                connection.send(JSON.stringify({
                    type: 'direction',
                    id: controllerId,
                    directions: newState
                }));
            },
            onButtonChange: function (ev) {
                connection.send(JSON.stringify({
                    button: ev.button,
                    id: controllerId,
                    type: 'button',
                    pressed: ev.pressed
                }));
            }
        });
        joystick.start();

        $('#container')
            .on('touchstart', function() {
                if (!isFullscreen) {
                    launchIntoFullscreen(document.documentElement);
                    isFullscreen = true;
                }
            });
    });
}(jQuery));
