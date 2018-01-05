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

namespace Acn.Slp.Packets
{
    /// <summary>
    /// Base class for request packets
    /// </summary>
    public abstract class SlpRequestPacket : SlpPacket
    {
        public SlpRequestPacket(SlpFunctionId functionId)
            : base(functionId)
        {
        }

        private List<string> prList = new List<string>();

        /// <summary>
        /// Gets or sets the Previous Responder List.
        /// </summary>
        /// <value>
        /// The PR list.
        /// </value>
        public List<string> PRList
        {
            get { return prList; }
            protected set { prList = value; }
        }

        /// <summary>
        /// Gets or sets the scope list.
        /// </summary>
        /// <value>
        /// The scope list.
        /// </value>
        public string ScopeList { get; set; }

        /// <summary>
        /// Gets or sets the SLP Security Parameter Index.
        /// Used for authentication - current unsupported
        /// </summary>
        /// <value>
        /// The SLP spi.
        /// </value>
        public string SlpSpi { get; set; }
    }
}
