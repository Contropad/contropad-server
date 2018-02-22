namespace Contropad.Core.Joystick
{
    public interface IJoystickUpdate
    {
        /// <summary>
        /// Sends the update to the feeder
        /// </summary>
        void Update(JoystickFeeder feeder);
    }
}
