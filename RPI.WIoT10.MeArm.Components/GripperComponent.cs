using System;
using System.Threading.Tasks;
using Devices.Components;
using Devices.Hardware.Actors;

namespace RPI.WIoT10.MeArm.Components
{
    public class GripperComponent : ComponentBase
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

        [Action("Disengage")]
        [ActionHelp("Unpowers the gripper at current position.")]
        private Task GripperComponentDisengage(MessageContainer data)
        {
            servo.Disengage();
            return Task.CompletedTask;
        }

        [Action("Close")]
        [ActionHelp("Closes the Gripper.")]
        private async Task GripperComponentClose(MessageContainer data)
        {
            await SetGripperPosition(closePosition);
        }

        [Action("Open")]
        [ActionHelp("Opens the Gripper.")]
        private async Task GripperComponentOpen(MessageContainer data)
        {
            await SetGripperPosition(openPosition);
        }

        [Action("Set")]
        [Action("Position")]
        [ActionParameter("Position")]
        [ActionHelp("Sets the Gripper to a position between 0.0 (open) and 100 (close).")]
        private async Task GripperComponentSet(MessageContainer data)
        {
            double position = Double.Parse(data.ResolveParameter("Position", 0));
            await SetGripperPosition(position);
        }

        [Action("Get")]
        [ActionHelp("Gets the current gripper position between 0.0 (open) and 100 (close).")]
        private async Task GripperComponentGet(MessageContainer data)
        {
            data.AddMultiPartValue("Position", servo.Position);
            await ComponentHandler.HandleOutput(data).ConfigureAwait(false);
        }

        public Task SetGripperPosition(double position)
        {
            servo.Position = position;
            return Task.CompletedTask;
        }

    }
}
