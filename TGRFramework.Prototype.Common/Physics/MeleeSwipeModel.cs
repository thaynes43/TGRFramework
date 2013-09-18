// -----------------------------------------------------------------------
// <copyright file="MeleeSwipeModel.cs" company="">
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
    public class MeleeSwipeModel : MeleeAttackModel
    {
        public MeleeSwipeModel(MeleeWeaponSprite meleeWeapon)
            : base(meleeWeapon)
        {
            SwingSpeed = .2f;
            SwingRange = 6f;
            WeaponStartLeft = (5 * MathHelper.Pi) / 4;
            WeaponStartRight = (3 * MathHelper.Pi) / 4;
        }

        public override void UpdateSpriteLocation()
        {
            // Check if we are still facing where we think we are
            this.UpdateSwingFacing();

            if (this.startOfSwing == null)
            {
                this.startOfSwing = this.MeleeWeapon.WeaponAngle;
            }

            if (this.MeleeWeapon.Facing == MeleeWeaponSprite.SwingFacing.Right)
            {
                // Swing facing right
                if (this.MeleeWeapon.WeaponAngle - this.startOfSwing <= 0 && this.Direction == SwingDirection.Up)
                {
                    // End swing
                    this.MeleeWeapon.Facing = MeleeWeaponSprite.SwingFacing.Undefined;
                    this.startOfSwing = null;
                    this.Direction = SwingDirection.Down;
                }

                if (this.MeleeWeapon.WeaponAngle - this.startOfSwing <= SwingRange / 2f && this.Direction == SwingDirection.Down)
                {
                    // Down swing
                    this.MeleeWeapon.WeaponAngle += SwingSpeed;
                }
                else if (this.MeleeWeapon.WeaponAngle - this.startOfSwing > 0)
                {
                    // Up swing
                    this.MeleeWeapon.WeaponAngle -= SwingSpeed;
                    this.Direction = SwingDirection.Up;
                }
            }
            else if (this.MeleeWeapon.Facing == MeleeWeaponSprite.SwingFacing.Left)
            {
                // Swing facing left
                if (this.MeleeWeapon.WeaponAngle - this.startOfSwing >= 0 && this.Direction == SwingDirection.Up)
                {
                    // End swing
                    this.MeleeWeapon.Facing = MeleeWeaponSprite.SwingFacing.Undefined;
                    this.startOfSwing = null;
                    this.Direction = SwingDirection.Down;
                }
                else if (this.startOfSwing - this.MeleeWeapon.WeaponAngle <= SwingRange / 2f && this.Direction == SwingDirection.Down)
                {
                    // Down swing
                    this.MeleeWeapon.WeaponAngle -= SwingSpeed;
                }
                else if (this.MeleeWeapon.WeaponAngle - this.startOfSwing < 0)
                {
                    // Up swing
                    this.MeleeWeapon.WeaponAngle += SwingSpeed;
                    this.Direction = SwingDirection.Up;
                }
            }
        }
    }
}
