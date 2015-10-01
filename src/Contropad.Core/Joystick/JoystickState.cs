using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Contropad.Core.Joystick
{
    public interface IJoystickUpdate
    {
        uint id { get; }
    }

    public class JoystickAxisState : IJoystickUpdate
    {
        public uint id { get; set; }

        /// <summary>
        /// Position of the X axis in percentages
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Position of the Y axis in percentages
        /// </summary>
        public int y { get; set; }
    }

    public class JoystickButtonState : IJoystickUpdate
    {
        public uint id { get; set; }

        public uint ButtonId { get; set; }
        public bool Pressed { get; set; }
    }
}
