// -----------------------------------------------------------------------
// <copyright file="Text.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TextSprite : ISprite
    {        
        public TextSprite(string output, string content, Vector2 fontPosition, float fontRotation = 0f)
        {
            this.OutputText = output;
            this.FontContent = content;
            this.FontPosition = fontPosition;
            this.FontRotation = fontRotation;
        }

        public virtual string OutputText { get; set; }

        private string FontContent { get; set; }

        private Vector2 FontPosition { get; set; }

        private float FontRotation { get; set; }

        private SpriteFont Font { get; set; }

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
            spritebatch.DrawString(this.Font, this.OutputText, updatedLocaton, Color.OrangeRed,
                this.FontRotation, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}
