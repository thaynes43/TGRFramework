// -----------------------------------------------------------------------
// <copyright file="MouseCursorSprite.cs" company="">
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
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MouseCursorSprite : ISprite
    {
        private Texture2D texture;
        private Vector2 position;
        private string contentString;

        public MouseCursorSprite(string content)
        {
            this.contentString = content;
            this.Visible = true;
        }

        public bool Visible { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.texture = content.Load<Texture2D>(this.contentString);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            this.position = new Vector2(Mouse.GetState().X + PositionsDataStore.Instance.CameraPosition.X, Mouse.GetState().Y + PositionsDataStore.Instance.CameraPosition.Y);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.texture, this.position, Color.White);
        }
    }
}
