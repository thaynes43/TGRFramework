﻿// -----------------------------------------------------------------------
// <copyright file="BackgroundSprite.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Update summary.
    /// </summary>
    public class BackgroundSprite : ISprite
    {
        public BackgroundSprite(string content, GraphicsDevice graphics)
        {
            this.Visible = true;
            this.BackgroundContent = content;
            this.GraphicsDevice = graphics;
        }

        public bool Visible { get; set; }

        private string BackgroundContent { get; set; }

        private Texture2D BackgroundTexture { get; set; }

        private Rectangle MainWindowSize { get; set; }

        private GraphicsDevice GraphicsDevice { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.BackgroundTexture = content.Load<Texture2D>(this.BackgroundContent);
            this.MainWindowSize = new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            // Nothing for now
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.BackgroundTexture, this.MainWindowSize, Color.White);
        }
    }
}
