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

namespace Acn.Helpers
{
    /// <summary>
    /// Possible states of a device
    /// </summary>
    public enum SlpDeviceState
    {
        /// <summary>
        /// Newly seen
        /// </summary>
        New,
        /// <summary>
        /// Missed one poll event
        /// </summary>
        MissedPoll,
        /// <summary>
        /// Seen once but missed many poll events
        /// </summary>
        Disappeared,
        /// <summary>
        /// Recently come back to active having missed polls
        /// </summary>
        ReAppeared,
        /// <summary>
        /// Device is responding
        /// </summary>
        Ok
    }

    /// <summary>
    /// Information about an slp device
    /// </summary>
    public class SlpDeviceInformation
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the time of the last contact.
        /// </summary>
        /// <value>
        /// The last contact.
        /// </value>
        public DateTime LastContact { get; set; }

        /// <summary>
        /// Gets or sets the last update id.
        /// </summary>
        /// <value>
        /// The last update id.
        /// </value>
        internal int LastUpdateId { get; set; }

        /// <summary>
        /// Gets or sets the number of missed updates.
        /// </summary>
        /// <value>
        /// The missed updates.
        /// </value>
        internal int MissedUpdates { get; set; }

        /// <summary>
        /// Gets or sets the id of the first update that we saw this device in
        /// </summary>
        /// <value>
        /// The first update id.
        /// </value>
        internal int FirstUpdateId { get; set; }

        private HashSet<SlpUserAgent> discoveryAgents = new HashSet<SlpUserAgent>();

        /// <summary>
        /// Gets or sets the discovery agents that have reported this device.
        /// This will almost always be only one but it's possible to get to a 
        /// remote device on more than one adaptor.
        /// </summary>
        /// <value>
        /// The discovery agents.
        /// </value>
        internal HashSet<SlpUserAgent> DiscoveryAgents
        {
            get { return discoveryAgents; }
            private set
            {
                if (discoveryAgents != value)
                {
                    discoveryAgents = value;
                }
            }
        }

        /// <summary>
        /// Called when an update is recived.
        /// </summary>
        /// <param name="updateId">The update id.</param>
        internal void UpdateRecieved(int updateId)
        {
            LastUpdateId = updateId;
            MissedUpdates = 0;
            LastContact = DateTime.Now;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public SlpDeviceState State
        {
            get
            {
                if (LastUpdateId == FirstUpdateId)
                {
                    return SlpDeviceState.New;
                }
                if (MissedUpdates == 1)
                {
                    return SlpDeviceState.MissedPoll;
                }
                if (MissedUpdates > 1)
                {
                    return SlpDeviceState.Disappeared;
                }
                return SlpDeviceState.Ok;
            }
        }


        /// <summary>
        /// Gets or sets the remote endpoint address of this device.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        internal System.Net.IPEndPoint Endpoint { get; set; }
    }
}
