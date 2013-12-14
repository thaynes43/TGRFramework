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
    public class GroundEnemyCharacterSprite : AggroPathingCharacter
    {
        public GroundEnemyCharacterSprite(PlayableCharacterSprite hero, MeleeWeaponSprite weapon, RangedWeaponSprite rangedWeapon, string leftContent, string rightContent, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(hero, weapon, rangedWeapon, leftContent, rightContent, startingPostion, movementSpeed, gfx, level)
        {
            this.restricted = true;
            this.HitPoints = 2;

            this.aggroSpeedMulti = 3f;
            this.aggroRange = 400f;
            this.range = 300f;
            this.aggroPastDist = 50f;

            this.rightAggro = "RobotAggroRight";
            this.leftAggro = "RobotAggroLeft";
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

        protected override bool ShouldAggro(float range, Rectangle heroBounds, Vector2 enemyPosition)
        {
            return ( Math.Abs(enemyPosition.X - heroBounds.X) < range &&
                ( enemyPosition.Y > heroBounds.Y - 50 &&
                  enemyPosition.Y < heroBounds.Y + 50 ));
        }
    }
}
