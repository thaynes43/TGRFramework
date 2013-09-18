// -----------------------------------------------------------------------
// <copyright file="HomingCharacterSprite.cs" company="">
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
    public class HomingCharacterSprite : EnemyCharacterSprite
    {
        public HomingCharacterSprite(PlayableCharacterSprite hero, MeleeWeaponSprite weapon, string content, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(hero, weapon, content, startingPostion, movementSpeed, gfx, level)
        {
            this.restricted = false;
            this.HitPoints = 0f;
        }

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, GameTime gameTime)
        {
            // Update path to hero
            if (!this.isTakingDamage)
            {
                float heroX = this.PlayableSprite.BoundingBox.X;
                float heroY = this.PlayableSprite.BoundingBox.Y;

                // Move 1 space at hero per movement speed. May not change if already at hero
                float newX = this.CharacterPosition.X;
                float newY = this.CharacterPosition.Y;

                Line lineToHero = new Line(heroY, this.CharacterPosition.Y, heroX, this.CharacterPosition.X);
                lineToHero.GetPointOnLineToHero(this.MovementSpeed, ref newX, ref newY);
                this.CharacterPosition = new Vector2(newX, newY);
            }

            base.Update(content, gameTime);
        }
    }
}
