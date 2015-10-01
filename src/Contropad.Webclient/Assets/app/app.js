(function ($) {

    var isFullscreen = false;

    $(function () {

        var connection = new WebSocket('ws://' + window.location.hostname + ':8181');

        connection.onopen = function () {

        };

        // Log errors
        connection.onerror = function (error) {

        };

        // Log messages from the server
        connection.onmessage = function (e) {

        };

        var joystick = new VirtualJoystick({
            container: document.getElementById('container'),
            mouseSupport: true
        });

        var lastState = {
            right: false,
            up: false,
            left: false,
            down: false
        };

        var updateState = function () {
            var changed = false,
                newDirections = {
                    right: joystick.right(),
                    up: joystick.up(),
                    left: joystick.left(),
                    down: joystick.down()
                };

            for (var prop in lastState) {
                if (lastState.hasOwnProperty(prop)) {
                    if (lastState[prop] !== newDirections[prop]) {
                        console.log('changed', prop);
                        changed = true;
                        break;
                    }
                }
            }


            if (changed) {
                lastState = newDirections;
                connection.send(JSON.stringify({
                    type: 'direction',
                    id: controllerId,
                    directions: newDirections
                }));
            }
        };

        $('[data-gamepad]')
            .on('touchstart', function (e) {
                if (!isFullscreen) {
                    launchIntoFullscreen(document.documentElement);
                    isFullscreen = true;
                }

                e.preventDefault();
                e.stopPropagation();

                connection.send(JSON.stringify({
                    button: $(this).data('gamepad'),
                    type: 'button',
                    id: controllerId,
                    pressed: true
                }));
            })
            .on('touchend', function (e) {
                e.preventDefault();
                e.stopPropagation();
                connection.send(JSON.stringify({
                    button: $(this).data('gamepad'),
                    id: controllerId,
                    type: 'button',
                    pressed: false
                }));
            });

        setInterval(function () {
            updateState();
        }, 10);
    });
}(jQuery));