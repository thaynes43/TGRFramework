// -----------------------------------------------------------------------
// <copyright file="HealthOrbSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using Microsoft.Xna.Framework;
using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HealthOrbSprite : SheetSprite
    {
        private const float CollectionDelay = 500f;
        private const float DespawnTime = 5000f;

        private float lifetimeCount;

        private Random random = new Random((int)DateTime.Now.Ticks);
        private int xDirection = 1;
        private int yDirection = -1;
        PlayableCharacterSprite playableCharacter;

        public HealthOrbSprite(PlayableCharacterSprite hero, string sheetContent, Vector2 location, int width, int height, float fps, int numRows, int numCols, int xGap, int yGap)
            : base(sheetContent, location, width, height, fps, numRows, numCols, xGap, yGap)
        {
            this.playableCharacter = hero;
        }

        public event Action<HealthOrbSprite, int> IntersectsPlayableCharacter;

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, GameTime gameTime)
        {
            if (lifetimeCount <= DespawnTime) lifetimeCount += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            Rectangle thisBounds = new Rectangle((int)this.SpriteLocation.X, (int)this.SpriteLocation.Y, this.Texture.Bounds.Width, this.Texture.Bounds.Height);

            if (lifetimeCount > CollectionDelay && thisBounds.Intersects(this.playableCharacter.BoundingBox))
            {
                this.RaiseIntersectsPlayableCharacter(1);
            }
            else if (lifetimeCount > DespawnTime)
            {
                this.RaiseIntersectsPlayableCharacter(0);
            }

            int changeXDirection = random.Next(0, 10);
            int changeYDirection = random.Next(0, 10);

            if (changeXDirection == 1)
            {
                xDirection *= -1;
            }

            if (changeYDirection == 1)
            {
                yDirection *= -1;
            }

            float xAdjust = (random.Next(0, 3) * this.xDirection) + this.SpriteLocation.X;
            float yAdjust = (random.Next(0, 3) * this.yDirection) + this.SpriteLocation.Y;

            this.SpriteLocation = new Vector2(xAdjust, yAdjust);

            base.Update(content, gameTime);
        }

        private void RaiseIntersectsPlayableCharacter(int hpGain)
        {
            if (this.IntersectsPlayableCharacter != null)
            {
                this.IntersectsPlayableCharacter(this, hpGain);
            }
        }
    }
}
