namespace Contropad.Core.Joystick
{
    public class JoystickAxisState : IJoystickUpdate
    {
        public uint Id { get; set; }

        /// <summary>
        /// Position of the X axis in percentages
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Position of the Y axis in percentages
        /// </summary>
        public int Y { get; set; }

        public void Update(JoystickFeeder feeder)
        {
            feeder.UpdateState(Id, this);
        }
    }
}