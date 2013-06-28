// -----------------------------------------------------------------------
// <copyright file="SplashScreen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.SlotSim.Appliance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SlotMachine.Prototype.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SplashScreen : Screen
    {
        public SplashScreen(ScreenCompleteDelegate<Screen, Screen> screenComplete, GraphicsDeviceManager graphics)
            : base(screenComplete, graphics, "SplashScreen")
        {
        }

        public override void Initialize()
        {
            Vector2 resetButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 300,
                this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 150);
            ButtonSprite resetButton = new ButtonSprite("ResetButtonUp", "ResetButtonDown", resetButtonPos);

            this.Sprites.Add(resetButton);

            resetButton.ButtonClickedEvent += this.OnStartGameClicked;
        }

        private void OnStartGameClicked()
        {
            Log.Info("Start game button clicked.");
            this.ScreenComplete();
        }
    }
}
