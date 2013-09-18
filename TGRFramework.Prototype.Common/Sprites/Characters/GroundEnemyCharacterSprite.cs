// -----------------------------------------------------------------------
// <copyright file="GroundEnemyCharacterSprite.cs" company="">
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
    /// Enemy character restricted to being on adjacent solid times
    /// Will attempt to get to hero if on screen
    /// </summary>
    public class GroundEnemyCharacterSprite : EnemyCharacterSprite
    {
        private float range = 300f;

        private float distanceTraveled = 0f;

        private float aggroRange = 400f;

        private const float arrgoSpeedMulti = 3f;

        public GroundEnemyCharacterSprite(PlayableCharacterSprite hero, MeleeWeaponSprite weapon, string leftContent, string rightContent, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(hero, weapon, rightContent, startingPostion, movementSpeed, gfx, level)
        {
            this.restricted = true;

            this.LeftContent = leftContent;
            this.Facing = MeleeWeaponSprite.SwingFacing.Right;

            this.StartingVector = startingPostion;
            this.HitPoints = 0f;
        }

        private string LeftContent { get; set; }

        public Vector2 StartingVector { get; set; }

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, GameTime gameTime)
        {
            if (!this.isTakingDamage)
            {
                // Aggro
                if (Math.Abs(this.CharacterPosition.X - this.PlayableSprite.BoundingBox.X) < this.aggroRange && (this.CharacterPosition.Y > this.PlayableSprite.BoundingBox.Y - 50 && this.CharacterPosition.Y < this.PlayableSprite.BoundingBox.Y + 50))
                {
                    // Character is to the left - Enemy will face right
                    if (this.CharacterPosition.X < this.PlayableSprite.BoundingBox.X)
                    {
                        if (this.CharacterPosition.X < PlatformerLevel.LevelWidth - this.CharacterTexture.Width)
                        {
                            this.CharacterTexture = content.Load<Texture2D>("RobotAggroRight"); // TODO not hardcoded
                            this.Facing = MeleeWeaponSprite.SwingFacing.Right;
                            this.TryMoveRight((int)(this.MovementSpeed * arrgoSpeedMulti));
                        }
                    }
                    else
                    {
                        if (this.CharacterPosition.X > 0)
                        {
                            this.CharacterTexture = content.Load<Texture2D>("RobotAggroLeft");
                            this.Facing = MeleeWeaponSprite.SwingFacing.Left;
                            this.TryMoveLeft((int)(this.MovementSpeed * arrgoSpeedMulti));
                        }
                    }
                }
                // Wander
                else
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
                        this.CharacterTexture = content.Load<Texture2D>(this.CharacterContent);
                        this.TryMoveRight((int)this.MovementSpeed);
                        this.distanceTraveled += this.MovementSpeed;
                    }
                    else
                    {
                        this.CharacterTexture = content.Load<Texture2D>(this.LeftContent);
                        this.TryMoveLeft((int)this.MovementSpeed);
                        this.distanceTraveled -= this.MovementSpeed;
                    }
                }
            }

            base.Update(content, gameTime);
        }

        public override bool TryMoveLeft(int speed)
        {
            // Don't fall off cliffs
            if (this.Level.IsAtCliff(this.BoundingBox, this.Facing))
            {
                return false;
            }
                
            return base.TryMoveLeft(speed);
        }

        public override bool TryMoveRight(int speed)
        {
            // Don't fall off cliffs
            if (this.Level.IsAtCliff(this.BoundingBox, this.Facing)) // TODO_HIGH Messes with IsAtStep
            {
                //PlatformerLevel.Log.Debug("Ground enemy is at a cliff and will not move.");
                return false;
            }

            return base.TryMoveRight(speed);
        }
    }
}
