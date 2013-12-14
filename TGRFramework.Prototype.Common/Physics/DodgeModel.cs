// -----------------------------------------------------------------------
// <copyright file="DodgeModel.cs" company="">
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
    public class DodgeModel : IPhysicsModel
    {
        private CharacterSprite character;
        private PlayableCharacterSprite hero;

        private Random random = new Random(DateTime.Now.Millisecond);

        public DodgeModel(CharacterSprite character, PlayableCharacterSprite hero)
        {
            this.character = character;
            this.hero = hero;
        }

        public event Action UpdateSpriteComplete;

        public void UpdateSpriteLocation()
        {
            int move = random.Next(0, 2);

            if (move == 0)
            {
                this.character.TryMoveRight(100);
            }
            else if (move == 1)
            {
                this.character.TryMoveLeft(100);
            }

            //if (CharacterUtil.PlayerToTheRight(this.hero.BoundingBox, this.character.CharacterTexture.Width, new Vector2(this.character.BoundingBox.X, this.character.BoundingBox.Y)))
            //{
            //    this.character.TryMoveRight(100);
            //}
            //else if (CharacterUtil.PlayerToTheLeft(this.hero.BoundingBox, new Vector2(this.character.BoundingBox.X, this.character.BoundingBox.Y)))
            //{
            //    this.character.TryMoveLeft(100);
            //}

            this.RaiseUpdateSpriteComplete();
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
