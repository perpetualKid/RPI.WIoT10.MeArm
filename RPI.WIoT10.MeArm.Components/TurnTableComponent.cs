using System;
using System.Threading.Tasks;
using Devices.Components;
using Devices.Hardware.Actors;

namespace RPI.WIoT10.MeArm.Components
{
    public class TurnTableComponent : ComponentBase
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

        [Action("Disengage")]
        [ActionHelp("Unpowers the turntable at current position.")]
        private Task TurnTableComponentDisengage(MessageContainer data)
        {
            servo.Disengage();
            return Task.CompletedTask;
        }

        [Action("Center")]
        [ActionHelp("Turns the unit to Center position.")]
        private async Task TurnTableComponentCenter(MessageContainer data)
        {
            await SetTurnTablePosition(90);
        }

        [Action("Set")]
        [Action("Position")]
        [ActionParameter("Position")]
        [ActionHelp("Sets the Gripper to a position between 0 and 180.")]
        private async Task TurnTableComponentSet(MessageContainer data)
        {
            double position = Double.Parse(data.ResolveParameter("Position", 0));
            await SetTurnTablePosition(position);
        }

        [Action("Get")]
        [ActionHelp("Gets the current turntable position between 0 and 180.")]
        private async Task TurnTableComponentGet(MessageContainer data)
        {
            data.AddValue("Position", servo.Position);
            await ComponentHandler.HandleOutput(data).ConfigureAwait(false);
        }

        public Task SetTurnTablePosition(double position)
        {
            servo.Position = position;
            return Task.CompletedTask;
        }

    }
}
