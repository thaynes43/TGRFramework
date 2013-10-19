// -----------------------------------------------------------------------
// <copyright file="LevelScreen.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.HeroGame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using TGRFramework.Prototype.Common;

    /// <summary>
    /// TODO: Make base class
    /// </summary>
    public class LevelScreen : Screen
    {
        public static TextSprite helperText;
        
        public float cameraPositionX = 0f;
        public float cameraPositionY = 0f;

        // TODO manage score
        public TextSprite scoreText;
        public TextSprite hpText;
        public int enemyHits = 0;
        public int heroHits = 0;


        public LevelScreen(IGameCompleteDelegate<IGame> screenComplete, GraphicsDeviceManager graphics, ContentManager content)
            : base(screenComplete, graphics, "LevelScreen")
        {
            this.ContentManager = content;
            this.HUDSprites = new List<ISprite>();
            this.EnemyManager = new EnemyManager(this);

            PlatformerLevel.Log = this.Log;
        }

        public HudManager HudManager { get; set; }

        public EnemyManager EnemyManager { get; set; }

        public PlatformerLevel LevelSprite { get; private set; }

        public PlayableCharacterSprite HeroSprite { get; private set; }

        public MeleeWeaponSprite WeaponSprite { get; private set; }

        public RangedWeaponSprite RangedWeaponSprite { get; private set; }

        public List<ISprite> HUDSprites { get; private set; }

        public ContentManager ContentManager { get; set; }

        public override void Initialize()
        {
            lock (this.SubsystemLock)
            {
                // ***** Button Sprites *****   
                Vector2 quitButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 620,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 75);

                ButtonSprite quitButton = new ButtonSprite("QuitButtonUp", "QuitButtonDown", quitButtonPos);

                // ***** Background Sprites *****   
                this.LevelSprite = new PlatformerLevel("DiagnosticLevel.txt", this.GraphicsDeviceManager.GraphicsDevice);

                // ***** Character Sprites *****

                this.HeroSprite = new PlayableCharacterSprite("TweakoLeft", "TweakoRight", Vector2.Zero, 7f, this.GraphicsDeviceManager.GraphicsDevice, this.LevelSprite);

                this.WeaponSprite = new MeleeWeaponSprite("GreenSword", this.HeroSprite, this.GraphicsDeviceManager.GraphicsDevice);
                this.WeaponSprite.LoadContent(this.ContentManager);

                this.RangedWeaponSprite = new RangedWeaponSprite("Shotgun", "Shotgun_Left", this.HeroSprite, this.GraphicsDeviceManager.GraphicsDevice, s =>
                {
                    this.AddMessage(new ActionMessage(new System.Action(() =>
                    {
                        lock (this.SubsystemLock)
                        {
                            this.Sprites.Add(s);
                            s.LoadContent(this.ContentManager);
                        }
                    })));
                },
                s =>
                {
                    this.AddMessage(new ActionMessage(new System.Action(() =>
                    {
                        lock (this.SubsystemLock)
                        {
                            this.Sprites.Remove(s);
                        }
                    })));
                }
                );

                this.RangedWeaponSprite.LoadContent(this.ContentManager);

                helperText = new TextSprite("Test", "Analysis", new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width, 100), Color.Black);

                this.scoreText = new TextSprite("Test", "Analysis",
                    new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width, 50), Color.Black);

                this.hpText = new TextSprite("HPText", "Analysis", new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width, 30), Color.Black);
                this.HeroSprite.UpdateHitPoints += HeroSprite_UpdateHitPoints;

                this.scoreText.LoadContent(this.ContentManager);
                
                this.RangedWeaponSprite.updateText += new Action<string>(text => { helperText.OutputText = text; });

                this.HudManager = new HudManager(this, HeroGame.HudManager.ScreenJustify.TopLeft); 

                /*
                 *  Add sprites 
                 */ 

                this.Sprites.Add(LevelSprite);

                this.HUDSprites.Add(helperText);

                this.HUDSprites.Add(this.scoreText);
                this.HUDSprites.Add(this.hpText);

                this.HUDSprites.Add(this.HudManager);

                //this.Sprites.Add(quitButton);
                this.Sprites.Add(this.WeaponSprite);
                this.Sprites.Add(this.HeroSprite);
                this.Sprites.Add(this.RangedWeaponSprite);

                this.EnemyManager.Initialize();
                this.HudManager.Initialize();

                quitButton.ButtonClickedEvent += this.OnQuitClicked;
            }
        }

        private void HeroSprite_UpdateHitPoints(float obj)
        {
            this.hpText.OutputText = string.Format("Hero HP: {0}", obj);
        }

        public override void LoadContent(ContentManager content)
        {
            lock (this.SubsystemLock)
            {
                // Must load before everything
                this.LevelSprite.LoadLevel(content);

                foreach (ISprite sprite in this.HUDSprites)
                {
                    sprite.LoadContent(content);
                }

                base.LoadContent(content);
            }
        }

        public override void Update(ContentManager content, GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                this.IGameComplete(typeof(HeroSplashScreen));
            }
            else
            {
                lock (this.SubsystemLock)
                {
                    foreach (ISprite sprite in this.Sprites)
                    {
                        bool update = true;
                        CharacterSprite characterSprite = sprite as CharacterSprite;
                        if (characterSprite != null)
                        {
                            // TODO_Enhancement some sprites may path off screen then stop - Add is two screens over ?
                            // TODO_High homing sprites get all sorts of screwed up if you aren't moving as they spawn off the screen
                            if (!characterSprite.IsOnScreen())
                            {
                                //this.Log.Debug("Skipping update for a {0} at {1}", characterSprite.GetType(), characterSprite.BoundingBox);
                                update = false;
                            }
                        }
                        if (update)
                        {
                            sprite.Update(content, gameTime);
                        }
                    }
                }             
            }
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            lock (this.SubsystemLock)
            {
                // Draw sprites which scroll with the game
                theSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, this.ScrollCamera());

                // TODO 
                PlatformerLevel.CameraPositionX = this.cameraPositionX;
                PlatformerLevel.CameraPositionY = this.cameraPositionY;

                foreach (ISprite sprite in this.Sprites)
                {
                    bool draw = true;
                    CharacterSprite characterSprite = sprite as CharacterSprite;
                    
                    if (characterSprite != null)
                    {
                        if (!characterSprite.IsOnScreen())
                        {
                            draw = false;
                        }
                    }

                    if (sprite.Visible && draw)
                    {
                        sprite.Draw(theSpriteBatch);
                    }
                }

                theSpriteBatch.End();

                // Draw stationary 'HUD' sprites
                theSpriteBatch.Begin();

                foreach (ISprite sprite in this.HUDSprites)
                {
                    if (sprite.Visible)
                    {
                        sprite.Draw(theSpriteBatch);
                    }
                }

                theSpriteBatch.End();
            }
        }

        

        /// <summary>
        /// Transition out of this StoryBoard
        /// </summary>
        private void OnQuitClicked()
        {
            Log.Info("Quit button clicked.");
            this.IGameComplete(typeof(HeroSplashScreen));
        }

        /// <summary>
        /// Scrolling level based off of hero sprite movement
        /// </summary>
        /// <returns>Translation matrix</returns>
        private Matrix ScrollCamera()
        {
            Viewport viewport = this.GraphicsDeviceManager.GraphicsDevice.Viewport;

            //
            // X Translation
            //
            const float HorizonMargin = 0.35f;
            float cameraMovementX = 0.0f;
            float marginWidth = viewport.Width * HorizonMargin;
            float marginLeft = this.cameraPositionX + marginWidth;
            float marginRight = this.cameraPositionX + viewport.Width - marginWidth;

            if (this.HeroSprite.BoundingBox.X < marginLeft)
            {
                // Here is before left margin, move left 
                cameraMovementX = this.HeroSprite.BoundingBox.X - marginLeft;
            }
            else if (this.HeroSprite.BoundingBox.X > marginRight)
            {
                // Here is past the right margin, move right
                cameraMovementX = this.HeroSprite.BoundingBox.X - marginRight;
            }

            // Do not scroll past width of level
            float maxX = PlatformerLevel.LevelWidth - viewport.Width;
            this.cameraPositionX = MathHelper.Clamp(this.cameraPositionX + cameraMovementX, 0.0f, maxX);

            //
            // Y Translation
            //
            const float TopMargin = 0.35f;
            const float BottomMargin = 0.35f;
            float cameraMovementY = 0.0f;
            float marginTop = this.cameraPositionY + viewport.Height * TopMargin;
            float marginBottom = this.cameraPositionY + viewport.Height - viewport.Height * BottomMargin;

            if (this.HeroSprite.BoundingBox.Y < marginTop)
            {
                // hero is above top margin, move down
                cameraMovementY = this.HeroSprite.BoundingBox.Y - marginTop;
            }
            else if (this.HeroSprite.BoundingBox.Y > marginBottom)
            {
                // hero is below bottom margin, move up
                cameraMovementY = this.HeroSprite.BoundingBox.Y - marginBottom;
            }

            // Do not scroll past height of level
            float maxY = PlatformerLevel.LevelHeight - viewport.Height;
            this.cameraPositionY = MathHelper.Clamp(this.cameraPositionY + cameraMovementY, 0.0f, maxY);

            PlatformerLevel.LevelHeightAdjustment = this.cameraPositionY;
            PlatformerLevel.LevelWidthAdjustment = this.cameraPositionX;

            // Apply translation
            return Matrix.CreateTranslation(-this.cameraPositionX, -this.cameraPositionY, 0.0f);
        }
    }
}
