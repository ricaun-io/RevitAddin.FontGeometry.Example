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
        public static List<Curve> ToCurves(this IEnumerable<XYZ> _points, bool closed = true)
        {
            var points = _points.ToList();

            //if (points.Count < 2)
            //{
            //    return new List<Curve>();
            //}

            if (closed)
            {
                points.Add(points.First());
            }

            var curves = new List<Curve>();
            XYZ lastPoint = null;
            for (int i = 0; i < points.Count() - 1; i++)
            {
                try
                {
                    var point1 = lastPoint ?? points.ElementAt(i);
                    var point2 = points.ElementAt(i + 1);

                    curves.Add(Line.CreateBound(point1, point2));
                    lastPoint = null;
                }
                catch 
                {
                    lastPoint ??= points.ElementAt(i);
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
