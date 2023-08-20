using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Geometry
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Translate(double Tx, double Ty)
        {
            X += Tx;
            Y += Ty;
        }

        public void Scale(double Sx, double Sy)
        {
            X *= Sx;
            Y *= Sy;
        }

        public void Rotate(double angle)
        {
            X = X * Math.Cos(angle) - Y * Math.Sin(angle);
            Y = X * Math.Sin(angle) + Y * Math.Cos(angle);
        }
    }
}
