// -----------------------------------------------------------------------
// <copyright file="HitPointsManager.cs" company="">
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
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HitPointsManager : ISprite
    {
        private int currentHp = 0;
        private int hpIndex;

        public HitPointsManager(GraphicsDevice gfx, int startingHp)
        {
            this.HitPointSprites = new List<HitPointSprite>();
            this.Visible = true;
            this.Graphics = gfx;
            this.currentHp = startingHp;
        }

        public enum Justify
        {
            BottomRight,
        }

        public Justify ScreenJustification { get; private set; }

        public List<HitPointSprite> HitPointSprites { get; private set; }

        public bool Visible { get; set; }

        public GraphicsDevice Graphics { get; private set; }

        public void LoadContent(ContentManager content)
        {
            //Vector2 startingVector = new Vector2(this.Graphics.Viewport.Height - 300, this.Graphics.Viewport.Width - 300);

            Vector2 startingVector = new Vector2(5, 5);

            int healthBlocks = (int)Math.Ceiling((double)this.currentHp / 3); // TODO manage

            for (int i = 0; i < currentHp; i++)
            {
                HitPointSprite hpSprite = new HitPointSprite("HP3", startingVector);
                hpSprite.LoadContent(content);
                this.HitPointSprites.Add(hpSprite);

                startingVector.X += (float)hpSprite.Texture.Width * 1.25f;
            }

            this.hpIndex = this.HitPointSprites.Count - 1;
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            foreach (ISprite sprite in this.HitPointSprites)
            {
                sprite.Update(content, gameTime);
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (ISprite sprite in this.HitPointSprites)
            {
                if (sprite.Visible)
                {
                    sprite.Draw(theSpriteBatch);
                }
            }
        }

        public void UpdateHitPoints(float obj)
        {
            if (this.HitPointSprites[this.hpIndex].TakeHit())
            {
                if (this.hpIndex > 0)
                {
                    this.hpIndex--;
                }
            }
        }
    }
}
