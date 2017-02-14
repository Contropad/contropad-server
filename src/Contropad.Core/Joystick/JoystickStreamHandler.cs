using System;

namespace Contropad.Core.Joystick
{
    public class JoystickStreamHandler : IObserver<IJoystickUpdate>
    {
        private readonly JoystickFeeder _feeder;

        public JoystickStreamHandler(JoystickFeeder feeder)
        {
            _feeder = feeder;
        }

        public void OnNext(IJoystickUpdate joystickUpdate)
        {
            joystickUpdate.Update(_feeder);
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }
    }
}
