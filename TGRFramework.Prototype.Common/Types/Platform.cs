// -----------------------------------------------------------------------
// <copyright file="Platform.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.IO;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

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
        public static Int16 Width = 15;

        public static Int16 Height = 15;

        public Texture2D Texture; // TODO_HIGH link Texture2D with ContentID

        public string ContentID;

        public Rectangle BoundingBox;

        public PlatformType Type;

        public Vector2 Location;

        // TODO_NEXT I need to preserve this so I can load in from txt - then gut once I know I can load in from binary
        public Platform(string content, Texture2D texture, PlatformType type, Vector2 location)
        {
            this.Texture = texture;
            this.ContentID = content;
            this.Type = type;
            this.Location = location;

            this.BoundingBox = new Rectangle((int)this.Location.X, (int)this.Location.Y, this.Texture.Bounds.Width, this.Texture.Bounds.Height);
        }

        public Platform(PlatformType type, Vector2 location)
        {
            this.Type = type;
            this.Location = location;

            this.Texture = null;
            this.ContentID = string.Empty;
            this.BoundingBox = new Rectangle((int)this.Location.X, (int)this.Location.Y, (int)Width, (int)Height);
        }

        /// <summary>
        /// Binary loadable constructor
        /// </summary>
        public Platform(string content, Int16 width, Int16 height, PlatformType type, Vector2 location)
        {
            this.Texture = null; // TODO_NEXT when do we want to this?
            this.ContentID = content;

            this.BoundingBox = new Rectangle((int)location.X, (int)location.Y, width, height);
            this.Type = type;
            this.Location = location;

            Platform.Width = width;
            Platform.Height = height;
        }

        public void LoadContent(ContentManager content)
        {
            if (this.ContentID != string.Empty)
            {
                this.Texture = content.Load<Texture2D>(this.ContentID);
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(this.ContentID);
            writer.Write(this.Type.ToString());
            writer.Write(this.Location.X);
            writer.Write(this.Location.Y);
            writer.Write(Platform.Width);
            writer.Write(Platform.Height);
        }

        public static Platform Load(BinaryReader reader)
        {
            string content = reader.ReadString();
            string type = reader.ReadString();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            Int16 width = reader.ReadInt16();
            Int16 height = reader.ReadInt16();
            return new Platform(content, width, height, (PlatformType)Enum.Parse(typeof(PlatformType), type), new Vector2(x, y));;
        }

        public override string ToString()
        {
            return string.Format("{0} Platform, Content={1}, Location={2}", this.Type, this.ContentID, this.Location);
        }
    }
}
