using System.Windows;

namespace Client
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MW = new MainWindow();
        public static MainScreen MS = new MainScreen();

        public static string ID;
        public static string guest_major;
        public static string guest_hakbun;
        public static bool first;
        public static bool guest;
        public static bool deleteSubject;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            first = true;
            guest = true;
            deleteSubject = false;
            MW.Show();     
        }
    }
}
