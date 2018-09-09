using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.Models;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Client
{
    /// <summary>
    /// SignUp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            String callUrl = "http://allcleapp.azurewebsites.net/api/Users";
            String[] data = new String[10];
            data[0] = ID_box.Text;              // id
            data[1] = PW_Box.Password;          // pw
            data[2] = PWCon_Box.Password;       //confirm pw
            if (data[1] == data[2])
            {
                String postData = "{ \"Id\" : \"" + data[0] + "\", \"Password\" : \"" + data[1] + "\"}";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(callUrl);// 인코딩 UTF-8
                byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);
                httpWebRequest.ContentType = "application/json; charset=UTF-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = sendData.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Close();
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                streamReader.ReadToEnd();
                streamReader.Close();
                httpWebResponse.Close();
            }
            else
                System.Windows.MessageBox.Show("비밀번호가 일치하지 않습니다");
        }

        private void ID_box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ID_box.Text == "ID를 입력해주세요")
            {
                ID_box.Foreground = Brushes.Black;
                ID_box.TextAlignment = TextAlignment.Left;
                ID_box.Text = "";
            }
        }

        private void ID_box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ID_box.Text == "")
            {
                ID_box.Foreground = Brushes.LightGray;
                ID_box.TextAlignment = TextAlignment.Center;
                ID_box.Text = "ID를 입력해주세요";
            }
        }
        private void PW_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PW_TextBox.Visibility = Visibility.Collapsed;
            PW_Box.Focus();
        }

        private void PW_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PW_Box.Password.Length == 0)
            {
                PW_TextBox.Visibility = Visibility.Visible;
            }
        }

        private void PWCon_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {            
                PWCon_TextBox.Visibility = Visibility.Collapsed;
                PWCon_Box.Focus();            
        }

        private void PWCon_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PWCon_Box.Password.Length == 0)
            {
                PWCon_TextBox.Visibility = Visibility.Visible;
            }
        }

        
    }
}
