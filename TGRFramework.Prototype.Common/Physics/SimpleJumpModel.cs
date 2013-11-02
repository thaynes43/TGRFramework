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
    public class SimpleJumpModel : IPhysicsModel
    {
        private static float maxJumpHeight = Platform.Height * 8f;

        private const int jumpSpeed = 10;

        private CharacterSprite character;

        private float currentJumpHeight = 0f;

        private GraphicsDevice graphics;

        public SimpleJumpModel(CharacterSprite character, GraphicsDevice gfx)
        {
            this.character = character;
            this.graphics = gfx;
        }

        public event Action UpdateSpriteComplete;

        public void UpdateSpriteLocation()
        {
            // Jump restrictions: jump specified height; don't allow jumping out of level constraints
            if (this.currentJumpHeight < SimpleJumpModel.maxJumpHeight && this.character.BoundingBox.Y > 0)
            {
                this.character.TryMoveUp(SimpleJumpModel.jumpSpeed);
                this.currentJumpHeight += SimpleJumpModel.jumpSpeed;
            }
            else
            {
                this.currentJumpHeight = 0;
                this.RaiseUpdateSpriteComplete();
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
