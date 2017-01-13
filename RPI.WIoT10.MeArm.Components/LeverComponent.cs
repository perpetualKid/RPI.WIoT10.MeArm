using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devices.Controllable;
using Devices.Hardware.Actors;

namespace RPI.WIoT10.MeArm.Components
{
    public class LeverComponent : ControllableComponent
    {
        private Servo servo;

        public LeverComponent(Servo servo) : base("Lever")
        {
            this.servo = servo;

        }
        public LeverComponent(Servo servo, string componentName) : base(componentName)
        {
            this.servo = servo;

        }

        protected override async Task InitializeDefaults()
        {
            servo.SetLimits(650, 2780, 0, 180, 90);
            servo.Disengage();
            await base.InitializeDefaults();
        }

        protected override async Task ComponentHelp(MessageContainer data)
        {
            data.AddMultiPartValue("Help", "LEVER HELP : Shows this help screen.");
            data.AddMultiPartValue("Help", "LEVER DISENGAGE : Unpowers the turntable at current position.");
            data.AddMultiPartValue("Help", "LEVER SET <Position> : Turns the unit to a position between 0 and 180");
            data.AddMultiPartValue("Help", "LEVER GET : Gets the current unit position between 0 and 180");
            await HandleOutput(data).ConfigureAwait(false);
        }

        protected override async Task ProcessCommand(MessageContainer data)
        {
            switch (data.ResolveParameter(nameof(MessageContainer.FixedPropertyNames.Action), 1).ToUpperInvariant())
            {
                case "HELP":
                    await ComponentHelp(data);
                    break;
                case "DISENGAGE":
                    await LeverComponentDisengage(data);
                    break;
                case "POSITION":
                case "SET":
                    await LeverComponentSet(data);
                    break;
                case "GET":
                    await LeverComponentGet(data);
                    break;
            }
        }
        private async Task LeverComponentDisengage(MessageContainer data)
        {
            servo.Disengage();
            await Task.CompletedTask;
        }

        private async Task LeverComponentSet(MessageContainer data)
        {
            double position = Double.Parse(data.ResolveParameter("Position", 0));
            await SetLeverPosition(position);
        }

        private async Task LeverComponentGet(MessageContainer data)
        {
            data.AddMultiPartValue("Position", servo.Position);
            await HandleOutput(data).ConfigureAwait(false);
        }

        public async Task SetLeverPosition(double position)
        {
            servo.Position = position;
            await Task.CompletedTask;
        }


    }
}
