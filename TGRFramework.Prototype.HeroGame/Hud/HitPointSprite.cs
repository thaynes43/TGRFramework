// -----------------------------------------------------------------------
// <copyright file="HitPointSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.HeroGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HitPointSprite : ISprite
    {
        private string content;
        private Vector2 position;

        private int hitCount = 0;
        private const int MAX_HITS = 3;

        private ContentManager contentManager;

        public HitPointSprite(string content, Vector2 startingPostion)
        {
            this.content = content;
            this.position = startingPostion;
            this.Visible = true;
        }

        public bool Visible { get; set; }

        public Texture2D Texture { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.Texture = this.Texture = content.Load<Texture2D>(this.content);
            this.contentManager = content;
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.Texture, this.position, Color.White);
        }

        public bool TakeHit()
        {
            this.hitCount++;

            if (hitCount == 1)
            {
                this.Texture = this.Texture = this.contentManager.Load<Texture2D>("HP2");
            }
            else if (hitCount == 2)
            {
                this.Texture = this.Texture = this.contentManager.Load<Texture2D>("HP1");
            }

            if (hitCount >= MAX_HITS)
            {
                this.Visible = false;
                return true;
            }

            return false;
        }
    }
}
