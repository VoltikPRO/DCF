using System;
using System.Windows.Media;

namespace DCF.Licensing
{
    public static class LicenseManager
    {
        private static readonly DateTime ExpirationDate = new DateTime(2026, 06, 01);

        public static bool IsValid => DateTime.Now <= ExpirationDate;

        public static string Info =>
            IsValid ? $"License valid until {ExpirationDate:dd.MM.yyyy}"
                    : $"Expired on {ExpirationDate:dd.MM.yyyy}";

        public static SolidColorBrush InfoColor =>
            IsValid ? Brushes.Black : Brushes.Red;
    }
}
