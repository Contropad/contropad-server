namespace Contropad.Core.Joystick
{
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