// -----------------------------------------------------------------------
// <copyright file="MeleeSwingModel.cs" company="">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MeleeSwingModel : MeleeAttackModel
    {
        public MeleeSwingModel(MeleeWeaponSprite meleeWeapon)
            : base (meleeWeapon)
        {
            SwingSpeed = .2f;
            SwingRange = 3f;
            WeaponStartLeft = (3 * MathHelper.Pi) / 4;
            WeaponStartRight = (5 * MathHelper.Pi) / 4; 
        }

        public override void UpdateSpriteLocation()
        {
            // Check if we are still facing where we think we are
            this.UpdateSwingFacing();

            if (this.startOfSwing == null)
            {
                this.startOfSwing = this.MeleeWeapon.WeaponAngle;
            }

            if (this.MeleeWeapon.WeaponAngle <= MathHelper.Pi && this.MeleeWeapon.WeaponAngle > 0)
            {
                // Swing facing left
                if (this.MeleeWeapon.WeaponAngle - this.startOfSwing >= 0 && this.Direction == SwingDirection.Up)
                {
                    this.MeleeWeapon.Facing = MeleeWeaponSprite.SwingFacing.Undefined;
                    this.startOfSwing = null;
                    this.Direction = SwingDirection.Down;
                }
                if (this.MeleeWeapon.WeaponAngle - this.startOfSwing >= -1 * (SwingRange / 2f) && this.Direction == SwingDirection.Down)
                {
                    // Swing down
                    this.MeleeWeapon.WeaponAngle -= SwingSpeed;
                }
                else if (this.MeleeWeapon.WeaponAngle - this.startOfSwing < 0)
                {
                    // Swing up
                    this.Direction = SwingDirection.Up;
                    this.MeleeWeapon.WeaponAngle += SwingSpeed;
                }
            }
            else if (this.MeleeWeapon.WeaponAngle <= MathHelper.TwoPi && this.MeleeWeapon.WeaponAngle > MathHelper.Pi)
            {
                // Swing facing right
                if (this.MeleeWeapon.WeaponAngle - this.startOfSwing <= 0 && this.Direction == SwingDirection.Up)
                {
                    this.MeleeWeapon.Facing = MeleeWeaponSprite.SwingFacing.Undefined;
                    this.startOfSwing = null;
                    this.Direction = SwingDirection.Down;
                }
                else if (this.MeleeWeapon.WeaponAngle - this.startOfSwing <= SwingRange / 2f && this.Direction == SwingDirection.Down)
                {
                    // Swing down
                    this.MeleeWeapon.WeaponAngle += SwingSpeed;
                }
                else if (this.MeleeWeapon.WeaponAngle - this.startOfSwing > 0)
                {
                    // Swing up
                    this.Direction = SwingDirection.Up;
                    this.MeleeWeapon.WeaponAngle -= SwingSpeed;
                }
            }
        }
    }
}
