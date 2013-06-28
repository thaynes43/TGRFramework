namespace SlotMachine.Prototype.SocketComm
{
    /// <inheritdoc />
    public abstract class Message : IMessage
    {
        /// <summary>
        /// Initialize a new Message with no parameters
        /// </summary>
        public Message()
        {
        }

        /// <summary>
        /// Create message from elements received over socket
        /// </summary>
        /// <param name="elements">string array split from data received over socket</param>
        /// <returns>Message</returns>
        public static IMessage CreateMessage(string[] elements)
        {
            IMessage message = null;

            switch (elements[0])
            {
                case SpinMessage.MessageName:
                message = SpinMessage.CreateMessage(elements);
                break;
            }

            return message;
        }
        
        /// <inheritdoc/>
        public virtual string ToMessage()
        {
            return string.Empty;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
