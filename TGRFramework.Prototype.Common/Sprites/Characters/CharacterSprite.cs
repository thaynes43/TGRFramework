// -----------------------------------------------------------------------
// <copyright file="PlayableCharacterSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using Microsoft.Xna.Framework;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CharacterSprite : ISprite
    {
        protected Vector2 CharacterPosition = Vector2.Zero;

        protected bool isTakingDamage = false;
        protected bool isFalling = false;

        public CharacterSprite(string content, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
        {
            this.Visible = true;
            this.CharacterContent = content;
            this.CharacterPosition = startingPostion;
            this.MovementSpeed = movementSpeed;
            this.GraphicsDevice = gfx;
            this.Level = level;

            this.FallModel = new SimpleFallModel(this);
            this.DamageModel = new CharacterDamageModel(this);
            this.JumpModel = this.LoadJumpModel();

            this.IsJumping = false;

            this.DamageModel.UpdateSpriteComplete += () => { this.isTakingDamage = false; }; // TODO bool event depending on state of action
            this.JumpModel.UpdateSpriteComplete += () => { this.IsJumping = false; };
            this.FallModel.UpdateSpriteComplete += () => { this.isFalling = true; };
        }

        public bool Visible { get; set; }

        // TODO too expensive to be a property
        //public Rectangle BoundingBox
        //{
        //    get
        //    {
        //        return new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
        //    }
        //}

        public Rectangle BoundingBox { get; protected set; }

        public int HitPoints { get; set; }

        public MeleeWeaponSprite.SwingFacing Facing { get; set; } // TODO move this enum

        protected string CharacterContent { get; set; }

        public Texture2D CharacterTexture { get; protected set; }

        protected GraphicsDevice GraphicsDevice { get; set; }

        public PlatformerLevel Level { get; set; }

        protected IPhysicsModel FallModel { get; set; }

        protected IPhysicsModel JumpModel { get; set; }

        protected IPhysicsModel DamageModel { get; set; }

        //protected Vector2 CharacterPosition { get; set; }

        protected float MovementSpeed { get; set; }

        protected bool IsJumping { get; set; }

        public virtual void LoadContent(ContentManager content)
        {
            this.CharacterTexture = content.Load<Texture2D>(this.CharacterContent);
            this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
        }

        public virtual void Update(ContentManager content, GameTime gameTime)
        {
            if (this is GroundEnemyCharacterSprite) // TODO What is this to work around?
            {
                this.isFalling = false;
            }

            if (!this.Level.IsOnGround(this.BoundingBox))
            {
                this.isFalling = true;
            }
            //else if (this.Level.IntersectsImpassiblePlatform(this.BoundingBox) && this.isFalling)            
            else if (this.Level.IntersectsImpassiblePlatform(this.BoundingBox) && this.IsJumping)
            {
                this.IsJumping = false;
                this.isFalling = true;
            }
            else
            {
                this.isFalling = false;
            }

            if (this.isTakingDamage) // Don't mess with damage physics
            {
                this.DamageModel.UpdateSpriteLocation();
            }
            else if (this.IsJumping) // Don't fall if we are jumping 
            {
                this.JumpModel.UpdateSpriteLocation();
            }
            else if (this.isFalling)
            {
                this.isFalling = false; // Set to true if fall is complete
                this.FallModel.UpdateSpriteLocation();
            }

            this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.CharacterTexture, this.CharacterPosition, Color.White);
        }

        public virtual bool TryTakeDamage(int damage)
        {
            if (!this.isTakingDamage)
            {
                this.isTakingDamage = true;
                this.HitPoints -= damage;
                return true;
            }

            return false;

        }

        public virtual bool TryMoveRight(int speed) // TODO move up and down 'stairs'
        {
            if (this.TryMoveHorizontal(speed))
            {
                this.Facing = MeleeWeaponSprite.SwingFacing.Right;
                this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
                return true;
            }

            return false;
        }

        public virtual bool TryMoveLeft(int speed)
        {
            if (this.TryMoveHorizontal(speed * -1))
            {
                this.Facing = MeleeWeaponSprite.SwingFacing.Left;
                this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
                return true;
            }

            return false;
        }

        private bool TryMoveHorizontal(int speed)
        {
            // Move level bounds
            if ((speed >= 0 && this.CharacterPosition.X + speed > PlatformerLevel.LevelWidth - this.CharacterTexture.Width) || (speed < 0 && this.CharacterPosition.X + speed < 0))
            {
                //PlatformerLevel.Log.Warn("Move attempts to leave viewspace, will be blocked.");
                return false;
            }

            Rectangle proposedMovement = new Rectangle((int)this.CharacterPosition.X + speed, (int)this.CharacterPosition.Y, this.CharacterTexture.Width, this.CharacterTexture.Height);
            if (!this.Level.IntersectsImpassiblePlatform(proposedMovement))
            {
                this.CharacterPosition.X += speed;
                return true;
            }
            else if (this.Level.IsAtStep(this.BoundingBox))
            {
                this.TryMoveUp((int)Platform.Height);

                proposedMovement = new Rectangle((int)this.CharacterPosition.X + speed, (int)this.CharacterPosition.Y, this.CharacterTexture.Width, this.CharacterTexture.Height);
                if (!this.Level.IntersectsImpassiblePlatform(proposedMovement))
                {
                    this.CharacterPosition.X += speed;
                    return true;
                }
            }
            else
            {
                //PlatformerLevel.Log.Debug("Cannot move horizontal! {0} is not at a step and the proposed movement distance = {1} intersects impassible.", this.GetType(), speed);
            }

            return false;
        }

        public virtual bool TryMoveUp(int speed)
        {
            if (this.TryMoveVertical(-1 * speed))
            {
                this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
                return true;
            }

            //PlatformerLevel.Log.Debug("Vertical move {0} up was blocked for {1}.", speed, this.GetType());
            return false;
        }

        public virtual bool TryMoveDown(int speed)
        {
            if (this.TryMoveVertical(speed))
            {
                this.BoundingBox = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y, this.CharacterTexture.Bounds.Width, this.CharacterTexture.Bounds.Height);
                return true;
            }

            //PlatformerLevel.Log.Debug("Vertical move {0} down was blocked for {1}.", speed, this.GetType());
            return false;
        }

        protected virtual IPhysicsModel LoadJumpModel()
        {
            return new SimpleJumpModel(this);
        }

        private bool TryMoveVertical(int speed)
        {
            if ((speed < 0 && this.CharacterPosition.Y + speed < 0) || (speed > 0 && this.CharacterPosition.Y + speed > PlatformerLevel.LevelHeight - this.CharacterTexture.Height))
            {
                return false;
            }

            Rectangle proposedMovement = new Rectangle((int)this.CharacterPosition.X, (int)this.CharacterPosition.Y + speed, this.CharacterTexture.Width, this.CharacterTexture.Height);
            if (this.Level.IntersectsImpassiblePlatform(proposedMovement))
            {
                return false;
            }

            this.CharacterPosition.Y += speed;

            return true;
        }

        // make property? 
        public bool IsOnScreen()
        {
            // TODO_HIGH OOP
            if (this is HomingCharacterSprite) return true;
            if (this.CharacterTexture == null) return false;

            return this.CharacterPosition.X >= PlatformerLevel.CameraPositionX - this.CharacterTexture.Width &&
                this.CharacterPosition.X <= this.GraphicsDevice.Viewport.Width + PlatformerLevel.CameraPositionX &&
                this.CharacterPosition.Y >= PlatformerLevel.CameraPositionY - this.CharacterTexture.Height &&
                this.CharacterPosition.Y <= this.GraphicsDevice.Viewport.Height + PlatformerLevel.CameraPositionY;        
        }
    }
}
