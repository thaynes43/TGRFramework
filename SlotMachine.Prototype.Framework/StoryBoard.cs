// -----------------------------------------------------------------------
// <copyright file="StoryManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using SlotMachine.Prototype.Common;
    using SlotMachine.Prototype.SlotSim.Appliance;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StoryBoard
    {        
        public StoryBoard()
        {
        }

        Dictionary<string, Subsystem> Subsystems { get; set; }

        List<Thread> SubsystemThreads { get; set; }

        public Subsystem FindOrCreateSubsystem(string name)
        {
            if (!this.Subsystems.ContainsKey(name))
            {

                Subsystem newSubsystem = this.CreateSubsystem(name);
                this.Subsystems.Add(name, newSubsystem);
                Thread thread = new Thread(() =>
                {
                    newSubsystem.Run();
                });

                this.SubsystemThreads.Add(thread);
                thread.Start();              
            }

            return this.Subsystems[name];
        }

        private Subsystem CreateSubsystem(string name)
        {
            Subsystem newSubsystem = null;

            switch (name)
            {
                case(MachineScreen.SubsystemName):
                    newSubsystem = new MachineScreen();
                    break;
            }

            return newSubsystem;
        }
    }
}