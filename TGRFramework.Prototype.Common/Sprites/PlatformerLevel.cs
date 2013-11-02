// -----------------------------------------------------------------------
// <copyright file="Level.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using TGRFramework.Prototype.Tools;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PlatformerLevel : ISprite
    {
        public static float GROUND_LINE = Platform.Height * 105;

        public PlatformerLevel(string levelMap, GraphicsDevice gfx)
        {
            this.Visible = true;
            this.Graphics = gfx;
            this.PlatformMap = null; // Load platforms with content
            this.LevelMap = levelMap;
            this.EnemyVectors = new List<Vector2>();
            this.LevelSoundQueue = new BlockingCollection<SoundEffect>();
            this.FilterLevelSounds();
        }

        // TODO add to datastore
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

        public BackgroundLayer[] Background { get; private set; }

        private string LevelMap { get; set; }

        protected PlatformMap PlatformMap { get; set; }

        protected GraphicsDevice Graphics { get; set; }

        public virtual void LoadContent(ContentManager content)
        {           
        }

        // TODO - do we need both?
        public void LoadLevel(ContentManager content)
        {
            this.Background = new BackgroundLayer[3];
            this.Background[0] = new BackgroundLayer("BGLong", this.Graphics, 0.9f);
            this.Background[0].LoadContent(content);

            this.Background[1] = new BackgroundLayer("Mountains", this.Graphics, 0.75f);
            this.Background[1].LoadContent(content);

            this.Background[2] = new BackgroundLayer("CloudTest", this.Graphics, 0.25f, PlatformerLevel.GROUND_LINE - 600f);
            this.Background[2].LoadContent(content);

            // Load from binary
            this.PlatformMap = PlatformMap.Load("testLevel", content);

            PlatformerLevel.LevelWidth = this.PlatformMap.PlatformsWide * Platform.Width;
            PlatformerLevel.LevelHeight = this.PlatformMap.PlatformsHigh * Platform.Height;

            return; // TODO_HIGH gut loading from txt after debugging

            // Initialize Platform array
            int height = File.ReadAllLines(this.LevelMap).Count();
            int width = 0;
            File.ReadAllLines(this.LevelMap).ToList().ForEach(s => { if (s.Length > width) width = s.Length; });

            Platform[,] tempPlatforms = new Platform[width, height];

            using (StreamReader reader = new StreamReader(this.LevelMap))
            {
                Vector2 location = Vector2.Zero;
                string line;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        tempPlatforms[i, count] = this.GetPlatform(line[i], location, content);
                        location.X += Platform.Width;
                    }

                    location.X = Vector2.Zero.X;
                    location.Y += Platform.Height;
                    count++;
                }
            }

            // TODO_NEXT load in what gets saved
            this.PlatformMap = new PlatformMap(1, "diagnosticLevel", tempPlatforms, (short)width, (short)height);
            this.PlatformMap.Save("testLevel");

            PlatformerLevel.LevelWidth = this.PlatformMap.PlatformsWide * Platform.Width;
            PlatformerLevel.LevelHeight = this.PlatformMap.PlatformsHigh * Platform.Height;
        }

        public virtual void Update(ContentManager content, GameTime gameTime)
        {
            // Level will not change
        }

        public virtual void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (BackgroundLayer bg in this.Background)
            {
                bg.Draw(theSpriteBatch);
            }

            // Calculate the visible range of tiles.
            int left = (int)Math.Floor(PlatformerLevel.CameraPositionX / (int)Platform.Width);
            int right = left + (theSpriteBatch.GraphicsDevice.Viewport.Width / (int)Platform.Width) + 3; // TODO Draw 3 extra  
            right = Math.Min(right, this.PlatformMap.PlatformsWide);

            int top = (int)Math.Floor(PlatformerLevel.CameraPositionY / (int)Platform.Height);
            int bottom = top + (theSpriteBatch.GraphicsDevice.Viewport.Height / (int)Platform.Height) + 3; // TODO Draw 3 extra
            bottom = Math.Min(bottom, this.PlatformMap.PlatformsHigh);

            // Draw Platforms
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    // Don't try to draw air
                    if (this.PlatformMap.Platforms[i, j].Texture != null)
                    {
                        theSpriteBatch.Draw(this.PlatformMap.Platforms[i, j].Texture, this.PlatformMap.Platforms[i, j].Location, Color.White);
                    }
                }
            }
        }

        // TODO_Optimization combine IsStep, IsOnGround, and IntersectsImpassible
        public bool IsAtStep(Rectangle bounds)
        {
            // Only check platforms intersecting with the sprite
            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            this.GetExactIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, bounds);

            // Character is at a step if a platform one over and one up has air above it
            if (xMin - 1 >= 0 && xMax + 1 <= this.PlatformMap.PlatformsWide)
            {
                for (int i = xMin - 1; i <= xMax + 1; i++)
                {
                    if (yMax - 2 >= 0)
                    {
                        if (this.PlatformMap.Platforms[i, yMax - 1].Type == PlatformType.Impassable && this.PlatformMap.Platforms[i, yMax - 2].Type == PlatformType.Passable)
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
                if (xMax + 1 < this.PlatformMap.PlatformsWide && yMax + 3 < this.PlatformMap.PlatformsHigh)
                {
                    if (this.PlatformMap.Platforms[xMax + 1, yMax].Type == PlatformType.Passable &&
                        this.PlatformMap.Platforms[xMax + 1, yMax + 1].Type == PlatformType.Passable &&
                        this.PlatformMap.Platforms[xMax + 1, yMax + 2].Type == PlatformType.Passable &&
                        this.PlatformMap.Platforms[xMax + 1, yMax + 3].Type == PlatformType.Passable)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (xMin - 1 > 0 && yMax + 3 < this.PlatformMap.PlatformsHigh)
                {
                    if (this.PlatformMap.Platforms[xMin - 1, yMax].Type == PlatformType.Passable &&
                        this.PlatformMap.Platforms[xMin - 1, yMax + 1].Type == PlatformType.Passable &&
                        this.PlatformMap.Platforms[xMin - 1, yMax + 2].Type == PlatformType.Passable &&
                        this.PlatformMap.Platforms[xMin - 1, yMax + 3].Type == PlatformType.Passable)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsOnGround(Rectangle bounds)
        {
            try
            {
                // Only check platforms intersecting with the sprite
                int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
                this.GetExactIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, bounds);

                // Bounds is on ground if there is only air above the lowest intersecting platforms
                for (int i = xMin; i <= xMax; i++)
                {
                    if (this.PlatformMap.Platforms[i, yMax].Type == PlatformType.Impassable && this.PlatformMap.Platforms[i, yMax - 1].Type == PlatformType.Passable)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // TODO - Check bounds or remove once level map does not contain blank spaces
                // This is just to avoid crashing when running with an incomplete platform map - will lag terribly instead of crashing
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
                    if (this.PlatformMap.Platforms[i, j].BoundingBox.Intersects(bounds) && this.PlatformMap.Platforms[i, j].Type == PlatformType.Impassable && !this.TouchingGround(this.PlatformMap.Platforms[i, j].BoundingBox, bounds)) 
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
                                    if (this.PlatformMap.Platforms[x, y].Type != PlatformType.Impassable)
                                    {
                                        found = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    // Check for air
                                    if (this.PlatformMap.Platforms[x, y].Type != PlatformType.Passable)
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
                        if (location.Y > GROUND_LINE) // TODO_HIGH how to determine if underground - can't empty platforms because of background
                        {
                            return new Platform("Dirt", content.Load<Texture2D>("Dirt"), PlatformType.Passable, location);
                        }
                        else
                        {
                            return new Platform(PlatformType.Passable, location);      
                        }
                    }
                case '+':
                    {
                        this.EnemyVectors.Add(new Vector2(location.X, location.Y - Platform.Height));
                        return new Platform(PlatformType.Passable, location);
                    }
                case ',':
                    {
                        return new Platform("Underground", content.Load<Texture2D>("Underground"), PlatformType.Impassable, location);
                    }
                case '_':
                    {
                        return new Platform("Underground", content.Load<Texture2D>("Grass_Top"), PlatformType.Impassable, location);
                    }
                case '<':
                    {
                        return new Platform("Underground", content.Load<Texture2D>("Grass_TopLeft"), PlatformType.Impassable, location);
                    }
                case '>':
                    {
                        return new Platform("Underground", content.Load<Texture2D>("Grass_TopRight"), PlatformType.Impassable, location);
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
            xMax = (int)Math.Ceiling(xMin + (bounds.Width / (double)Platform.Width)) + 1;

            yMin = (int)Math.Floor((float)bounds.Y / Platform.Height); // TODO floor made robots not on ground
            yMax = (int)Math.Ceiling(yMin + (bounds.Height / (double)Platform.Height)) + 1;

            // Protect against going outside bounds of array
            xMin = Math.Max(xMin, 0);
            yMin = Math.Max(yMin, 0);

            xMax = Math.Min(xMax, this.PlatformMap.PlatformsWide);
            yMax = Math.Min(yMax, this.PlatformMap.PlatformsHigh);
        }

        /// <summary>
        /// Get Platform index range for array which correspond with a given rectangle
        /// </summary>
        protected void GetExactIntersectingPlatformIndices(ref int xMin, ref int xMax, ref int yMin, ref int yMax, Rectangle bounds)
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

            xMax = Math.Min(xMax, this.PlatformMap.PlatformsWide);
            yMax = Math.Min(yMax, this.PlatformMap.PlatformsHigh);
        }

        // TODO - I think I mixed these two names up

        /// <summary>
        /// Get Platform index range for array which correspond with a given rectangle
        /// </summary>
        protected void GetTruncatedIntersectingPlatformIndices(ref int xMin, ref int xMax, ref int yMin, ref int yMax, Rectangle bounds)
        {
            xMin = (int)((float)bounds.X / Platform.Width);
            xMax = (int)(xMin + (bounds.Width / Platform.Width));

            yMin = (int)((float)bounds.Y / Platform.Height);
            yMax = (int)(yMin + (bounds.Height / Platform.Height));

            // Protect against going outside bounds of array
            xMin = Math.Max(xMin, 0);
            yMin = Math.Max(yMin, 0);

            xMax = Math.Min(xMax, this.PlatformMap.PlatformsWide);
            yMax = Math.Min(yMax, this.PlatformMap.PlatformsHigh);
        }

        private bool TouchingGround(Rectangle platform, Rectangle character)
        {
            return platform.Y - (Platform.Height - 1) < character.Y + character.Height && platform.Y + (Platform.Height - 1) > character.Y + character.Height;
        }

        // TODO This should be its an independent object... ILevelSoundManager
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
