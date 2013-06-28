using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SlotMachine.Prototype.Common
{
    public class TextImgSprite
    {
        private Texture2D texture;

        // Set the coordinates to draw the sprite at.
        private Vector2 spritePosition = new Vector2(400f,150f);
        private float scale = 1f;
        bool scaleUp;
        bool pulse = true;

        public TextImgSprite()
        {
            
        }

        public void LoadContent(ContentManager content, string theAssetName)
        {
            texture = content.Load<Texture2D>(theAssetName);
        }

        public void Update(ContentManager content)
        {
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            if (pulse)
            {
                
                if (this.scale <= 1f)
                {
                    scaleUp = true;
                }
                else if (this.scale >= 2f)
                {
                    scaleUp = false;
                }

                this.scale = scaleUp ? this.scale + 0.01f : this.scale - 0.01f;
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(texture, spritePosition, null,
        Color.White, 0f, new Vector2(texture.Width/2f, texture.Height/2f), scale, SpriteEffects.None, 0f);
        }

        private bool MouseOverSprite(Vector2 mousePosition)
        {
            if ((mousePosition.X >= spritePosition.X) && mousePosition.X < (spritePosition.X + texture.Width) &&
            mousePosition.Y >= spritePosition.Y && mousePosition.Y < (spritePosition.Y + texture.Height))
                return true;
            else 
                return false;
        }
    }


}
