using System;
using System.Threading.Tasks;
using Devices.Components;
using Devices.Hardware.Actors;

namespace RPI.WIoT10.MeArm.Components
{
    public class LeverComponent : ComponentBase
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

        [Action("Disengage")]
        [ActionHelp("Unpowers the lever motor at current position.")]
        private Task LeverComponentDisengage(MessageContainer data)
        {
            servo.Disengage();
            return Task.CompletedTask;
        }

        [Action("Set")]
        [Action("Position")]
        [ActionParameter("Position")]
        [ActionHelp("Sets the lever to a position between 0 and 180.")]
        private async Task LeverComponentSet(MessageContainer data)
        {
            double position = Double.Parse(data.ResolveParameter("Position", 0));
            await SetLeverPosition(position);
        }

        [Action("Get")]
        [ActionHelp("Gets the current lever position between 0 and 180.")]
        private async Task LeverComponentGet(MessageContainer data)
        {
            data.AddValue("Position", servo.Position);
            await ComponentHandler.HandleOutput(data).ConfigureAwait(false);
        }

        public Task SetLeverPosition(double position)
        {
            servo.Position = position;
            return Task.CompletedTask;
        }


    }
}
