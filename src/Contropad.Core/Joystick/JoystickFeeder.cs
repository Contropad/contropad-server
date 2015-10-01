using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace Contropad.Core.Joystick
{
    public class JoystickFeeder
    {
        protected ConcurrentDictionary<uint, JoystickAxisState> joystickStates;

        private vJoy _joy;

        private long maxval;
        private decimal percentage;

        public JoystickFeeder()
        {
            _joy = new vJoy();
            joystickStates = new ConcurrentDictionary<uint, JoystickAxisState>();

            _joy.GetVJDAxisMax(1, HID_USAGES.HID_USAGE_X, ref maxval);
            percentage = maxval / 100;
        }

        public void UpdateState(uint id, JoystickAxisState axisState)
        {
            if (!joystickStates.ContainsKey(id))
                ActivateJoystick(id);

            joystickStates.AddOrUpdate(id, axisState, (u, joystickState) => axisState);
        }

        public void UpdateButtonState(uint id, JoystickButtonState buttonState)
        {
            if (!joystickStates.ContainsKey(id))
                ActivateJoystick(id);

            _joy.SetBtn(buttonState.Pressed, buttonState.id, buttonState.ButtonId);
        }

        protected VjdStat VerifyStatus(uint id)
        {
            VjdStat status = _joy.GetVJDStatus(id);
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
            };

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
                    foreach (var joystick in joystickStates)
                    {
                        var state = joystick.Value;
                        var id = joystick.Key;
                        _joy.SetAxis(state.x * (int)percentage, id, HID_USAGES.HID_USAGE_X);
                        _joy.SetAxis(state.y * (int)percentage, id, HID_USAGES.HID_USAGE_Y);
                    }
                    await Task.Delay(20, token);
                }
            }, token);
        }

    }
}
