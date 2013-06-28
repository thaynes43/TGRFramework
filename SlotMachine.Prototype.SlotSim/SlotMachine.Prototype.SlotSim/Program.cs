using System;

namespace SlotMachine.Prototype.SlotSim
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "SlotSim.config");

            using (SlotSim game = new SlotSim())
            {
                game.Run();
            }
        }
    }
#endif
}

