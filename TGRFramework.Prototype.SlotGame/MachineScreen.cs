// -----------------------------------------------------------------------
// <copyright file="MachineScreen.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using System.Threading;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using TGRFramework.Prototype.Common;

    /// <summary>
    /// Update summary.
    /// </summary>
    public class MachineScreen : Screen
    {
        // TODO user inputs credits
        public static int STARTING_CREDITS = 100; // TODO Get config to work.. int.Parse(ConfigurationManager.AppSettings["startingCredits"]);;
        private bool spinToggle = false;

        /// <summary>
        /// Constructor for this screen as stand along game
        /// </summary>
        public MachineScreen(IGameCompleteDelegate<IGame> screenComplete, GraphicsDeviceManager graphics)
            : base(screenComplete, graphics, "SlotMachineScreen")
        {
            // TODO_HIGH this was a good exercise - we now see that DataStore needs to depend on DataBase, top down there buddy 
            var tempDb = new SlotSimDatabaseXML("TempDBLog");

            this.DataStore = new SlotSimDataStore(tempDb, "SlotSimDataStore");

            tempDb.Initialize();

            ThreadPool.QueueUserWorkItem(t => { this.DataStore.Run(); });

            this.ReelManager = new ReelManager(this, this.Log);
            this.ReelManager.SpinComplete += this.OnSpinComplete;

            ThreadPool.QueueUserWorkItem(t => { this.Run(); });
        }

        /// <summary>
        /// Constructor for this screen as a member of a complex game
        /// </summary>
        public MachineScreen(IGameCompleteDelegate<IGame> screenComplete, SlotSimDataStore dataStore, GraphicsDeviceManager graphics)
            : base(screenComplete, graphics, "SlotMachineScreen")
        {
            lock (this.SubsystemLock)
            {
                this.DataStore = dataStore;
                
                this.ReelManager = new ReelManager(this, this.Log);
                this.ReelManager.SpinComplete += this.OnSpinComplete;
            }
        }

        // TODO do we want to encapsulate any of these?

        /// <summary>
        /// Database subsystem for persisting data out of memory
        /// </summary>
        public SlotSimDataStore DataStore { get; set; }

        public TextSprite ClickCountDisplay { get; set; }

        public TextSprite SpinTempTest { get; set; }

        public IncrementalTextSprite CreditsDisplay { get; set; }

        public TextSprite PayoutText { get; set; }
  
        public ReelManager ReelManager { get; set; }

        public override void Initialize()
        {
            lock (this.SubsystemLock)
            {
                this.DataStore.Credits = STARTING_CREDITS;

                // ***** Symbol Sprites *****
                this.ReelManager.Initialize(this.GraphicsDeviceManager);

                // ***** Background Sprites *****
                BackgroundSprite background = new BackgroundSprite("SlotBackground", this.GraphicsDeviceManager.GraphicsDevice);
                this.Sprites.Add(background);

                // ***** Button Sprites *****
                Vector2 resetButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 765f,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 123f);
                ButtonSprite resetButton = new ButtonSprite("ResetButtonUp", "ResetButtonDown", resetButtonPos);

                Vector2 minusButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 644f,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 123f);
                ButtonSprite minusButton = new ButtonSprite("MinusBetUp", "MinusBetDown", minusButtonPos);

                Vector2 plusButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 524f,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 123f);
                ButtonSprite plusButton = new ButtonSprite("PlusBetUp", "PlusBetDown", plusButtonPos);

                Vector2 spinButtonPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 123f,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 123f);
                ButtonSprite spinButton = new ButtonSprite("Spin2Up", "Spin2Down", spinButtonPos);

                resetButton.ButtonClickedEvent += this.OnResetButtonClicked;
                minusButton.ButtonClickedEvent += this.OnMinusButtonClicked;
                plusButton.ButtonClickedEvent += this.OnPlusButtonClicked;
                spinButton.ButtonClickedEvent += this.OnSpinButtonClicked;

                this.Sprites.Add(spinButton);
                this.Sprites.Add(plusButton);
                this.Sprites.Add(resetButton);
                this.Sprites.Add(minusButton);

                // ***** Text Sprites *****
                // Credits per spin
                Vector2 creditsPerSpinPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 350f,
                            this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 202f);
                this.ClickCountDisplay = new TextSprite(string.Format("{0}", this.DataStore.CreditsPerSpin.ToString()), "JingJing", creditsPerSpinPos, Color.OrangeRed);

                // Total credits user has input
                Vector2 position3 = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 508,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 202f);
                this.CreditsDisplay = new IncrementalTextSprite(200f, string.Format("{0}", this.DataStore.Credits.ToString()), "JingJing", position3, Color.OrangeRed);

                // Credits won after spin
                Vector2 position4 = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 110,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 202f);
                this.PayoutText = new TextSprite(string.Format("{0}", 0), "JingJing", position4, Color.OrangeRed);

                // Debug string output for reels
                Vector2 tempOutputPos = new Vector2(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width - 300f,
                    this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height - 142f);
                this.SpinTempTest = new TextSprite(string.Format(this.DataStore.TempOutput, this.DataStore.CreditsPerSpin.ToString()), "JingJing", tempOutputPos, Color.OrangeRed);

                this.Sprites.Add(this.ClickCountDisplay);
                this.Sprites.Add(this.CreditsDisplay);
                this.Sprites.Add(this.SpinTempTest);
                this.Sprites.Add(this.PayoutText);
            }
        }

        public override void Update(ContentManager content, GameTime gameTime)
        {
            this.ClickCountDisplay.OutputText = string.Format("{0}", this.DataStore.CreditsPerSpin.ToString());
            this.CreditsDisplay.OutputText = string.Format("{0}", this.DataStore.Credits.ToString());
            this.SpinTempTest.OutputText = this.DataStore.TempOutput;
            base.Update(content, gameTime);
        }

        private void OnSpinButtonClicked()
        {
            this.Log.Info("Spin button clicked");
            this.spinToggle = !this.spinToggle;
            this.AddMessage(new SpinMessage(this.spinToggle, this.Log));
        }

        private void OnSpinComplete(int payout)
        {
            this.spinToggle = false;
            this.DataStore.Credits += payout;
            this.PayoutText.OutputText = string.Format("{0}", payout);
        }

        private void OnResetButtonClicked()
        {
            this.Log.Info("Reset button clicked.");
            this.AddMessage(new ResetMessage(this.Log));

            // This allows message queue to empty before switching screens
            this.IGameComplete(typeof(SlotSplashScreen));
        }

        private void OnMinusButtonClicked()
        {
            this.Log.Info("Minus button clicked.");
            this.AddMessage(new ReduceBetMessage(this.Log));
        }

        private void OnPlusButtonClicked()
        {
            this.Log.Info("Plus button clicked.");
            this.AddMessage(new IncreaseBetMessage(this.Log));
        }
    }
}
