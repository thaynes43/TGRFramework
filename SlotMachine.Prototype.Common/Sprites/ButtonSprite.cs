using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SlotMachine.Prototype.Common
{
    public class ButtonSprite : ISprite
    {
        public event Action ButtonClickedEvent;
        private bool inClick = false;

        public ButtonSprite(string mouseUpContent, string mouseDownContent, Vector2 position)
        {
            this.MouseUpContent = mouseUpContent;
            this.MouseDownContent = mouseDownContent;
            this.ButtonPosition = position;
        }

        private Texture2D ButtonTexture { get; set; }

        private string MouseUpContent { get; set; }

        private string MouseDownContent { get; set; }

        private Vector2 ButtonPosition { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.ButtonTexture = content.Load<Texture2D>(this.MouseUpContent);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            if (this.MouseOverSprite(mousePosition) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!this.inClick)
                {
                    this.inClick = true;
                    this.ButtonTexture = content.Load<Texture2D>(this.MouseDownContent);
                    this.RaiseButtonClickedEvent();
                }
            }
            else
            {
                this.inClick = false;
                this.ButtonTexture = content.Load<Texture2D>(this.MouseUpContent);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.ButtonTexture, this.ButtonPosition, Color.White);
        }

        private bool MouseOverSprite(Vector2 mousePosition)
        {
            if ((mousePosition.X >= ButtonPosition.X) && mousePosition.X < (ButtonPosition.X + ButtonTexture.Width) &&
            mousePosition.Y >= ButtonPosition.Y && mousePosition.Y < (ButtonPosition.Y + ButtonTexture.Height))
                return true;
            else 
                return false;
        }

        private void RaiseButtonClickedEvent()
        {
            if (this.ButtonClickedEvent != null)
            {
                this.ButtonClickedEvent();
            }
        }
    }
}
