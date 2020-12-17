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
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

using LXProtocols.Acn.Slp;

namespace LXProtocols.Acn.Helpers
{
    /// <summary>
    /// This class provides a helper service for software that needs to discover
    /// and track SLP devices.
    /// </summary>
    public class SlpDeviceManager : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlpDeviceManager"/> class.
        /// You should subscribe clients to the <see cref="UnhandledException"/> event after construction.
        /// </summary>
        public SlpDeviceManager()
        {
            FetchAttributes = true;
#if !(MONOANDROID || XAMARIN_IOS)
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
#endif
            pollTimer = new System.Threading.Timer(new System.Threading.TimerCallback(pollTimerTick), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            CreateAgents();
        }

        /// <summary>
        /// Called when the network IP changes or a network is connected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessAddressChanged), null);
        }

        /// <summary>
        /// Handles re-connect when the network IP address changes.
        /// </summary>
        /// <param name="state">not used</param>
        private void ProcessAddressChanged(object state)
        {
            try
            {
                //Wait for the network adapter to become available.
                Thread.Sleep(6000);
                
                //Need to create a listener on the new interface or remove one from the old interface.
                RefreshAgents();              
            }
            catch (Exception ex)
            {
                if (!RaiseUnhandledException(ex))
                    throw;
            }   
        }

        #region Control properties

        private string scope = "DEFAULT";

        /// <summary>
        /// Gets or sets the SLP scope.
        /// </summary>
        /// <remarks>Defaults to DEFAULT</remarks>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope
        {
            get { return scope; }
            set
            {
                if (scope != value)
                {
                    scope = value;
                    foreach (SlpAgent agent in agents.Keys)
                    {
                        agent.Scope = value;
                    }
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
                    lock (agents)
                    {
                        if (agents.Keys.Any(a => a.Active))
                        {
                            throw new InvalidOperationException("SlpDeviceManager does not support changing the ServiceType after discovery has started");
                        }
                    }
                    serviceType = value;
                }
            }
        }


        private TimeSpan pollInterval = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Gets or sets the poll interval.
        /// This is how often the network will be polled for devices.
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

        /// <summary>
        /// Whether the well-known SLP port should be bound to receive SLP datagrams; this is usually
        /// used to join SLP multicast groups. Note that the well-known port is normally port 427,
        /// which requires elevated privileges to bind in some environments.
        ///
        /// Takes effect when the SLP sockets are next opened, either during a call to
        /// <see cref="Start" /> or <see cref="RefreshAgents" />.
        /// </summary>
        public bool OpenWellKnownPort { get; set; } = true;

        #endregion

        #region Public interface


        /// <summary>
        /// Starts polling.
        /// </summary>
        public void Start()
        {
            Running = true;

            lock (agents)
            {
                foreach (var agent in agents.Keys)
                    agent.Open(OpenWellKnownPort);
            }

            UpdateDevices();
            RequestPollCallback();
        }

