// -----------------------------------------------------------------------
// <copyright file="SplashScreen.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// SlotSim game splash screen TODO_EASY refactor to implement SplashScreen class! 
    /// </summary>
    public class SlotSplashScreen : Screen
    {
        public SlotSplashScreen(IGameCompleteDelegate<IGame> screenComplete, IGameCompleteDelegate<IGame> storyComplete, GraphicsDeviceManager graphics)
            : base(screenComplete, graphics, "SplashScreen")
        {
            this.StoryComplete = storyComplete;
        }

        /// <summary>
        /// Handler to complete this StoryBoard
        /// </summary>
        private IGameCompleteDelegate<IGame> StoryComplete { get; set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            // ***** Background Sprites *****
            BackgroundSprite background = new BackgroundSprite("SplashScreen", this.GraphicsDeviceManager.GraphicsDevice);
            this.Sprites.Add(background);

            // ***** Button Sprites *****
            Vector2 playButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 620,
                 this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 350);

            ButtonSprite playButton = new ButtonSprite("PlayButtonUp", "PlayButtonDown", playButtonPos);

            Vector2 quitButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 620,
                this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 175);

            ButtonSprite quitButton = new ButtonSprite("QuitButtonUp", "QuitButtonDown", quitButtonPos);

            this.Sprites.Add(playButton);
            this.Sprites.Add(quitButton);

            playButton.ButtonClickedEvent += this.OnPlayClicked;
            quitButton.ButtonClickedEvent += this.OnQuitClicked;
        }

        /// <summary>
        /// Transition into MachineScreen to play the game
        /// </summary>
        private void OnPlayClicked()
        {
            Log.Info("Play button clicked.");
            this.IGameComplete(typeof(MachineScreen));
        }

        /// <summary>
        /// Transition out of this StoryBoard
        /// </summary>
        private void OnQuitClicked()
        {
            Log.Info("Quit button clicked.");
            this.StoryComplete(null);
        }
    }
}
