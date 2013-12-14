// -----------------------------------------------------------------------
// <copyright file="SaucerBossCharacterSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SaucerBossCharacterSprite : AggroPathingCharacter
    {
        private bool isDodging = false;
        private IPhysicsModel dodgeModel;
        private RemoveSpriteDelegate removeSprite;
        private AddSpriteDelegate addSprite;

        private Random random = new Random(DateTime.Now.Millisecond); // TODO we can have one static random everyone uses - quit allocating space for new randoms

        public SaucerBossCharacterSprite(AddSpriteDelegate addSprite, RemoveSpriteDelegate removeSprite, PlayableCharacterSprite hero, MeleeWeaponSprite weapon, RangedWeaponSprite rangedWeapon, string content, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(hero, weapon, rangedWeapon, content, content, startingPostion, movementSpeed, gfx, level)
        {
            this.restricted = false;
            this.HitPoints = 100;

            this.aggroSpeedMulti = 6f;
            this.aggroRange = 400f;
            this.range = 300f;
            this.aggroPastDist = 200f;

            this.dodgeModel = new DodgeModel(this, this.PlayableSprite);
            this.dodgeModel.UpdateSpriteComplete += () => { this.isDodging = false; };

            this.ActiveBullets = new List<BulletSprite>();
            this.addSprite = addSprite;
            this.removeSprite = removeSprite;
        }

        public List<BulletSprite> ActiveBullets { get; set; } // TODO how to share this with RangedWeaponSprite... 

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, GameTime gameTime)
        {
            // Remove bullets that have flown off the screen TODO ... SHARE
            for (int i = 0; i < this.ActiveBullets.Count; i++)
            {
                if (!(this.ActiveBullets[i].BoundingBox.X >= PlatformerLevel.CameraPositionX - this.ActiveBullets[i].BoundingBox.Width &&
                    this.ActiveBullets[i].BoundingBox.X <= this.GraphicsDevice.Viewport.Width + PlatformerLevel.CameraPositionX &&
                    this.ActiveBullets[i].BoundingBox.Y >= PlatformerLevel.CameraPositionY - this.ActiveBullets[i].BoundingBox.Height &&
                    this.ActiveBullets[i].BoundingBox.Y <= this.GraphicsDevice.Viewport.Height + PlatformerLevel.CameraPositionY))
                {
                    this.RemoveBulletSprite(this.ActiveBullets[i]);
                }
            }

            if (this.isDodging)
            {
                this.dodgeModel.UpdateSpriteLocation();
            }
            else
            {
                if (this.BoundingBox.Y > 200) // TODO this is for debugging
                    this.TryMoveUp(10);
            }

            base.Update(content, gameTime);

            int attackQuestionMark = random.Next(0, 4);
            if (attackQuestionMark == 2)
                this.Attack(); // TODO move this around see what works best

            // EEEEEEEEEEEEEEEEEEEEEEEEE
            for (int i = this.ActiveBullets.Count - 1; i >= 0; i--)
            {
                if (this.ActiveBullets[i].BoundingBox.Intersects(this.PlayableSprite.BoundingBox))
                {
                    this.PlayableSprite.TryTakeDamage(this.ActiveBullets[i].Damage);
                    this.RemoveBulletSprite(this.ActiveBullets[i]);
                }
            }
        }

        public override bool TryTakeDamage(int damage)
        {
            if (!this.isTakingDamage)
            {
                this.isDodging = true;
            }

            return base.TryTakeDamage(damage);
        }

        protected override bool ShouldAggro(float range, Rectangle heroBounds, Vector2 enemyPosition)
        {
            return !this.isDodging && Math.Abs(enemyPosition.X - heroBounds.X) < range;
        }

        private void Attack()
        {
            Vector2 enemyVector = new Vector2(this.PlayableSprite.BoundingBox.X, this.PlayableSprite.BoundingBox.Y);

            Vector2 myShootVector = new Vector2(this.BoundingBox.X + (this.BoundingBox.Width / 2), this.BoundingBox.Y + this.BoundingBox.Height);

            Vector2 vectorToPlayer = enemyVector - myShootVector;
            float distance = vectorToPlayer.Length();

            if (distance == 0) return;

            Vector2 attackVelocity = Vector2.Normalize(vectorToPlayer / distance);

            BulletSprite ammoSprite = new BulletSprite("Bullet", myShootVector, this.PlayableSprite.Facing, attackVelocity);
            this.addSprite(ammoSprite);
            this.ActiveBullets.Add(ammoSprite);
        }

        private void RemoveBulletSprite(BulletSprite sprite) // TODO needs to be called once intersects w/ Player. Enemy sprite currently does this for "Ranged weapon, not the                
        {                                                    // right approach however as it does not scale to work with this implementation
            this.removeSprite(sprite);
            this.ActiveBullets.Remove(sprite);
        }
    }
}
