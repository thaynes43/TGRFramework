// -----------------------------------------------------------------------
// <copyright file="XYCoordinate.cs" company="">
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
    public class XYCoordinate
    {
        public XYCoordinate(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float X { get; internal set; }

        public float Y { get; internal set; }

        // TODO Operator overloads?
    }
}
