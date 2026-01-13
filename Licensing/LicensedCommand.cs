using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DCF.Commands
{
    public abstract class LicensedCommand : IExternalCommand
    {
        // Реалізується конкретною командою
        protected abstract Result ExecuteProtected(ExternalCommandData commandData, ref string message, ElementSet elements);

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!DCF.Licensing.LicenseManager.IsValid)
            {
                DCF.Helpers.DebugHelper.Log("❌ License expired - command not executed");
                DCF.Helpers.UserNotifier.Show("The license has expired. Please contact support.");
                return Result.Cancelled;
            }

            DCF.Helpers.DebugHelper.Log("▶ License valid - executing command");
            return ExecuteProtected(commandData, ref message, elements);
        }
    }
}
