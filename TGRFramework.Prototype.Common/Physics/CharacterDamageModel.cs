// -----------------------------------------------------------------------
// <copyright file="CharacterDamageModel.cs" company="">
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
    public class CharacterDamageModel : IPhysicsModel
    {
        private const int DAMAGE_LIMITER_MS = 250; 
        private const int DAMAGE_STEPS = 25;
        private DateTime lastDamage = DateTime.Now;

        private int currentStep = 0;

        private int damageHeight = 7;
        private int damageWidth = 5;

        public CharacterDamageModel(CharacterSprite character)
        {
            this.CharacterSprite = character;
        }

        public event Action UpdateSpriteComplete;

        public CharacterSprite CharacterSprite { get; set; }

        public void UpdateSpriteLocation()
        {
            //if ((DateTime.Now - this.lastDamage).TotalMilliseconds > DAMAGE_LIMITER_MS) // Don't let spam damage occur

            if (this.currentStep < DAMAGE_STEPS)
            {
                // TODO Do we want a physics model here?
                if (this.CharacterSprite.Facing == MeleeWeaponSprite.SwingFacing.Right)
                {
                    this.CharacterSprite.TryMoveRight(this.damageWidth);
                }
                else
                {
                    this.CharacterSprite.TryMoveLeft(this.damageWidth);
                }

                if (this.currentStep < (DAMAGE_STEPS / 2))
                {
                    this.CharacterSprite.TryMoveUp(this.damageHeight);
                }
                else
                {
                    this.CharacterSprite.TryMoveDown(this.damageHeight);
                }
 
                this.currentStep++;
            }
            else
            {
                this.RaiseUpdateSpriteComplete();
                this.currentStep = 0;
            }
        }

        private void RaiseUpdateSpriteComplete()
        {
            if (this.UpdateSpriteComplete != null)
            {
                this.UpdateSpriteComplete();
            }
        }
    }
}
