// -----------------------------------------------------------------------
// <copyright file="PositionsDataStore.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Maintain all position data
    /// </summary>
    public class PositionsDataStore
    {
        private static PositionsDataStore instance;

        private GraphicsDevice graphics;

        private PositionsDataStore()
        {
            this.CameraPosition = new XYCoordinate(0f, 0f);
        }

        public static PositionsDataStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PositionsDataStore();
                }

                return instance;
            }
        }

        public void Initialize(GraphicsDevice graphics)
        {
            this.graphics = graphics;
        }

        public XYCoordinate CameraPosition { get; private set; }

        public Rectangle ViewportBounds 
        {
            get
            {
                return this.graphics.Viewport.Bounds;
            } 
        }

        /// <summary>
        /// Update x and y of camera position
        /// </summary>
        public void UpdateCameraPosition(float newPositionX, float newPositionY)
        {
            this.CameraPosition.X = newPositionX;
            this.CameraPosition.Y = newPositionY;
        }
    }
}
