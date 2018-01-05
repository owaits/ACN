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

namespace Acn.Helpers
{
    /// <summary>
    /// Event args for an Slp Device Manager event
    /// </summary>
    public class SlpDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public SlpDeviceInformation Device { get; set; }
    }

    /// <summary>
    ///  Event args for an Slp Device Manager update event
    /// </summary>
    public class SlpUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new list of devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        public IEnumerable<SlpDeviceInformation> Devices { get; set; }

        /// <summary>
        /// Helper to get a list of devices filtered by state
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public IEnumerable<SlpDeviceInformation> GetDevices(SlpDeviceState state)
        {
            return Devices.Where(d => d.State == state);
        }
    }
}
