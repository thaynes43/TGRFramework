// -----------------------------------------------------------------------
// <copyright file="RangedWeaponSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public delegate void AddSpriteDelegate(ISprite sprite);
    public delegate void RemoveSpriteDelegate(ISprite sprite);

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RangedWeaponSprite : WeaponSprite
    {
        private AddSpriteDelegate addSprite;
        private RemoveSpriteDelegate removeSprite;
        private string contentRight;
        private string contentLeft;
        private ContentManager contentManager;

        public RangedWeaponSprite(string contentRight, string contentLeft, PlayableCharacterSprite owner, GraphicsDevice gfx, AddSpriteDelegate addSprite, RemoveSpriteDelegate removeSprite)
            : base(contentRight, owner, gfx, 200)
        {
            this.addSprite = addSprite;
            this.removeSprite = removeSprite;
            this.contentRight = contentRight;
            this.contentLeft = contentLeft;
            this.ActiveBullets = new List<BulletSprite>();
        }

        public event Action<string> updateText; 

        public List<BulletSprite> ActiveBullets { get; set; }

        public float WeaponAngle { get; private set; }

        public Vector2 BulletVelocity { get; private set; }

        public override void LoadContent(ContentManager content)
        {
            this.contentManager = content;
            base.LoadContent(content);
        }

        public override void Update(ContentManager content, GameTime gameTime)
        {
            // Remove bullets that have flown off the screen
            for (int i = 0; i < this.ActiveBullets.Count; i++)
            {
                if (!(this.ActiveBullets[i].BoundingBox.X >= PlatformerLevel.CameraPositionX - this.ActiveBullets[i].BoundingBox.Width &&
                    this.ActiveBullets[i].BoundingBox.X <= this.graphicsDevice.Viewport.Width + PlatformerLevel.CameraPositionX &&
                    this.ActiveBullets[i].BoundingBox.Y >= PlatformerLevel.CameraPositionY - this.ActiveBullets[i].BoundingBox.Height &&
                    this.ActiveBullets[i].BoundingBox.Y <= this.graphicsDevice.Viewport.Height + PlatformerLevel.CameraPositionY))
                {
                    this.RemoveBulletSprite(this.ActiveBullets[i]);
                }
            }

            Vector2 weaponLocation = new Vector2(this.weaponPosition.X - PlatformerLevel.CameraPositionX, this.weaponPosition.Y - PlatformerLevel.CameraPositionY);

            // TODO HIGH Input director
            if (!GamePad.GetState(PlayerIndex.One).IsConnected)
            {

                // Calculate angle based on mouse location
                Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                Vector2 direction = mouseLocation - weaponLocation;

                this.BulletVelocity = Vector2.Normalize(direction);

                if (this.WeaponOwner.Facing == MeleeWeaponSprite.SwingFacing.Right)
                {
                    this.WeaponAngle = (float)(Math.Atan2(direction.Y, direction.X));

                    if (this.WeaponAngle > MathHelper.PiOver2 || this.WeaponAngle < MathHelper.PiOver2 * -1)
                    {
                        this.BulletVelocity = new Vector2(this.BulletVelocity.X * -1, this.BulletVelocity.Y);

                        // Mouse is behind character
                        this.WeaponAngle = (float)(Math.Atan2(direction.X, direction.Y)) + MathHelper.PiOver2;
                    }
                }
                else
                {
                    this.WeaponAngle = (float)(Math.Atan2(-1 * direction.Y, -1 * direction.X));

                    if (this.WeaponAngle > MathHelper.PiOver2 || this.WeaponAngle < MathHelper.PiOver2 * -1)
                    {
                        this.BulletVelocity = new Vector2(this.BulletVelocity.X * -1, this.BulletVelocity.Y);

                        // Mouse is behind character
                        this.WeaponAngle = (float)(Math.Atan2(-1 * direction.X, -1 * direction.Y)) + MathHelper.PiOver2;
                    }
                }
            }
            else
            {
                // Calculate angle based on right thumb stick rotation
                Vector2 rightThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
                if (this.WeaponOwner.Facing == MeleeWeaponSprite.SwingFacing.Right)
                {
                    this.WeaponAngle = (rightThumb.Y * MathHelper.PiOver2) * -1;
                }
                else
                {
                    this.WeaponAngle = rightThumb.Y * MathHelper.PiOver2;
                }

                this.BulletVelocity = new Vector2((float)Math.Cos(this.WeaponAngle), (float)Math.Sin(this.WeaponAngle));

                if (this.WeaponOwner.Facing == MeleeWeaponSprite.SwingFacing.Left)
                {
                    this.BulletVelocity *= -1;
                }

                this.updateText(string.Format("Right Thumb: {0} \nWeapon Angle = {1} \nBullet Velocity {2}", rightThumb, this.WeaponAngle, this.BulletVelocity));
            }

            // TODO - Restrict range of motion, currently 0 - pi/2
            //if (this.WeaponAngle > (MathHelper.Pi / 4))
            //{
            //    this.WeaponAngle = (MathHelper.Pi / 4);
            //}
            //else if (this.WeaponAngle < (MathHelper.Pi / 4) * -1)
            //{
            //    this.WeaponAngle = (MathHelper.Pi / 4) * -1;
            //}

            //this.updateText(string.Format("Mouse: {0} \nWeapon Adjusted {1} \nWeapon Actual {2} \nAngle {3}", mouseLocation.ToString(), weaponLocation.ToString(), this.weaponPosition.ToString(), this.WeaponAngle));

            base.Update(content, gameTime);
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            Vector2 origin = Vector2.Zero; // new Vector2(this.weaponTexture.Width / 2, this.weaponTexture.Height / 2);

            SpriteEffects effect = SpriteEffects.None;
            if (this.WeaponOwner.Facing == MeleeWeaponSprite.SwingFacing.Left)
            {
                effect = SpriteEffects.FlipHorizontally;
                origin = new Vector2(this.weaponTexture.Width, 0);
            }

            theSpriteBatch.Draw(this.weaponTexture, this.weaponPosition + origin, null, Color.White, this.WeaponAngle, origin, 1.0f, effect, 0f);
        }

        public void RemoveBulletSprite(BulletSprite sprite)
        {
            this.removeSprite(sprite);
            this.ActiveBullets.Remove(sprite);
        }

        protected override void StartAttack()
        {
            this.Visible = true;
            Vector2 bulletPosition; 

            // Adjust to tip of weapon
            if (this.WeaponOwner.Facing == MeleeWeaponSprite.SwingFacing.Right)
            {

                float x = (float)(Math.Cos(this.WeaponAngle) * this.weaponTexture.Width + this.weaponPosition.X);
                float y = (float)(Math.Sin(this.WeaponAngle) * this.weaponTexture.Width + this.weaponPosition.Y);
                bulletPosition = new Vector2(x, y);
            }
            else
            {
                // Angles reversed when facing left 
                float x = (float)(Math.Cos(this.WeaponAngle + MathHelper.Pi) * this.weaponTexture.Width + this.weaponPosition.X);
                float y = (float)(Math.Sin(this.WeaponAngle + MathHelper.Pi) * this.weaponTexture.Width + this.weaponPosition.Y);

                // Sprite point of origin is also opposite what it is facing right
                bulletPosition = new Vector2(x, y) + new Vector2(this.weaponTexture.Width - 8f, (float)this.weaponTexture.Height - 15f); // TODO - tweaks
            }

            //this.updateText(string.Format("Angle {0} \nBullet Spawn {1} \nWeapon Position{2}", this.WeaponAngle, bulletPosition, this.weaponPosition));

            BulletSprite ammoSprite = new BulletSprite("Bullet", bulletPosition, this.WeaponOwner.Facing, this.BulletVelocity);
            this.addSprite(ammoSprite);
            this.ActiveBullets.Add(ammoSprite);
        }

        protected override Microsoft.Xna.Framework.Vector2 FindNewPosition()
        {
            Vector2 newPosition = base.FindNewPosition();

            // Adjust to hero if facing left
            if (this.WeaponOwner.Facing == MeleeWeaponSprite.SwingFacing.Left)
            {
                newPosition.X -= this.weaponTexture.Bounds.Width;
            }

            return newPosition;
        }
    }
}
