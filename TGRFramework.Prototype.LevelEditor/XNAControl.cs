// -----------------------------------------------------------------------
// <copyright file="XNAControl.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.LevelEditor
{
    using System.Windows.Forms;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Form control for XNA Framework
    /// </summary>
    public abstract class XNAControl : Control
    {
        private GraphicsDeviceService graphicsDeviceService; // TODO wtf are these warnings...?
        private bool runUpdateThread;
        private ContentManager contentManager;
        private SpriteBatch spriteBatch;

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return this.graphicsDeviceService.GraphicsDevice;
            }
        }

        public ServiceProvider Services { get; private set; }

        /// <summary>
        /// Initialize control prior to run
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Load content of control
        /// </summary>
        protected abstract void LoadContent(ContentManager content);

        /// <summary>
        /// Update control on thread separate from GUI
        /// </summary>
        protected abstract void Update(ContentManager content, GameTime gameTime);

        /// <summary>
        /// Draw control on GUI thread
        /// </summary>
        protected abstract void Draw(SpriteBatch theSpriteBatch);

        protected override void OnCreateControl()
        {
            if (!this.DesignMode)
            {
                // Create dependencies
                this.Services = new ServiceProvider();
                this.graphicsDeviceService = new GraphicsDeviceService(this.Handle, this.ClientSize.Height, this.ClientSize.Width);
                this.Services.AddService<IGraphicsDeviceService>(this.graphicsDeviceService);
                this.contentManager = new ContentManager(this.Services, "Content");
                this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
                Mouse.WindowHandle = this.Handle;
                
                // Initialize derived classes
                this.Initialize();

                // Load all sprites
                this.LoadContent(this.contentManager);

                // Begin control loops
                this.runUpdateThread = true;
                Application.Idle += delegate { this.Invalidate(); };
                System.Threading.Tasks.Task.Factory.StartNew(() => { while (this.runUpdateThread) { this.Update(this.contentManager, null); } });
            }

            base.OnCreateControl();
        }

        protected override void OnPaint(PaintEventArgs e)
        {   
            // Avoid crashing Designer screen
            if (this.graphicsDeviceService == null)
            {
                e.Graphics.Clear(System.Drawing.Color.CornflowerBlue);
                using (System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                {
                    using (System.Drawing.StringFormat format = new System.Drawing.StringFormat())
                    {
                        format.Alignment = System.Drawing.StringAlignment.Center;
                        format.LineAlignment = System.Drawing.StringAlignment.Center;

                        e.Graphics.DrawString(this.GetType().ToString(), this.Font, brush, this.ClientRectangle, format);
                    }
                }
                
                return;
            }

            // Not in designer screen, draw game
            if (this.ClientSize.Width > this.GraphicsDevice.PresentationParameters.BackBufferWidth ||
                this.ClientSize.Height > this.GraphicsDevice.PresentationParameters.BackBufferHeight)
            {
                this.graphicsDeviceService.Resize(this.ClientSize.Width, this.ClientSize.Height);
            }

            Viewport viewport = new Viewport();
            viewport.X = 0;
            viewport.Y = 0;
            viewport.Width = this.ClientSize.Width;
            viewport.Height = this.ClientSize.Height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;
            this.GraphicsDevice.Viewport = viewport;

            this.Draw(this.spriteBatch);

            try
            {
                Rectangle sourceRectangle = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                this.GraphicsDevice.Present(sourceRectangle, null, this.Handle);
            }
            catch
            {
                // Do nothing
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Do nothing
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.runUpdateThread = false;

                // TODO wait until thread exits -- ?
                this.contentManager.Unload();
                this.graphicsDeviceService.Dispose();
                this.graphicsDeviceService = null;
            }

            base.Dispose(disposing);
        }
    }
}
