// -----------------------------------------------------------------------
// <copyright file="HudManager.cs" company="">
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
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HudManager : ISprite
    {
        public HudManager(LevelScreen parentScreen, ScreenJustify justify)
        {
            this.ParentScreen = parentScreen;
            this.Justify = justify;
            this.Sprites = new List<ISprite>();
            this.Visible = true;
        }

        public enum ScreenJustify
        {
            TopLeft,
        }

        public LevelScreen ParentScreen { get; set; }

        public ScreenJustify Justify { get; set; }

        public IHitPoints HitPointsSprites { get; set; }

        public CoinCountSprite CoinCountSprite { get; set; }

        public List<ISprite> Sprites { get; set; }

        public bool Visible { get; set; }

        public void Initialize()
        {
            this.HitPointsSprites = new HitPointsSprites(this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, (int)this.ParentScreen.HeroSprite.HitPoints, new Vector2(5, 5));
            this.ParentScreen.HeroSprite.UpdateHitPoints += this.HitPointsSprites.UpdateHitPoints;

            this.CoinCountSprite = new CoinCountSprite("Coin", new Vector2(5, 50));

            this.Sprites.Add(this.HitPointsSprites);
            this.Sprites.Add(this.CoinCountSprite);
        }

        public void LoadContent(ContentManager content)
        {
            foreach (ISprite sprite in this.Sprites)
            {
                sprite.LoadContent(content);
            }
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            foreach (ISprite sprite in this.Sprites)
            {
                sprite.Update(content, gameTime);
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (ISprite sprite in this.Sprites)
            {
                if (sprite.Visible)
                {
                    sprite.Draw(theSpriteBatch);
                }
            }
        }
    }
}
