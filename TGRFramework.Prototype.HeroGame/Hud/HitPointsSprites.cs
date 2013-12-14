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
    public class HitPointsSprites : IHitPoints
    {
        private int herosHP = 0;

        /// <summary>
        /// Number of damage represented by each hit point sprite
        /// </summary>
        private int hitsPerHitPointSprite = 3; 

        /// <summary>
        /// Index of current hit point sprite
        /// </summary>
        private int hpIndex;

        private Vector2 referenceVector;
        private HitPointSprite[] hitPointSprites;
        private GraphicsDevice graphics;
        private Justify screenJustification;

        public HitPointsSprites(GraphicsDevice gfx, int startingHp, Vector2 startVector)
        {
            this.Visible = true;
            this.graphics = gfx;
            this.herosHP = startingHp;
            this.referenceVector = startVector;
        }

        public enum Justify // TODO not used - implement
        {
            BottomRight,
        }

        public bool Visible { get; set; }

        public void LoadContent(ContentManager content)
        {
            int healthBlocks = (int)Math.Ceiling((double)this.herosHP / this.hitsPerHitPointSprite); // n hp per block TODO 0 blocks = dead, right now we are 1 HP ahead
            this.hitPointSprites = new HitPointSprite[healthBlocks];

            for (int i = 0; i < healthBlocks; i++)
            {
                HitPointSprite hpSprite = new HitPointSprite("HP3", this.referenceVector);
                hpSprite.LoadContent(content);
                this.hitPointSprites[i] = hpSprite;

                this.referenceVector.X += (float)hpSprite.Texture.Width * 1.25f;
            }

            this.hpIndex = this.hitPointSprites.Length - 1;
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            foreach (ISprite sprite in this.hitPointSprites)
            {
                sprite.Update(content, gameTime);
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (ISprite sprite in this.hitPointSprites)
            {
                if (sprite.Visible)
                {
                    sprite.Draw(theSpriteBatch);
                }
            }
        }

        // TODO_NEXT + Hp

        public void UpdateHitPoints(int newHP)
        {
            int hitPointDifferent = Math.Abs(this.herosHP - newHP);

            if (newHP < this.herosHP)
            {
                this.TakeHit(hitPointDifferent);
            }
            else
            {
                this.RestoreHp(hitPointDifferent);
            }

            this.herosHP = newHP;
        }

        private void TakeHit(int damage)
        {
            int distributedSpriteDamage = damage;

            do
            {
                bool advanceIndex = false;

                distributedSpriteDamage = this.hitPointSprites[this.hpIndex].TakeHit(damage, ref advanceIndex);

                if (advanceIndex && this.hpIndex > 0) this.hpIndex--;
            } 
            while (distributedSpriteDamage != 0);       
        }

        private void RestoreHp(int hp)
        {
            int distributedHp = hp;

            do
            {
                bool advanceToNextHpSprite = false;

                distributedHp = this.hitPointSprites[this.hpIndex].RestoreHp(hp, ref advanceToNextHpSprite);

                if (advanceToNextHpSprite && this.hpIndex < this.hitPointSprites.Length) this.hpIndex++;
            }
            while (distributedHp != 0);
        }
    }
}
