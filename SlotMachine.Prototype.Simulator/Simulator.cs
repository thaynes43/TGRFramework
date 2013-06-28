using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SlotMachine.Prototype.Common;
using SlotMachine.Prototype.SocketComm;
using SlotMachine.Prototype.Tools;

namespace SlotMachine.Prototype.Simulator
{
    public class Simulator : IDisposable
    {
        public static ILogTool Log = new LogTool("SimulatorLog");

        private Surface screen; //video screen
        private Surface background; // screen background

        private string dataDirectory = @"..\..\..\Data";
        
        ButtonSpriteSDL button;
        SpriteCollection sprites = new SpriteCollection();

        public Simulator()
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

            this.ClientSocket = new SocketWrapper("Simulator.Comm");

            this.ClientSocket.RegisterMessage(typeof(SpinMessage), this.OnSpinMessageReceived);
            this.button.ButtonClicked += () => this.ClientSocket.Send(new SpinMessage(100));
        }

        /// <summary>
        /// Destroy object
        /// </summary>
        ~Simulator()
        {
            Dispose(false);
        }

        public ISocket ClientSocket {get; set;}

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
            Simulator simulator = new Simulator();
            simulator.Go();
        }

        private void OnSpinMessageReceived(IMessage message)
        {
            Log.Debug("Spin Message Received : {0}", message.ToString());
        }

        private void Go()
        {
            screen = Video.SetVideoMode(800, 600);
            
            this.AddHandlers();
            Events.Run();
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
