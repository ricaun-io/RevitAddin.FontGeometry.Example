using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin.FontGeometry.Example.Services
{
    public static class CurveExtension
    {
        const double ShortCurveTolerance = 0.0025602645572916664;

        public static List<XYZ> RemoveShortCurve(this IEnumerable<XYZ> _points, bool closed = true)
        {
            var points = _points.ToList();

            for (int i = 0; i < points.Count - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                if (p1.DistanceTo(p2) < ShortCurveTolerance)
                {
                    points.RemoveAt(i + 1);
                    i--;
                }
            }

            if (closed)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    if (points.First().DistanceTo(points.Last()) < ShortCurveTolerance)
                    {
                        points.RemoveAt(points.Count - 1);
                    }
                }
            }

            return points;
        }

        public static List<Curve> ToCurves(this IEnumerable<XYZ> _points, bool closed = true)
        {
            var points = _points.RemoveShortCurve(closed);

            if (points.Count <= 2)
            {
                return new List<Curve>();
            }

            if (closed)
            {
                points.Add(points.First());
            }

            var curves = new List<Curve>();
            for (int i = 0; i < points.Count() - 1; i++)
            {
                try
                {
                    var point1 = points.ElementAt(i);
                    var point2 = points.ElementAt(i + 1);
                    curves.Add(Line.CreateBound(point1, point2));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return curves;

        }

        public static List<CurveLoop> ToCurveLoop(this IEnumerable<Curve> curves)
        {
            var curveLoops = new List<CurveLoop>();
            var curveLoop = new CurveLoop();
            foreach (var curve in curves)
            {
                curveLoop.Append(curve);
                if (!curveLoop.IsOpen())
                {
                    curveLoops.Add(curveLoop);
                    curveLoop = new CurveLoop();
                }
            }
            return curveLoops;
        }

        public static Solid ToSolidExtrusionGeometry(this IEnumerable<Curve> curves)
        {
            var curveLoops = curves.ToCurveLoop();

            if (curveLoops.Count == 0)
                return null;

            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoops, XYZ.BasisZ, Application.MinimumThickness);
            return solid;
        }
    }
}
