namespace Contropad.Core.Joystick
{
    public interface IJoystickUpdate
    {
        /// <summary>
        /// Sends the update to the feeder
        /// </summary>
        void Update(JoystickFeeder feeder);
    }

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

    public class JoystickButtonState : IJoystickUpdate
    {
        public uint Id { get; set; }

        public uint ButtonId { get; set; }
        public bool Pressed { get; set; }


        public void Update(JoystickFeeder feeder)
        {
            feeder.UpdateButtonState(Id, this);
        }
    }
}
