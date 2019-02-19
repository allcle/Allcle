using System;
using System.Collections.Generic;
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

namespace Client
{
    /// <summary>
    /// DefineGuestMajor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DefineGuestMajor : Window
    {
        public DefineGuestMajor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.guest_major = major_cbox_text.Text;
            App.guest_hakbun = hakbun_cbox_text.Text;
            App.MS.Show();                                              //메인 화면 띄우기
            this.Hide();                                                //로그인창 hide
        }
        private void hakbun_cbx_DropDownClosed(object sender, EventArgs e)
        {
            hakbun_cbox_text.Text = hakbun_cbox.Text;
        }
        private void major_cbx_DropDownClosed(object sender, EventArgs e)
        {
            major_cbox_text.Text = major_cbox.Text;
        }
    }
}
