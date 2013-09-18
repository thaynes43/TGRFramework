// -----------------------------------------------------------------------
// <copyright file="CoinSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CoinSprite : CharacterSprite
    {
        private SoundEffect collectedSound;
        private IPhysicsModel spawnMoveModel;

        private bool isJustSpawned = true;

        public CoinSprite(PlayableCharacterSprite hero, string content, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(content, startingPostion, movementSpeed, gfx, level)
        {
            this.PlayableCharacterSprite = hero;
            this.Facing = this.PlayableCharacterSprite.Facing;
            //this.CharacterPosition.Y -= 30f;

            this.spawnMoveModel = new CharacterDamageModel(this);
            this.spawnMoveModel.UpdateSpriteComplete += () => { this.isJustSpawned = false; };
        }

        public event Action<CoinSprite> CoinCollected;

        public PlayableCharacterSprite PlayableCharacterSprite { get; set; }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            this.collectedSound = content.Load<SoundEffect>("Coin1");
            this.collectedSound.Name = "Coin1";

            base.LoadContent(content);
        }

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, GameTime gameTime)
        {
            if (this.isJustSpawned)
            {
                this.spawnMoveModel.UpdateSpriteLocation();
            }
            else if (this.BoundingBox.Intersects(this.PlayableCharacterSprite.BoundingBox))
            {
                this.Level.LevelSoundQueue.Add(this.collectedSound);

                this.RaiseCoinCollected();
            }

            
            base.Update(content, gameTime);
        }

        /// <summary>
        /// Override to allow coins to move down if they are in an impassible area.
        /// This is so coins do not get stuck in walls.
        /// </summary>
        public override bool TryMoveDown(int speed)
        {
            if (!this.Level.IsOnGround(this.BoundingBox))
            {
                this.CharacterPosition.Y += speed;
                this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
            }
            return true;
        }

        private void RaiseCoinCollected()
        {
            if (this.CoinCollected != null)
            {
                this.CoinCollected(this);
            }
        }
    }
}
