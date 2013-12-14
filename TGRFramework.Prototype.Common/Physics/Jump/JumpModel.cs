// -----------------------------------------------------------------------
// <copyright file="JumpModel.cs" company="">
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
    public abstract class JumpModel : IPhysicsModel
    {
        protected readonly float MaxJumpHeight = Platform.Height * 8f;
        protected const int JumpSpeed = 10;

        public JumpModel(CharacterSprite character)
        {
            this.CharacterSprite = character;
            this.CurrentJumpHeight = 0f;
        }

        protected CharacterSprite CharacterSprite { get; set; }

        protected float CurrentJumpHeight { get; set; }

        public event Action UpdateSpriteComplete;

        public void UpdateSpriteLocation()
        {
            if (this.TakeAJump())
            {
                this.CharacterSprite.TryMoveUp(JumpSpeed);
                this.CurrentJumpHeight += JumpSpeed;
            }
            else
            {
                this.RaiseUpdateSpriteComplete();
                this.CurrentJumpHeight = 0;
            }
        }

        protected abstract bool TakeAJump();

        private void RaiseUpdateSpriteComplete()
        {
            if (this.UpdateSpriteComplete != null)
            {
                this.UpdateSpriteComplete();
            }
        } 
    }
}
