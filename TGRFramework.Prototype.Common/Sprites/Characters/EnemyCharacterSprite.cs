// -----------------------------------------------------------------------
// <copyright file="EnemySprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EnemyCharacterSprite : CharacterSprite
    {
        protected bool restricted = true;
        private float damage = 10f;
        private DateTime lastDamage = DateTime.Now;

        public EnemyCharacterSprite(PlayableCharacterSprite hero, MeleeWeaponSprite weapon, RangedWeaponSprite rangedWeapon, string content, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(content, startingPostion, movementSpeed, gfx, level)
        {
            this.PlayableSprite = hero;

            // TODO need a way to manage various weapon types
            this.MeleeWeapon = weapon;
            this.RangedWeapon = rangedWeapon;
        }

        /// <summary>
        /// Two colliding sprites
        /// </summary>
        public event Action<ISprite, EnemyCharacterSprite> CollisionWithSprite;

        private SoundEffect HitSound { get; set; }

        protected PlayableCharacterSprite PlayableSprite { get; set; }

        protected MeleeWeaponSprite MeleeWeapon { get; set; }

        protected RangedWeaponSprite RangedWeapon { get; set; }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            this.HitSound = content.Load<SoundEffect>("Hit");
            this.HitSound.Name = "Hit";

            base.LoadContent(content);
        }

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, GameTime gameTime)
        {
            bool takeDamange = false;
            if (this.BoundingBox.Intersects(this.PlayableSprite.BoundingBox))
            {
                //this.RaiseCollisionWithSprite(this.PlayableSprite); TODO do we need this?
                //this.PlayableSprite.TryTakeDamage(this.damage);

                // Both parties take damage
                if (this.MeleeWeapon.Facing != MeleeWeaponSprite.SwingFacing.Undefined && this.Facing != this.PlayableSprite.Facing)
                {
                    takeDamange = true;
                }
                else
                {
                    this.PlayableSprite.TryTakeDamage(this.damage);
                }
            }

            // Check if being attacked
            if (this.MeleeWeapon.Intersects(this) || takeDamange)
            {
                this.TryTakeDamage(this.MeleeWeapon.WeaponDamage);
            }

            foreach (BulletSprite bullet in this.RangedWeapon.ActiveBullets)
            {
                if (bullet.BoundingBox.Intersects(this.BoundingBox))
                {
                    this.TryTakeDamage(bullet.Damage);
                    this.RangedWeapon.RemoveBulletSprite(bullet);
                    break;
                }
            }

            // Physical constraints of environment
            if (this.restricted)
            {
                base.Update(content, gameTime);
            }
            else if (this.isTakingDamage)
            {
                this.DamageModel.UpdateSpriteLocation();
            }

            if (!this.restricted) // TODO manage this better
            {
                this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
            }
        }

        public override bool TryTakeDamage(float damage)
        {
            if (base.TryTakeDamage(damage))
            {
                this.Facing = this.MeleeWeapon.Facing;

                this.Level.LevelSoundQueue.Add(this.HitSound);
                if (this.HitPoints <= 0)
                {
                    this.RaiseCollisionWithSprite(this.MeleeWeapon);
                }

                return true;
            }

            return false;
        }

        private void RaiseCollisionWithSprite(ISprite sprite)
        {
            if (this.CollisionWithSprite != null)
            {
                this.CollisionWithSprite(sprite, this);
            }
        }
    }
}
