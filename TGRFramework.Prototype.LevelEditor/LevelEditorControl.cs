// -----------------------------------------------------------------------
// <copyright file="LevelEditorControl.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.LevelEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using System.Windows.Forms;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework.Input;
    using TGRFramework.Prototype.Tools;

    public delegate bool IntersectsFormControlDelegate(Rectangle rectangle);

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LevelEditorControl : XNAControl
    {
        private float cameraPositionX;
        private float cameraPositionY;

        private static MouseState _mouseState = default(MouseState); // Freeze mouse state under certain condition

        /// <summary>
        /// Only sprite we care about for this XNA instance
        /// </summary>
        public EditablePlatformerLevel LevelSprite { get; set; }

        /* 
         * TODO, Hokey input from XNA form control:
         *    Keyboard.GetState() is not getting input when running as form control,
         *    need a better solution than the following..
         */
        public static bool DeleteKeyDown { get; set; }

        public static bool HorizontalLockKeyDown { get; set; }

        public static bool VerticalLockKeyDown { get; set; }

        public static bool MenuOpen { get; set; }

        public static string ComboBoxSelection { get; set; }

        public static string[] ComboBoxItems { get; set; }

        public List<Rectangle> FormControlLocations = new List<Rectangle>(); // TODO not sure when to initialize this

        public static MouseState MouseState 
        { 
            get
            {
                if (_mouseState == default(MouseState))
                {
                    return Mouse.GetState();
                }
                else
                {
                    return _mouseState;
                }
            }
            set 
            {
                _mouseState = value;

                if (_mouseState == default(MouseState))
                {
                    PlatformerLevel.Log.Debug("Mouse unfrozen");
                }
                else
                {
                    PlatformerLevel.Log.Debug("Mouse frozen at {0}", _mouseState);
                }
            } 
        }

        public float ScrollX { get; set; }
        public float ScrollY { get; set; }

        public bool IntersectsFormControl(Rectangle rectangle)
        {
            foreach (Rectangle rect in this.FormControlLocations)
            {
                if (rect.Intersects(rectangle))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void Initialize()
        {
            this.LevelSprite = new EditablePlatformerLevel("DiagnosticLevel.txt", this.GraphicsDevice, this.IntersectsFormControl);
            EditablePlatformerLevel.Log = new LogTool("LevelEditorLog");
        }

        protected override void LoadContent(ContentManager content)
        {
            this.LevelSprite.LoadLevel(content);
            this.LevelSprite.LoadContent(content); // TODO can these be the same

        }

        protected override void Update(ContentManager content, GameTime gameTime)
        {
            this.LevelSprite.Update(content, gameTime);
        }

        protected override void Draw(SpriteBatch theSpriteBatch)
        {
            //System.Drawing.Point point = this.PointToClient(Control.MousePosition);
            //MousePosition = new Vector2(point.X, point.Y);

            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            theSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, this.ScrollCamera());

            this.LevelSprite.Draw(theSpriteBatch);

            theSpriteBatch.End();
        }

        /// <summary>
        /// Move camera around level
        /// </summary>
        /// <returns>Translation matrix</returns>
        private Matrix ScrollCamera()
        {
            Viewport viewport = this.GraphicsDevice.Viewport;

            //
            // X Translation
            //
            float cameraMovementX = 0.0f;

            if (this.ScrollX < this.cameraPositionX || this.ScrollX > this.cameraPositionX)
            {
                cameraMovementX = this.ScrollX - this.cameraPositionX;
            }

            // Do not scroll past width of level
            float maxX = PlatformerLevel.LevelWidth - viewport.Width;
            this.cameraPositionX = MathHelper.Clamp(this.cameraPositionX + cameraMovementX, 0.0f, maxX);

            //
            // Y Translation
            //
            float cameraMovementY = 0.0f;

            if (this.ScrollY < this.cameraPositionY || this.ScrollY > this.cameraPositionY)
            {
                // hero is above top margin, move down
                cameraMovementY = this.ScrollY - this.cameraPositionY;
            }

            // Do not scroll past height of level
            float maxY = PlatformerLevel.LevelHeight - viewport.Height;
            this.cameraPositionY = MathHelper.Clamp(this.cameraPositionY + cameraMovementY, 0.0f, maxY);

            PlatformerLevel.CameraPositionX = this.cameraPositionX;
            PlatformerLevel.CameraPositionY = this.cameraPositionY;

            // Apply translation
            return Matrix.CreateTranslation(-this.cameraPositionX, -this.cameraPositionY, 0.0f);
        }
    }
}
