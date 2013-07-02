using System;

namespace TGRFramework.Prototype.SocketComm
{
    /// <summary>
    /// Manage sending and receiving message objects
    /// </summary>
    public interface IMessageProxyProtocol
    {
        #region Proxy        
        /// <summary>
        /// Register message callback
        /// </summary>
        /// <param name="msgType">Message object type</param>
        /// <param name="handler">Message callback</param>
        void RegisterMessage(Type msgType, MessageReceivedHandler handler);

        /// <summary>
        /// Handle messaged received by socket
        /// </summary>
        /// <param name="msg">message received</param>
        void OnReceiveMessage(IMessage message);
        #endregion

        #region Protocol
        /// <summary>
        /// Serialize message into format usable for socket communication
        /// </summary>
        /// <param name="message">message object</param>
        /// <returns>message in string form to be sent over socket as byte stream</returns>
        string Serialize(IMessage message);

        /// <summary>
        /// Deserialize message into object format
        /// </summary>
        /// <param name="message">message from byte stream as string</param>
        /// <returns>message object</returns>
        IMessage Deserialize(string message);
        #endregion
    }
}