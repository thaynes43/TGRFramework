// -----------------------------------------------------------------------
// <copyright file="SymbolSprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.SlotSim
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using SlotMachine.Prototype.Common;
    using SlotMachine.Prototype.Tools;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// // TODO Make Sprite baseclass with 3 methods and 2 properties...
    public class SymbolSprite : ISprite
    {
        public SymbolSprite(ILogTool log, string content, float minY, float maxY)
        {
            this.Log = log;
            this.SymbolContent = content;

            this.MinY = minY;
            this.MaxY = maxY;
        }

        // TODO Symbol sprite should have ref to symbol
        public string SymbolContent { get; set; }

        public Texture2D SymbolTexture { get; set; }

        public Vector2 SymbolPosition { get; set; }

        public System.Threading.Timer SpinTimer { get; set; }

        public float MinY { get; set; }

        public float MaxY { get; set; }

        private ILogTool Log { get; set; }

        public void LoadContent(ContentManager content)
        {
            this.SymbolTexture = content.Load<Texture2D>(this.SymbolContent);
        }

        public void Update(ContentManager content, GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(this.SymbolTexture, this.SymbolPosition, Color.White);
        }
    }
}
