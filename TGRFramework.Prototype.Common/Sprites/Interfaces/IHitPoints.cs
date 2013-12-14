// -----------------------------------------------------------------------
// <copyright file="IHitPoints.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public delegate int AdjustHitPoints(int newHp);

    public interface IHitPoints : ISprite
    {
        void UpdateHitPoints(int newHp);

    }
}
