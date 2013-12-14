// -----------------------------------------------------------------------
// <copyright file="AnimatedSprite.cs" company="">
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
    public class SheetSprite : ISprite
    {
        private float timeCount = 0f;
        private int currentRow = 1;
        private int currentCol = 1;
        
        public SheetSprite(string sheetContent, Vector2 location, int width, int height, float fps, int numRows, int numCols, int xGap, int yGap)
        {
            this.Visible = true;
            this.SheetContent = sheetContent;
            this.SpriteLocation = location;
            this.Width = width;
            this.Height = height;
            this.Interval = fps;
            this.NumRows = numRows;
            this.NumColumns = numCols;
            this.HorizontalGap = xGap;
            this.VerticalGap = yGap;
        }

        public bool Visible { get; set; }

        public Texture2D Texture { get; set; }

        public string SheetContent { get; set; }

        /// <summary>
        /// Where on the screen to draw
        /// </summary>
        public Vector2 SpriteLocation;

        public int Width { get; set; }

        public int Height { get; set; }

        public int NumRows { get; set; }

        public int NumColumns { get; set; }

        public int HorizontalGap { get; set; }

        public int VerticalGap { get; set; }

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
            this.Texture = content.Load<Texture2D>(this.SheetContent);

        }

        public virtual void Update(ContentManager content, GameTime gameTime)
        {
            timeCount += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timeCount > this.Interval)
            {
                this.currentRow++;
                this.timeCount = 0f;
            }

            if (this.currentRow == this.NumRows)
            {
                this.currentRow = 1;

                if (this.currentCol < this.NumColumns)
                    this.currentCol++;
                else
                    this.currentCol = 1;
            }

            int x = ((this.currentRow - 1) * this.HorizontalGap) + ((this.currentRow - 1) * this.Width);
            int y = ((this.currentCol - 1) * this.VerticalGap) + ((this.currentCol - 1) * this.Height);

            this.FrameRectangle = new Rectangle(x, y, this.Width, this.Height);
            this.SpriteOrigin = Vector2.Zero; //new Vector2(this.FrameRectangle.Width / 2, this.FrameRectangle.Height / 2);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.Texture, this.SpriteLocation, this.FrameRectangle, Color.White, 0f, this.SpriteOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
