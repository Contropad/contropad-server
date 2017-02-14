using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contropad.Core.Joystick
{
    public class JoystickStreamHandler : IObserver<IJoystickUpdate>
    {
        private JoystickFeeder _feeder;

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
