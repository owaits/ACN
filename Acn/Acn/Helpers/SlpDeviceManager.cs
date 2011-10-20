#region Copyright © 2011 Mark Daniel
//______________________________________________________________________________________________________________
// Service Location Protocol
// Copyright © 2011 Mark Daniel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Slp;
using System.Net.NetworkInformation;

namespace Acn.Helpers
{
    /// <summary>
    /// This class provides a helper service for software that needs to discover
    /// and track SLP devices.
    /// </summary>
    public class SlpDeviceManager : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlpDeviceManager"/> class.
        /// </summary>
        public SlpDeviceManager()
        {
            FetchAttributes = true;

            pollTimer = new System.Threading.Timer(new System.Threading.TimerCallback(pollTimerTick), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            CreateAgents();
        }


        #region Control properties

        /// <summary>
        /// Gets or sets the SLP scope.
        /// </summary>
        /// <remarks>Defaults to DEFAULT</remarks>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope
        {
            get { return agents.Keys.First().Scope; }
            set
            {
                foreach (SlpAgent agent in agents.Keys)
                {
                    agent.Scope = value;
                }
            }
        }

        private string serviceType;

        /// <summary>
        /// Gets or sets the type of the service the device manager is to look for.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public string ServiceType
        {
            get { return serviceType; }
            set
            {
                if (serviceType != value)
                {
                    if (agents.Keys.Any(a => a.Active))
                    {
                        throw new InvalidOperationException("SlpDeviceManager does not support changing the ServiceType after discovery has started");
                    }
                    serviceType = value;
                }
            }
        }


        private TimeSpan pollInterval = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Gets or sets the poll interval.
        /// This is how oftern the network will be polled for devices.
        /// Defaults to 15 seconds.
        /// </summary>
        /// <value>
        /// The poll interval.
        /// </value>
        public TimeSpan PollInterval
        {
            get { return pollInterval; }
            set
            {
                if (pollInterval != value)
                {
                    pollInterval = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to attempt to fetch attributes.
        /// For services that are discovered.
        /// </summary>
        /// <value>
        ///   <c>true</c> to fetch attributes; otherwise, <c>false</c>.
        /// </value>
        public bool FetchAttributes { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SlpDeviceManager"/> is running (viz Polling).
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        public bool Running { get; private set; }

        #endregion

        #region Public interface


        /// <summary>
        /// Starts polling.
        /// </summary>
        public void Start()
        {
            Running = true;
            foreach (var agent in agents.Keys)
            {
                agent.Open();
            }
            UpdateDevices();
            RequestPollCallback();

        }

        /// <summary>
        /// Starts an update imediately.
        /// </summary>
        public void Update()
        {
            UpdateDevices();
        }


        /// <summary>
        /// Stops polling.
        /// </summary>
        public void Stop()
        {
            foreach (var agent in agents.Keys)
            {
                agent.Close();
            }
            StopPollTimer();
        }


        /// <summary>
        /// Gets the devices that have been discovered.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SlpDeviceInformation> GetDevices()
        {
            lock (devicesLock)
            {
                return devices.Values.ToList();
            }
        }

        #endregion

        #region Public events

        /// <summary>
        /// Called whenever a device or the devices list is updated.
        /// Can be called quite frequently
        /// </summary>
        public event EventHandler DeviceUpdated;

        /// <summary>
        /// Called whenever the devices list is updated.
        /// </summary>
        protected virtual void OnDevicesUpdated()
        {
            if (DeviceUpdated != null)
            {
                DeviceUpdated(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// Occurs when a device changes state.
        /// </summary>
        public event EventHandler<SlpDeviceEventArgs> DeviceStateChange;

        /// <summary>
        /// Called when a device state changes.
        /// </summary>
        /// <param name="device">The device.</param>
        protected virtual void OnDeviceStateChange(SlpDeviceInformation device)
        {
            if (DeviceStateChange != null)
            {
                DeviceStateChange(this, new SlpDeviceEventArgs() { Device = device });
            }
        }

        /// <summary>
        /// Occurs when a periodic update has completed.
        /// </summary>
        public event EventHandler<SlpUpdateEventArgs> UpdateComplete;

        /// <summary>
        /// Called when the device update has completed.
        /// </summary>
        protected virtual void OnDeviceUpdateComplete()
        {
            if (UpdateComplete != null)
            {
                IEnumerable<SlpDeviceInformation> deviceList;
                lock (devicesLock)
                {
                    deviceList = devices.Values.ToList();
                }
                UpdateComplete(this, new SlpUpdateEventArgs() { Devices = deviceList });
            }
        }

        #endregion

        #region Implimentation

        /// <summary>
        /// Timer for polling
        /// </summary>
        private System.Threading.Timer pollTimer;

        /// <summary>
        /// SlpAgent, to do the discovery
        /// </summary>
        private Dictionary<SlpUserAgent, int> agents = new Dictionary<SlpUserAgent, int>();

        /// <summary>
        /// Dictionary of devices, indexed by URL
        /// </summary>
        private Dictionary<string, SlpDeviceInformation> devices = new Dictionary<string, SlpDeviceInformation>();

        /// <summary>
        /// Lock object for the dictionary
        /// </summary>
        private object devicesLock = new object();

        /// <summary>
        /// Creates the SLP user agents.
        /// We need one for each network adaptor
        /// </summary>
        private void CreateAgents()
        {
            lock(devicesLock)
            {
                lock(attributeRequestLog)
                {
                    // If we have any agents get rid of them 
                    foreach(SlpUserAgent agent in agents.Keys)
                    {
                        agent.ServiceFound -= agent_ServiceFound;
                        agent.AttributeReply -= agent_AttributeReply;
                        agent.Dispose();
                    }
                    agents.Clear();

                    // We also need to clear them from the devices
                    foreach (SlpDeviceInformation device in devices.Values)
                    {
                        device.DiscoveryAgents.Clear();
                    }

                    // Now create a new agent for each adaptor
                    foreach (var localAddress in NetworkInterface.GetAllNetworkInterfaces()
                        .Where(i => i.OperationalStatus == OperationalStatus.Up)
                        .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                        .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
                    {
                        SlpUserAgent agent = new SlpUserAgent();
                        agent.NetworkAdapter = localAddress.Address;
                        agent.ServiceFound += agent_ServiceFound;
                        agent.AttributeReply += agent_AttributeReply;
                        agents.Add(agent, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Called wehn the poll timer ticks
        /// </summary>
        /// <param name="state">The state.</param>
        private void pollTimerTick(object state)
        {
            if (Running)
            {
                UpdateDevices();
                RequestPollCallback();
            }
        }

        /// <summary>
        /// Requests a poll callback after the poll interval.
        /// </summary>
        private void RequestPollCallback()
        {
            pollTimer.Change(PollInterval, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Stops the poll timer.
        /// </summary>
        private void StopPollTimer()
        {
            Running = false;
            pollTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Updates the devices.
        /// </summary>
        private void UpdateDevices()
        {
            // Look for devices that haven't responded 
            lock (devicesLock)
            {
                foreach (var device in devices.Values.Where(d => d.LastUpdateId < agents[d.DiscoveryAgents.First()]))
                {
                    device.MissedUpdates++;
                }
            }
            // Send the update event
            OnDeviceUpdateComplete();

            // Start a new update cycle
            lock (attributeRequestLog)
            {
                attributeRequestLog.Clear();
            }

            foreach (var agent in agents.Keys.ToList())
            {
                agents[agent] = agent.Find(ServiceType);
            }
        }




        /// <summary>
        /// Handles the ServiceFound event of the agent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Acn.Slp.ServiceFoundEventArgs"/> instance containing the event data.</param>
        void agent_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            foreach (UrlEntry url in e.Urls)
            {
                bool newDevice = false;
                SlpDeviceInformation device;
                lock (devicesLock)
                {
                    if (!devices.TryGetValue(url.Url, out device))
                    {
                        device = new SlpDeviceInformation() { Url = url.Url, FirstUpdateId = e.RequestId };
                        devices[url.Url] = device;
                    }
                }
                device.Endpoint = e.Address;
                device.DiscoveryAgents.Add(sender as SlpUserAgent);
                device.UpdateRecieved(e.RequestId);
                RequestAttributes(device);
                if (newDevice)
                {
                    OnDeviceStateChange(device);
                }
            }
            OnDevicesUpdated();
        }

        /// <summary>
        /// Log of attribute request ids to the devices
        /// </summary>
        private Dictionary<int, SlpDeviceInformation> attributeRequestLog = new Dictionary<int, SlpDeviceInformation>();

        /// <summary>
        /// Requests the attributes for a device.
        /// </summary>
        /// <param name="device">The device.</param>
        private void RequestAttributes(SlpDeviceInformation device)
        {
            if (FetchAttributes && device.DiscoveryAgents.Count > 0)
            {
                int requestId = device.DiscoveryAgents.First().RequestAttributes(device.Endpoint, device.Url);
                lock (attributeRequestLog)
                {
                    attributeRequestLog[requestId] = device;
                }
            }
        }

        /// <summary>
        /// Handles the AttributeReply event of the agent.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Acn.Slp.AttributeReplyEventArgs"/> instance containing the event data.</param>
        void agent_AttributeReply(object sender, AttributeReplyEventArgs e)
        {
            SlpDeviceInformation device;
            lock (attributeRequestLog)
            {
                if (attributeRequestLog.TryGetValue(e.RequestId, out device))
                {
                    device.Attributes = e.Attributes;
                    device.LastContact = DateTime.Now;
                    attributeRequestLog.Remove(e.RequestId);
                    OnDevicesUpdated();
                }
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
            foreach (var agent in agents.Keys)
            {
                agent.ServiceFound -= agent_ServiceFound;
                agent.AttributeReply -= agent_AttributeReply;
                agent.Dispose();
            }
            agents.Clear();
        }

        #endregion

    }
}
