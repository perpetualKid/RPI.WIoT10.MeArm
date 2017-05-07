using System;
using System.Threading.Tasks;
using Devices.Communication;
using Devices.Controllers.Base;
using Devices.Util.Extensions;
using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RPI.WIoT10.MeArm.Control.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : Page
    {

        ApplicationDataContainer settings;
        private GenericController landingPageController;

        public LandingPage()
        {
            this.InitializeComponent();
            settings = ApplicationData.Current.LocalSettings;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            landingPageController = await ControllerBase.GetNamedInstance<GenericController>("LandingPage", "BrickPi.NxtColor.Port_S3");
            landingPageController.OnResponseReceived += LandingPageController_OnResponseReceived;
            base.OnNavigatedTo(e);
        }

        private void LandingPageController_OnResponseReceived(object sender, JsonObject e)
        {
            throw new NotImplementedException();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            landingPageController.OnResponseReceived -= LandingPageController_OnResponseReceived;
            base.OnNavigatedFrom(e);
        }


        private async void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            await Task.CompletedTask;
            //if (DeviceConnectionController.Instance.ConnectionStatus == ConnectionStatus.Connected)
            //{
            //    JsonObject gripper = new JsonObject();
            //    gripper.AddValue("Target", "Gripper");
            //    gripper.AddValue("Action", "Set");
            //    gripper.AddValue("Position", e.NewValue);
            //    await DeviceConnectionController.Instance.Send("LandingPage", gripper);
            //    JsonObject gripperPosition = new JsonObject();
            //    gripperPosition.AddValue("Target", "Gripper");
            //    gripperPosition.AddValue("Action", "Get");
            //    await DeviceConnectionController.Instance.Send("LandingPage", gripperPosition);
            //}

            //if (ControllerHandler.Connection.ConnectionStatus == ConnectionStatus.Connected)
            //{
            //    JsonObject gripper = new JsonObject();
            //    gripper.AddValue("Target", "Gripper");
            //    gripper.AddValue("Action", "Set");
            //    gripper.AddValue("Position", e.NewValue);
            //    await ControllerHandler.Connection.Send("LandingPage", gripper);
            //    JsonObject gripperPosition = new JsonObject();
            //    gripperPosition.AddValue("Target", "Gripper");
            //    gripperPosition.AddValue("Action", "Get");
            //    await ControllerHandler.Connection.Send("LandingPage", gripperPosition);
            //}

        }

        private async void GripperSlider_Moved(object sender, Controls.SliderEventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                JsonObject command = new JsonObject();
                command.AddValue("Target", "Gripper");
                command.AddValue("Action", "Set");
                command.AddValue("Position", Map(-100, 100, 0, 100, e.Distance));
                await landingPageController.SendRequest(command, true);
            }
        }
        private async void GripperSlider_Captured(object sender, EventArgs e)
        {
            //if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            //{
            //    await landingPageController.SendRequest("Engage", "Gripper");
            //}
        }

        private async void GripperSlider_Released(object sender, Controls.SliderEventArgs e)
        {
            //if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            //{
            //    await landingPageController.SendRequest("Disengage", "Gripper");
            //}
        }

        private static double Map(double minInput, double maxInput, double minValue, double maxValue, double current)
        {
            return (current - minInput) * (maxValue- minValue) / (maxInput- minInput) + minValue;
        }

        private async void TurnTableSlider_Moved(object sender, Controls.SliderEventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                JsonObject command = new JsonObject();
                command.AddValue("Target", "TurnTable");
                command.AddValue("Action", "Set");
                command.AddValue("Position", Map(-100, 100, 180, 0, e.Distance));
                await landingPageController.SendRequest(command, true);
            }
        }

        private async void TurnTableSlider_Captured(object sender, EventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                await landingPageController.SendRequest("Engage", "TurnTable");
            }
        }

        private async void TurnTableSlider_Released(object sender, Controls.SliderEventArgs e)
        {
            //if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            //{
            //    await landingPageController.SendRequest("Disengage", "TurnTable");
            //}
        }

        private async void UpperArmSlider_Moved(object sender, Controls.SliderEventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                JsonObject command = new JsonObject();
                command.AddValue("Target", "UpperLever");
                command.AddValue("Action", "Set");
                command.AddValue("Position", Map(-100, 100, 180, 0, e.Distance));
                await landingPageController.SendRequest(command, true);
            }
        }

        private async void UpperArmSlider_Captured(object sender, EventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                await landingPageController.SendRequest("Engage", "UpperLever");
            }
        }

        private async void UpperArmSlider_Released(object sender, Controls.SliderEventArgs e)
        {
            //if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            //{
            //    await landingPageController.SendRequest("Disengage", "UpperLever");
            //}
        }

        private async void LowerArmSlider_Moved(object sender, Controls.SliderEventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                JsonObject command = new JsonObject();
                command.AddValue("Target", "LowerLever");
                command.AddValue("Action", "Set");
                command.AddValue("Position", Map(-100, 100, 0, 180, e.Distance));
                await landingPageController.SendRequest(command, true);
            }
        }

        private async void LowerArmSlider_Captured(object sender, EventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                await landingPageController.SendRequest("Engage", "LowerLever");
            }
        }

        private async void LowerArmSlider_Released(object sender, Controls.SliderEventArgs e)
        {
            //if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            //{
            //    await landingPageController.SendRequest("Disengage", "LowerLever");
            //}
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (ControllerHandler.ConnectionStatus == ConnectionStatus.Connected)
            {
                await landingPageController.SendRequest("Disengage", "LowerLever");
                await landingPageController.SendRequest("Disengage", "UpperLever");
                await landingPageController.SendRequest("Disengage", "TurnTable");
                await landingPageController.SendRequest("Disengage", "Gripper");
            }

        }
    }
}
