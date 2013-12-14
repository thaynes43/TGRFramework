// -----------------------------------------------------------------------
// <copyright file="AggroPathingCharacter.cs" company="">
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
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class AggroPathingCharacter : EnemyCharacterSprite
    {
        protected float range;
        protected float aggroRange;
        protected float aggroSpeedMulti;
        protected float aggroPastDist;

        protected string rightAggro = string.Empty;
        protected string leftAggro = string.Empty;

        private float distanceTraveled = 0f;
        private string leftContent;

        private bool prevAggroWasLeft = false;
        private bool aggroPast = false;

        public AggroPathingCharacter(PlayableCharacterSprite hero, MeleeWeaponSprite weapon, RangedWeaponSprite rangedWeapon, string leftContent, string rightContent, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(hero, weapon, rangedWeapon, rightContent, startingPostion, movementSpeed, gfx, level)
        {
            this.leftContent = leftContent;
            this.Facing = MeleeWeaponSprite.SwingFacing.Right;
        }

        public override void Update(ContentManager content, GameTime gameTime)
        {
            if (!this.isTakingDamage)
            {
                if (this.ShouldAggro(this.aggroRange, this.PlayableSprite.BoundingBox, this.CharacterPosition))
                {
                    this.Aggro(content);
                }
                else
                {
                    this.Path(content);
                }
            }

            base.Update(content, gameTime);
        }

        protected abstract bool ShouldAggro(float range, Rectangle heroBounds, Vector2 enemyPosition);

        private void Aggro(ContentManager content)
        {
            if (Math.Abs(this.BoundingBox.X - this.PlayableSprite.BoundingBox.X) < this.aggroPastDist)
            {
                if (prevAggroWasLeft)
                {
                    this.TryMoveLeft((int)(this.MovementSpeed * aggroSpeedMulti));
                }
                else
                {
                    this.TryMoveRight((int)(this.MovementSpeed * aggroSpeedMulti));
                }
            }
            else if (CharacterUtil.PlayerToTheRight(this.PlayableSprite.BoundingBox, this.BoundingBox))
            {
                this.prevAggroWasLeft = false;

                if (this.rightAggro != string.Empty)
                    this.CharacterTexture = content.Load<Texture2D>(this.rightAggro);

                this.Facing = MeleeWeaponSprite.SwingFacing.Right;
                this.TryMoveRight((int)(this.MovementSpeed * aggroSpeedMulti));
            }
            else if (CharacterUtil.PlayerToTheLeft(this.PlayableSprite.BoundingBox, this.BoundingBox))
            {
                this.prevAggroWasLeft = true;

                if (this.leftAggro != string.Empty)
                    this.CharacterTexture = content.Load<Texture2D>(this.leftAggro);

                this.Facing = MeleeWeaponSprite.SwingFacing.Left;
                this.TryMoveLeft((int)(this.MovementSpeed * aggroSpeedMulti));
            }
        }

        private void Path(ContentManager content)
        {
            // Check we are facing the correct way
            if (this.distanceTraveled >= this.range)
            {
                this.Facing = MeleeWeaponSprite.SwingFacing.Left;
            }
            else if (this.distanceTraveled <= 0f)
            {
                this.Facing = MeleeWeaponSprite.SwingFacing.Right;
            }

            // Move in the direction we are facing
            if (this.Facing == MeleeWeaponSprite.SwingFacing.Right)
            {
                if (this.rightAggro != string.Empty)
                    this.CharacterTexture = content.Load<Texture2D>(this.CharacterContent);

                this.TryMoveRight((int)this.MovementSpeed);
                this.distanceTraveled += this.MovementSpeed;
            }
            else
            {
                if (this.leftAggro != string.Empty)
                    this.CharacterTexture = content.Load<Texture2D>(this.leftContent);

                this.TryMoveLeft((int)this.MovementSpeed);
                this.distanceTraveled -= this.MovementSpeed;
            }
        }
    }
}
