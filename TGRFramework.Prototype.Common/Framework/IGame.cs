// -----------------------------------------------------------------------
// <copyright file="IGame.cs" company="">
// Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// API to integrate framework with XNA
    /// </summary>
    public interface IGame
    {
        void Initialize();

        void Stop();

        void LoadContent(ContentManager content);

        void UnloadContent(ContentManager content);

        void Update(ContentManager content, GameTime gameTime);

        void Draw(SpriteBatch theSpriteBatch);
    }
}
