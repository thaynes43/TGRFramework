// -----------------------------------------------------------------------
// <copyright file="WeaponSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class WeaponSprite : ISprite
    {
        private DateTime lastAttackTime = DateTime.MinValue;
        private int attackLimiter = 0;

        protected GraphicsDevice graphicsDevice;
        protected Vector2 weaponPosition = Vector2.Zero;
        protected string content;
        protected Texture2D weaponTexture;

        public WeaponSprite(string content, PlayableCharacterSprite owner, GraphicsDevice gfx, int attackLimiter=0)
        {
            this.content = content;
            this.WeaponOwner = owner;
            this.graphicsDevice = gfx;
            this.attackLimiter = attackLimiter;
            this.Visible = false; // weapons start 'holstered'
        }

        protected PlayableCharacterSprite WeaponOwner { get; set; }

        protected abstract bool AttackButtonDown { get; }

        public bool Visible { get; set; }

        public virtual void LoadContent(ContentManager content)
        {
            this.weaponTexture = content.Load<Texture2D>(this.content);
        }

        public virtual void Update(ContentManager content, GameTime gameTime)
        {
            this.weaponPosition = this.FindNewPosition();

            double totalMS = (DateTime.Now - this.lastAttackTime).TotalMilliseconds;

            if (totalMS >= this.attackLimiter &&
                this.AttackButtonDown)
            {
                this.lastAttackTime = DateTime.Now;
                this.StartAttack();
            }
            else if (!this.AttackButtonDown)
            {
                this.Visible = false;
            }
        }

        public abstract void Draw(SpriteBatch theSpriteBatch);

        protected abstract void StartAttack();

        protected virtual Vector2 FindNewPosition()
        {
            float x = (float)this.WeaponOwner.BoundingBox.Center.X;
            float y = this.WeaponOwner.BoundingBox.Y + 20f;
            return new Vector2(x, y);
        }
    }
}
