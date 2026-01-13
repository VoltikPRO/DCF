using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DCF.UI;
using DCF.Helpers;

namespace DCF.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class AboutCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                DebugHelper.Log("▶ DCF AboutCommand executed: відкриття AboutWindow");

                var window = new AboutWindow();
                window.ShowDialog();

                DebugHelper.Log("✅ DCF AboutWindow закрито користувачем");

                return Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                DebugHelper.Log("❌ DCF AboutCommand Exception: " + ex.Message);
                UserNotifier.ShowError(
                    "Failed to display Direct Connection Finder About window.\nSee Debug log for details.");

                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
