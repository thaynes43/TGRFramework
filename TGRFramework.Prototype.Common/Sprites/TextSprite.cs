// -----------------------------------------------------------------------
// <copyright file="Text.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Update summary.
    /// </summary>
    public class TextSprite : ISprite
    {
        public Vector2 FontPosition;

        public TextSprite(string output, string content, Vector2 fontPosition, Color color, float fontRotation = 0f)
        {
            this.Visible = true;
            this.OutputText = output;
            this.FontContent = content;
            this.FontPosition = fontPosition;
            this.FontRotation = fontRotation;
            this.Color = color;
        }

        public bool Visible { get; set; }

        public virtual string OutputText { get; set; }

        private string FontContent { get; set; }

        private float FontRotation { get; set; }

        private SpriteFont Font { get; set; }

        private Color Color { get; set; }

        public virtual void LoadContent(ContentManager content)
        {
            this.Font = content.Load<SpriteFont>(this.FontContent);
        }

        public virtual void Update(ContentManager content, GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spritebatch)
        {
            // Find the center of the string
            Vector2 fontOrigin = Font.MeasureString(this.OutputText) / 2;

            // Draw the string
            Vector2 updatedLocaton = new Vector2(this.FontPosition.X - (this.Font.MeasureString(this.OutputText).X / 2), this.FontPosition.Y - (this.Font.MeasureString(this.OutputText).Y /2));
            spritebatch.DrawString(this.Font, this.OutputText, updatedLocaton, this.Color,
                this.FontRotation, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}
