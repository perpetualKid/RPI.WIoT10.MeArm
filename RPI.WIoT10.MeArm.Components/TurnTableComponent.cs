using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devices.Controllable;
using Devices.Hardware.Actors;

namespace RPI.WIoT10.MeArm.Components
{
    public class TurnTableComponent : ControllableComponent
    {
        private Servo servo;

        public TurnTableComponent(Servo servo): base("TurnTable")
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
            data.AddMultiPartValue("Help", "TURNTABLE HELP : Shows this help screen.");
            data.AddMultiPartValue("Help", "TURNTABLE DISENGAGE : Unpowers the turntable at current position.");
            data.AddMultiPartValue("Help", "TURNTABLE CENTER : Turns the unit to Center position.");
            data.AddMultiPartValue("Help", "TURNTABLE SET <Position> : Turns the unit to a position between 0 and 180");
            data.AddMultiPartValue("Help", "TURNTABLE GET : Gets the current unit position between 0 and 180");
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
                    await TurnTableComponentDisengage(data);
                    break;
                case "CENTER":
                    await TurnTableComponentCenter(data);
                    break;
                case "POSITION":
                case "SET":
                    await TurnTableComponentSet(data);
                    break;
                case "GET":
                    await TurnTableComponentGet(data);
                    break;
            }
        }

        private async Task TurnTableComponentDisengage(MessageContainer data)
        {
            servo.Disengage();
            await Task.CompletedTask;
        }

        private async Task TurnTableComponentCenter(MessageContainer data)
        {
            await SetTurnTablePosition(90);
        }

        private async Task TurnTableComponentSet(MessageContainer data)
        {
            double position = Double.Parse(data.ResolveParameter("Position", 0));
            await SetTurnTablePosition(position);
        }

        private async Task TurnTableComponentGet(MessageContainer data)
        {
            data.AddMultiPartValue("Position", servo.Position);
            await HandleOutput(data).ConfigureAwait(false);
        }

        public async Task SetTurnTablePosition(double position)
        {
            servo.Position = position;
            await Task.CompletedTask;
        }

    }
}
