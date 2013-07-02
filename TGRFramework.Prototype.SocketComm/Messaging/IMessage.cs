using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGRFramework.Prototype.SocketComm
{
    /// <summary>
    /// Message object to travel across TCP socket
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// String representation of message which will be sent over socket
        /// </summary>
        string ToMessage();

        /// <summary>
        /// String representation of message detailing parameters
        /// </summary>
        string ToString();
    }
}
