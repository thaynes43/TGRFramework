// -----------------------------------------------------------------------
// <copyright file="SlotMachine.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.SlotSim.Appliance
{
    using System;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using SlotMachine.Prototype.Common;
    using Microsoft.Xna.Framework;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MachineSimulator : StoryBoard
    {
        public MachineSimulator(GraphicsDeviceManager graphics, ContentManager content)
            : base(graphics, content)
        {
        }

        public override void Initialize()
        {
            lock (this.SubsystemLock)
            {
                // Create subsystems - TODO_HIGH do NOT allow creation of things dependent upon this WITHOUT this....
                this.FindOrCreateSubsystem(typeof(SlotSimDatabase));

                this.ActiveScreen = this.FindOrCreateSubsystem(typeof(SplashScreen)) as SplashScreen;
            
                // Initialize subsystems
                base.Initialize();
            }
        }

        protected override Subsystem CreateSubsystem(Type type)
        {
            Subsystem newSubsystem = null;

            if (type == typeof(SplashScreen))
            {
                newSubsystem = new SplashScreen(this.OnScreenComplete<SplashScreen, MachineScreen>, this.GraphicsDeviceManager);
            }
            else if (type == typeof(MachineScreen))
            {
                newSubsystem = new MachineScreen(this.OnScreenComplete<MachineScreen, SplashScreen>, this.FindOrCreateSubsystem(typeof(SlotSimDataStore)) as SlotSimDataStore, this.GraphicsDeviceManager);
            }
            else if (type == typeof(SlotSimDatabase))
            {
                newSubsystem = SlotSimDatabase.CreateDatabase(this.FindOrCreateSubsystem(typeof(SlotSimDataStore)) as SlotSimDataStore, "SlotSimDatabase");
            }
            else if (type == typeof(SlotSimDataStore))
            {
                newSubsystem = new SlotSimDataStore("SlotSimDataStore");
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
    }
}
