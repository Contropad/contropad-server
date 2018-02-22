using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using Contropad.Core.Joystick;
using Fleck;
using Newtonsoft.Json.Linq;

namespace Contropad.Core.Streams
{
    public class WebsocketStream : IDisposable
    {
        private readonly string _hostname;
        private WebSocketServer _server;
        private readonly ConcurrentDictionary<int, IObserver<IJoystickUpdate>> _observers;

        private int _counter;

        public WebsocketStream(string hostname)
        {
            _hostname = hostname;
            _observers = new ConcurrentDictionary<int, IObserver<IJoystickUpdate>>();
        }

        public void Start()
        {
            _server = new WebSocketServer(_hostname);
            _server.Start(HandleWebsocket);
        }

        public IObservable<IJoystickUpdate> CreateStream()
        {
            return Observable.Create<IJoystickUpdate>(o =>
            {
                var id = _counter++;
                _observers.AddOrUpdate(id, o, (x, y) => o);
                return () => _observers.TryRemove(id, out o);
            });
        }

        /// <summary>
        /// Handle Websocket Connection
        /// </summary>
        private void HandleWebsocket(IWebSocketConnection socket)
        {
            socket.OnMessage = message =>
            {
                var update = DecodeMessage(message);
                foreach (var observer in _observers.Values)
                    observer.OnNext(update);
            };
        }
    
        /// <summary>
        /// Handles a JSON string message
        /// </summary>
        private IJoystickUpdate DecodeMessage(string message)
        {
            var obj = JObject.Parse(message);
            var id = (uint)obj["id"];

            switch ((string)obj["type"])
            {
                case "button":
                    var button = (uint)obj["button"];
                    var pressed = (bool)obj["pressed"];

                    return new JoystickButtonState
                    {
                        ButtonId = button,
                        Pressed = pressed,
                        Id = id
                    };
                case "direction":
                    int x = 50;
                    int y = 50;
                    var left = (bool)obj["directions"]["left"];
                    var right = (bool)obj["directions"]["right"];
                    var up = (bool)obj["directions"]["up"];
                    var down = (bool)obj["directions"]["down"];

                    if (left) x = 0;
                    if (right) x = 100;
                    if (up) y = 0;
                    if (down) y = 100;

                    return new JoystickAxisState
                    {
                        X = x,
                        Y = y,
                        Id = id
                    };
                default:
                    throw new Exception("Invalid message received!");
            }
        }

        public void Dispose()
        {
            foreach(var observer in _observers.Values)
                observer.OnCompleted();

            _server.Dispose();
        }
    }
}
