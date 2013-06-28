using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlotMachine.Prototype.Tools;

namespace SlotMachine.Prototype.SocketComm
{
    /// <summary>
    /// TCP Client Socket
    /// </summary>
    public class SocketWrapper : ISocket
    {
        #region Fields
        private const int BufferSize = 1024;
        private object socketLock = new object();
        private ManualResetEvent connectComplete = new ManualResetEvent(false);
        private ManualResetEvent sendComplete = new ManualResetEvent(false);
        private ManualResetEvent receiveComplete = new ManualResetEvent(false);
        #endregion 

        #region Constructors
        /// <summary>
        /// Client socket constructor
        /// </summary>
        /// <param name="commLogName">Specify log file name</param>
        public SocketWrapper(string commLogName = null)
        {
            this.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (!string.IsNullOrEmpty(commLogName))
            {
                this.Log = new LogTool(commLogName);
            }
            else
            {
                this.Log = null;
            }

            this.Initialize(true);
        }

        /// <summary>
        /// Server socket constructor
        /// </summary>
        /// <param name="socket">Passed on connection to client</param>
        /// <param name="commLogName">Specify log file name<</param>
        public SocketWrapper(Socket socket, ILogTool commLog = null)
        {
            this.Log = commLog;
            this.ClientSocket = socket;
            this.Initialize(false);
            this.BeginReceive();
        }
        #endregion

        #region Properties
        /// <summary>
        /// TCP Socket
        /// </summary>
        public Socket ClientSocket { get; set; }

        /// <summary>
        /// Builder for message received
        /// </summary>
        public StringBuilder SocketStringBuilder { get; set; }

        /// <summary>
        /// Is socket connected
        /// </summary>
        private bool ConnectedToServer { get; set; }

        /// <summary>
        /// Is socket in message receiving loop
        /// </summary>
        private bool ReceivingMessages { get; set; }

        /// <summary>
        /// Byte buffer coming and going from socket 
        /// </summary>
        private byte[] SocketBuffer { get; set; }

        /// <summary>
        /// Log tool for all communication using this socket
        /// </summary>
        private ILogTool Log { get; set; }

        /// <summary>
        /// Manage message objects
        /// </summary>
        private IMessageProxyProtocol MessageProtocol { get; set; }
        #endregion

        #region Public Methods
        /// <ineritdoc />
        public void Send(IMessage message)
        {
            lock (this.socketLock)
            {
                if (this.ConnectedToServer)
                {
                    if (this.Log != null)
                    {
                        this.Log.Info("Send : {0}", message.ToString());
                    }

                    this.sendComplete.Reset();
                    byte[] byteData = Encoding.ASCII.GetBytes(this.MessageProtocol.Serialize(message));
                    this.ClientSocket.BeginSend(byteData, 0, byteData.Length, 0, this.SendCallback, this);
                    this.sendComplete.WaitOne();
                }
                else
                {
                    // TODO_MEDIUM Investigate if we want to queue messages if connection is down
                    if (this.Log != null)
                    {
                        this.Log.Error("Cannot send {0}, not connected to server.", message.GetType());
                    }
                }
            }
        }

        /// <inheritdoc />
        public void RegisterMessage(Type msgType, MessageReceivedHandler handler)
        {
            lock (this.socketLock)
            {
                this.MessageProtocol.RegisterMessage(msgType, handler);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize socket
        /// </summary>
        private void Initialize(bool connectToServer)
        {
            this.ReceivingMessages = false;
            this.SocketStringBuilder = new StringBuilder();
            this.SocketBuffer = new byte[SocketWrapper.BufferSize];
            this.MessageProtocol = new MessageProxyProtocol(this.Log);

            if (connectToServer)
            {
                this.ConnectedToServer = false;
                this.BeginConnectToServer();
            }
            else
            {
                this.ConnectedToServer = true;
            }
        }

        /// <summary>
        /// Wait for a connection to the server to be established
        /// </summary>
        private void BeginConnectToServer()
        {
            Task.Factory.StartNew(() =>
            {
                while (!ConnectedToServer)
                {
                    this.connectComplete.Reset();
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddress = null;
                    ipHostInfo.AddressList.ToList().ForEach((ip) => 
                    { 
                        if (ip.AddressFamily == AddressFamily.InterNetwork) 
                        { 
                            ipAddress = ip; 
                        }
                    });

                    if (ipAddress == null)
                    {
                        throw new ArgumentException("Could not resolve internetwork IP address from {0}", Dns.GetHostName());
                    }

                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

                    // Attempt to establish a connection
                    this.ClientSocket.BeginConnect(localEndPoint, new AsyncCallback(OnConnect), this);
                    this.connectComplete.WaitOne();

                    // Connection succeeded - begin listening for messages over the socket
                    if (this.ConnectedToServer)
                    {
                        this.BeginReceive();
                    }
                    
                    System.Threading.Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// Begin socket communication
        /// </summary>
        private void BeginReceive()
        {
            if (this.ConnectedToServer)
            {
                if (!this.ReceivingMessages)
                {
                    Task.Factory.StartNew(() =>
                    {
                        this.ReceivingMessages = true;
                        while (ReceivingMessages)
                        {
                            this.receiveComplete.Reset();
                            this.ClientSocket.BeginReceive(this.SocketBuffer, 0, SocketWrapper.BufferSize, 0, this.ReceiveCallback, this);
                            this.receiveComplete.WaitOne();
                        }
                    });
                }
            }
            else
            {
                if (this.Log != null)
                {
                    this.Log.Error("Cannot begin receiving messaged. Not connected to server.");
                }
            }        
        }

        #region Callbacks    
        /// <summary>
        /// On connect to server
        /// </summary>
        /// <param name="ar">connection result</param>
        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                SocketWrapper client = (SocketWrapper)ar.AsyncState;
                client.ClientSocket.EndConnect(ar);
                this.connectComplete.Set();
                this.ConnectedToServer = true;

                if (this.Log != null)
                {
                    this.Log.Info("********** Connected to Server **********");
                }
            }
            catch (Exception e)
            {
                if (this.Log != null)
                {
                    this.Log.Error("{0}\n{1}", e.Message, e.StackTrace);
                }
                this.connectComplete.Set();
            }
        }

        /// <summary>
        /// On message sent over socket
        /// </summary>
        /// <param name="ar">send result</param>
        private void SendCallback(IAsyncResult ar)
        {
            this.sendComplete.Set();
        }

        /// <summary>
        /// On message received over socket
        /// </summary>
        /// <param name="ar">message received result</param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            SocketWrapper client = (SocketWrapper)ar.AsyncState;
            int bytesRead = client.ClientSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                client.SocketStringBuilder.Append(Encoding.ASCII.GetString(client.SocketBuffer, 0, bytesRead));
                if (client.SocketStringBuilder.ToString().IndexOf("#") > -1)
                {
                    string response = client.SocketStringBuilder.ToString();
                    client.SocketStringBuilder = new StringBuilder();

                    IMessage msg = this.MessageProtocol.Deserialize(response);
                    if (this.Log != null)
                    {
                        this.Log.Info("Receive : {0}", msg.ToString());
                    }
                    this.MessageProtocol.OnReceiveMessage(msg);
                }
                else
                {
                    this.ClientSocket.BeginReceive(this.SocketBuffer, 0, SocketWrapper.BufferSize, 0, this.ReceiveCallback, this);
                }             
            }
            this.receiveComplete.Set();
        }
        #endregion
        #endregion
    }
}
