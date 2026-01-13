using Autodesk.Revit.UI;

namespace DCF.Helpers
{
    public static class UserNotifier
    {
        private const string DefaultTitle = "Lightning Protection";

        /// <summary>
        /// Показує інформаційне повідомлення користувачу.
        /// </summary>
        public static void Show(string message)
        {
            TaskDialog.Show(DefaultTitle, message);
        }

        /// <summary>
        /// Показує повідомлення про помилку користувачу.
        /// </summary>
        public static void ShowError(string message)
        {
            TaskDialog.Show(DefaultTitle + " - Error", message);
        }
    }
}
