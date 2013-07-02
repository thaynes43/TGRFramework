using System;

namespace TGRFramework.Prototype.XNAGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (XNAGame game = new XNAGame())
            {
                game.Run();
            }
        }
    }
#endif
}

