// -----------------------------------------------------------------------
// <copyright file="PlayableCharacterSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PlayableCharacterSprite : CharacterSprite
    {
        private bool firstKeyDown = false;
        private DateTime lastDamageTime = DateTime.Now;
        private const int DAMAGE_LIMITER_MS = 666;
        private const int MAXHP = 30;

        public PlayableCharacterSprite(string leftContent, string content, Vector2 startingPostion, float movementSpeed, GraphicsDevice gfx, PlatformerLevel level)
            : base(content, startingPostion, movementSpeed, gfx, level)
        {
            this.LeftContent = leftContent;
            this.Facing = MeleeWeaponSprite.SwingFacing.Undefined;
            this.HitPoints = MAXHP;
        }

        public string LeftContent { get; set; }

        public event Action<int> UpdateHitPoints;

        private SoundEffect HitSound { get; set; }

        public override void LoadContent(ContentManager content)
        {
            this.HitSound = content.Load<SoundEffect>("HeroHit");
            this.HitSound.Name = "HeroHit";
            base.LoadContent(content);
        }

        public override void Update(ContentManager content, GameTime gameTime)
        {
            float leftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;

            if (Math.Abs(leftThumb) < 0.5f)
            {
                leftThumb = 0.0f;
            }
        
            // +1 = all the way right
            // -1 = all the way left
            if (!this.isTakingDamage) // TODO Input static class to make this readable
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight) || leftThumb > 0)
                {
                    this.CharacterTexture = content.Load<Texture2D>(this.CharacterContent);

                    this.TryMoveRight((int)this.MovementSpeed);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft) || leftThumb < 0)
                {
                    this.CharacterTexture = content.Load<Texture2D>(this.LeftContent);

                     this.TryMoveLeft((int)this.MovementSpeed);
                }

                if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)) && !this.firstKeyDown)
                {
                    this.IsJumping = true;
                    this.firstKeyDown = this.JumpModel is RestrictedJumpModel ?  true : false; // TODO 'flying' does not work with this
                }
                else if (!(Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)) && this.firstKeyDown)
                {
                    if (!this.isFalling)
                        this.firstKeyDown = false;
                }
            }
            else
            {
                //PlatformerLevel.Log.Debug("PlayableCharacter cannot move while taking damage.");
            }
            
            base.Update(content, gameTime);
        }

        public override bool TryTakeDamage(int damage)
        {
            if ((DateTime.Now - this.lastDamageTime).TotalMilliseconds < DAMAGE_LIMITER_MS) // Filter out damage spam
            {
                return false;
            }
            else if (base.TryTakeDamage(damage))
            {
                this.Level.LevelSoundQueue.Add(this.HitSound);
                this.RaiseUpdateHitPoints(this.HitPoints);
                this.lastDamageTime = DateTime.Now;
                return true;
            }

            return false;
        }
        
        public bool TryRestoreHp(int hp)
        {
            if (this.HitPoints + hp > MAXHP)
            {
                return false;
            }

            this.HitPoints += hp;
            this.RaiseUpdateHitPoints(this.HitPoints);

            return true;
        }

        protected override IPhysicsModel LoadJumpModel()
        {
            return new RestrictedJumpModel(this);
        } 

        private void RaiseUpdateHitPoints(int newHP)
        {
            if (this.UpdateHitPoints != null)
            {
                this.UpdateHitPoints(newHP);
            }
        }
    }
}
