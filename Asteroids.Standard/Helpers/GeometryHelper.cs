using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Asteroids.Standard.Helpers
{
    public static class GeometryHelper
    {
        #region Distance Between Two Points

        /// <summary>
        /// Calculate the distance between two points.
        /// </summary>
        public static double DistanceTo(this Point point1, Point point2)
        {
            return point1.DistanceTo(point2.X, point2.Y);
        }

        /// <summary>
        /// Calculate the distance between two points.
        /// </summary>
        public static double DistanceTo(this Point point1, int X2, int Y2)
        {
            return Math.Sqrt(Math.Pow(point1.X - X2, 2) + Math.Pow(point1.Y - Y2, 2));
        }

        #endregion

        #region IsInsidePolygon

        /// <summary>
        /// Determines is a <see cref="Point"/> is inside a polygon.
        /// </summary>
        /// <remarks>
        /// http://csharphelper.com/blog/2014/07/determine-whether-a-point-is-inside-a-polygon-in-c/
        /// </remarks>
        public static bool IsInsidePolygon(this Point point, IList<Point> polygonPoints)
        {
            // Get the angle between the point and the first and last vertices.
            var firstPoint = polygonPoints.First();
            var lastPoint = polygonPoints.Last();

            var total_angle = GetAngle(
                lastPoint.X, lastPoint.Y,
                point.X, point.Y,
                firstPoint.X, firstPoint.Y
            );

            // Add the angles from the point  to each other pair of vertices.
            for (var i = 0; i < polygonPoints.Count - 1; i++)
            {
                total_angle += GetAngle(
                    polygonPoints[i].X, polygonPoints[i].Y,
                    point.X, point.Y,
                    polygonPoints[i + 1].X, polygonPoints[i + 1].Y
                );
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return Math.Abs(total_angle) > 0.000001;
        }

        /// <summary>
        /// Determines if any point in a collection is contained in a polygon.
        /// </summary>
        /// <param name="ptsPolygon">Collection of points that make up the polygon.</param>
        /// <param name="ptsCheck">Collection of points to check if any are contained.</param>
        /// <returns>Indication if ANY point is contained in the polygon.</returns>
        public static bool ContainsAnyPoint(this IList<Point> ptsPolygon, IList<Point> ptsCheck)
        {
            var inside = false;
            foreach (var pt in ptsCheck)
            {
                if (!pt.IsInsidePolygon(ptsPolygon))
                    continue;

                inside = true;
                break;
            }

            return inside;
        }

        #endregion

        #region GetAngle

        /// <summary>
        /// Return the angle ABC.
        /// </summary>
        /// <returns>Angle in radians.</returns>
        /// <remarks>
        /// Return a value between PI and -PI. Note that the value is the opposite of what you 
        /// might expect because Y coordinates increase downward.
        /// </remarks>
        public static double GetAngle(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            var dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            var cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }
        
        /// <summary>
        /// Get the angle of line AB from angle 0 assuming a right-angle triangle.
        /// </summary>
        /// <returns>Angle in radians.</returns>
        /// <remarks>
        /// Return a value between PI and -PI. Note that the value is the opposite of what you 
        /// might expect because Y coordinates increase downward.
        /// </remarks>
        public static double GetAngle(Point A, Point B)
        {
            //dot product, cross product from the 0 angle
            var cross_product = A.X - B.X;
            var dot_product = B.Y - A.Y;

            return Math.Atan2(cross_product, dot_product);
        }

        #endregion

        #region Cross and Dot products

        /// <summary>
        /// Return the cross product AB x BC.
        /// </summary>
        /// <remarks>
        /// The cross product is a vector perpendicular to AB and BC having length |AB| * |BC| * Sin(theta) and
        /// with direction given by the right-hand rule. For two vectors in the X-Y plane, the result is a
        /// vector with X and Y components 0 so the Z component gives the vector's length and direction.
        /// </remarks>
        public static double CrossProductLength(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            var BAx = Ax - Bx;
            var BAy = Ay - By;
            var BCx = Cx - Bx;
            var BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        /// <summary>
        /// Return the dot product AB · BC.
        /// </summary>
        /// <remarks>
        /// Note that AB · BC = |AB| * |BC| * Cos(theta).
        /// </remarks>
        private static double DotProduct(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            var BAx = Ax - Bx;
            var BAy = Ay - By;
            var BCx = Cx - Bx;
            var BCy = Cy - By;

            // Calculate the dot product.
            return BAx * BCx + BAy * BCy;
        }

        #endregion
    }
}
