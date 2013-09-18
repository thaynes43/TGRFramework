// -----------------------------------------------------------------------
// <copyright file="Line.cs" company="">
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
    public struct Line
    {
        float? Slope;
        float Intercept;
        float HeroY;
        float HeroX;

        /// <summary>
        /// y = mx + b
        /// </summary>
        public Line(float heroY, float homerY, float heroX, float homerX)
        {
            if (heroX == homerX)
                Slope = null; // undefined slope
            else if (heroY == homerY)
                Slope = 0;
            else
                Slope = (homerY - heroY) / (homerX - heroX);

            if (Slope != null)
            {
                Intercept = heroY - (Slope.Value * heroX);
            }
            else
            {
                Intercept = 0;
            }

            HeroY = heroY;
            HeroX = heroX;
        }

        public void GetPointOnLineToHero(float distance, ref float x, ref float y)
        {
            if (HeroY == y)
            {
                // move vertically
                if (HeroY > y)
                    y = y + distance;
                else
                    y = y - distance;
            }
            else if (HeroX == x)
            {
                // move horizontally
                if (HeroX > x)
                    x = x + distance;
                else
                    x = x - distance;
            }
            else
            {
                // move diagonally
                float x1 = 0f;
                float x2 = 0f;
                float y1 = 0f;
                float y2 = 0f;

                // point moving in y direction
                if (HeroY > y)
                    y1 = y + distance;
                else
                    y1 = y - distance;

                x1 = (y1 - Intercept) / Slope.Value;

                // point moving in x direction
                if (HeroX > x)
                    x2 = x + distance;
                else
                    x2 = x - distance;

                y2 = (Slope.Value * x2) + Intercept;

                // Use nearest point
                double dist1 = (Math.Pow(x1 - x, 2) + Math.Pow(y1 - y, 2));
                double dist2 = (Math.Pow(x2 - x, 2) + Math.Pow(y2 - y, 2));

                if (dist1 < dist2)
                {
                    x = x1;
                    y = y1;
                }
                else
                {
                    x = x2;
                    y = y2;
                }
            }
        }
    }
}
