// -----------------------------------------------------------------------
// <copyright file="CharacterUtil.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CharacterUtil
    {
        public static bool PlayerToTheRight(Rectangle heroBounds, Rectangle enemyBounds)
        {
            int enemyCenterX = enemyBounds.X + (enemyBounds.Width / 2);
            int heroCenterX = heroBounds.X + (heroBounds.Width / 2);

            return enemyCenterX < heroCenterX &&
                enemyBounds.X < PlatformerLevel.LevelWidth - enemyBounds.Width;
        }

        public static bool PlayerToTheLeft(Rectangle heroBounds, Rectangle enemyBounds)
        {
            int enemyCenterX = enemyBounds.X + (enemyBounds.Width / 2);
            int heroCenterX = heroBounds.X + (heroBounds.Width / 2);

            return enemyCenterX >= heroCenterX &&
                enemyBounds.X > 0;
        }
    }
}
