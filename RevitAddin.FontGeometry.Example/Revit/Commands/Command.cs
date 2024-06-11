using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAddin.FontGeometry.Example.Services;
using ricaun.Revit.DB.Shape;
using System.Collections.Generic;
using System.Linq;

namespace RevitAddin.FontGeometry.Example.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            Document document = uiapp.ActiveUIDocument.Document;

            var fontPathGeometryService = new FontPathGeometryService();
            fontPathGeometryService.FontWeight = System.Windows.FontWeights.Bold;

            fontPathGeometryService.Tolerance = 2;

            var text = "Revit";
            var pathGeometry = fontPathGeometryService.CreateText(text, 100);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("CreateShape");

                document.DeleteDirectShape();

                var curvesText = new List<Curve>();

                foreach (var points in pathGeometry.GetPoints())
                {
                    var revitPoints = points.Select(e => e.ToRevitXYZ()).ToList();
                    foreach (var point in revitPoints)
                    {
                        document.CreateDirectShape(Point.Create(point));
                    }
                    var curves = revitPoints.ToCurves();
                    document.CreateDirectShape(curves);
                    curvesText.AddRange(curves);
                    //try
                    //{
                    //    document.CreateDirectShape(curves.ToSolidExtrusionGeometry());
                    //}
                    //catch (Exception)
                    //{

                    //}
                }

                //document.CreateDirectShape(curvesText.ToSolidExtrusionGeometry());

                var textFace = curvesText.ToSolidExtrusionGeometry().Faces.OfType<Face>().Skip(1).First();
                var textFaceModel = document.CreateDirectShape(textFace.Triangulate());
                textFaceModel.Location.Move(XYZ.BasisZ);

                transaction.Commit();
            }

            return Result.Succeeded;
        }


    }

}
