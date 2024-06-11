using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAddin.FontGeometry.Example.Services;
using ricaun.Revit.DB.Shape;
using System;
using System.Linq;

namespace RevitAddin.FontGeometry.Example.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CommandSample : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            Document document = uiapp.ActiveUIDocument.Document;

            var text = "0123456789 abcdefghijklmnopqrstuvwxyz";
            var fontNames = new[] { "Arial", "Calibri", "Comic Sans MS", "Times New Roman" };
            var y = 0.0;
            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Create");
                document.DeleteDirectShape();
                foreach (var fontName in fontNames)
                {
                    var fontPathGeometryService = new FontPathGeometryService(fontName);

                    fontPathGeometryService.Tolerance = 2;
                    {
                        var pathGeometry = fontPathGeometryService.CreateText(fontName, 100);

                        var curves = pathGeometry.GetPoints().SelectMany(e => e.Select(e => e.ToRevitXYZ()).ToCurves()).ToList();
                        var textFace = curves.ToSolidExtrusionGeometry().Faces.OfType<Face>().Skip(1).First();

                        var textFaceModel = document.CreateDirectShape(textFace.Triangulate());
                        textFaceModel.Location.Move(y * XYZ.BasisY);
                        y -= 0.5;
                    }
                    {
                        var pathGeometry = fontPathGeometryService.CreateText(text, 100);

                        var curves = pathGeometry.GetPoints().SelectMany(e => e.Select(e => e.ToRevitXYZ()).ToCurves()).ToList();
                        var textFace = curves.ToSolidExtrusionGeometry().Faces.OfType<Face>().Skip(1).First();

                        var textMesh = textFace.Triangulate();
                        Console.WriteLine($"FontName:{fontName} NumTriangles:{textMesh.NumTriangles}");
                        var textFaceModel = document.CreateDirectShape(textMesh);
                        textFaceModel.Location.Move(y * XYZ.BasisY);
                        y -= 0.5;
                    }

                }

                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }

}
