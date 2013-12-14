// -----------------------------------------------------------------------
// <copyright file="SimpleJumpModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SimpleJumpModel : JumpModel
    {
        public SimpleJumpModel(CharacterSprite character)
            : base(character)
        {
        }

        protected override bool TakeAJump()
        {
            // Jump restrictions: jump specified height; don't allow jumping out of level constraints
            return this.CurrentJumpHeight < MaxJumpHeight && this.CharacterSprite.BoundingBox.Y > 0;
        }
    }
}
