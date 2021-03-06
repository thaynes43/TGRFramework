﻿// -----------------------------------------------------------------------
// <copyright file="SplashScreen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.HeroGame
{
    using Microsoft.Xna.Framework;
    using TGRFramework.Prototype.Common;

    /// <summary>
    /// TODO: Common class to Exit game and return to parent selection screen
    /// </summary>
    public class HeroSplashScreen : SplashScreen
    {
        public HeroSplashScreen(IGameCompleteDelegate<IGame> storyComplete, ButtonSprite playButton, ButtonSprite quitButton, IGameCompleteDelegate<IGame> screenComplete, GraphicsDeviceManager graphics)
            : base(storyComplete, playButton, quitButton, screenComplete, graphics, "HeroSplashScreen")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnPlayButtonClicked()
        {
            Log.Info("Play button clicked.");
            this.IGameComplete(typeof(LevelScreen));
        }

        protected override void OnQuitButtonClicked()
        {
            Log.Info("Quit button clicked.");
            this.StoryComplete(null);
        }
    }
}
