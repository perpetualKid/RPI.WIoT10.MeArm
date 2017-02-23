using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Communication;
using Devices.Components;
using Devices.Components.Common.Communication;
using Devices.Components.Common.Media;
using Devices.Hardware.Actors;
using RPI.WIoT10.FEZUtility;
using RPI.WIoT10.MeArm.Components;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;

namespace RPI.WIoT10.MeArm.App
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;
        FEZUtilityShield shield;
        GripperComponent gripper;
        TurnTableComponent turntable;
        LeverComponent upperLever;
        LeverComponent lowerLever;
        CameraComponent frontCamera;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            List<Task> setupTasks = new List<Task>();

            setupTasks.Add(ComponentHandler.RegisterComponent(new SocketListener(8027)));
            setupTasks.Add(ComponentHandler.RegisterComponent(new SocketListener(8031, DataFormat.Json)));

            shield = await FEZUtilityShield.CreateAsync().ConfigureAwait(false);

            gripper = new GripperComponent(new Servo(new PCA9685PWMChannel(shield.PCA9685PWM, (int)FEZUtilityShield.PwmPin.P3)));
            setupTasks.Add(ComponentHandler.RegisterComponent(gripper));
            turntable = new TurnTableComponent(new Servo(new PCA9685PWMChannel(shield.PCA9685PWM, (int)FEZUtilityShield.PwmPin.P0)));
            setupTasks.Add(ComponentHandler.RegisterComponent(turntable));
            lowerLever = new LeverComponent(new Servo(new PCA9685PWMChannel(shield.PCA9685PWM, (int)FEZUtilityShield.PwmPin.P1)), "LowerLever");
            setupTasks.Add(ComponentHandler.RegisterComponent(lowerLever));
            upperLever = new LeverComponent(new Servo(new PCA9685PWMChannel(shield.PCA9685PWM, (int)FEZUtilityShield.PwmPin.P2)), "UpperLever");
            setupTasks.Add(ComponentHandler.RegisterComponent(upperLever));

            var videoDevices = await CameraComponent.GetAllVideoDevices().ConfigureAwait(false);

            if (videoDevices.Count > 0)
            {
                frontCamera = new CameraComponent("FrontCamera",
                    new MediaCaptureInitializationSettings
                    {
                        StreamingCaptureMode = StreamingCaptureMode.Video,
                        PhotoCaptureSource = PhotoCaptureSource.Auto,
                        AudioDeviceId = string.Empty,
                        VideoDeviceId = videoDevices[0].Id
                    });
                setupTasks.Add(ComponentHandler.RegisterComponent(frontCamera));
            }
            await Task.WhenAll(setupTasks).ConfigureAwait(false);

        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //a few reasons that you may be interested in.
            switch (reason)
            {
                case BackgroundTaskCancellationReason.Abort:
                    //app unregistered background task (amoung other reasons).
                    break;
                case BackgroundTaskCancellationReason.Terminating:
                    //system shutdown
                    break;
                case BackgroundTaskCancellationReason.ConditionLoss:
                    break;
                case BackgroundTaskCancellationReason.SystemPolicy:
                    break;
            }
            deferral.Complete();
        }
    }
}
