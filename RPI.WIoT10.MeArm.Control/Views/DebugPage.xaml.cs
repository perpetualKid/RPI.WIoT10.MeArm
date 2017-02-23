using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Devices.Controllers.Base;
using Devices.Controllers.Common;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RPI.WIoT10.MeArm.Control.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DebugPage : Page
    {
        private DebugController debugController;

        public DebugPage()
        {
            this.InitializeComponent();
            debugController = DebugController.GetNamedInstance<DebugController>("DebugController").Result;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            debugController.OnDataReceived += DebugController_OnDataReceived;
            textBox.Text = debugController.Textbuffer;
        }

        private async void DebugController_OnDataReceived(object sender, string data)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                textBox.Text = debugController.Textbuffer;
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            debugController.OnDataReceived -= DebugController_OnDataReceived;
            base.OnNavigatedFrom(e);
        }
    }
}
