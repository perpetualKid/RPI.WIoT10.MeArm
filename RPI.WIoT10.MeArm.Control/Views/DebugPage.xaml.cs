using System;
using Devices.Controllers.Base;
using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RPI.WIoT10.MeArm.Control.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DebugPage : Page
    {
        DebugHandler debugHandler;
        GenericController debugController;

        public DebugPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            debugController = await GenericController.GetNamedInstance<GenericController>("DebugController", string.Empty);
            debugHandler = DebugHandler.Instance;
            debugHandler.OnDataReceived += DebugController_OnReceivedTextUpdated;
            debugHandler.OnDataSent += DebugController_OnSentTextUpdated;
            txtTextReceived.Text = debugHandler.DataReceived;
            txtTextSent.Text = debugHandler.DataSent;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            debugHandler.OnDataReceived -= DebugController_OnReceivedTextUpdated;
            debugHandler.OnDataSent -= DebugController_OnSentTextUpdated;
            base.OnNavigatedFrom(e);
        }

        private async void DebugController_OnSentTextUpdated(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                txtTextSent.Text = debugHandler.DataSent;
            });
        }

        private async void DebugController_OnReceivedTextUpdated(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                txtTextReceived.Text = debugHandler.DataReceived;
            });
        }

        private void btnTextReceivedClear_Click(object sender, RoutedEventArgs e)
        {
            debugHandler?.ClearReceivedBuffer();
        }

        private void btnTextSentClear_Click(object sender, RoutedEventArgs e)
        {
            debugHandler?.ClearSentBuffer();
        }

        private async void btnCommandAction_Click(object sender, RoutedEventArgs e)
        {
            JsonObject data;
            if (JsonObject.TryParse(txtTextCommand.Text, out data))
            {
                await debugController.SendRequest(data, true);
            }
            else
            {
                ContentDialog invalidJsonDialog = new ContentDialog()
                {
                    Title = "Json Command not valid",
                    Content = "Please specify a valid Json string to be sent to the device host.",
                    PrimaryButtonText = "Ok"
                };
                await invalidJsonDialog.ShowAsync();
            }
        }
    }
}
