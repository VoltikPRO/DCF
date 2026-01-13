using DCF.Licensing;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace DCF.UI
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            // Підставляємо версію з Assembly
            VersionText.Text = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
            LicenseInfo.Text = LicenseManager.Info;
            LicenseInfo.Foreground = LicenseManager.InfoColor;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GitHub_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://voltikpro.github.io/VP-Hub/")
            {
                UseShellExecute = true
            });
        }
    }
}
