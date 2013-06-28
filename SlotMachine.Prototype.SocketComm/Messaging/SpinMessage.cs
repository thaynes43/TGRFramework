using System.Text;

namespace SlotMachine.Prototype.SocketComm
{
    /// <summary>
    /// Message specific to machine scoring
    /// </summary>
    public class SpinMessage : Message
    {
        public const string MessageName = "SPIN_MESSAGE";

        public SpinMessage(int spinScore)
        {
            this.SpinScore = spinScore;
        }

        public enum Fields
        {
            SpinScore = 1,
            NumberFields
        }

        /// <summary>
        /// Not sure what this will be yet
        /// </summary>
        public int SpinScore { get; set; }

        /// <summary>
        /// Create new SpinMessage from parameter array
        /// </summary>
        public static new IMessage CreateMessage(string[] elements)
        {
            SpinMessage msg = null;
            try
            {
                if (elements.Length == (int)Fields.NumberFields)
                {
                    int spinScore = int.Parse(elements[(int)Fields.SpinScore]);
                    msg = new SpinMessage(spinScore);
                }
            }
            catch
            {
                msg = null;
            }
            return msg;
        }

        /// <inheritdoc />
        public override string ToMessage()
        {
            StringBuilder sb = new StringBuilder(SpinMessage.MessageName);
            sb.AppendFormat(" {0}", this.SpinScore);
            sb.Append("#");
            return sb.ToString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[" + SpinMessage.MessageName);
            sb.AppendFormat(" SpinScore = {0}", this.SpinScore);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
