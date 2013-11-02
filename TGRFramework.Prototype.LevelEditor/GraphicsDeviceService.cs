// -----------------------------------------------------------------------
// <copyright file="GraphicsDeviceService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.LevelEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GraphicsDeviceService : IGraphicsDeviceService, IDisposable
    {
        private PresentationParameters presentationParameters;

        public GraphicsDeviceService(IntPtr handle, int width, int height)
        {
            this.presentationParameters = new PresentationParameters();

            this.presentationParameters.BackBufferWidth = width;
            this.presentationParameters.BackBufferHeight = height;
            this.presentationParameters.BackBufferFormat = SurfaceFormat.Color;
            this.presentationParameters.DepthStencilFormat = DepthFormat.Depth24;
            this.presentationParameters.DeviceWindowHandle = handle;
            this.presentationParameters.PresentationInterval = PresentInterval.Immediate;
            this.presentationParameters.IsFullScreen = false;

            this.GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, this.presentationParameters);
        }

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;

        public event EventHandler<EventArgs> DeviceReset;

        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public void Resize(int width, int height)
        {
            if (this.DeviceResetting != null)
            {
                this.DeviceResetting(this, EventArgs.Empty);
            }

            this.presentationParameters.BackBufferWidth = Math.Max(this.presentationParameters.BackBufferWidth, width);
            this.presentationParameters.BackBufferHeight = Math.Max(this.presentationParameters.BackBufferHeight, height);
            this.GraphicsDevice.Reset(this.presentationParameters);

            if (this.DeviceReset != null)
            {
                this.DeviceReset(this, EventArgs.Empty);
            }
        }

        // TODO this right
        public void Dispose()
        {
            if (this.DeviceDisposing != null)
            {
                this.DeviceDisposing(this, EventArgs.Empty);
            }

            this.GraphicsDevice.Dispose();
            this.GraphicsDevice = null;
        }
    }
}
