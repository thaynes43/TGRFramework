// -----------------------------------------------------------------------
// <copyright file="MeleeDownSwipeModel.cs" company="">
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
    public class MeleeDownSwipeModel : MeleeAttackModel
    {
        public MeleeDownSwipeModel(MeleeWeaponSprite meleeWeapon)
            : base(meleeWeapon)
        {
            SwingSpeed = .25f;
            SwingRange = 3f;
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

            if (Math.Abs((double)(this.MeleeWeapon.WeaponAngle - this.startOfSwing)) <= SwingRange)
            {               
                if (this.MeleeWeapon.Facing == MeleeWeaponSprite.SwingFacing.Right)
                {
                    // Down swing to the right
                    this.MeleeWeapon.WeaponAngle += SwingSpeed;
                }
                else
                {
                    // Down swing to the left
                    this.MeleeWeapon.WeaponAngle -= SwingSpeed;
                }
            }
            else
            {
                this.MeleeWeapon.Facing = MeleeWeaponSprite.SwingFacing.Undefined; // TODO protected set
                this.lastSwingFacing = MeleeWeaponSprite.SwingFacing.Undefined;
                this.startOfSwing = null;

            }
        }
    }
}
