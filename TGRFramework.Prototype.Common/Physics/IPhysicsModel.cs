// -----------------------------------------------------------------------
// <copyright file="IFallModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TGRFramework.Prototype.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IPhysicsModel
    {
        event Action UpdateSpriteComplete;

        void UpdateSpriteLocation();
    }
}
