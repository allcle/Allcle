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
namespace test
{
    /// <summary>
    /// MainScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainScreen : Window
    {
        public MainScreen()
        {
            InitializeComponent();
            binddatagrid();
        }
        private void binddatagrid()
        {

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            con.Open();
            SqlCommand cmd = new SqlCommand();
            MainWindow mw = new MainWindow();

            cmd.CommandText = "select * from [HongikTable$] where 과목명=N'" + Search_Box.Text + "'";
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("HongikTalbe$");
            da.Fill(dt);
            g1.ItemsSource = dt.DefaultView;
        }

        private void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            binddatagrid();
        }

    }
}
