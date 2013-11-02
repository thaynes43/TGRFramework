// -----------------------------------------------------------------------
// <copyright file="MeleeWeaponSprite.cs" company="">
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
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// TODO: Weapon base class when we add another weapon type
    /// </summary>
    public class MeleeWeaponSprite : ISprite
    {
        private Vector2 weaponPosition = Vector2.Zero;

        public MeleeWeaponSprite(string content, PlayableCharacterSprite owner, GraphicsDevice gfx)
        {
            this.Visible = false;
            this.ContentName = content;
            this.WeaponOwner = owner;

            this.WeaponDamage = 10f;
            this.Facing = SwingFacing.Undefined;

            this.MeleeSwingModel = new MeleeDownSwipeModel(this);

            this.Graphics = gfx;
        }

        public bool Visible { get; set; }

        // TODO Move to somewhere more global
        public enum SwingFacing
        {
            Undefined,
            Right,
            Left,
        }

        public float WeaponDamage { get; set; }

        public float WeaponAngle { get; set; }

        public SwingFacing Facing { get; set; }

        private string ContentName { get; set; }

        private PlayableCharacterSprite WeaponOwner { get; set; }

        private Texture2D WeaponTexture { get; set; }

        private MeleeAttackModel MeleeSwingModel { get; set; }

        private Rectangle HelperRectangle { get; set; }

        private Texture2D HelperTexture { get; set; }

        private GraphicsDevice Graphics { get; set; }

        private SoundEffect SwingSound { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.WeaponTexture = content.Load<Texture2D>(this.ContentName);
            this.SwingSound = content.Load<SoundEffect>("Swing");
            this.SwingSound.Name = "Swing";
        }

        private DateTime lastSwing = DateTime.Now;
        private const int SWING_LIMITER_MS = 400;

        public void Update(ContentManager content, GameTime gameTime)
        {
            if (this.Facing != SwingFacing.Undefined) // Weapon is in swing
            {
                this.HelperRectangle = this.CalculateIntersectRectangle(this.weaponPosition.X, this.weaponPosition.Y, this.WeaponAngle); // Calculate ONCE per update!!!!!

                // TODO this shouldn't happen, remove check
                if (this.HelperRectangle.Height > 0 && this.HelperRectangle.Width > 0)
                {
                    // TODO Diagnostic mode
                    this.HelperTexture = new Texture2D(this.Graphics, this.HelperRectangle.Width, this.HelperRectangle.Height);
                    Color[] data = new Color[this.HelperRectangle.Width * this.HelperRectangle.Height];
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.LightSeaGreen;
                    this.HelperTexture.SetData(data);
                }

                this.MeleeSwingModel.UpdateSpriteLocation();
            }
            else
            {
                MouseState mouseState = Mouse.GetState();

                double totalMS = (DateTime.Now - this.lastSwing).TotalMilliseconds;

                if  ( totalMS >= SWING_LIMITER_MS && 
                    ( GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) || 
                    ( mouseState.LeftButton == ButtonState.Pressed && mouseState.X <= this.Graphics.Viewport.Width && mouseState.Y <= this.Graphics.Viewport.Height )))
                {
                    this.lastSwing = DateTime.Now;

                    this.WeaponOwner.Level.LevelSoundQueue.Add(this.SwingSound);

                    if (this.WeaponOwner.Facing == SwingFacing.Left)
                    {
                        this.Facing = SwingFacing.Left;
                    }
                    else
                    {
                        this.Facing = SwingFacing.Right;
                    }

                    this.Visible = true;
                }
                else
                {
                    // hide weapon
                    this.Visible = false;             
                }              
            }
        }

        public event Action<string> updateText; 

        public void Draw(SpriteBatch theSpriteBatch)
        {
            // Position weapon relative to character
            float x = (float)this.WeaponOwner.BoundingBox.Center.X;
            float y = this.WeaponOwner.BoundingBox.Y + 20f;
            this.weaponPosition = new Vector2(x, y);

            //string weaponText = string.Format("X = {0}, Y = {1}, Theta = {2}", this.weaponPosition.X, this.weaponPosition.Y, this.WeaponAngle);
            //this.updateText(weaponText);

            theSpriteBatch.Draw(this.WeaponTexture, this.weaponPosition, null, Color.White, this.WeaponAngle, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

            // TODO Diagnostic mode
            if (this.HelperTexture != null)
            {
                //theSpriteBatch.Draw(this.HelperTexture, new Vector2(this.HelperRectangle.X, this.HelperRectangle.Y), Color.White);
            }
        }

        public bool Intersects(CharacterSprite sprite)
        {
            if (this.Facing == SwingFacing.Undefined || this.WeaponAngle == 0f || (int)this.weaponPosition.Y < 0)
            {
                return false;
            }

            //string weaponText = string.Format("New Width = {0}, New Height = {1} : Old Width = {2}, Old Height = {3} : theta = {4}", this.HelperRectangle.Width, this.HelperRectangle.Height, this.WeaponTexture.Bounds.Width, this.WeaponTexture.Bounds.Height, theta);

            string weaponText = string.Format("X = {0}, Y = {1}", GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);
            //this.updateText(weaponText);

            return sprite.BoundingBox.Intersects(this.HelperRectangle);
        }

        private Rectangle CalculateIntersectRectangle(float weaponX, float weaponY, float weaponTheta)
        {
            // Adjust sprite rotate theta to pi/2 y+, 2pi x+, pi x-, 3pi/2 y-
            float theta = (weaponTheta + MathHelper.PiOver2) % (MathHelper.TwoPi);

            int x = 0;
            int y = 0;
            float width = 0f;
            float height = 0f;

            // TODO document and optimize, may need new approach
            if (theta > 0 && theta < MathHelper.PiOver2)
            {
                width = (float)this.WeaponTexture.Bounds.Height * (float)Math.Cos(theta);
                height = (float)this.WeaponTexture.Bounds.Height * (float)Math.Sin(theta);

                x = (int)weaponX;
                y = (int)weaponY;
            }
            else if (theta > MathHelper.PiOver2 && theta < MathHelper.Pi)
            {
                theta -= MathHelper.PiOver2;

                width = (float)this.WeaponTexture.Bounds.Height * (float)Math.Sin(theta);
                height = (float)this.WeaponTexture.Bounds.Height * (float)Math.Cos(theta);

                x = (int)weaponX - (int)width;
                y = (int)weaponY;
            }
            else if (theta > MathHelper.Pi && theta < ((3 * MathHelper.Pi) / 2))
            {
                theta -= MathHelper.Pi;

                width = (float)this.WeaponTexture.Bounds.Height * (float)Math.Cos(theta);
                height = (float)this.WeaponTexture.Bounds.Height * (float)Math.Sin(theta);

                x = (int)weaponX - (int)width;
                y = (int)weaponY - (int)height;
            }
            else if (theta > ((3 * MathHelper.Pi) / 2) && theta < MathHelper.TwoPi)
            {
                theta -= ((3 * MathHelper.Pi) / 2);

                width = (float)this.WeaponTexture.Bounds.Height * (float)Math.Sin(theta);
                height = (float)this.WeaponTexture.Bounds.Height * (float)Math.Cos(theta);

                x = (int)weaponX;
                y = (int)weaponY - (int)height;
            }

            return new Rectangle(x, y, (int)width, (int)height);
        }
    }
}
