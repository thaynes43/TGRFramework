// -----------------------------------------------------------------------
// <copyright file="SlotMachine.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using TGRFramework.Prototype.Common;

    /// <summary>
    /// SlotSim Game
    /// </summary>
    public class MachineSimulator : StoryBoard
    {
        public MachineSimulator(IGameCompleteDelegate<IGame> gameComplete, GraphicsDeviceManager graphics, ContentManager content, string logName)
            : base(gameComplete, graphics, content, logName)
        {
            this.ActiveGame = this.FindOrCreateSubsystem(typeof(SplashScreen)) as SplashScreen;
        }

        /// <inheritdoc />
        protected override Subsystem CreateSubsystem(Type type)
        {
            Log.Info("  Creating {0}.", type);

            Subsystem newSubsystem = null;

            if (type == typeof(SplashScreen))
            {
                newSubsystem = new SplashScreen(this.OnIGameComplete<SplashScreen>, this.OnMachineSimulatorComplete<SplashScreen>, this.GraphicsDeviceManager);
            }
            else if (type == typeof(MachineScreen))
            {
                newSubsystem = new MachineScreen(this.OnIGameComplete<MachineScreen>, this.FindOrCreateSubsystem(typeof(SlotSimDataStore)) as SlotSimDataStore, this.GraphicsDeviceManager);
            }
            else if (type == typeof(SlotSimDataStore))
            {
                newSubsystem = new SlotSimDataStore(this.FindOrCreateSubsystem(typeof(SlotSimDatabase)) as SlotSimDatabase, "SlotSimDataStore");
            }
            else if (type == typeof(SlotSimDatabase))
            {
                newSubsystem = SlotSimDatabase.CreateDatabase("SlotSimDatabase");
            }
            else
            {
                newSubsystem = base.CreateSubsystem(type);
            }

            if (newSubsystem == null)
            {
                throw new ArgumentException(string.Format("Could not create subsystem of type {0}", type));
            }

            return newSubsystem;
        }

        /// <summary>
        /// Forward request to complete to parent
        /// </summary>
        /// <typeparam name="T">Screen to stop</typeparam>
        /// <param name="nextGame">Screen to transition to</param>
        protected void OnMachineSimulatorComplete<T>(Type nextGame)
        {
            this.IGameComplete(nextGame);
        }
    }
}
