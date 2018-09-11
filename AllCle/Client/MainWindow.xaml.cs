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
using System.Windows.Media.Animation;
using System.Windows.Forms.VisualStyles;
using System.Net;

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

        private void Next_btn_Click(object sender, RoutedEventArgs e)      // login 버튼을 클릭하면, MainWindow.xaml을 close하고, MainScreen.xaml show하는 메소드
        {
            string urlBase = @"https://allcleapp.azurewebsites.net/api/Users"; //기본 url
            string url = null;  //json으로 쓰일 url
            url = urlBase + "/" + ID_Box.Text;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            if (Unicode == "true")
            {
                ID.Visibility = Visibility.Collapsed;
                ID_Box.Visibility = Visibility.Collapsed;
                PW.Visibility = Visibility.Visible;
                Text.Text = "Password";
                PW_Box.Focus();
            }
            else if(ID_Box.Text == "")
                System.Windows.MessageBox.Show("아이디를 입력해주세요");
            else
                System.Windows.MessageBox.Show(ID_Box.Text + "는 존재하지 않는 아이디 입니다");
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

        private void ID_Box_GotFocus(object sender, RoutedEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            Back.BorderThickness = new Thickness(0, 0, 0, 5);
            Back.BorderBrush = Brushes.Blue;
            Back.BeginAnimation(OpacityProperty, doubleAnimation);
            ID_.Visibility = Visibility.Collapsed;
            Text.Foreground = Brushes.Blue;
            Text.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void ID_Box_LostFocus(object sender, RoutedEventArgs e)
        {           
            Back.BorderBrush = Brushes.Gray;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 1;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.01);
            if (ID_Box.Text == "")
            {
                ID_.Visibility = Visibility.Visible;
                Text.BeginAnimation(OpacityProperty, doubleAnimation);
            }
            else
            {
                Text.Foreground = Brushes.Gray;
            }
        }

        private void PW_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //PW_TextBox.Visibility = Visibility.Collapsed;            
            PW_Box.Focus();
        }

        private void PW_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            Back.BorderBrush = Brushes.Gray;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 1;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.01);            
            if (PW_Box.Password.Length == 0)
            {
                PW_.Visibility = Visibility.Visible;
                Text.BeginAnimation(OpacityProperty, doubleAnimation);
            }
            else
                Text.Foreground = Brushes.Gray;           
        }
        
        private void Login_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ID__GotFocus(object sender, RoutedEventArgs e)
        {
            ID_Box.Focus();
        }

        private void ID_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ID_Box.Focus();
        }

        private void PW_Box_GotFocus(object sender, RoutedEventArgs e)
        {
            PW_.Visibility = Visibility.Collapsed;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            Back.BorderThickness = new Thickness(0, 0, 0, 5);
            Back.BorderBrush = Brushes.Blue;
            Back.BeginAnimation(OpacityProperty, doubleAnimation);
            Text.Foreground = Brushes.Blue;
            Text.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void PW__GotFocus(object sender, RoutedEventArgs e)
        {
            PW_Box.Focus();
        }

        private void PW_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PW_Box.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (ID_img.IsMouseOver == true)
            //    ID_Box.Focus();
            //  //  System.Windows.MessageBox.Show("a");
            if (ID_Box.IsFocused == true || PW_Box.IsFocused == true)
                temp.Focus();
        }
    }
}
