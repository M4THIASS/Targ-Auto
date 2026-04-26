using System.Windows;

namespace WpfTargAuto
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                var window = new MainWindow();
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message + "\n\n" + ex.StackTrace,
                    "Eroare la pornire");
            }
        }
    }
}