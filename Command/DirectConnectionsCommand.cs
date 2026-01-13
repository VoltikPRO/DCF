using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DCF.Core;

namespace DCF.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class DirectConnectionsCommand : LicensedCommand
    {
        protected override Result ExecuteProtected(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            return Core.DirectConnectionsService.Run(commandData, ref message);
        }
    }
}
