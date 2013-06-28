// -----------------------------------------------------------------------
// <copyright file="SlotMachineManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.SlotSim.Appliance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using SlotMachine.Prototype.Tools;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReelManager
    {
        public ReelManager(MachineScreen parentScreen, ILogTool log)
        {
            this.Log = log;
            this.ParentScreen = parentScreen;
            this.SlotSimulation = new Slots(this.ParentScreen.DataStore);
            this.Reels = new List<Reel>();
        }

        public event Action<int> SpinComplete;

        public MachineScreen ParentScreen { get; set; }

        public Slots SlotSimulation { get; set; }

        public List<Reel> Reels { get; set; }

        private ILogTool Log { get; set; }

        private int NextPayout { get; set; }

        public void Initialize(GraphicsDeviceManager graphics)
        {          
            // TODO Dynamically adjust based on reels configured
            Vector2 reel1Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width - 704f,
                graphics.GraphicsDevice.Viewport.Height - 440);
            Reel reel1 = new Reel(this.Log, reel1Pos);
            reel1.Initialize();

            Vector2 reel2Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width - 483.5f, // 220.5 to the right
                graphics.GraphicsDevice.Viewport.Height - 440);
            Reel reel2 = new Reel(this.Log, reel2Pos);
            reel2.Initialize();

            Vector2 reel3Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width - 263f, // 220.5 to the right
                graphics.GraphicsDevice.Viewport.Height - 440);
            Reel reel3 = new Reel(this.Log, reel3Pos);
            reel3.Initialize();

            reel3.SpinComplete += () => this.RaiseSpinComplete(this.NextPayout);

            this.Reels.Add(reel1);
            this.Reels.Add(reel2);
            this.Reels.Add(reel3);

            // TODO Move above bg sprite
            ParentScreen.Sprites.Add(reel1);
            ParentScreen.Sprites.Add(reel2);
            ParentScreen.Sprites.Add(reel3);
        }

        public void Spin()
        {
            string output;
            List<Symbol> spunSymbols;
            this.NextPayout = this.SlotSimulation.SpinReels(out output, out spunSymbols) * ParentScreen.DataStore.CreditsPerSpin;

            // TODO Pass winning symbol
            int duration = 1000;
            for (int i = 0; i < this.Reels.Count; i++)
            {
                this.Reels[i].Spin(duration, spunSymbols[i]);
                duration += 500;
            }

            this.ParentScreen.DataStore.TempOutput = output;
            this.ParentScreen.DataStore.Credits -= this.ParentScreen.DataStore.CreditsPerSpin;
        }

        public void ForceStopSpin()
        {
            foreach (Reel reel in this.Reels)
            {
                reel.ForceStopSpin();
            }
        }

        public void Reset()
        {     
            foreach (Reel reel in this.Reels)
            {
                reel.Reset();
            }
        }
        
        private void RaiseSpinComplete(int payout)
        {
            if (this.SpinComplete != null)
            {
                this.SpinComplete(payout);
            }
        }
    }
}
