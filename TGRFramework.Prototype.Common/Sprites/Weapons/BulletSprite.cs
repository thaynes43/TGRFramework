// -----------------------------------------------------------------------
// <copyright file="Ammo.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BulletSprite : ISprite
    {
        private string content;
        private Texture2D bulletTexture;
        private Vector2 bulletPosition;
        private MeleeWeaponSprite.SwingFacing facing;

        public BulletSprite(string content, Vector2 startingVector, MeleeWeaponSprite.SwingFacing facing, Vector2 bulletVelocity)
        {
            this.content = content;
            this.bulletPosition = startingVector;
            this.facing = facing;
            this.BulletVelocity = bulletVelocity;
            this.Visible = true;
        }

        public bool Visible { get; set; }

        public Rectangle BoundingBox { get; set; }

        public Vector2 BulletVelocity { get; private set; }

        public float Damage 
        {
            get
            {
                return 5f;
            }
        }

        public void LoadContent(ContentManager content)
        {
            this.bulletTexture = content.Load<Texture2D>(this.content);
            this.BoundingBox = new Rectangle((int)this.bulletPosition.X, (int)this.bulletPosition.Y, this.bulletTexture.Bounds.Width, this.bulletTexture.Bounds.Height);

            // Set velocity
            this.BulletVelocity *= 15f;
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            if (this.facing == MeleeWeaponSprite.SwingFacing.Right)
            {
                this.bulletPosition += this.BulletVelocity; //10f;
            }
            else
            {
                this.bulletPosition += this.BulletVelocity; //10f;
            }

            this.BoundingBox = new Rectangle((int)this.bulletPosition.X, (int)this.bulletPosition.Y, this.bulletTexture.Bounds.Width, this.bulletTexture.Bounds.Height);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.bulletTexture, this.bulletPosition, Color.White);
        }
    }
}
