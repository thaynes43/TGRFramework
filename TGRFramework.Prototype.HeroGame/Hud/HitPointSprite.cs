// -----------------------------------------------------------------------
// <copyright file="HitPointSprite.cs" company="">
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
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HitPointSprite : ISprite
    {
        private string content;
        private Vector2 position;

        private const int MaxHits = 3;
        private int hitsLeft = 3; // TODO manage

        private ContentManager contentManager;

        public HitPointSprite(string content, Vector2 startingPostion)
        {
            this.content = content;
            this.position = startingPostion;
            this.Visible = true;
        }

        public bool Visible { get; set; }

        public Texture2D Texture { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.Texture = this.Texture = content.Load<Texture2D>(this.content);
            this.contentManager = content;
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.Texture, this.position, Color.White);
        }

        public int TakeHit(int damageTaken, ref bool advanceIndex)
        {
            // Update damage counts
            if (this.hitsLeft < 0)
            {
                return 0; // TODO_NEXT ?? Forget why
            }

            int retDamage = Math.Max(0, damageTaken - this.hitsLeft);
            this.hitsLeft = this.hitsLeft - damageTaken;
            this.AdjustSpriteForHitChange();

            if (this.hitsLeft == 0) advanceIndex = true;

            return retDamage;
        }

        public int RestoreHp(int hpToRestore, ref bool advanceHpSprite)
        {
            int hpToFull = MaxHits - this.hitsLeft;
            int retHp = Math.Max(0, hpToRestore - hpToFull);
            this.hitsLeft = this.hitsLeft + hpToRestore > 3 ? 3 : this.hitsLeft + hpToRestore;
            this.AdjustSpriteForHitChange();
            
            if (hpToRestore > hpToFull) advanceHpSprite = true;
            return retHp;
        }

        private void AdjustSpriteForHitChange()
        {
            if (this.hitsLeft > 0 && !this.Visible) this.Visible = true;

            if (this.hitsLeft == 3)
            {
                this.Texture = this.Texture = this.contentManager.Load<Texture2D>("HP3");
            }
            else if (this.hitsLeft == 2)
            {
                this.Texture = this.Texture = this.contentManager.Load<Texture2D>("HP2");
            }
            else if (this.hitsLeft == 1)
            {
                this.Texture = this.Texture = this.contentManager.Load<Texture2D>("HP1");
            }
            else if (this.hitsLeft == 0)
            {
                this.Visible = false;
            }
        }
    }
}
