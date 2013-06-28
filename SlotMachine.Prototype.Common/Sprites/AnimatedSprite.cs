// -----------------------------------------------------------------------
// <copyright file="AnimatedSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnimatedSprite : ISprite
    {
        private float timeCount = 0f;
        private int currentFrame = 1;

        public AnimatedSprite(string sheetContent, Vector2 location, int width, int height, float fps)
        {
            this.SheetContent = sheetContent;
            this.SpriteLocation = location;
            this.Width = width;
            this.Height = height;
            this.Interval = fps;
        }

        public Texture2D SheetSprite { get; set; }

        public string SheetContent { get; set; }

        /// <summary>
        /// Where on the screen to draw
        /// </summary>
        public Vector2 SpriteLocation { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public float Interval { get; set; }

        /// <summary>
        /// Size of frame on sheet
        /// </summary>
        private Rectangle FrameRectangle { get; set; }

        /// <summary>
        /// Frame origin from sheet
        /// </summary>
        private Vector2 SpriteOrigin { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.SheetSprite = content.Load<Texture2D>(this.SheetContent);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
            timeCount += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timeCount > this.Interval)
            {
                currentFrame++;
                this.timeCount = 0f;
            }

            if (currentFrame == 12)
            {
                currentFrame = 0;
            }

            this.FrameRectangle = new Rectangle(currentFrame * this.Width, 0, this.Height, this.Height);
            this.SpriteOrigin = Vector2.Zero; //new Vector2(this.FrameRectangle.Width / 2, this.FrameRectangle.Height / 2);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.SheetSprite, this.SpriteLocation, this.FrameRectangle, Color.White, 0f, this.SpriteOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
