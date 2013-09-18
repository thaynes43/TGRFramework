// -----------------------------------------------------------------------
// <copyright file="SelectionScreen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.XNAGame
{
    using Microsoft.Xna.Framework;
    using TGRFramework.Prototype.SlotGame;
    using TGRFramework.Prototype.Common;
    using TGRFramework.Prototype.HeroGame;

    /// <summary>
    /// Screen to navigate between storyboards
    /// </summary>
    public class SelectionScreen : Screen
    {
        public SelectionScreen(IGameCompleteDelegate<IGame> screenComplete, GraphicsDeviceManager graphics, string screenLogger)
            : base(screenComplete, graphics, screenLogger)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            // ***** Background Sprites *****
            BackgroundSprite background = new BackgroundSprite("GameSelectionScreen", this.GraphicsDeviceManager.GraphicsDevice);
            this.Sprites.Add(background);

            // ***** Button Sprites *****
            Vector2 selectSlotButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 666,
                this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 443);

            ButtonSprite selectSlotButton = new ButtonSprite("SelectSlotUp", "SelectSlotDown", selectSlotButtonPos);

            Vector2 selectHeroButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 364,
                this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 443);

            ButtonSprite selectHeroButton = new ButtonSprite("SelectHeroUp", "SelectHeroDown", selectHeroButtonPos);

            this.Sprites.Add(selectSlotButton);
            this.Sprites.Add(selectHeroButton);

            selectSlotButton.ButtonClickedEvent += this.OnSelectSlotClicked;
            selectHeroButton.ButtonClickedEvent += this.OnSelectHeroClicked;
        }

        /// <summary>
        /// Select SlotSim game
        /// </summary>
        private void OnSelectSlotClicked()
        {
            Log.Info("SelectSlot button clicked.");
            this.IGameComplete(typeof(MachineSimulator));
        }

        /// <summary>
        /// Select Hero Game
        /// </summary>
        private void OnSelectHeroClicked()
        {
            Log.Info("SelectHero button clicked.");
            this.IGameComplete(typeof(HeroGameStoryBoard));
        }
    }
}
