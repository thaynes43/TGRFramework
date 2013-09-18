// -----------------------------------------------------------------------
// <copyright file="Level.cs" company="">
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
    using System.IO;
using Microsoft.Xna.Framework.Audio;
using System.Threading.Tasks;
    using TGRFramework.Prototype.Tools;
    using System.Collections.Concurrent;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PlatformerLevel : ISprite
    {
        public PlatformerLevel(string levelMap, GraphicsDevice gfx)
        {
            this.Visible = true;
            this.Graphics = gfx;
            // Load these with content
            this.Platforms = null;
            this.LevelMap = levelMap;

            this.EnemyVectors = new List<Vector2>();
            this.LevelSoundQueue = new BlockingCollection<SoundEffect>();
            this.FilterLevelSounds(); // $$$
        }

        // TODO is this the right approach for this data? Maybe add to datastore
        public static float LevelWidth { get; private set; }

        public static float LevelHeight { get; private set; }

        public static float LevelWidthAdjustment { get; set; }

        public static float LevelHeightAdjustment { get; set; }

        public static float CameraPositionX { get; set; }

        public static float CameraPositionY { get; set; }

        public static ILogTool Log;

        public bool Visible { get; set; }

        public List<Vector2> EnemyVectors { get; set; }

        public BlockingCollection<SoundEffect> LevelSoundQueue { get; private set; }

        private string LevelMap { get; set; }

        private Platform[,] Platforms { get; set; }

        private GraphicsDevice Graphics { get; set; }

        private int PlatformsWide
        {
            get
            {
                return this.Platforms.GetLength(0);
            }
        }

        private int PlatformsHigh
        {
            get
            {
                return this.Platforms.GetLength(1);
            }
        }

        public void LoadContent(ContentManager content)
        {           
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            // Calculate the visible range of tiles.
            int left = (int)Math.Floor(PlatformerLevel.CameraPositionX / (int)Platform.Width);
            int right = left + (theSpriteBatch.GraphicsDevice.Viewport.Width / (int)Platform.Width) + 3; // TODO Draw 3 extra  
            right = Math.Min(right, this.PlatformsWide);

            int top = (int)Math.Floor(PlatformerLevel.CameraPositionY / (int)Platform.Height);
            int bottom = top + (theSpriteBatch.GraphicsDevice.Viewport.Height / (int)Platform.Height) + 3; // TODO Draw 3 extra
            bottom = Math.Min(bottom, this.PlatformsHigh);

            // Draw Platforms
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    // Don't try to draw air
                    if (this.Platforms[i, j].Texture != null)
                    {
                        theSpriteBatch.Draw(this.Platforms[i, j].Texture, this.Platforms[i, j].Location, Color.White);
                    }
                }
            }
        }

        public void LoadLevel(ContentManager content)
        {
            // Initialize Platform array
            int height = File.ReadAllLines(this.LevelMap).Count();
            int width = 0;
            File.ReadAllLines(this.LevelMap).ToList().ForEach(s => { if (s.Length > width) width = s.Length; });

            this.Platforms = new Platform[width, height];

            using (StreamReader reader = new StreamReader(this.LevelMap))
            {
                Vector2 location = Vector2.Zero;
                string line;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        this.Platforms[i, count] = this.GetPlatform(line[i], location, content);
                        location.X += Platform.Width;
                    }

                    location.X = Vector2.Zero.X;
                    location.Y += Platform.Height;
                    count++;
                }
            }

            PlatformerLevel.LevelWidth = this.PlatformsWide * Platform.Width;
            PlatformerLevel.LevelHeight = this.PlatformsHigh * Platform.Height;
        }

        // TODO_Optimization combine IsStep, IsOnGround, and IntersectsImpassible
        public bool IsAtStep(Rectangle bounds)
        {
            // Only check platforms intersecting with the sprite
            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            this.GetExactIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, bounds);

            // Character is at a step if a platform one over and one up has air above it
            if (xMin - 1 >= 0 && xMax + 1 <= this.PlatformsWide)
            {
                for (int i = xMin - 1; i <= xMax + 1; i++)
                {
                    if (yMax - 2 >= 0)
                    {
                        if (this.Platforms[i, yMax - 1].Type == PlatformType.Impassable && this.Platforms[i, yMax - 2].Type == PlatformType.Passable)
                        {
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsAtCliff(Rectangle bounds, MeleeWeaponSprite.SwingFacing facing)
        {
            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            this.GetExactIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, bounds);

            // Allow large steps down to compensate for wider sprite (arm)
            if (facing == MeleeWeaponSprite.SwingFacing.Right)
            {
                if (xMax + 1 < this.PlatformsWide && yMax + 3 < this.PlatformsHigh)
                {
                    if (this.Platforms[xMax + 1, yMax].Type == PlatformType.Passable &&
                        this.Platforms[xMax + 1, yMax + 1].Type == PlatformType.Passable &&
                        this.Platforms[xMax + 1, yMax + 2].Type == PlatformType.Passable &&
                        this.Platforms[xMax + 1, yMax + 3].Type == PlatformType.Passable)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (xMin - 1 > 0 && yMax + 3 < this.PlatformsHigh)
                {
                    if (this.Platforms[xMin - 1, yMax].Type == PlatformType.Passable &&
                        this.Platforms[xMin - 1, yMax + 1].Type == PlatformType.Passable &&
                        this.Platforms[xMin - 1, yMax + 2].Type == PlatformType.Passable &&
                        this.Platforms[xMin - 1, yMax + 3].Type == PlatformType.Passable)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsOnGround(Rectangle bounds)
        {
            // Only check platforms intersecting with the sprite
            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            this.GetExactIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, bounds);

            // Bounds is on ground if there is only air above the lowest intersecting platforms
            for (int i = xMin; i <= xMax; i++)
            {
                if (this.Platforms[i, yMax].Type == PlatformType.Impassable && this.Platforms[i, yMax - 1].Type == PlatformType.Passable)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IntersectsImpassiblePlatform(Rectangle bounds)
        {
            // Only check platforms intersecting with the sprite
            // TODO See why GetExact doesn't work here
            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            this.GetIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, bounds);

            // Do not allow sprites to move through impassible platforms
            // Acts as a safety for IsOnGround and IsStep movements
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    // TODO_Enhancement will need to modify if we want 'passable ground' which the toon can move through
                    if (this.Platforms[i, j].BoundingBox.Intersects(bounds) && this.Platforms[i, j].Type == PlatformType.Impassable && !this.TouchingGround(this.Platforms[i, j].BoundingBox, bounds)) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<Rectangle> GetAreaRectangleFits(Rectangle areaToCheck, Rectangle characterSize)
        {
            List<Rectangle> possibleSpawns = new List<Rectangle>();

            if (areaToCheck.X < 0 || areaToCheck.Y < 0)
            {
                return possibleSpawns;
            }

            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            this.GetExactIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, areaToCheck);


            int widthPlatforms = (int)Math.Ceiling((double)characterSize.Width / Platform.Width);
            int heightPlatforms = (int)Math.Ceiling((double)characterSize.Height / Platform.Height);

            // Check entire area for possible ground
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    // Check surroundings
                    if (i + widthPlatforms < xMax && j + heightPlatforms < yMax)
                    {
                        bool found = true;
                        for (int y = j; y < j + heightPlatforms; y++)
                        {
                            for (int x = i; x < i + widthPlatforms; x++)
                            {
                                if (y == j + heightPlatforms - 1)
                                {
                                    // Check for ground
                                    if (this.Platforms[x, y].Type != PlatformType.Impassable)
                                    {
                                        found = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    // Check for air
                                    if (this.Platforms[x, y].Type != PlatformType.Passable)
                                    {
                                        found = false;
                                        break;
                                    }
                                }
                            }
                            if (!found)
                            {
                                break;
                            }
                        }
                        if (found)
                        {
                            //possibleSpawns.Add(new Rectangle(i * (int)Platform.Width + characterSize.Width, j * (int)Platform.Height - characterSize.Height, characterSize.Width, characterSize.Height));
                            possibleSpawns.Add(new Rectangle(i * (int)Platform.Width, j * (int)Platform.Height, characterSize.Width, characterSize.Height));
                        }
                    }
                }
            }

            return possibleSpawns;
        }

        private Platform GetPlatform(char c, Vector2 location, ContentManager content)
        {
            switch (c)
            {
                case '.':
                    {
                        return new Platform(content.Load<Texture2D>("LightBluePlatform"), PlatformType.Passable, location);
                    }
                case '+':
                    {
                        this.EnemyVectors.Add(new Vector2(location.X, location.Y - Platform.Height));
                        return new Platform(content.Load<Texture2D>("LightBluePlatform"), PlatformType.Passable, location);
                    }
                case ',':
                    {
                        return new Platform(content.Load<Texture2D>("Underground"), PlatformType.Impassable, location);
                    }
                case '_':
                    {
                        return new Platform(content.Load<Texture2D>("Ground"), PlatformType.Impassable, location);
                    }
                case '<':
                    {
                        return new Platform(content.Load<Texture2D>("GroundLeftEdge"), PlatformType.Impassable, location);
                    }
                case '>':
                    {
                        return new Platform(content.Load<Texture2D>("GroundRightEdge"), PlatformType.Impassable, location);
                    }
            }

            return new Platform();
        }

        /// <summary>
        /// Get Platform index range for array which correspond with a given rectangle
        /// TODO can't use 'exact' indices for IsOnGround
        /// </summary>
        private void GetIntersectingPlatformIndices(ref int xMin, ref int xMax, ref int yMin, ref int yMax, Rectangle bounds)
        {
            xMin = (int)Math.Floor((float)bounds.X / Platform.Width);
            xMax = (int)Math.Ceiling(xMin + (bounds.Width / Platform.Width)) + 1;

            yMin = (int)Math.Floor((float)bounds.Y / Platform.Height); // TODO floor made robots not on ground
            yMax = (int)Math.Ceiling(yMin + (bounds.Height / Platform.Height)) + 1;

            // Protect against going outside bounds of array
            xMin = Math.Max(xMin, 0);
            yMin = Math.Max(yMin, 0);

            xMax = Math.Min(xMax, this.PlatformsWide);
            yMax = Math.Min(yMax, this.PlatformsHigh);
        }

        /// <summary>
        /// Get Platform index range for array which correspond with a given rectangle
        /// </summary>
        private void GetExactIntersectingPlatformIndices(ref int xMin, ref int xMax, ref int yMin, ref int yMax, Rectangle bounds)
        {
            float xMinF = ((float)bounds.X / Platform.Width);
            float xMaxF = (xMinF + (bounds.Width / Platform.Width));

            float yMinF = ((float)bounds.Y / Platform.Height);
            float yMaxF = (yMinF + (bounds.Height / Platform.Height));

            // Truncate after adding min to max
            xMin = (int)xMinF;
            xMax = (int)xMaxF;
            yMin = (int)yMinF;
            yMax = (int)yMaxF;

            // Protect against going outside bounds of array
            xMin = Math.Max(xMin, 0);
            yMin = Math.Max(yMin, 0);

            xMax = Math.Min(xMax, this.PlatformsWide);
            yMax = Math.Min(yMax, this.PlatformsHigh);
        }

        /// <summary>
        /// Get Platform index range for array which correspond with a given rectangle
        /// </summary>
        private void GetTruncatedIntersectingPlatformIndices(ref int xMin, ref int xMax, ref int yMin, ref int yMax, Rectangle bounds)
        {
            xMin = (int)((float)bounds.X / Platform.Width);
            xMax = (int)(xMin + (bounds.Width / Platform.Width));

            yMin = (int)((float)bounds.Y / Platform.Height);
            yMax = (int)(yMin + (bounds.Height / Platform.Height));

            // Protect against going outside bounds of array
            xMin = Math.Max(xMin, 0);
            yMin = Math.Max(yMin, 0);

            xMax = Math.Min(xMax, this.PlatformsWide);
            yMax = Math.Min(yMax, this.PlatformsHigh);
        }

        private bool TouchingGround(Rectangle platform, Rectangle character)
        {
            //return platform.Y - 10 < character.Y + character.Height && platform.Y + 10 > character.Y + character.Height;
            return platform.Y - (Platform.Height - 1) < character.Y + character.Height && platform.Y + (Platform.Height - 1) > character.Y + character.Height;
        }


        // TODO Own class?
        // TODO ILevelSoundManager owned here
        private string lastSoundName = string.Empty;
        private DateTime lastSoundPlayTime = DateTime.Now;

        // Do not play sounds over each other
        private void FilterLevelSounds()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    SoundEffect nextSound = this.LevelSoundQueue.Take();
                    if (nextSound != null)
                    {
                        bool playsound = false;

                        if ((DateTime.Now - this.lastSoundPlayTime).TotalMilliseconds <= 50)
                        {
                            if (nextSound.Name != this.lastSoundName)
                            {
                                playsound = true;
                            }
                        }
                        else
                        {
                            playsound = true;
                        }

                        if (playsound)
                        {
                            this.lastSoundPlayTime = DateTime.Now;
                            this.lastSoundName = nextSound.Name;
                            nextSound.Play();
                        }
                    }
                }
            });
        }
    }
}
