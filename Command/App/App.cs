using Autodesk.Revit.UI;
using DCF.Licensing;
using DCF.Helpers;
using System;

namespace DCF.App
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                DebugHelper.Log("▶ DCF OnStartup: створення вкладки та панелі Ribbon");

                // 1. Create Ribbon Tab
                string tabName = "DCF";
                try
                {
                    application.CreateRibbonTab(tabName);
                    DebugHelper.Log($"Ribbon tab '{tabName}' створено");
                }
                catch
                {
                    DebugHelper.Log($"Ribbon tab '{tabName}' вже існує");
                }

                // 2. Create Ribbon Panel
                string panelName = "Direct Connection Finder";
                RibbonPanel panel = application.CreateRibbonPanel(tabName, panelName);
                DebugHelper.Log($"Ribbon panel '{panelName}' створено");

                // 3. Assembly path
                string assemblyPath = typeof(App).Assembly.Location;
                DebugHelper.Log($"Assembly path: {assemblyPath}");

                // 4. Buttons
                var directConnections = new PushButtonData(
                    "DirectConnections",
                    "Direct \nConnections",
                    assemblyPath,
                    "DCF.Commands.DirectConnectionsCommand")
                {
                    ToolTip = "Find and list all directly connected electrical elements."
                };

                var aboutCommand = new PushButtonData(
                    "About",
                    "About",
                    assemblyPath,
                    "DCF.Commands.AboutCommand")
                {
                    ToolTip = "About Direct Connection Finder plugin"
                };

                // 5. Add buttons (license-aware)
                panel.AddItem(directConnections).Enabled = LicenseManager.IsValid;
                panel.AddItem(aboutCommand).Enabled = true;

                DebugHelper.Log(
                    "DCF Ribbon initialized. License status: " +
                    (LicenseManager.IsValid ? "VALID" : "EXPIRED"));

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DebugHelper.Log("❌ DCF OnStartup Exception: " + ex.Message);
                UserNotifier.ShowError(
                    "Failed to initialize Direct Connection Finder.\nSee Debug log for details.");

                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            DebugHelper.Log("▶ DCF OnShutdown: завершення роботи додатку");
            UserNotifier.Show("Direct Connection Finder shutdown completed.");
            return Result.Succeeded;
        }
    }
}
