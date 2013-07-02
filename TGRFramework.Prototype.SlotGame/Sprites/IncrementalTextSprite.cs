// -----------------------------------------------------------------------
// <copyright file="IncrementalTextSprite.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.SlotGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TGRFramework.Prototype.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// Update summary.
    /// </summary>
    public class IncrementalTextSprite : TextSprite
    {
        private float timeCount = 0f;
        private int maxValue = 0;

        private float intervalScale;

        private bool beginUpdate = false;

        public IncrementalTextSprite(float interval, string output, string content, Vector2 fontPosition, float fontRotation = 0f)
            : base(output, content, fontPosition, fontRotation)
        {
            this.intervalScale = interval;
            this.Interval = interval;
            this.IncrementalValue = int.Parse(output);
        }

        public float Interval { get; set; }

        public SoundEffect IncrementSound { get; set; }

        public override string OutputText
        {
            get
            {
                return this.IncrementalValue.ToString();
            }
            set
            {
                // TODO what to do here?
                if (!this.beginUpdate)
                {
                    this.maxValue = int.Parse(value);
                    if (this.maxValue <= this.IncrementalValue)
                    {
                        // this.Interval = this.intervalScale + (this.IncrementalValue - this.maxValue) * (this.IncrementalValue - this.maxValue); TODO adjust this in max value getter
                        this.IncrementalValue = this.maxValue;
                    }
                    else
                    {
                        beginUpdate = true;
                    } 
                }
            }
        }

        private int IncrementalValue { get; set; }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.LoadContent(content);

            this.IncrementSound = content.Load<SoundEffect>("Coin1");
        }

        public override void Update(Microsoft.Xna.Framework.Content.ContentManager content, GameTime gameTime)
        {
            if (this.IncrementalValue == this.maxValue)
            {
                this.beginUpdate = false;     
            }
            else if (this.beginUpdate)
            {
                timeCount += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timeCount > this.Interval)
                {
                    this.IncrementSound.Play();
                    this.IncrementalValue++;
                    this.timeCount = 0f;
                }
            }
        }
    }
}
