using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGRFramework.Prototype.SocketComm
{
    /// <summary>
    /// TCP socket communication
    /// </summary>
    public interface ISocket
    {
        void Send(IMessage message);

        void RegisterMessage(Type msgType, MessageReceivedHandler handler);
    }
}
