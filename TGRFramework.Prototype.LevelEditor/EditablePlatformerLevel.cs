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
            this.textureMap.Add("delete", this.CreatePlatformSelectTexture(Color.Red, 255 / 2));

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
            Platform platformMouseIsOver = this.mouseIntersectPlatform();

            MouseState mouseState = Mouse.GetState();

            if (LevelEditorControl.DeleteKeyDown && !LevelEditorControl.MenuOpen &&
                !(this.IntersectsFormControl(new Rectangle(mouseState.X, mouseState.Y, (int)Platform.Width, (int)Platform.Height))) && // Dont remove when mouse is over form control items
                mouseState.LeftButton == ButtonState.Pressed && // Hold left shift to select platforms
                (mouseState.X <= this.Graphics.Viewport.Width && mouseState.Y <= this.Graphics.Viewport.Height) && // Only select if mouse is over viewport
                (mouseState.X >= 0 && mouseState.Y >= 0) && // Only select if mouse is over viewport
                this.selectedLocations.Contains(platformMouseIsOver.Location)) // Platform is selected
            {
                // Locations will be removed and list cleared on draw thread
                lock (this.locationsToRemove) 
                {
                    this.locationsToRemove.Add(platformMouseIsOver.Location);                     
                }
            }
            else if (!LevelEditorControl.DeleteKeyDown && !LevelEditorControl.MenuOpen &&
                !(this.IntersectsFormControl(new Rectangle(mouseState.X, mouseState.Y, (int)Platform.Width, (int)Platform.Height))) && // Dont draw when mouse is over form control items
                mouseState.LeftButton == ButtonState.Pressed && // Hold left shift to select platforms
                (mouseState.X <= this.Graphics.Viewport.Width && mouseState.Y <= this.Graphics.Viewport.Height) && // Only select if mouse is over viewport
                (mouseState.X >= 0 && mouseState.Y >= 0) && // Only select if mouse is over viewport
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
            Vector2 pointerPosition = new Vector2(pointer.X - 7 + PlatformerLevel.CameraPositionX, pointer.Y - 5 + PlatformerLevel.CameraPositionY);
            
            if (LevelEditorControl.DeleteKeyDown)
            {
                theSpriteBatch.Draw(this.textureMap["delete"], pointerPosition, Color.White);
            }
            else
            {
                theSpriteBatch.Draw(this.textureMap[LevelEditorControl.ComboBoxSelection], pointerPosition, Color.White);
            }
        }
        private Platform mouseIntersectPlatform()
        {
            MouseState mouseState = LevelEditorControl.MouseState;
            int mouseX = mouseState.X + (int)PlatformerLevel.CameraPositionX;
            int mouseY = mouseState.Y + (int)PlatformerLevel.CameraPositionY;
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
        /// </summary>
        private void GrowGrass()
        {
            string air = string.Empty;

            for (int x = 1; x < this.PlatformMap.PlatformsWide - 1; x++)
            {
                for (int y = 1; y < this.PlatformMap.PlatformsHigh - 1; y++)
                {
                    if (this.PlatformMap.Platforms[x, y].ContentID == "Underground")
                    {
                        Platform platformToLeft = this.PlatformMap.Platforms[x - 1, y];
                        Platform platformToRight = this.PlatformMap.Platforms[x + 1, y];
                        Platform platformAbove = this.PlatformMap.Platforms[x, y - 1];
                        Platform platformBelow = this.PlatformMap.Platforms[x, y + 1];

                        if (platformToLeft.ContentID != air && platformToRight.ContentID != air && platformAbove.ContentID != air && platformBelow.ContentID != air)
                        {
                            // Platform is underground
                            continue;
                        }
                        else if (platformToLeft.ContentID == air && platformToRight.ContentID == air && platformAbove.ContentID == air && platformBelow.ContentID == air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_TopBottomRightLeft";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID == air && platformToRight.ContentID != air && platformAbove.ContentID == air && platformBelow.ContentID == air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_TopBottomLeft";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID != air && platformToRight.ContentID == air && platformAbove.ContentID == air && platformBelow.ContentID == air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_TopBottomRight";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID == air && platformToRight.ContentID == air && platformAbove.ContentID == air && platformBelow.ContentID != air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_TopRightLeft";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID != air && platformToRight.ContentID != air && platformAbove.ContentID == air && platformBelow.ContentID == air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_TopBottom";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID == air && platformToRight.ContentID != air && platformAbove.ContentID == air && platformBelow.ContentID != air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_TopLeft";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID != air && platformToRight.ContentID == air && platformAbove.ContentID == air && platformBelow.ContentID != air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_TopRight";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID == air && platformToRight.ContentID == air && platformAbove.ContentID != air && platformBelow.ContentID != air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_RightLeft";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID == air && platformToRight.ContentID != air && platformAbove.ContentID != air && platformBelow.ContentID == air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_BottomLeft";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID != air && platformToRight.ContentID == air && platformAbove.ContentID != air && platformBelow.ContentID == air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_BottomRight";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID != air && platformToRight.ContentID != air && platformAbove.ContentID != air && platformBelow.ContentID == air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_Bottom";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID != air && platformToRight.ContentID != air && platformAbove.ContentID == air && platformBelow.ContentID != air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_Top";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID == air && platformToRight.ContentID != air && platformAbove.ContentID != air && platformBelow.ContentID != air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_Left";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
                        }
                        else if (platformToLeft.ContentID != air && platformToRight.ContentID == air && platformAbove.ContentID != air && platformBelow.ContentID != air)
                        {
                            this.PlatformMap.Platforms[x, y].ContentID = "Grass_Right";
                            this.PlatformMap.Platforms[x, y].Texture = this.textureMap[this.PlatformMap.Platforms[x, y].ContentID];
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
                    texture = this.CreatePlatformSelectTexture(Color.Black, 255 / 2);
                    texture.Name = "Air";
                    break;
                case "Underground":
                case "Dirt":
                    texture = content.Load<Texture2D>(item);
                    texture.Name = item;
                    break;
                default:
                    texture = this.CreatePlatformSelectTexture(Color.Chartreuse, 255);
                    texture.Name = "Default";
                    break;
            }

            return texture;
        }

        private Texture2D CreatePlatformSelectTexture(Color color, byte alpha)
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
