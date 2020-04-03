using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Exceptions
{
    /// <summary>
    /// The root exception used when an error ocurrs within the TCNet.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class TCNetException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TCNetException(string message):base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public TCNetException(string message,Exception ex) : base(message,ex)
        {
        }
    }
}
