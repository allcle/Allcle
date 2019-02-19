using System;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// ModifyInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifyInfo : Window
    {
        public ModifyInfo()
        {
            InitializeComponent();
            wrongConPW_tbx.Visibility = Visibility.Collapsed;
            wrongCurPW_tbx.Visibility = Visibility.Collapsed;
            wrongNewPW_tbx.Visibility = Visibility.Collapsed;
        }

        private void curPW_tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            curPW_tbx.Visibility = Visibility.Collapsed;
            curPW_pwx.Focus();
        }


        private void newPW_tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            newPW_tbx.Visibility = Visibility.Collapsed;
            newPW_pwx.Focus();
        }

        private void curPW_pwx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (curPW_pwx.Password.Length == 0)
            {
                curPW_tbx.Visibility = Visibility.Visible;
            }
            else
            {

            }
        }

        private void newPW_pwx_LostFocus(object sender, RoutedEventArgs e)
        {
            if(newPW_pwx.Password.Length == 0)
            {
                newPW_tbx.Visibility = Visibility.Visible;
            }
            else
            {

            }
        }

        private void conPW_tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            conPW_tbx.Visibility = Visibility.Collapsed;
            conPW_pwx.Focus();
        }

        private void conPW_pwx_LostFocus(object sender, RoutedEventArgs e)
        {
            if(conPW_pwx.Password.Length == 0)
            {
                conPW_tbx.Visibility = Visibility.Visible;
            }
            else
            {

            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            //활성화 됐을 때 기본 적인 정보 다 initializing
        }

        private void College_cbx_DropDownClosed(object sender, EventArgs e)
        {
            Major_normal.Visibility = Visibility.Collapsed;
            Major_engineer.Visibility = Visibility.Collapsed;
            Major_architecture.Visibility = Visibility.Collapsed;
            if (College_cbx.Text == "일반대학")
                Major_normal.Visibility = Visibility.Visible;
            else if (College_cbx.Text == "공과대학")
                Major_engineer.Visibility = Visibility.Visible;
            else if (College_cbx.Text == "건축대학")
                Major_architecture.Visibility = Visibility.Visible;
            else
            {
                System.Windows.MessageBox.Show("error");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)     //수정하기 눌렀을 때
        {

        }
    }
}
