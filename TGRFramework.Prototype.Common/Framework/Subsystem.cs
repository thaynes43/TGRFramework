// -----------------------------------------------------------------------
// <copyright file="Subsystem.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using TGRFramework.Prototype.Tools;

    /// <summary>
    /// System component which performs a specific task
    /// </summary>
    public class Subsystem
    {
        #region Fields
        private bool acceptingMessages = false;

        // Hold subsystem thread while running
        private ManualResetEvent subsystemAliveEvent = new ManualResetEvent(false);
        #endregion

        #region Constructors
        public Subsystem(string subsystemLog)
        {
            this.SubsystemLock = new object();

            //lock (this.SubsystemLock)
            //{     
                this.Log = new LogTool(subsystemLog);
                this.MessageQueue = new BlockingCollection<Message>();
            //}
        }
        #endregion

        #region Properties
        /// <summary>
        /// Subsystem specific logger
        /// </summary>
        public ILogTool Log { get; set; }

        /// <summary>
        /// Whomever initializes the subsystem will set this
        /// </summary>
        public bool Initialized { get;set; }

        /// <summary>
        /// Thread safe subsystem
        /// </summary>
        protected object SubsystemLock { get; set; }

        /// <summary>
        /// Synchronous message queue
        /// </summary>
        private BlockingCollection<Message> MessageQueue { get; set; }

        /// <summary>
        /// Message queue thread
        /// </summary>
        private Thread PumpQueueThread { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize subsystem
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Run subsystem
        /// </summary>
        public void Run()
        {
            this.Log.Info("****** Run subsystem. ******");
            this.PumpQueues();
            this.acceptingMessages = true;
            this.subsystemAliveEvent.WaitOne();           
        }

        /// <summary>
        /// Finish active jobs and then stop subsystem - TODO Blocking call?
        /// </summary>
        /// <param name="subsystemShutdown">Signal subsystem stopped</param>
        public virtual void Stop()
        {
            lock (this.SubsystemLock)
            {
                this.Log.Info("****** Stop subsystem. ******");

                // Wait for queue to empty
                this.acceptingMessages = false;
                while (this.MessageQueue.Count > 0)
                {
                    System.Threading.Thread.Sleep(100);
                }

                // Wait until final message is processed
                lock (this.MessageQueue)
                {
                    this.PumpQueueThread.Abort();
                    this.PumpQueueThread.Join();
                    this.PumpQueueThread = null;
                }

                this.MessageQueue.Dispose();

                // Free subsystem thread
                this.subsystemAliveEvent.Set();
            }
        }

        /// <summary>
        /// Add message for subsystem to process
        /// </summary>
        /// <param name="message">Message</param>
        public void AddMessage(Message message)
        {
            lock (this.SubsystemLock)
            {
                if (this.acceptingMessages)
                {
                    this.MessageQueue.Add(message);
                }
                else
                {
                    Log.Error("{0} Message dropped.", message.GetType());
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Process messages sent to subsystem
        /// </summary>
        private void PumpQueues()
        {
            Log.Info("Begin pump queue thread.");
            ManualResetEvent queuesPumping = new ManualResetEvent(false);
            this.PumpQueueThread = new Thread(() =>
            {
                while (true)
                {                 
                    Message message = this.MessageQueue.Take();
                    lock (this.MessageQueue)
                    {
                        try
                        {
                            message.Parent = this;
                            message.Execute();
                        }
                        catch (Exception e)
                        {
                            // TODO_HIGH how do we handle these exceptions?
                            this.Log.Error("Exception when trying to process message of type {0}.\n{1}\n{2}", message.GetType(), e.Message, e.StackTrace);
                        }
                    }
                }
            });

            string[] name= this.GetType().ToString().Split('.');

            this.PumpQueueThread.Name = string.Format("{0}.PumpQueueThread", name[name.Length-1]);
            this.PumpQueueThread.Start();
        }
        #endregion
    }
}
