using System;
using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Input;

namespace TGRFramework.Prototype.Common
{
    public class ButtonSpriteSDL : Sprite
    {
        public ButtonSpriteSDL(Surface initialSurface, Surface keyDownSurface, Point coordinates)
            : base(initialSurface, coordinates)
        {
            this.KeyUpSurface = initialSurface;
            this.KeyDownSurface = keyDownSurface;
        }

        public event Action ButtonClicked;

        public Surface KeyUpSurface { get; set; }

        public Surface KeyDownSurface { get; set; }

        public override void Update(MouseButtonEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (args.ButtonPressed && this.IntersectsWith(new Point(args.X, args.Y)))
            {
                if (args.Button == MouseButton.PrimaryButton)
                {
                    this.Surface = this.KeyDownSurface;
                    this.RaiseButtonClicked();
                }
            }
            else if (this.Surface == this.KeyDownSurface)
            {
                this.Surface = this.KeyUpSurface;
            }
        }

        private void RaiseButtonClicked()
        {
            if (this.ButtonClicked != null)
            {
                this.ButtonClicked();
            }
        }

        #region IDisposable
        private bool disposed;

        /// <summary>
        /// Destroys the surface object and frees its memory
        /// </summary>
        /// <param name="disposing">If ture, dispose unmanaged resources</param>

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                    }
                    this.disposed = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion IDisposable
    }
}
