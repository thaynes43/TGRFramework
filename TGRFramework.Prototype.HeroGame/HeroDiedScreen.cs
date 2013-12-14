// -----------------------------------------------------------------------
// <copyright file="HeroDiedScreen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.HeroGame
{
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HeroDiedScreen : SplashScreen
    {
        public HeroDiedScreen(IGameCompleteDelegate<IGame> storyComplete, ButtonSprite playButton, ButtonSprite quitButton, IGameCompleteDelegate<IGame> screenComplete, GraphicsDeviceManager graphics)
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
