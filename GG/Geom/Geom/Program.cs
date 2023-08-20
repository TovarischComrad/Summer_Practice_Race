using System;

namespace Geom
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
    public class Program
    {
        static double[] Line(Point A, Point B)
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

        static bool Intersect(Point A1, Point A2, Point B1, Point B2)
        {
            bool fl1 = (OneSide(A1, A2, B1, B2));
            bool fl2 = (OneSide(B1, B2, A1, A2));
            return fl1 || fl2;
        }

        static void Main(string[] args)
        {
            Point A1 = new Point(1, 3);
            Point A2 = new Point(3, 1);

            Point A3 = new Point(2, 0);
            Point A4 = new Point(2, 3);

            Console.WriteLine(Intersect(A1, A2, A3, A4));
        }
    }
}
