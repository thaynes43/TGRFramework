// -----------------------------------------------------------------------
// <copyright file="EditablePlatformLevel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.LevelEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EditablePlatformerLevel : PlatformerLevel
    {
        private List<Vector2> selectedLocations = new List<Vector2>();
        private List<Vector2> locationsToRemove = new List<Vector2>();

        private Dictionary<string, Texture2D> textureMap = new Dictionary<string, Texture2D>();

        private IntersectsFormControlDelegate IntersectsFormControl;

        /// <summary>
        /// Allow locking Y on command to draw horizontal lines
        /// </summary>
        private int lockableMouseY;

        /// <summary>
        /// Allow locking X on command to draw vertical lines
        /// </summary>
        private int lockableMouseX;

        public EditablePlatformerLevel(string levelMap, GraphicsDevice gfx, IntersectsFormControlDelegate intersectDelegate)
            : base(levelMap, gfx)
        {
            this.IntersectsFormControl = intersectDelegate;
        }

        public void Apply()
        {
            for (int x = 0; x < this.PlatformMap.PlatformsWide; x++)
            {
                for (int y = 0; y < this.PlatformMap.PlatformsHigh; y++)
                {
                    if (this.selectedLocations.Contains(this.PlatformMap.Platforms[x, y].Location))
                    {
                        // TODO need better way to manage these properties
                        Texture2D newTexture = LevelEditorControl.ComboBoxSelection == "Air" ? null : textureMap[LevelEditorControl.ComboBoxSelection];
                        this.PlatformMap.Platforms[x, y].Texture = newTexture;
                        this.PlatformMap.Platforms[x, y].ContentID = newTexture == null ? string.Empty : newTexture.Name;
                        this.PlatformMap.Platforms[x, y].Type = LevelEditorControl.ComboBoxSelection == "Air" || LevelEditorControl.ComboBoxSelection == "Dirt" ? PlatformType.Passable : PlatformType.Impassable;
                        this.selectedLocations.Remove(this.PlatformMap.Platforms[x, y].Location);
                    }
                }
            }

            this.selectedLocations.Clear();

            this.GrowGrass();
        }

        /// <summary>
        /// Brute force fill algorithm
        /// TODO - would be a good exercise to optimize this
        /// </summary>
        public void Fill()
        {
            /*
             * Find all juxtaposed vectors 
             */
            List<Vector2> allJuxtaposedVectors = new List<Vector2>(); // TODO this wont isolate connections

            // Find sets of connected locations
            foreach (Vector2 outerLoc in this.selectedLocations)
            {
                List<Vector2> otherVectors = this.selectedLocations.ToList(); // Test against all other vectors
                otherVectors.Remove(outerLoc);

                foreach (Vector2 innerLoc in otherVectors)
                {
                    if (this.Juxtaposed(outerLoc, innerLoc)) 
                    {

                        allJuxtaposedVectors.Add(innerLoc);                     
                    }
                }
            }

            allJuxtaposedVectors = allJuxtaposedVectors.Distinct().ToList(); // TODO optimize, currently adding the same vectors many times

            /*
             * TODO Separate juxtaposed vectors OR new approach
             */ 
            //List<List<Vector2>> seperatedJuxtaposedVectors = new List<List<Vector2>>();
            //if (allJuxtaposedVectors.Count == 0)
            //{
            //    return; // nothing to fill
            //}

            //seperatedJuxtaposedVectors.Add(new List<Vector2> { allJuxtaposedVectors[0] });

            //for (int i = 1; i < allJuxtaposedVectors.Count; i++) // Skip first, already in a bin
            //{
            //    Vector2 currentVector = allJuxtaposedVectors[i];

            //    bool found = false;

            //    for (int j = 0; j < seperatedJuxtaposedVectors.Count; j++)
            //    {
            //        for (int k = 0; k < seperatedJuxtaposedVectors[j].Count; k++)
            //        {
            //            if (this.Juxtaposed(currentVector, seperatedJuxtaposedVectors[j][k]))
            //            {
            //                seperatedJuxtaposedVectors[j].Add(currentVector);
            //                break;
            //            }
            //        }

            //        if (found) break;
            //    }

            //    if (!found) seperatedJuxtaposedVectors.Add(new List<Vector2> { currentVector });
            //}

            /*
             * Go through each set of juxtaposed vectors and fill
             */

            // Find bounds to potentially fill
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;

            foreach (Vector2 vector in allJuxtaposedVectors)
            {
                if (vector.X < minX)
                {
                    minX = (int)vector.X;
                }
                if (vector.X > maxX)
                {
                    maxX = (int)vector.X;
                }
                if (vector.Y < minY)
                {
                    minY = (int)vector.Y;
                }
                if (vector.Y > maxY)
                {
                    maxY = (int)vector.Y;
                }
            }

            // See if platforms in range are within the juxtaposed vectors
            minX = minX / Platform.Width;
            maxX = maxX / Platform.Width;
            minY = minY / Platform.Height;
            maxY = maxY / Platform.Height;

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    // Check if platform is one we want to fill
                    if (this.Encompesses(allJuxtaposedVectors, this.PlatformMap.Platforms[x, y].Location))
                    {
                        this.selectedLocations.Add(this.PlatformMap.Platforms[x, y].Location);
                    }
                }
            }
            

            this.selectedLocations = this.selectedLocations.Distinct().ToList(); // TODO optimize, currently adding the same vectors many times
        }

        public void DeleteCurrentLocation()
        {
            Platform platformToDelete = this.IntersectsPlatform((int)LevelEditorControl.MouseState.X, (int)LevelEditorControl.MouseState.Y);
            this.selectedLocations.Remove(platformToDelete.Location);
        }

        public void ClearSelectedLocations()
        {
            this.selectedLocations.Clear();
        }

        public void Save(string filename)
        {
            this.Apply();
            this.PlatformMap.Save(filename);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            this.textureMap.Add("delete", this.CreateColoredTexture(Color.Red, 255 / 2));

            foreach (string s in LevelEditorControl.ComboBoxItems)
            {
                this.textureMap.Add(s, this.GetTextureFromComboBoxItem(s, content));
            }
 
            // Grass sprites
            this.textureMap.Add("Grass_Top", content.Load<Texture2D>("Grass_Top"));
            this.textureMap.Add("Grass_Bottom", content.Load<Texture2D>("Grass_Bottom"));
            this.textureMap.Add("Grass_Left", content.Load<Texture2D>("Grass_Left"));
            this.textureMap.Add("Grass_Right", content.Load<Texture2D>("Grass_Right"));
            this.textureMap.Add("Grass_BottomRight", content.Load<Texture2D>("Grass_BottomRight"));
            this.textureMap.Add("Grass_BottomLeft", content.Load<Texture2D>("Grass_BottomLeft"));
            this.textureMap.Add("Grass_TopRight", content.Load<Texture2D>("Grass_TopRight"));
            this.textureMap.Add("Grass_TopLeft", content.Load<Texture2D>("Grass_TopLeft"));
            this.textureMap.Add("Grass_RightLeft", content.Load<Texture2D>("Grass_RightLeft"));
            this.textureMap.Add("Grass_TopBottom", content.Load<Texture2D>("Grass_TopBottom"));
            this.textureMap.Add("Grass_TopRightLeft", content.Load<Texture2D>("Grass_TopRightLeft"));
            this.textureMap.Add("Grass_TopBottomLeft", content.Load<Texture2D>("Grass_TopBottomLeft"));
            this.textureMap.Add("Grass_TopBottomRight", content.Load<Texture2D>("Grass_TopBottomRight"));
            this.textureMap.Add("Grass_TopBottomRightLeft", content.Load<Texture2D>("Grass_TopBottomRightLeft"));

            base.LoadContent(content);
        }

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (!LevelEditorControl.HorizontalLockKeyDown)
            {
                this.lockableMouseY = mouseState.Y;
            }

            if (!LevelEditorControl.VerticalLockKeyDown)
            {
                this.lockableMouseX = mouseState.X;
            }

            Platform platformMouseIsOver = this.mouseIntersectPlatform();


            if (LevelEditorControl.DeleteKeyDown && !LevelEditorControl.MenuOpen &&
                !(this.IntersectsFormControl(new Rectangle(this.lockableMouseX, this.lockableMouseY, (int)Platform.Width, (int)Platform.Height))) && // Dont remove when mouse is over form control items
                mouseState.LeftButton == ButtonState.Pressed && // Hold left shift to select platforms
                (this.lockableMouseX <= this.Graphics.Viewport.Width && this.lockableMouseY <= this.Graphics.Viewport.Height) && // Only select if mouse is over viewport
                (this.lockableMouseX >= 0 && this.lockableMouseY >= 0) && // Only select if mouse is over viewport
                this.selectedLocations.Contains(platformMouseIsOver.Location)) // Platform is selected
            {
                // Locations will be removed and list cleared on draw thread
                lock (this.locationsToRemove) 
                {
                    this.locationsToRemove.Add(platformMouseIsOver.Location);                     
                }
            }
            else if (!LevelEditorControl.DeleteKeyDown && !LevelEditorControl.MenuOpen &&
                !(this.IntersectsFormControl(new Rectangle(this.lockableMouseX, this.lockableMouseY, (int)Platform.Width, (int)Platform.Height))) && // Dont draw when mouse is over form control items
                mouseState.LeftButton == ButtonState.Pressed && // Hold left shift to select platforms
                (this.lockableMouseX <= this.Graphics.Viewport.Width && this.lockableMouseY <= this.Graphics.Viewport.Height) && // Only select if mouse is over viewport
                (this.lockableMouseX >= 0 && this.lockableMouseY >= 0) && // Only select if mouse is over viewport
                !this.selectedLocations.Contains(platformMouseIsOver.Location)) // Keep list distinct
            {
                this.selectedLocations.Add(new Vector2(platformMouseIsOver.Location.X, platformMouseIsOver.Location.Y));      
            }   
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        { 
            lock (this.locationsToRemove)
            {
                // Cached locations from update thread
                this.selectedLocations.RemoveAll(i => this.locationsToRemove.Contains(i));
                this.locationsToRemove.Clear();
            }

            base.Draw(theSpriteBatch);

            int currentLocationCount = this.selectedLocations.Count; // Will be updated as we draw

            for (int i = 0; i < currentLocationCount; i++)
            {
                theSpriteBatch.Draw(this.textureMap[LevelEditorControl.ComboBoxSelection], this.selectedLocations[i], Color.White);
            }

            MouseState pointer = Mouse.GetState();
            Vector2 pointerPosition = new Vector2(pointer.X - 7 + PositionsDataStore.Instance.CameraPosition.X, pointer.Y - 5 + PositionsDataStore.Instance.CameraPosition.Y);
            
            if (LevelEditorControl.DeleteKeyDown)
            {
                theSpriteBatch.Draw(this.textureMap["delete"], pointerPosition, Color.White);
            }
            else
            {
                theSpriteBatch.Draw(this.textureMap[LevelEditorControl.ComboBoxSelection], pointerPosition, Color.White);
            }
        }

        private bool Encompesses(List<Vector2> juxtaposedVectors, Vector2 vector1)
        {
            bool above = false, below = false, left = false, right = false;

            foreach (Vector2 vector2 in juxtaposedVectors)
            {
                if (vector1.X == vector2.X && vector1.Y > vector2.Y)
                {
                    below = true;
                }
                else if (vector1.X == vector2.X && vector1.Y < vector2.Y)
                {
                    above = true;
                }
                else if (vector1.X < vector2.X && vector1.Y == vector2.Y)
                {
                    left = true;
                }
                else if (vector1.X > vector2.X && vector1.Y == vector2.Y)
                {
                    right = true;
                }
            }

            return (above && below && left && right);
        }

        private bool Juxtaposed(Vector2 vector1, Vector2 vector2)
        {
            if ((vector1.X + Platform.Width == vector2.X && vector1.Y == vector2.Y) ||
                (vector1.X - Platform.Width == vector2.X && vector1.Y == vector2.Y) ||
                (vector1.X == vector2.X && vector1.Y + Platform.Height == vector2.Y) ||
                (vector1.X == vector2.X && vector1.Y - Platform.Height == vector2.Y) ||
                (vector1.X - Platform.Width == vector2.X && vector1.Y - Platform.Height == vector2.Y) ||
                (vector1.X - Platform.Width == vector2.X && vector1.Y + Platform.Height == vector2.Y) ||
                (vector1.X + Platform.Width == vector2.X && vector1.Y - Platform.Height == vector2.Y) ||
                (vector1.X + Platform.Width == vector2.X && vector1.Y + Platform.Height == vector2.Y))
            {
                return true;
            }

            return false;
        }

        private Platform mouseIntersectPlatform()
        {
            MouseState mouseState = LevelEditorControl.MouseState;
            int mouseX = this.lockableMouseX + (int)PositionsDataStore.Instance.CameraPosition.X;
            int mouseY = this.lockableMouseY + (int)PositionsDataStore.Instance.CameraPosition.Y;
            return this.IntersectsPlatform(mouseX, mouseY);
        }

        private Platform IntersectsPlatform(int x, int y) // TODO - What exactly are we intersecting (it works..)
        {
            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            this.GetExactIntersectingPlatformIndices(ref xMin, ref xMax, ref yMin, ref yMax, new Rectangle(x, y, 1, 1));
            return this.PlatformMap.Platforms[xMin, yMin];
        }

        /// <summary>
        /// Check conditions of surrounding platforms to see if it would be appropriate to grow grass 
        /// TODO_Enhancement - Certain corners need grass to blend better
        /// </summary>
        private void GrowGrass()
        {
            string air = string.Empty;

            for (int x = 0; x < this.PlatformMap.PlatformsWide; x++)
            {
                for (int y = 0; y < this.PlatformMap.PlatformsHigh; y++)
                {
                    if (this.PlatformMap.Platforms[x, y].ContentID != air)
                    {
                        Platform platformToLeft = x > 0 ? this.PlatformMap.Platforms[x - 1, y] : new Platform("", 15, 15, PlatformType.Passable, Vector2.Zero);
                        Platform platformToRight = x < this.PlatformMap.PlatformsWide - 1 ? this.PlatformMap.Platforms[x + 1, y] : new Platform("", 15, 15, PlatformType.Passable, Vector2.Zero);
                        Platform platformAbove = y > 0 ? this.PlatformMap.Platforms[x, y - 1] : new Platform("", 15, 15, PlatformType.Passable, Vector2.Zero);
                        Platform platformBelow = y < this.PlatformMap.PlatformsHigh - 1? this.PlatformMap.Platforms[x, y + 1] : new Platform("", 15, 15, PlatformType.Passable, Vector2.Zero);

                        if (platformToLeft.ContentID != air && platformToRight.ContentID != air && platformAbove.ContentID != air && platformBelow.ContentID != air)
                        {
                            // Platform is underground
                            if (this.PlatformMap.Platforms[x, y].ContentID != "Underground" && this.PlatformMap.Platforms[x, y].Type == PlatformType.Impassable)
                            {
                                this.PlatformMap.Platforms[x, y].ContentID = "Underground";
                                this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];                
                            }

                            continue;
                        }
                        else if (this.PlatformMap.Platforms[x, y].Type == PlatformType.Impassable)
                        {
                            List<string> grassCriteria = new List<string>();

                            // Texture criteria
                            if (platformToLeft.ContentID == air)
                            {
                                grassCriteria.Add("Left");
                            }
                            if (platformToRight.ContentID == air)
                            {
                                grassCriteria.Add("Right");
                            }
                            if (platformAbove.ContentID == air)
                            {
                                grassCriteria.Add("Top");
                            }
                            if (platformBelow.ContentID == air)
                            {
                                grassCriteria.Add("Bottom");
                            }

                            // Find texture which fits criteria
                            foreach (var kvp in this.textureMap)
                            {
                                bool found = true;

                                foreach (string s in grassCriteria)
                                {
                                    if (!kvp.Key.Contains(s))
                                    {
                                        found = false;
                                        break;
                                    }
                                }

                                // Apply texture when found
                                if (found)
                                {
                                    this.PlatformMap.Platforms[x, y].ContentID = kvp.Key;
                                    this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private Texture2D GetTextureFromComboBoxItem(string item, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture2D texture;

            switch (item)
            {
                case "Air":
                    texture = this.CreateColoredTexture(Color.Black, 255 / 2);
                    texture.Name = "Air";
                    break;
                case "Underground":
                case "Dirt":
                    texture = content.Load<Texture2D>(item);
                    texture.Name = item;
                    break;
                default:
                    texture = this.CreateColoredTexture(Color.Chartreuse, 255);
                    texture.Name = "Default";
                    break;
            }

            return texture;
        }

        private Texture2D CreateColoredTexture(Color color, byte alpha)
        {
            color.A = alpha;
            Texture2D retTexture = new Texture2D(this.Graphics, (int)Platform.Height, (int)Platform.Width);
            Color[] data = new Color[(int)Platform.Height * (int)Platform.Width];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            retTexture.SetData(data);

            return retTexture;
        }
    }
}
