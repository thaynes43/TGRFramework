﻿// -----------------------------------------------------------------------
// <copyright file="SymbolSprite.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using TGRFramework.Prototype.Common;
    using TGRFramework.Prototype.Tools;

    /// <summary>
    /// Update summary.
    /// </summary>
    /// // TODO Make Sprite baseclass with 3 methods and 2 properties...
    public class SymbolSprite : ISprite
    {
        public SymbolSprite(ILogTool log, string content, float minY, float maxY)
        {
            this.Visible = true;
            this.Log = log;
            this.SymbolContent = content;

            this.MinY = minY;
            this.MaxY = maxY;
        }

        public bool Visible { get; set; }

        // TODO Symbol sprite should have ref to symbol
        public string SymbolContent { get; set; }

        public Texture2D SymbolTexture { get; set; }

        public Vector2 SymbolPosition { get; set; }

        public System.Threading.Timer SpinTimer { get; set; }

        public float MinY { get; set; }

        public float MaxY { get; set; }

        private ILogTool Log { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.SymbolTexture = content.Load<Texture2D>(this.SymbolContent);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.SymbolTexture, this.SymbolPosition, Color.White);
        }
    }
}
