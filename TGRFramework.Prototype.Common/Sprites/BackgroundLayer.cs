// -----------------------------------------------------------------------
// <copyright file="BackgroundLayer.cs" company="">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BackgroundLayer : ISprite
    {
        private string contentID;
        private Texture2D[] texture = new Texture2D[2];
        private float scrollRate;
        private int repeatX = 0;
        private int repeatY = 0;

        private float maxY;

        public BackgroundLayer(string content, GraphicsDevice gfx, float scrollRate, float maxY = float.MaxValue)
        {
            this.contentID = content;
            this.scrollRate = scrollRate;
            this.Graphics = gfx;
            this.maxY = maxY;
        }

        public bool Visible { get; set; }

        public GraphicsDevice Graphics { get; set; }

        public void LoadContent(ContentManager content)
        {
            // Assumes one of textures will cover all viewable space
            this.texture[0] = content.Load<Texture2D>(this.contentID);
            this.texture[1] = content.Load<Texture2D>(this.contentID);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            float segmentWidth = texture[0].Width;
            float segmentHeight = texture[0].Height;

            int viewWidth = this.Graphics.Viewport.Width;
            int viewHeight = this.Graphics.Viewport.Width;

            float x = PositionsDataStore.Instance.CameraPosition.X * this.scrollRate;
            float y = PositionsDataStore.Instance.CameraPosition.Y * this.scrollRate;

            // Number of segments required to cover the viewspace 
            int requiredSegmentsX = (int)Math.Ceiling(viewWidth / segmentWidth) + 1;
            int requiredSegmentsY = (int)Math.Ceiling(viewHeight / segmentHeight) + 1;

            Vector2[,] segments = new Vector2[requiredSegmentsX, requiredSegmentsY];

            // Add repeating segments
            for (int i = 0; i < requiredSegmentsX; i++)
            {
                for (int j = 0; j < requiredSegmentsY; j++)
                {
                    segments[i,j] = new Vector2(x + ((this.repeatX + i) * segmentWidth), y + ((this.repeatY + j) * segmentHeight));
                }
            }

            // Segment bounds to determine if repeat is necessary based off of camera position
            float maxSegmentX = segments[requiredSegmentsX - 1, 0].X;
            float maxSegmentY = segments[0, requiredSegmentsY - 1].Y;
            float minSegmentX = segments[0, 0].X;
            float minSegmentY = segments[0, 0].Y;

            // Check if texture needs to repeat horizontally
            if (PositionsDataStore.Instance.CameraPosition.X > maxSegmentX + segmentWidth - viewWidth)
            {
                // repeat to the right
                repeatX++;

                // Swap textures for smooth transition TODO - allow n textures 
                //Texture2D temp = this.texture[0];
                //this.texture[0] = this.texture[1];
                //this.texture[1] = this.texture[0];

                PlatformerLevel.Log.Debug("Repeat to right for X Axis. RepeatX = {0}", this.repeatX);
            }
            else if (PositionsDataStore.Instance.CameraPosition.X < minSegmentX)
            {
                // repeat to the left - go back one
                repeatX--;
                PlatformerLevel.Log.Debug("Repeat to left for X Axis. RepeatX = {0}", this.repeatX);
            }

            // Check if texture needs to repeat vertically
            if (PositionsDataStore.Instance.CameraPosition.Y > maxSegmentY + segmentHeight - viewHeight)
            {
                // repeat below
                repeatY++;
                PlatformerLevel.Log.Debug("Repeat below for Y Axis. RepeatY = {0}", this.repeatY);
            }
            else if (PositionsDataStore.Instance.CameraPosition.Y < minSegmentY)
            {
                // repeat above - go back one
                repeatY--;
                PlatformerLevel.Log.Debug("Repeat up for Y Axis. RepeatY = {0}", this.repeatY);
            }

            // TODO - Different textured segments
            for (int i = 0; i < requiredSegmentsX; i++)
            {
                for (int j = 0; j < requiredSegmentsY; j++)
                {
                    if (segments[i, j].Y < this.maxY) // TODO - refactor this restriction on draw
                    {
                        if (j < 1) // TODO - refactor this Hack for multiple textures
                        {
                            theSpriteBatch.Draw(this.texture[0], segments[i, j], Color.White);
                        }
                        else
                        {
                            theSpriteBatch.Draw(this.texture[1], segments[i, j], Color.White);
                        }
                    }
                }
            }
        }
    }
}
