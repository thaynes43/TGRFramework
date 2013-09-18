// -----------------------------------------------------------------------
// <copyright file="Platform.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    public enum PlatformType
    {
        Passable,
        Impassable,
    }
    
    /// <summary>
    /// TODO: Update summary. 
    /// </summary>
    public struct Platform
    {
        public const float Width = 15f;

        public const float Height = 15f;

        public Texture2D Texture;

        public Rectangle BoundingBox;

        public PlatformType Type;

        public Vector2 Location;

        public Platform(Texture2D texture, PlatformType type, Vector2 location)
        {
            Texture = texture;
            Type = type;
            Location = location;

            BoundingBox = new Rectangle((int)this.Location.X, (int)this.Location.Y, this.Texture.Bounds.Width, this.Texture.Bounds.Height);
        }
    }
}
