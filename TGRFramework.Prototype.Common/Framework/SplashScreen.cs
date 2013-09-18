// -----------------------------------------------------------------------
// <copyright file="SplashScreen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TRGFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Intended to be the first screen in the game. Handles exiting to menus above the scope of the StoryBoard
    /// </summary>
    public abstract class SplashScreen : Screen
    {
        public SplashScreen(IGameCompleteDelegate<IGame> storyComplete, ButtonSprite playButton, ButtonSprite quitButton, IGameCompleteDelegate<IGame> screenComplete, GraphicsDeviceManager graphics, string loggerName)
            : base(screenComplete, graphics, loggerName)
        {
            this.StoryComplete = storyComplete;
            this.PlayButton = playButton;
            this.QuitButton = quitButton;
        }

        /// <summary>
        /// Handler to complete this StoryBoard
        /// </summary>
        protected IGameCompleteDelegate<IGame> StoryComplete { get; set; }

        /// <summary>
        /// Button to advance in this story board
        /// </summary>
        protected ButtonSprite PlayButton { get; set; }

        /// <summary>
        /// Button to exit to storyboards at a higher level
        /// </summary>
        protected ButtonSprite QuitButton { get; set; }

        /// <summary>
        /// Initialize necessary components of SplashScreen. If overridden parent must call base.
        /// </summary>
        public override void Initialize()
        {
            this.Sprites.Add(this.PlayButton);
            this.Sprites.Add(this.QuitButton);

            this.PlayButton.ButtonClickedEvent += this.OnPlayButtonClicked;
            this.QuitButton.ButtonClickedEvent += this.OnQuitButtonClicked;
        }

        protected abstract void OnPlayButtonClicked();

        protected abstract void OnQuitButtonClicked();
    }
}
