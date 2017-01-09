using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devices.Hardware.Interfaces;
using GHIElectronics.UWP.LowLevelDrivers;

namespace RPI.WIoT10.FEZUtility
{
    public class PCA9685PWMChannel : IPWMChannel
    {
        private readonly PCA9685 pwm;
        private readonly int channel;

        public PCA9685PWMChannel(PCA9685 pwm, int channel)
        {
            this.pwm = pwm;
            this.channel = channel;
        }

        public int Frequency
        {
            get { return this.pwm.Frequency; }
            set { this.pwm.Frequency = value; }
        }

        public int Resolution
        {
            get { return 0x1000; }
        }

        public void Release()
        {
            this.SetPulse(0x1000);
        }

        public void SetDutyCycle(double dutyCycle)
        {
            this.pwm.SetDutyCycle(channel, dutyCycle);
        }

        public void SetPulse(ushort pulseLength)
        {
            this.pwm.SetChannel(channel, 0x00, pulseLength);
        }
    }
}
