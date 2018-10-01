using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        public static FilterOption FO = new FilterOption();
        public static MyGroup MG = new MyGroup();
        public static string ID;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MW.Show();     
        }
    }
}
