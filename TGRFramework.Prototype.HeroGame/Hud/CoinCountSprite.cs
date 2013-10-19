// -----------------------------------------------------------------------
// <copyright file="CoinCountSprite.cs" company="">
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
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CoinCountSprite : ISprite
    {
        private int coinCount = 0;
        private int digitCheck = 10;
        private string contentString = string.Empty;
        private Vector2 position;
        private Texture2D texture;

        public CoinCountSprite(string content, Vector2 position)
        {
            this.Visible = true;
            this.contentString = content;
            this.position = position;

            Vector2 coinPosition = new Vector2(position.X + 30, position.Y + 15);
            this.CoinCountText = new TextSprite("0", "CoinCount", coinPosition, Color.Black);
        }

        public bool Visible { get; set; }

        public TextSprite CoinCountText { get; set; }

        public void LoadContent(ContentManager contentManager)
        {
            this.texture = contentManager.Load<Texture2D>(contentString);
            this.CoinCountText.LoadContent(contentManager);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            this.CoinCountText.Update(content, gameTime);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.texture, this.position, Color.White);
            this.CoinCountText.Draw(theSpriteBatch);
        }

        public void OnCoinCollected(CoinSprite coin)
        {
            this.coinCount++;

            // Adjust text location as string gets larger
            if (this.coinCount % digitCheck == 0)
            {
                digitCheck *= 10;
                this.CoinCountText.FontPosition.X += 10;
            }

            this.CoinCountText.OutputText = this.coinCount.ToString();
        }
    }
}
