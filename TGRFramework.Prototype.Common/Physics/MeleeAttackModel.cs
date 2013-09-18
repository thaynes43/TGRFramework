// -----------------------------------------------------------------------
// <copyright file="MeleeAttackModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class MeleeAttackModel : IPhysicsModel
    {
        protected MeleeWeaponSprite.SwingFacing lastSwingFacing = MeleeWeaponSprite.SwingFacing.Undefined;
        protected float? startOfSwing = null;

        public MeleeAttackModel(MeleeWeaponSprite meleeWeapon)
        {
            // Start with downswing
            this.Direction = MeleeSwipeModel.SwingDirection.Down;
            this.MeleeWeapon = meleeWeapon;
        }

        public enum SwingDirection
        {
            Up,
            Down,
        }

        public event Action UpdateSpriteComplete;

        public MeleeWeaponSprite MeleeWeapon { get; private set; }

        public SwingDirection Direction { get; protected set; }

        public float SwingSpeed { get; protected set; }

        public float SwingRange { get; protected set; }

        public float WeaponStartLeft { get; protected set; }

        public float WeaponStartRight { get; protected set; }

        public abstract void UpdateSpriteLocation();

        protected void UpdateSwingFacing()
        {
            // Swing in the direction the weapon expects
            if (this.lastSwingFacing != this.MeleeWeapon.Facing)
            {
                if (this.MeleeWeapon.Facing == MeleeWeaponSprite.SwingFacing.Right)
                {
                    this.MeleeWeapon.WeaponAngle = WeaponStartRight;
                }
                else if (this.MeleeWeapon.Facing == MeleeWeaponSprite.SwingFacing.Left)
                {
                    this.MeleeWeapon.WeaponAngle = WeaponStartLeft;
                }
            }

            this.lastSwingFacing = this.MeleeWeapon.Facing;
        }
    }
}
