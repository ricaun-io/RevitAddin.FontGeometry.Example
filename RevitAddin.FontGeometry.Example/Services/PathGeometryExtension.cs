using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace RevitAddin.FontGeometry.Example.Services
{
    public static class PathGeometryExtension
    {
        public static List<Point[]> GetPoints(this PathGeometry pathGeometry)
        {
            return pathGeometry.Figures.Select(e => e.GetPoints()).ToList();
        }

        public static Point[] GetPoints(this PathFigure figure)
        {
            var points = new List<Point>();
            foreach (var segment in figure.Segments)
            {
                if (segment is System.Windows.Media.PolyLineSegment lineSegment)
                {
                    points.AddRange(lineSegment.Points);
                }
            }
            return points.ToArray();
        }

        public static Autodesk.Revit.DB.XYZ ToRevitXYZ(this Point point)
        {
            var fator = 2.75 / 600;
            return new Autodesk.Revit.DB.XYZ(point.X, -point.Y, 0) * fator;
        }
    }
}
