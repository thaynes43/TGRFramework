// -----------------------------------------------------------------------
// <copyright file="SimpleFallModel.cs" company="">
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
    public class SimpleFallModel : IPhysicsModel
    {
        private const int fallSpeed = 10;

        private CharacterSprite character;

        public SimpleFallModel(CharacterSprite character)
        {
            this.character = character;
        }

        public event Action UpdateSpriteComplete;

        public void UpdateSpriteLocation()
        {
            this.character.TryMoveDown(SimpleFallModel.fallSpeed);
        }
    }
}
