using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_btn_Click(object sender, RoutedEventArgs e)      // login 버튼을 클릭하면, MainWindow.xaml을 close하고, MainScreen.xaml show하는 메소드
        {
            this.Hide();
            App.MS.Show();

        }
        
        private void Forget_btn_Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Geust_Login_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SingUP_btn_Click(object sender, RoutedEventArgs e)
        {
            App.SU.Show();
        }
    }
}
