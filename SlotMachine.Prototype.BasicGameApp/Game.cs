using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SdlDotNet.Graphics;
using SdlDotNet.Core;
using SdlDotNet.Graphics.Sprites;
using System.IO;
using System.Drawing;
using System.Collections.ObjectModel;
using SlotMachine.Prototype.Common;
using SlotMachine.Prototype.SocketComm;
using SlotMachine.Prototype.Tools;

namespace SlotMachine.Prototype.BasicGameApp
{
    public class Game : IDisposable
    {
        public static ILogTool Log = new LogTool("GameLog");

        private Surface screen; //video screen
        private Surface background; // screen background

        private string dataDirectory = @"..\..\..\Data";
        
        ButtonSpriteSDL button;
        SpriteCollection sprites = new SpriteCollection();

        public Game()
        {
            Surface keyDown = new Surface(Path.Combine(dataDirectory, "button.png"));
            Surface keyUp = new Surface(Path.Combine(dataDirectory, "buttonPressed.png"));
            keyUp.Transparent = true;
            keyUp.TransparentColor = Color.White;
            keyDown.Transparent = true;
            keyDown.TransparentColor = Color.White;
            button = new ButtonSpriteSDL(keyDown, keyUp, new Point(300, 200));

            sprites.Add(button);

            sprites.EnableMouseButtonEvent();

            this.ServerSocket = new SocketListener("Game.Comm");
            // this.ServerSocket.ListenForClient();

            this.ServerSocket.RegisterMessage(typeof(SpinMessage), this.OnSpinMessageReceived);
            this.button.ButtonClicked += () => this.ServerSocket.Send(new SpinMessage(989898));
        }

        /// <summary>
        /// Destroy object
        /// </summary>
        ~Game()
        {
            Dispose(false);
        }

        public ISocket ServerSocket { get; set; }

        /// <summary>
        /// For purposes of the demo browser only.
        /// </summary>
        public static string Title
        {
            get
            {
                return "Template: Used as a very basic template for new SdlDotNet apps.";
            }
        }

        [STAThread]
        public static void Main()
        {
            Game game = new Game();
            game.Go();
        }

        private void Go()
        {
            screen = Video.SetVideoMode(800, 600);
            
            this.AddHandlers();
            Events.Run();
        }

        private void OnSpinMessageReceived(IMessage message)
        {
            Log.Debug("Spin Message Received : {0}", message.ToString());
        }

        private void AddHandlers()
        {
            Events.Quit += new EventHandler<QuitEventArgs>(this.Events_Quit);
            Events.Tick += new EventHandler<TickEventArgs>(this.Events_Tick);
        }

        private void RemoveHandlers()
        {
            Events.Quit -= new EventHandler<QuitEventArgs>(this.Events_Quit);
            Events.Tick -= new EventHandler<TickEventArgs>(this.Events_Tick);
        }

        private void Events_Tick(object sender, TickEventArgs e)
        {
            //Video.Screen.Update();
            Collection<Rectangle> rects = screen.Blit(sprites);
            screen.Update(rects);
        }

        private void Events_Quit(object sender, QuitEventArgs e)
        {
            RemoveHandlers();
            Events.QuitApplication();
        }

        #region IDisposable Members
        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.background != null)
                    {
                        this.background.Dispose();
                        this.background = null;
                    }
                }
                this.disposed = true;
            }
        }
        #endregion
    }
}
