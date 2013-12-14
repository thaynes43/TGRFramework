// -----------------------------------------------------------------------
// <copyright file="SymbolSpriteManager.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using TGRFramework.Prototype.Common;
    using TGRFramework.Prototype.Tools;

    /// <summary>
    /// Update summary.
    /// </summary>
    public class Reel : ISprite
    {
        private object symbolReelManagerLock = new object();
        const float SPEED = 15f;
        private bool InSpin = false;

        public Reel(ILogTool log, Vector2 reelJust)
        {
            this.Visible = true;
            this.Log = log;
            this.ReelJustification = reelJust;
            this.SymbolSprites = new List<SymbolSprite>();
            this.SpinStopwatch = new Stopwatch();
            this.SpinDurationMS = 0;
        }

        public event Action SpinComplete;

        public bool Visible { get; set; }

        // Bottom symbol location
        private Vector2 ReelJustification { get; set; }

        // Middle symbol location
        private Vector2 WinJustification { get; set; }

        // private Screen ParentScreen { get; set; }

        private List<SymbolSprite> SymbolSprites { get; set; }

        private SheetSprite SpinSprite { get; set; }

        private ILogTool Log { get; set; }

        // TODO use game time
        public Stopwatch SpinStopwatch { get; set; }

        public int SpinDurationMS { get; set; }

        public Symbol SpunSymbol { get; set; }

        public void Initialize()
        {
            lock (this.symbolReelManagerLock)
            {
                float xMin = 0f;
                float xMax = 0f;

                SymbolSprite symbol1 = new SymbolSprite(this.Log, "Symbol1", xMin, xMax);
                SymbolSprite symbol2 = new SymbolSprite(this.Log, "Symbol2", xMin, xMax);
                SymbolSprite symbol3 = new SymbolSprite(this.Log, "Symbol3", xMin, xMax);
                SymbolSprite symbol4 = new SymbolSprite(this.Log, "Symbol4", xMin, xMax);

                this.SymbolSprites.Add(symbol1);
                this.SymbolSprites.Add(symbol2);
                this.SymbolSprites.Add(symbol3);
                this.SymbolSprites.Add(symbol4);
            }
        }

        public void LoadContent(ContentManager content)
        {
            lock (this.symbolReelManagerLock)
            {
                int width = 0;
                int height = 0;

                foreach (SymbolSprite sprite in this.SymbolSprites)
                {
                    sprite.LoadContent(content);

                    if (height == 0 && width == 0)
                    {
                        width = sprite.SymbolTexture.Width;
                        height = sprite.SymbolTexture.Height * 3;                        
                    }
                    
                }
                this.PositionSymbols();

                this.SpinSprite = new SheetSprite("SpinSpriteSheet", this.ReelJustification, width, height, SPEED, 12, 1, 0, 0);
                this.SpinSprite.LoadContent(content);
            }
        }

        public void PositionSymbols()
        {
            // Must wait until sprite content is loaded to perform these tasks
            float heightCount = 0f;
            for (int i = 0; i < this.SymbolSprites.Count; i++)
            {
                if (i == 0)
                {
                    this.SymbolSprites[i].SymbolPosition = this.ReelJustification;
                }
                else
                {
                    heightCount += this.SymbolSprites[i].SymbolTexture.Height;
                    Vector2 nextPos = new Vector2(this.ReelJustification.X, this.ReelJustification.Y + heightCount);
                    this.SymbolSprites[i].SymbolPosition = nextPos;

                    if (i == 1)
                    {
                        this.WinJustification = nextPos;
                    }
                }
            }

            foreach (SymbolSprite symbol in this.SymbolSprites)
            {
                symbol.MinY = this.ReelJustification.Y - (heightCount / this.SymbolSprites.Count);
                symbol.MaxY = this.ReelJustification.Y + heightCount;
            }
        }

        public void Update(ContentManager content, GameTime gameTime)
        {           
            lock (this.symbolReelManagerLock)
            {
                if (this.InSpin)
                {
                    if (this.SpinStopwatch.ElapsedMilliseconds >= this.SpinDurationMS)
                    {
                        // TODO call this only in one place
                        this.ShowSpinWinner();
                        this.RaiseSpinComplete();
                    }
                    else
                    {
                        this.SpinSprite.Update(content, gameTime);
                    }
                }
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            lock (this.symbolReelManagerLock)
            {
                if (!this.InSpin)
                {
                    foreach (SymbolSprite sprite in this.SymbolSprites)
                    {
                        sprite.Draw(theSpriteBatch);
                    }
                }
                else
                {
                    this.SpinSprite.Draw(theSpriteBatch);
                }
            }
        }

        public void Spin(int spinDuration, Symbol endOnSymbol)
        {
            lock (this.symbolReelManagerLock)
            {
                this.InSpin = true;
                this.SpinDurationMS = spinDuration;
                this.SpunSymbol = endOnSymbol;

                this.SpinStopwatch.Start();
            }
        }

        public void ForceStopSpin()
        {
            this.InSpin = false;
            this.ShowSpinWinner();
            this.SpinStopwatch.Reset();
            this.RaiseSpinComplete();
        }

        public void Reset()
        {
            this.InSpin = false;
            this.PositionSymbols();
        }

        private void ShowSpinWinner()
        {
            int winIndex = -1;
            for (int j = 0; j < this.SymbolSprites.Count; j++)
            {
                if (this.SymbolSprites[j].SymbolContent == this.SpunSymbol.Content)
                {
                    winIndex = j;
                    break;
                }
            }

            float heightCount = 0f;
            for (int i = 0; i < this.SymbolSprites.Count; i++)
            {
                Vector2 nextPos = new Vector2(this.ReelJustification.X, this.ReelJustification.Y + heightCount);
                heightCount += this.SymbolSprites[i].SymbolTexture.Height;
                this.SymbolSprites[i].SymbolPosition = nextPos;
            }

            if (this.SymbolSprites[winIndex].SymbolPosition != this.WinJustification)
            {
                foreach (SymbolSprite symbol in this.SymbolSprites)
                {
                    if (symbol.SymbolPosition == this.WinJustification)
                    {
                        Vector2 temp = this.SymbolSprites[winIndex].SymbolPosition;
                        this.SymbolSprites[winIndex].SymbolPosition = this.WinJustification;
                        symbol.SymbolPosition = temp;
                        break;
                    }
                }
            }

            this.SpinStopwatch.Reset();
            this.InSpin = false;
        }

        private void RaiseSpinComplete()
        {
            if (this.SpinComplete != null)
            {
                this.SpinComplete();
            }
        }
    }
}
