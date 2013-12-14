// -----------------------------------------------------------------------
// <copyright file="AdvancedJumpModel.cs" company="">
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
    public class RestrictedJumpModel : SimpleJumpModel
    {
        private const float JumpHightRestriction = 400f;

        public RestrictedJumpModel(CharacterSprite character)
            : base(character)
        {
        }

        protected override bool TakeAJump()
        {
            return this.CurrentJumpHeight + JumpSpeed < JumpHightRestriction && base.TakeAJump();
        }
    }
}
