using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devices.Controllable;
using Devices.Hardware.Actors;

namespace RPI.WIoT10.MeArm.Components
{
    public class GripperComponent : ControllableComponent
    {
        private Servo servo;
        private double openPosition = 0;
        private double closePosition = 100;


        public GripperComponent(Servo servo): base("Gripper")
        {
            this.servo = servo;
        }

        protected override async Task InitializeDefaults()
        {
            servo.SetLimits(1050, 2750, 0, 100, 0);
            servo.Disengage();
            await base.InitializeDefaults();
        }

        protected override async Task ComponentHelp(MessageContainer data)
        {
            data.AddMultiPartValue("Help", "GRIPPER HELP : Shows this help screen.");
            data.AddMultiPartValue("Help", "GRIPPER DISENGAGE : Unpowers the gripper at current position.");
            data.AddMultiPartValue("Help", "GRIPPER CLOSE: Closes the Gripper.");
            data.AddMultiPartValue("Help", "GRIPPER OPEN: Opens the Gripper.");
            data.AddMultiPartValue("Help", "GRIPPER SET <Position> : Sets the Gripper to a position between 0.0 (open) and 100 (close).");
            data.AddMultiPartValue("Help", "GRIPPER GET : Gets the current Gripper position between 0.0 (open) and 100 (close).");
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
                    await GripperComponentDisengage(data);
                    break;
                case "CLOSE":
                    await GripperComponentClose(data);
                    break;
                case "OPEN":
                    await GripperComponentOpen(data);
                    break;
                case "POSITION":
                case "SET":
                    await GripperComponentSet(data);
                    break;
                case "GET":
                    await GripperComponentGet(data);
                    break;
            }
        }

        private async Task GripperComponentDisengage(MessageContainer data)
        {
            servo.Disengage();
            await Task.CompletedTask;
        }

        private async Task GripperComponentClose(MessageContainer data)
        {
            await SetGripperPosition(closePosition);
        }

        private async Task GripperComponentOpen(MessageContainer data)
        {
            await SetGripperPosition(openPosition);
        }

        private async Task GripperComponentSet(MessageContainer data)
        {
            double position = Double.Parse(data.ResolveParameter("Position", 0));
            await SetGripperPosition(position);
        }

        private async Task GripperComponentGet(MessageContainer data)
        {
            data.AddMultiPartValue("Position", servo.Position);
            await HandleOutput(data).ConfigureAwait(false);
        }

        public async Task SetGripperPosition(double position)
        {
            servo.Position = position;
            await Task.CompletedTask;
        }

    }
}
