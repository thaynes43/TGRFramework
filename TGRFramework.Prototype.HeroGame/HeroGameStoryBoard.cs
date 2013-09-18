// -----------------------------------------------------------------------
// <copyright file="HeroGameStoryBoard.cs" company="">
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
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HeroGameStoryBoard : StoryBoard
    {
        public HeroGameStoryBoard (IGameCompleteDelegate<IGame> gameComplete, GraphicsDeviceManager graphics, ContentManager content, string logName)
            : base(gameComplete, graphics, content, logName)
        {
            this.ActiveGame = this.FindOrCreateSubsystem(typeof(HeroSplashScreen)) as HeroSplashScreen;
        }

        /// <inheritdoc />
        protected override Subsystem CreateSubsystem(Type type)
        {
            Log.Info("  Creating {0}.", type);

            Subsystem newSubsystem = null;

            if (type == typeof(HeroSplashScreen))
            {
                // ***** Button Sprites *****
                Vector2 playButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 620,
                     this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 350);

                ButtonSprite playButton = new ButtonSprite("PlayButtonUp", "PlayButtonDown", playButtonPos);

                Vector2 quitButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 620,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 175);

                ButtonSprite quitButton = new ButtonSprite("QuitButtonUp", "QuitButtonDown", quitButtonPos);

                newSubsystem = new HeroSplashScreen(this.OnMachineSimulatorComplete<HeroSplashScreen> , playButton, quitButton, this.OnIGameComplete<HeroSplashScreen>, this.GraphicsDeviceManager);
            }
            else if (type == typeof(CharacterSelectScreen))
            {
                newSubsystem = new CharacterSelectScreen(this.OnIGameComplete<CharacterSelectScreen>, this.GraphicsDeviceManager);
            }
            else if (type == typeof(LevelScreen))
            {
                newSubsystem = new LevelScreen(this.OnIGameComplete<LevelScreen>, this.GraphicsDeviceManager, this.ContentManager);
            }

            return newSubsystem;
        }

        /// <summary>
        /// Forward request to complete to parent : TODO_HIGH this is copy pasta
        /// </summary>
        /// <typeparam name="T">Screen to stop</typeparam>
        /// <param name="nextGame">Screen to transition to</param>
        protected void OnMachineSimulatorComplete<T>(Type nextGame)
        {
            this.IGameComplete(nextGame);
        }
    }
}
