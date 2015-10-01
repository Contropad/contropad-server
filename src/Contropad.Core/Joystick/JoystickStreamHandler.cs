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

        public void OnNext(IJoystickUpdate buttonState)
        {
            if (buttonState is JoystickAxisState)
            {
                _feeder.UpdateState(buttonState.id, (JoystickAxisState)buttonState);
            }
            else if (buttonState is JoystickButtonState)
            {
                _feeder.UpdateButtonState(buttonState.id, (JoystickButtonState)buttonState);
            }

        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }
    }
}
