using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHIElectronics.UWP.LowLevelDrivers;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace RPI.WIoT10.FEZUtility
{
    public class FEZUtilityShield : IDisposable
    {

        /// <summary>
        /// The possible analog pins.
        /// </summary>
        public enum AnalogPin
        {
            A0 = 0,
            A1 = 1,
            A2 = 2,
            A3 = 3,
            A4 = 4,
            A5 = 5,
            A6 = 6,
            A7 = 7,
        }

        /// <summary>
        /// The possible pwm pins.
        /// </summary>
        public enum PwmPin
        {
            P0 = 0,
            P1 = 1,
            P2 = 2,
            P3 = 3,
            P4 = 4,
            P5 = 5,
            P6 = 6,
            P7 = 7,
            P8 = 8,
            P9 = 9,
            P10 = 10,
            P11 = 11,
            P12 = 12,
            P13 = 13,
            P14 = 14,   //LED3
            P15 = 15,   //LED4
        }

        /// <summary>
        /// The possible digital pins.
        /// </summary>
        public enum DigitalPin
        {
            V00 = 0,
            V01 = 1,
            V02 = 2,
            V03 = 3,
            V04 = 4,
            V05 = 5,
            V06 = 6,
            V07 = 7,
            V10 = 10,
            V11 = 11,
            V12 = 12,
            V13 = 13,
            V14 = 14,
            V15 = 15,
        }

        /// <summary>
        /// The possible LEDs.
        /// </summary>
        public enum Led
        {
            Led1 = 16,
            Led2 = 17,
            Led3 = 14,  //PWM
            Led4 = 15,  //PWM
        }

        private PCA9685 pwm;
        private PCA9535 gpio;
        private ADS7830 analog;

        private static FEZUtilityShield instance;

        public static async Task<FEZUtilityShield> CreateAsync()
        {
            if (null == instance)
            {
                GpioController gpioController;

                instance = new FEZUtilityShield();

                    gpioController = await GpioController.GetDefaultAsync().AsTask().ConfigureAwait(false);
                    DeviceInformation i2cController;
                    i2cController = (await DeviceInformation.FindAllAsync(I2cDevice.GetDeviceSelector("I2C1")).AsTask().ConfigureAwait(false))[0];

                    instance.analog = new ADS7830(await I2cDevice.FromIdAsync(i2cController.Id, new I2cConnectionSettings(ADS7830.GetAddress(false, false))));
                    instance.pwm = new PCA9685(await I2cDevice.FromIdAsync(i2cController.Id, new I2cConnectionSettings(PCA9685.GetAddress(true, true, true, true, true, true))));
                    instance.gpio = new PCA9535(await I2cDevice.FromIdAsync(i2cController.Id, new I2cConnectionSettings(PCA9535.GetAddress(true, true, false))), gpioController.OpenPin(22));
            }

            return instance;
        }

        public PCA9685 PCA9685PWM { get { return this.pwm; } }
        private PCA9535 PCA9535GPIO { get { return this.gpio; } }
        private ADS7830 ADS7830Analog { get { return this.analog; } }


        /// <summary>
        /// Sets the duty cycle of the given pwm pin.
        /// </summary>
        /// <param name="pin">The pin to set the duty cycle for.</param>
        /// <param name="value">The new duty cycle between 0 (off) and 1 (on).</param>
        public void SetPwmDutyCycle(PwmPin pin, double value)
        {
            if (value < 0.0 || value > 1.0) throw new ArgumentOutOfRangeException(nameof(value));
            if (!Enum.IsDefined(typeof(PwmPin), pin)) throw new ArgumentException(nameof(pin));

            this.pwm.SetDutyCycle((int)pin, value);
        }

        /// <summary>
        /// Sets the drive mode of the given pin.
        /// </summary>
        /// <param name="pin">The pin to set.</param>
        /// <param name="driveMode">The new drive mode of the pin.</param>
        public void SetDigitalDriveMode(DigitalPin pin, GpioPinDriveMode driveMode)
        {
            if (!Enum.IsDefined(typeof(DigitalPin), pin)) throw new ArgumentException(nameof(pin));

            this.gpio.SetDriveMode((int)pin, driveMode);
        }

        /// <summary>
        /// Write the given value to the given pin.
        /// </summary>
        /// <param name="pin">The pin to set.</param>
        /// <param name="state">The new state of the pin.</param>
        public void WriteDigital(DigitalPin pin, bool state)
        {
            if (!Enum.IsDefined(typeof(DigitalPin), pin)) throw new ArgumentException(nameof(pin));

            this.gpio.Write((int)pin, state);
        }

        /// <summary>
        /// Reads the current state of the given pin.
        /// </summary>
        /// <param name="pin">The pin to read.</param>
        /// <returns>True if high, false is low.</returns>
        public bool ReadDigital(DigitalPin pin)
        {
            if (!Enum.IsDefined(typeof(DigitalPin), pin)) throw new ArgumentException(nameof(pin));

            return this.gpio.Read((int)pin);
        }

        /// <summary>
        /// Sets the state of the given onboard LED.
        /// </summary>
        /// <param name="led">The LED to set.</param>
        /// <param name="state">The new state of the LED.</param>
        public void SetLedState(Led led, bool state)
        {
            if (!Enum.IsDefined(typeof(Led), led)) throw new ArgumentException(nameof(led));

            if (led == Led.Led1 || led == Led.Led2)
            {
                this.gpio.Write((int)led, state);
            }
            else
            {
                this.pwm.SetDutyCycle((int)led, state ? 1.00 : 0.00);
            }
        }

        /// <summary>
        /// Reads the current voltage on the given pin.
        /// </summary>
        /// <param name="pin">The pin to read.</param>
        /// <returns>The voltage between 0 (0V) and 1 (3.3V).</returns>
        public double ReadAnalog(AnalogPin pin)
        {
            if (!Enum.IsDefined(typeof(AnalogPin), pin)) throw new ArgumentException(nameof(pin));

            return this.analog.Read((byte)pin);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
