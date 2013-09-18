// -----------------------------------------------------------------------
// <copyright file="SplashScreen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.HeroGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CharacterSelectScreen : Screen
    {
        public CharacterSelectScreen(IGameCompleteDelegate<IGame> screenComplete, GraphicsDeviceManager graphics)
            : base(screenComplete, graphics, "CharacterSelectScreen")
        {
        }

        public override void Initialize()
        {
            lock (this.SubsystemLock)
            {
                // ***** Button Sprites *****
                Vector2 playButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 620,
                     this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 450);

                ButtonSprite playButton = new ButtonSprite("PlayButtonUp", "PlayButtonDown", playButtonPos);

                Vector2 quitButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 620,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 275);

                ButtonSprite quitButton = new ButtonSprite("QuitButtonUp", "QuitButtonDown", quitButtonPos);

                this.Sprites.Add(playButton);
                this.Sprites.Add(quitButton);

                playButton.ButtonClickedEvent += this.OnPlayClicked;
                quitButton.ButtonClickedEvent += this.OnQuitClicked;
            }
        }

        /// <summary>
        /// Transition into MachineScreen to play the game
        /// </summary>
        private void OnPlayClicked()
        {
            Log.Info("Play button clicked.");
            this.IGameComplete(typeof(LevelScreen));
        }

        /// <summary>
        /// Transition out of this StoryBoard
        /// </summary>
        private void OnQuitClicked()
        {
            Log.Info("Quit button clicked.");
            this.IGameComplete(typeof(HeroSplashScreen));
        }
    }
}
