using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGRFramework.Prototype.Tools;

namespace TGRFramework.Prototype.SocketComm
{
    public delegate void MessageReceivedHandler(IMessage message);

    /// <inheritdoc />
    public class MessageProxyProtocol : IMessageProxyProtocol
    {
        #region Fields     
        private object messageLock = new object();
        #endregion

        #region Constructors    
        public MessageProxyProtocol(ILogTool log = null)
        {
            this.Log = log;
            this.MessageCollection = new Dictionary<Type, MessageReceivedHandler>();
            this.MessageQueue = new BlockingCollection<IMessage>();
            this.BeginProcessing();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Optional logger
        /// </summary>
        private ILogTool Log {  get; set; }

        /// <summary>
        /// Thread safe received message queue
        /// </summary>
        private BlockingCollection<IMessage> MessageQueue { get; set; } 

        /// <summary>
        /// Message received callbacks
        /// </summary>
        private Dictionary<Type, MessageReceivedHandler> MessageCollection { get; set; }
        #endregion

        #region Public Methods
        /// <inheritdoc />
        public void RegisterMessage(Type msgType, MessageReceivedHandler handler)
        {
            lock (this.messageLock)
            {
                if (this.Log != null)
                {
                    this.Log.Info("Comm protocol registering message of type {0}", msgType);
                }

                if (!this.MessageCollection.ContainsKey(msgType))
                {
                    this.MessageCollection.Add(msgType, handler);
                }
                else
                {
                    if (this.Log != null)
                    {
                        this.Log.Warn("{0} previously registered. Will overwrite with new handler.", msgType);
                    }
                    this.MessageCollection[msgType] = handler;
                }
            }
        }

        /// <inheritdoc />
        public void OnReceiveMessage(IMessage msg)
        {
            // Thread safe message processing
            lock (this.messageLock)
            {
                this.MessageQueue.Add(msg);
            }
        }

        /// <inheritdoc />
        public string Serialize(IMessage message)
        {
            return message.ToMessage();
        }

        /// <inheritdoc />
        public IMessage Deserialize(string message)
        {
            lock (this.messageLock)
            {
                message = message.Trim('#');
                string[] elements = message.Split(' ');
                return Message.CreateMessage(elements);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Synchronously process messages from queue
        /// </summary>
        private void BeginProcessing()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    IMessage message = this.MessageQueue.Take();
                    try
                    {
                        this.MessageCollection[message.GetType()](message);
                    }
                    catch (Exception e)
                    {
                        if (this.Log != null)
                        {
                            this.Log.Error("Exception when trying to process message of type {0}.\n{1}\n{2}", message.GetType(), e.Message, e.StackTrace);
                        }                       
                    }
                }
            });
        }
        #endregion
    }
}