using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ricaun.Revit.UI;
using System;

namespace RevitAddin.FontGeometry.Example.Revit
{
    [AppLoader]
    public class App : IExternalApplication
    {
        private RibbonPanel ribbonPanel;
        public Result OnStartup(UIControlledApplication application)
        {
            ribbonPanel = application.CreatePanel("FontGeometry");
            ribbonPanel.CreatePushButton<Commands.Command>("Revit")
                .SetLargeImage("Resources/Revit.ico");
            ribbonPanel.CreatePushButton<Commands.CommandSample>("Sample")
                .SetLargeImage("Resources/Revit.ico");
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            ribbonPanel?.Remove();
            return Result.Succeeded;
        }
    }

}