        /// <summary>
        /// Starts an update immediately.
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
            lock (agents)
            {
                foreach (var agent in agents.Keys)
                {
                    agent.Close();
                }
            }
            StopPollTimer();
        }

        /// <summary>
        /// Creates new agents for each network interface and opens them again.
        /// </summary>
        public void RefreshAgents()
        {
            lock (agents)
            {
                CreateAgents();
                if (Running)
                {
                    //If we are running start the agents.
                    foreach (var agent in agents.Keys)
                        agent.Open(OpenWellKnownPort);
                }
            }
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
        /// Occurs when an unhandled exception has occurred.
        /// </summary>
        /// <remarks>
        /// Should be logged by user code.
        /// </remarks>
        public event UnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// Called to indicate an unhandled exception has occurred.
        /// </summary>
        /// <param name="ex">The exception that has not been handled.</param>
        protected virtual bool RaiseUnhandledException(Exception ex)
        {
            if (UnhandledException != null)
            {
                UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
                return true;
            }

            return false;
        }

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

        #region Implementation

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
            lock (devicesLock)
            {
                lock (attributeRequestLog)
                {
                    lock (agents)
                    {
                        // If we have any agents get rid of them 
                        foreach (SlpUserAgent agent in agents.Keys)
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
                        foreach (var localAddress in GetAllIpAddresses())
                        {
                            SlpUserAgent agent = new SlpUserAgent();
                            agent.NetworkAdapter = localAddress;
                            agent.Scope = Scope;
                            agent.ServiceFound += agent_ServiceFound;
                            agent.AttributeReply += agent_AttributeReply;
                            agents.Add(agent, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the available IP addresses.
        /// </summary>
        /// <returns>An Enumerable of IP addresses</returns>
        private static IEnumerable<System.Net.IPAddress> GetAllIpAddresses()
        {
#if MONOANDROID
            return Java.Net.NetworkInterface.NetworkInterfaces.
                Cast<Java.Net.NetworkInterface>().
                Where(i => i.IsUp && !i.IsLoopback).
                SelectMany(i => i.InetAddresses.OfType<Java.Net.Inet4Address>()).
                Select(a => a.ToIPAddress());
#else
            return NetworkInterface.GetAllNetworkInterfaces()
				// On IOS all the interface status are marked a Unknown
					.Where(i => i.OperationalStatus == OperationalStatus.Up || i.OperationalStatus == OperationalStatus.Unknown)
                                        .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                                        .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                        .Select(a => a.Address);
#endif
        }

        /// <summary>
        /// Called when the poll timer ticks
        /// </summary>
        /// <param name="state">The state.</param>
        private void pollTimerTick(object state)
        {
            try
            {
                if (Running)
                {
                    UpdateDevices();
                    RequestPollCallback();
                }
            }            
            catch (Exception ex)
            {
                RaiseUnhandledException(ex);
            }  
        }

        // Random source designed to stop lots of nodes on a network 
        // polling in lock-step if they start at the same time.
        // The seed is generated from a GUID which should be to some extent hardware dependent.
        private Random randomSource = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Requests a poll callback after the poll interval.
        /// </summary>
        private void RequestPollCallback()
        {
            pollTimer.Change(PollInterval + TimeSpan.FromMilliseconds(randomSource.Next(1000)), TimeSpan.FromMilliseconds(-1));
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
                //Search through all devices that have not responded recently.
                foreach (var device in devices.Values.Where(d => (d.DiscoveryAgents.Count == 0 || d.LastUpdateId < agents[d.DiscoveryAgents.First()])))
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

            lock (agents)
            {
                foreach (var agent in agents.Keys.ToList())
                {
                    if (agent.IsOpen())
                    {
                        agents[agent] = agent.Find(ServiceType);
                    }
                }
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

                    device.Endpoint = e.Address;
                    device.DiscoveryAgents.Add(sender as SlpUserAgent);
                    device.UpdateRecieved(e.RequestId);
                }

                RequestAttributes(device);
                if (newDevice)
                {
                    OnDeviceStateChange(device);
                }
            }
            OnDevicesUpdated();
        }

        /// <summary>
        /// Struct to hold a composite key for a alp attribute request.
        /// Holds the agent and the request id
        /// </summary>
        private struct UserAgentRequest
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UserAgentRequest"/> struct.
            /// </summary>
            /// <param name="agent">The agent.</param>
            /// <param name="request">The request.</param>
            public UserAgentRequest(SlpUserAgent agent, int request)
            {
                Agent = agent;
                RequestId = request;
            }

            SlpUserAgent Agent;
            int RequestId;

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj is UserAgentRequest)
                {
                    UserAgentRequest request = (UserAgentRequest)obj;
                    return request.Agent == Agent && request.RequestId == RequestId;
                }
                return base.Equals(obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return Agent.GetHashCode() ^ RequestId;
            }
        }

        /// <summary>
        /// Log of attribute request ids to the devices
        /// </summary>
        private Dictionary<UserAgentRequest, SlpDeviceInformation> attributeRequestLog = new Dictionary<UserAgentRequest, SlpDeviceInformation>();

        /// <summary>
        /// Requests the attributes for a device.
        /// </summary>
        /// <param name="device">The device.</param>
        private void RequestAttributes(SlpDeviceInformation device)
        {
            if (FetchAttributes && device.DiscoveryAgents.Count > 0)
            {
                SlpUserAgent agent = device.DiscoveryAgents.First();
                int requestId = agent.RequestAttributes(device.Endpoint, device.Url);
                lock (attributeRequestLog)
                {
                    attributeRequestLog[new UserAgentRequest(agent, requestId)] = device;
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
                UserAgentRequest requestKey = new UserAgentRequest(sender as SlpUserAgent, e.RequestId);
                if (attributeRequestLog.TryGetValue(requestKey, out device))
                {
                    device.Attributes = e.Attributes;
                    device.LastContact = DateTime.Now;
                    attributeRequestLog.Remove(requestKey);
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

    #if MONOANDROID

    /// <summary>
    /// Extension class to improve Android Java bindings
    /// </summary>
    public static class AndroidExtensions
    {
        /// <summary>
        /// Cast a IEnumeration to an IEnumerable<typeparamref name="T" />. Will throw an InvalidCastException if any element in the enumeration cannot be cast.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>An enumerable with elements of the specified type.</returns>
        /// <exception cref="System.InvalidCastException">If one of the elements cannot be cast.</exception>
        public static IEnumerable<T> Cast<T>(this Java.Util.IEnumeration enumeration)
            where T : Java.Lang.Object
        {
            while (enumeration.HasMoreElements)
            {
                yield return (T)enumeration.NextElement();
            }
        }

        /// <summary>
        /// Return an IEnumerable<typeparamref name="T"/> with all the elements in the IEnumeration which match the required type, other elements are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>An enumerable with elements that match the specified type.</returns>
        public static IEnumerable<T> OfType<T>(this Java.Util.IEnumeration enumeration)
            where T : Java.Lang.Object
        {
            while (enumeration.HasMoreElements)
            {
                Java.Lang.Object element = enumeration.NextElement();
                if (element is T)
                    yield return (T)element;
            }
        }

        /// <summary>
        /// Convert Java InetAddress to .Net IPAddress.
        /// </summary>
        /// <param name="inetAddress">The InetAddress object.</param>
        /// <returns>The equivalent .Net IPAddress object.</returns>
        /// <exception cref="System.ArgumentException">Failed to parse InetAddress: ...</exception>
        public static System.Net.IPAddress ToIPAddress(this Java.Net.InetAddress inetAddress)
        {
            if (inetAddress == null)
                return null;

            // Use get the bytes and create an IP address from that
            if (inetAddress.GetAddress() != null && inetAddress.GetAddress().Length > 0)
                return new System.Net.IPAddress(inetAddress.GetAddress());

            // Use the host name string which should be in the standard "1.2.3.4" format
            System.Net.IPAddress ipAddress;
            if (System.Net.IPAddress.TryParse(inetAddress.HostAddress, out ipAddress))
                return ipAddress;

            // In Android 7 (Nougat) both of the above return null for IPv4 addresses however ToString still appears to work

            // The IP address should be the bit following the slash
            string str = inetAddress.ToString().Split('/').LastOrDefault();
            if (System.Net.IPAddress.TryParse(str, out ipAddress))
                return ipAddress;

            // Don't know what else to try
            throw new ArgumentException(string.Format("Failed to parse InetAddress: {0}", inetAddress.ToString()));
        }
    }

    #endif
}
