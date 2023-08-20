using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Geometry
{
    static public class Functions
    {
        static public double[] Line(Point A, Point B)
        {
            double k = (B.Y - A.Y) / (B.X - A.X);
            double b = A.Y - (A.X * (B.Y - A.Y)) / (B.X - A.X);
            double[] arr = new double[2];
            arr[0] = k;
            arr[1] = b;
            return arr;
        }

        static bool OneSide(Point p1, Point p2, Point A, Point B)
        {
            if (B.X != A.X)
            {
                double[] arr = Line(A, B);
                double k = arr[0];
                double b = arr[1];
                return (p1.Y < k * p1.X + b && p2.Y > k * p2.X + b) || (p1.Y > k * p1.X + b && p2.Y < k * p2.X + b)
                    || (p1.Y != k * p1.X + b && p2.Y == k * p2.X + b) || (p1.Y == k * p1.X + b && p2.Y != k * p2.X + b)
                    || (p1.Y == p2.Y && p2.Y == A.Y && A.Y == B.Y && ((p1.X >= Math.Min(A.X, B.X) && p1.X <= Math.Max(A.X, B.X)) || ((p2.X >= Math.Min(A.X, B.X) && p2.X <= Math.Max(A.X, B.X)))));
            }
            return (p1.X < A.X && p2.X > A.X) || (p1.X > A.X && p2.X < A.X) || (p1.X != A.X && p2.X == A.X) || (p1.X == A.X && p2.X != A.X)
                || (p1.X == p2.X && p2.X == A.X && A.X == B.X && ((p1.Y >= Math.Min(A.Y, B.Y) && p1.Y <= Math.Max(A.Y, B.Y)) || ((p2.Y >= Math.Min(A.Y, B.Y) && p2.Y <= Math.Max(A.Y, B.Y)))));
        }

        static public bool Intersect(Point A1, Point A2, Point B1, Point B2)
        {
            bool fl1 = (OneSide(A1, A2, B1, B2));
            bool fl2 = (OneSide(B1, B2, A1, A2));
            return fl1 && fl2;
        }
    }
}
