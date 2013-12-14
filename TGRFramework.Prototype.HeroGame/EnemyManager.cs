// -----------------------------------------------------------------------
// <copyright file="EnemyManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.HeroGame
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using TGRFramework.Prototype.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EnemyManager
    {
        private Random random = new Random((int)DateTime.Now.Ticks);
        private bool generateEnemies = true;

        public EnemyManager(LevelScreen parentScreen)
        {
            this.ParentScreen = parentScreen;
        }

        public LevelScreen ParentScreen { get; set; }

        /// <summary>
        /// Closest point off screen to the right
        /// </summary>
        public int OffScreenRight
        {
            get
            {
                return (int)this.ParentScreen.cameraPositionX + this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width; 
            }
        }

        /// <summary>
        /// Point off screen to the right.
        /// </summary>
        public int OffScreenLeft
        {
            get
            {
                return (int)this.ParentScreen.cameraPositionX; // Need to subtract the width of what we are spawning to the left
            }
        }

        public int CurrentOffscreenY
        {
            get
            {
                return (int)this.ParentScreen.cameraPositionY + this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height;
            }
        }

        public void Initialize()
        {
            //
            // Load in configured enemies - TODO txt file depreciated, how do we want this to look
            // TODO this is a good model to follow for ItemManager
            //
            foreach (Vector2 vector in this.ParentScreen.LevelSprite.EnemyVectors)
            {
                GroundEnemyCharacterSprite groundEnemy = new GroundEnemyCharacterSprite(this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "RobotLeft", "RobotRight", vector, 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
                this.ParentScreen.Sprites.Add(groundEnemy);
                groundEnemy.CollisionWithSprite += this.OnSpriteCollision;
            }
        }

        public void LoadContent()
        {
            //
            // Starting enemies
            //

            // Generate some initial ground enemies in case user resizes window before we can generate offscreen
            this.AddGroundEnemySprite();
            this.AddGroundEnemySprite();
            this.AddGroundEnemySprite();
            this.AddGroundEnemySprite();

            //
            // RNG Spawn Enemies
            //

            // Homing character loop
            Task.Factory.StartNew(() =>
            {
                while (this.generateEnemies)
                {
                    lock (this.ParentScreen.SubsystemLock)
                    {
                        this.AddHomingCharacterSprite();
                    }

                    System.Threading.Thread.Sleep(2000);
                }
            });

            // Ground character loop
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (this.generateEnemies)
                    {
                        this.AddGroundEnemySprite();

                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    this.ParentScreen.Log.Error("Exception when generating ground enemy sprite! Loop is now down.\n{0}\n{1}", e.Message, e.StackTrace);
                }
            });

            this.AddSaucerBoss();
        }

        private void AddSaucerBoss()
        {
            SaucerBossCharacterSprite bossRef = new SaucerBossCharacterSprite(s => this.ParentScreen.AddSpriteCallback(s), s => this.ParentScreen.RemoveSpriteCallback(s),
                this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, 
                "Saucer", Vector2.Zero, 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
            
            bossRef.LoadContent(this.ParentScreen.ContentManager);

            //Rectangle playingField = new Rectangle(0, 0, (int)PlatformerLevel.LevelWidth, (int)PlatformerLevel.LevelHeight);

            Rectangle playingField = new Rectangle((int)PlatformerLevel.LevelWidth - 1600, 0, 800, 800); // TODO when I don't use 1600 I hit a exception

            List<Rectangle> bossSpawn = this.ParentScreen.LevelSprite.GetAreaRectangleFits(playingField, bossRef.BoundingBox);
            bossRef = null;

            //if (bossSpawn.Count == 0) return; 

            int index = this.random.Next(0, bossSpawn.Count - 1);

            SaucerBossCharacterSprite boss = new SaucerBossCharacterSprite(s => this.ParentScreen.AddSpriteCallback(s), s => this.ParentScreen.RemoveSpriteCallback(s),
                this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "Saucer",
                new Vector2(bossSpawn[index].X, bossSpawn[index].Y), 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);

            lock (this.ParentScreen.SubsystemLock)
            {
                boss.LoadContent(this.ParentScreen.ContentManager);
                boss.CollisionWithSprite += this.OnSpriteCollision;
                this.ParentScreen.Sprites.Add(boss);
            }
        }

        private void AddGroundEnemySprite()
        {
            List<EnemyCharacterSprite> newEnemies = this.CreateGroundEnemySprite();

            if (newEnemies.Count > 0)
            {
                lock (this.ParentScreen.SubsystemLock)
                {
                    foreach (EnemyCharacterSprite newEnemy in newEnemies)
                    {
                        newEnemy.LoadContent(this.ParentScreen.ContentManager);
                        newEnemy.CollisionWithSprite += this.OnSpriteCollision;
                        this.ParentScreen.Sprites.Add(newEnemy);
                    }

                }
            }
        }

        private List<EnemyCharacterSprite> CreateGroundEnemySprite()
        {
            // Find if there is ground outside the screen

            int viewHeight = this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height;

            GroundEnemyCharacterSprite groundEnemyReference = new GroundEnemyCharacterSprite(this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "RobotLeft", "RobotRight", Vector2.Zero, 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
            groundEnemyReference.LoadContent(this.ParentScreen.ContentManager);

            this.ParentScreen.Log.Info("Attempting to place ground enemy sprite. Screen = ({0},{1}), Offscreen = ({2},{3})", (int)this.ParentScreen.cameraPositionX, (int)this.ParentScreen.cameraPositionY, this.OffScreenRight, this.CurrentOffscreenY );

            // I'm sure I could do this more dynamically..
            List<Rectangle> possibleSpawns = new List<Rectangle>();

            // All possible off-screen spawns to the right of the screen
            List<Rectangle> spawnRight = this.ParentScreen.LevelSprite.GetAreaRectangleFits(new Rectangle(
                (int)this.OffScreenRight + (groundEnemyReference.CharacterTexture.Width), (int)this.ParentScreen.cameraPositionY,
                this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height),
                groundEnemyReference.BoundingBox); // TODO_OPTIMIZATION Just pass xy?

            // All possible off-screen spawns to the left of the screen
            int leftX = (int)this.ParentScreen.cameraPositionX - this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width; // TODO requires 1 viewport distance away from 0 to spawn
            List<Rectangle> spawnLeft = this.ParentScreen.LevelSprite.GetAreaRectangleFits(new Rectangle(
                leftX, (int)this.ParentScreen.cameraPositionY,
                this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height),
                groundEnemyReference.BoundingBox); // TODO_OPTIMIZATION Just pass xy?

            // All possible off-screen spawns above the screen
            List<Rectangle> spawnUp = this.ParentScreen.LevelSprite.GetAreaRectangleFits(new Rectangle(
                (int)this.ParentScreen.cameraPositionX, (int)this.ParentScreen.cameraPositionY - viewHeight - (groundEnemyReference.CharacterTexture.Height),
                this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height),
                groundEnemyReference.BoundingBox); // TODO_OPTIMIZATION Just pass xy?

            // All possible off-screen spawns below the screen
            List<Rectangle> spawnDown = this.ParentScreen.LevelSprite.GetAreaRectangleFits(new Rectangle(
                (int)this.ParentScreen.cameraPositionX, (int)this.ParentScreen.cameraPositionY + viewHeight + (groundEnemyReference.CharacterTexture.Height),
                this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height),
                groundEnemyReference.BoundingBox); // TODO_OPTIMIZATION Just pass xy?
 
            // TODO_HIGH limit number of enemies spawned and allow pathing from offscreen to on screen

            List<EnemyCharacterSprite> spritsSpawned = new List<EnemyCharacterSprite>();

            // If we can, spawn on random spot of ground

            if (spawnRight.Count > 0) // TODO_LOW Investigate edge case - needed to add ground at the bottom of the level
            {
                int index = this.random.Next(0, spawnRight.Count - 1);
                GroundEnemyCharacterSprite groundEnemyRight = new GroundEnemyCharacterSprite(this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "RobotLeft", "RobotRight", new Vector2(spawnRight[index].X, spawnRight[index].Y), 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
                this.ParentScreen.Log.Debug("Spawning enemy sprite right at {0}. Hero is at {1}", groundEnemyRight.BoundingBox, this.ParentScreen.HeroSprite.BoundingBox);
                spritsSpawned.Add(groundEnemyRight);
            }

            if (spawnLeft.Count > 0)
            {
                int index = this.random.Next(0, spawnLeft.Count - 1);
                GroundEnemyCharacterSprite groundEnemyLeft = new GroundEnemyCharacterSprite(this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "RobotLeft", "RobotRight", new Vector2(spawnLeft[index].X, spawnLeft[index].Y), 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
                this.ParentScreen.Log.Debug("Spawning enemy sprite left at {0}. Hero is at {1}", groundEnemyLeft.BoundingBox, this.ParentScreen.HeroSprite.BoundingBox);
                spritsSpawned.Add(groundEnemyLeft);
            }

            if (spawnUp.Count > 0)
            {
                int index = this.random.Next(0, spawnUp.Count - 1);
                GroundEnemyCharacterSprite groundEnemyUp = new GroundEnemyCharacterSprite(this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "RobotLeft", "RobotRight", new Vector2(spawnUp[index].X, spawnUp[index].Y), 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
                this.ParentScreen.Log.Debug("Spawning enemy sprite up at {0}. Hero is at {1}", groundEnemyUp.BoundingBox, this.ParentScreen.HeroSprite.BoundingBox);
                spritsSpawned.Add(groundEnemyUp);
            }

            if (spawnDown.Count > 0)
            {
                int index = this.random.Next(0, spawnDown.Count - 1);
                GroundEnemyCharacterSprite groundEnemyDown = new GroundEnemyCharacterSprite(this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "RobotLeft", "RobotRight", new Vector2(spawnDown[index].X, spawnDown[index].Y), 2.5f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
                this.ParentScreen.Log.Debug("Spawning enemy sprite down at {0}. Hero is at {1}", groundEnemyDown.BoundingBox, this.ParentScreen.HeroSprite.BoundingBox);
                spritsSpawned.Add(groundEnemyDown);
            }

            return spritsSpawned;
        }

        private void AddHomingCharacterSprite()
        {
            Vector2 nextVector = Vector2.Zero;
            int side = this.random.Next(0, 4);

            // TODO this doesn't exit clean
            switch (side)
            {
                case 0:
                    {
                        // top
                        int x = this.random.Next(0, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width + (int)this.ParentScreen.cameraPositionX);
                        nextVector = new Vector2(x, -50);
                        break;
                    }
                case 1:
                    {
                        // bottom
                        int x = this.random.Next(0, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width + (int)this.ParentScreen.cameraPositionX);
                        nextVector = new Vector2(x, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height + (int)this.ParentScreen.cameraPositionY + 50);
                        break;
                    }
                case 2:
                    {
                        // right
                        int y = this.random.Next(0, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height + (int)this.ParentScreen.cameraPositionY);
                        nextVector = new Vector2(-50, y);
                        break;
                    }
                case 3:
                    {
                        // left
                        int y = this.random.Next(0, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Height + (int)this.ParentScreen.cameraPositionY);
                        nextVector = new Vector2(this.ParentScreen.GraphicsDeviceManager.GraphicsDevice.Viewport.Width + (int)this.ParentScreen.cameraPositionX + 50, y);
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("Invalid side to generate a homing character!");
                    }
            }

            HomingCharacterSprite killerSprite = new HomingCharacterSprite(this.ParentScreen.HeroSprite, this.ParentScreen.WeaponSprite, this.ParentScreen.RangedWeaponSprite, "FireBall", nextVector, 3f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
            killerSprite.LoadContent(this.ParentScreen.ContentManager);
            this.ParentScreen.Sprites.Add(killerSprite);
            killerSprite.CollisionWithSprite += this.OnSpriteCollision;
        }

        // TODO rename - this removes a sprite that is unexpected
        private void OnSpriteCollision(ISprite sprite1, EnemyCharacterSprite sprite2)
        {
            sprite2.CollisionWithSprite -= this.OnSpriteCollision;
            this.ParentScreen.Log.Info("Collision between {0} and {1}", sprite1.GetType(), sprite2.GetType());

            // Need to push to message, currently in update for this collection. Will be processed after current 'update'
            // Will need to process +hp and +exp logic here (TODO investigate if we want to refactor and isolate (with delegates?))..
            this.ParentScreen.AddMessage(new ActionMessage(new System.Action(() => 
            { 
                lock (this.ParentScreen.SubsystemLock) 
                { 
                    this.ParentScreen.Sprites.Remove(sprite2);
                    int numCoins;

                    if (sprite2 is SaucerBossCharacterSprite) //TODO - Hopefully my last hack before "The refactoring".. OOP. Note that 1000 coins fly all over the place
                    {
                        numCoins = 1000;
                    }
                    else
                    {
                        numCoins = random.Next(0, 10);
                    }

                    for (int i = 0; i < numCoins; i++)
                    {
                        CoinSprite coin = new CoinSprite(this.ParentScreen.HeroSprite, "Coin", new Vector2(sprite2.BoundingBox.X + 6 * i, sprite2.BoundingBox.Y + 3 * i), 10f, this.ParentScreen.GraphicsDeviceManager.GraphicsDevice, this.ParentScreen.LevelSprite);
                        coin.CoinCollected += this.OnCoinCollected;
                        coin.CoinCollected += this.ParentScreen.HudManager.CoinCountSprite.OnCoinCollected;
                        coin.LoadContent(this.ParentScreen.ContentManager);
                        this.ParentScreen.Sprites.Add(coin);
                    }

                    int spawn = this.random.Next(0, 4);
                    if (spawn == 2)
                    {
                        HealthOrbSprite hpOrb = new HealthOrbSprite(this.ParentScreen.HeroSprite, "HPOrb", new Vector2(sprite2.BoundingBox.X + 6, sprite2.BoundingBox.Y + 3), 20, 20, 100f, 5, 3, 80, 80);
                        hpOrb.LoadContent(this.ParentScreen.ContentManager);
                        hpOrb.IntersectsPlayableCharacter += this.OnHealthOrbSpriteCollected;
                        this.ParentScreen.Sprites.Add(hpOrb);
                    }

                }
            })));

            if (sprite1 is PlayableCharacterSprite)
            {
                this.ParentScreen.heroHits++;
            }
            else if (sprite1 is WeaponSprite)
            {
                this.ParentScreen.enemyHits++;
            }

            this.ParentScreen.scoreText.OutputText = string.Format("Hero Hit {0}, Enemy Hit {1}", this.ParentScreen.heroHits, this.ParentScreen.enemyHits);
        }
        
        private void OnHealthOrbSpriteCollected(HealthOrbSprite orb, int healthGain)
        {
            this.ParentScreen.AddMessage(new ActionMessage(new System.Action(() => 
            { 
                lock (this.ParentScreen.SubsystemLock) 
                {
                    // TODO add to data store
                    this.ParentScreen.HeroSprite.TryRestoreHp(1);

                    orb.IntersectsPlayableCharacter -= this.OnHealthOrbSpriteCollected;
                    this.ParentScreen.Sprites.Remove(orb);
                }
            })));    
        }

        // TODO these share a base - CollectableSprite. Encapsulate & abstract these

        private void OnCoinCollected(CoinSprite coin)
        {
            this.ParentScreen.AddMessage(new ActionMessage(new System.Action(() => 
            { 
                lock (this.ParentScreen.SubsystemLock) 
                {
                    // TODO add to data store

                    coin.CoinCollected -= this.OnCoinCollected;
                    this.ParentScreen.Sprites.Remove(coin);
                }
            })));
        }
    }
}
