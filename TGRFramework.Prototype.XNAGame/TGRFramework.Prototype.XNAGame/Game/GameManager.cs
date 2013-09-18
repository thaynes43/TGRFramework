// -----------------------------------------------------------------------
// <copyright file="GameManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.XNAGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;
    using TGRFramework.Prototype.SlotGame;
    using System.Threading;
    using TGRFramework.Prototype.HeroGame;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Manage transitions between different games
    /// </summary>
    public class GameManager : StoryBoard
    {
        public GameManager(IGameCompleteDelegate<IGame> gameComplete, GraphicsDeviceManager graphics, ContentManager content)
            : base (gameComplete, graphics, content, "GameManager")
        {
            //this.ActiveGame = this.FindOrCreateSubsystem(typeof(MachineSimulator)) as MachineSimulator;

            this.ActiveGame = this.FindOrCreateSubsystem(typeof(GameSelection)) as GameSelection;

            // TODO_MEDIUM highest leveled subsystem needs to start itself. Need to streamline for to ease switching from game at any level
            Thread thisThread = new Thread(() => { this.Run(); });
            string[] name = this.GetType().ToString().Split('.');
            thisThread.Name = string.Format("{0}", name[name.Length - 1]);
            thisThread.Start();        
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch theSpriteBatch)
        {
            base.Draw(theSpriteBatch);
        }

        /// <inheritdoc />
        protected override Subsystem CreateSubsystem(Type type)
        {
            Log.Info("  Creating {0}.", type);

            Subsystem newSubsystem = null;

            if (type == typeof(MachineSimulator))
            {
                newSubsystem = new MachineSimulator(this.OnIGameComplete<MachineSimulator>, this.GraphicsDeviceManager, this.ContentManager, "MachineSimulator");
            }
            else if (type == typeof(HeroGameStoryBoard))
            {
                newSubsystem = new HeroGameStoryBoard(this.OnIGameComplete<HeroGameStoryBoard>, this.GraphicsDeviceManager, this.ContentManager, "MachineSimulator");
            }
            else if (type == typeof(GameSelection))
            {
                // TODO_HIGH how does this report back what screen was selected?
                newSubsystem = new GameSelection(this.OnIGameComplete<GameSelection>, this.GraphicsDeviceManager, this.ContentManager, "GameSelection");
            }
            if (newSubsystem == null)
            {
                throw new ArgumentException(string.Format("Could not create subsystem of type {0}", type));
            }

            return newSubsystem;
        }

        /// <inheritdoc />
        protected override void OnIGameComplete<T>(Type nextGame)
        {
            // Child does not know what type parent is, needs to request transition through null type TODO_LOW review this
            Type nextType = nextGame;
            if (nextGame == null)
            {
                nextType = typeof(GameSelection);
            }

            base.OnIGameComplete<T>(nextType);
        }
    }
}
