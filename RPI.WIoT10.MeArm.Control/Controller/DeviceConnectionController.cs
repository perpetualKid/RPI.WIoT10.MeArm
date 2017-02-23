﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Communication;
using Devices.Communication.Channels;
using Devices.Communication.Sockets;
using Devices.Util.Extensions;
using Windows.Data.Json;

namespace RPI.WIoT10.MeArm.Control.Controller
{
    public class DeviceConnectionController
    {
        public enum FixedNames
        {
            Target,
            Sender,
            Action,
        }

        private static DeviceConnectionController instance;
        private SocketClient socketClient;
        private Dictionary<string, System.Delegate> eventRoutingTable;

        public event EventHandler<ConnectionStatusChangedEventArgs> OnConnectionStatusChanged;
        public delegate void DataReceivedEventHandler(JsonObject data);

        private DeviceConnectionController()
        {
            eventRoutingTable = new Dictionary<string, Delegate>();
            this.socketClient = new SocketClient();
            socketClient.OnConnectionStatusChanged += SocketClient_OnConnectionStatusChanged;
        }

        #region event handling
        private void SocketClient_OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            OnConnectionStatusChanged?.Invoke(this, e);
        }

        private void SocketClient_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            JsonObject json = (e as JsonMessageArgs).Json;
            if (eventRoutingTable.ContainsKey("*"))
                ((DataReceivedEventHandler)(eventRoutingTable["*"])).Invoke(json);
            if (json.ContainsKey(nameof(FixedNames.Sender)) && eventRoutingTable.ContainsKey(json.GetNamedString(nameof(FixedNames.Sender))))
                ((DataReceivedEventHandler)(eventRoutingTable[json.GetNamedString(nameof(FixedNames.Sender))])).Invoke(json);
        }

        public void RegisterOnDataReceivedEvent(string name, DataReceivedEventHandler handler)
        {
            lock (eventRoutingTable)
            {
                if (!eventRoutingTable.ContainsKey(name))
                    eventRoutingTable.Add(name, null);
                eventRoutingTable[name] = (DataReceivedEventHandler)eventRoutingTable[name] + handler;
            }
        }

        public void UnRegisterOnDataReceivedEvent(string name, DataReceivedEventHandler handler)
        {
            lock (eventRoutingTable)
            {
                if (!eventRoutingTable.ContainsKey(name))
                    eventRoutingTable.Add(name, null);
                eventRoutingTable[name] = (DataReceivedEventHandler)eventRoutingTable[name] - handler;
            }
        }
        #endregion

        public static DeviceConnectionController Instance
        {
            get
            {
                if (null == instance)
                    instance = new DeviceConnectionController();
                return instance;
            }
        }

        public ConnectionStatus ConnectionStatus { get { return socketClient.ConnectionStatus; } }

        public async Task<bool> Connect(string host, string port)
        {
            if (socketClient.ConnectionStatus != ConnectionStatus.Connected)
            {
                ChannelBase channel = await socketClient.Connect(host, port, DataFormat.Json);
                if (channel?.ConnectionStatus == ConnectionStatus.Connected)
                    channel.OnMessageReceived += SocketClient_OnMessageReceived;
            }
            return socketClient.ConnectionStatus == ConnectionStatus.Connected;
        }

        public async Task Send(string sender, JsonObject data)
        {
            data.AddValue(nameof(FixedNames.Sender), sender);
            if (socketClient.ConnectionStatus == ConnectionStatus.Connected)
            {
                await socketClient.Send(Guid.Empty, data);
            }
        }
    }
}
