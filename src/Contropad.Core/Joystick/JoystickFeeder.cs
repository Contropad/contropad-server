using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace Contropad.Core.Joystick
{
    public class JoystickFeeder
    {
        private readonly ConcurrentDictionary<uint, JoystickAxisState> _joystickStates;

        private vJoy _joy;

        private long maxval;
        private decimal percentage;

        public JoystickFeeder()
        {
            _joy = new vJoy();
            _joystickStates = new ConcurrentDictionary<uint, JoystickAxisState>();

            _joy.GetVJDAxisMax(1, HID_USAGES.HID_USAGE_X, ref maxval);
            percentage = maxval / 100;
        }

        public void UpdateState(uint id, JoystickAxisState axisState)
        {
            if (!_joystickStates.ContainsKey(id))
                ActivateJoystick(id);

            _joystickStates.AddOrUpdate(id, axisState, (u, joystickState) => axisState);
        }

        public void UpdateButtonState(uint id, JoystickButtonState buttonState)
        {
            if (!_joystickStates.ContainsKey(id))
                ActivateJoystick(id);

            _joy.SetBtn(buttonState.Pressed, buttonState.Id, buttonState.ButtonId);
        }

        protected VjdStat VerifyStatus(uint id)
        {
            var status = _joy.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", id);
                    break;

                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                    throw new Exception();
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                    throw new Exception();
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                    throw new Exception();
            }

            return status;
        }

        public void ActivateJoystick(uint id)
        {
            VerifyStatus(id);
            _joy.AcquireVJD(id);
            _joy.ResetVJD(id);
        }

        public void DeactivateJoystick(uint id)
        {
            _joy.RelinquishVJD(id);
        }

        public Task StartFeeding(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var joystick in _joystickStates)
                    {
                        var state = joystick.Value;
                        var id = joystick.Key;
                        _joy.SetAxis(state.X * (int)percentage, id, HID_USAGES.HID_USAGE_X);
                        _joy.SetAxis(state.Y * (int)percentage, id, HID_USAGES.HID_USAGE_Y);
                    }
                    await Task.Delay(5, token);
                }
            }, token);
        }

    }
}
