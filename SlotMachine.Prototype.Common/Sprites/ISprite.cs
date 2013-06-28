// -----------------------------------------------------------------------
// <copyright file="ISprite.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SlotMachine.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISprite
    {
        void LoadContent(ContentManager content);

        void Update(ContentManager content, GameTime gameTime);

        void Draw(SpriteBatch theSpriteBatch);
    }
}
