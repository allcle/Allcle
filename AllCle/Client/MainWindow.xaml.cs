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
using System.Security.Cryptography;
using System.IO;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        string setkey = "allcle";
        public MainWindow()
        {
            InitializeComponent();
        }

        public string Encrypt(string strToEncrypt, string strKey)               //암호화
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteBuff;
            string strTempKey = strKey;
            byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            objHashMD5 = null;
            objDESCrypto.Key = byteHash;
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
            return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length)); ;
        }

        private void Next_btn_Click(object sender, RoutedEventArgs e)      // login 버튼을 클릭하면, MainWindow.xaml을 close하고, MainScreen.xaml show하는 메소드
        {
            Login_id();
        }
        
        private void Forget_btn_Button_Click(object sender, RoutedEventArgs e)
        {
            App.MS.Show();                                              //메인 화면 띄우기
            this.Hide();                                                //로그인창 hide
        }

        private void Geust_Login_Button_Click(object sender, RoutedEventArgs e)
        {
            App.MS.Show();                                              //메인 화면 띄우기
            this.Hide();                                                //로그인창 hide
        }

        private void SingUP_btn_Click(object sender, RoutedEventArgs e)         //회원가입
        {
            SignUp SU = new SignUp();
            SU.Show();
        }

        private void ID_Box_GotFocus(object sender, RoutedEventArgs e)          //ID박스에 포커스 맞춰지면
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            Back.BorderThickness = new Thickness(0, 0, 0, 5);
            Back.BorderBrush = Brushes.Blue;                            //파란색 밑줄
            Back.BeginAnimation(OpacityProperty, doubleAnimation);      //파란색 밑줄 애니메이션
            ID_.Visibility = Visibility.Collapsed;
            Text.Foreground = Brushes.Blue;                             //글자 파랗게
            Text.BeginAnimation(OpacityProperty, doubleAnimation);      //글자 파랗게 애니메이션
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
            Login_PW();
        }

        private void ID__GotFocus(object sender, RoutedEventArgs e)     //ID 칸 클릭시
        {   
            ID_Box.Focus();                                             //ID입력칸으로 focus
        }

        private void ID_img_MouseDown(object sender, MouseButtonEventArgs e) //이미지 클릭시 인데 이상함
        {
            ID_Box.Focus();                                             //ID입력칸으로 focus
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

        private void PW__GotFocus(object sender, RoutedEventArgs e)         //패스워드 누르는 칸 클릭시
        {      
            PW_Box.Focus();                                                 //패스워드 창으로 포커스 가도록
        }

        private void PW_img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PW_Box.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)       // 프로그램 아무곳이나 누르면 lostfocus
        {
            if (ID_Box.IsFocused == true || PW_Box.IsFocused == true)
                temp.Focus();
        }

        private void ID_Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Login_id();
            }
        }

        private void Login_id()
        {
            string urlBase = @"https://allcleapp.azurewebsites.net/api/Users"; //기본 url
            string url = null;  //json으로 쓰일 url
            url = urlBase + "/" + ID_Box.Text;                              //id 있는지 확인
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            if (Unicode == "true")                                          //있다
            {
                App.ID = ID_Box.Text;
                ID.Visibility = Visibility.Collapsed;
                ID_Box.Visibility = Visibility.Collapsed;
                PW.Visibility = Visibility.Visible;
                Text.Text = "Password";
                PW_Box.Focus();
            }
            else if (ID_Box.Text == "")                                      //id 입력을 안 했을 때
                System.Windows.MessageBox.Show("아이디를 입력해주세요");
            else                                                            //없다
                System.Windows.MessageBox.Show(ID_Box.Text + "는 존재하지 않는 아이디 입니다");
        } 

        private void Login_PW()
        {
            string urlBase = @"https://allcleapp.azurewebsites.net/api/Users"; //기본 url
            string url = null;  //json으로 쓰일 url
            url = urlBase + "/" + App.ID;
            string encrypted = Encrypt(PW_Box.Password, setkey);
            String postData = "{ \"Id\" : \"" + App.ID + "\", \"Password\" : \"" + encrypted + "\"}";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);// 인코딩 UTF-8
            byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = sendData.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Close();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            string result = streamReader.ReadToEnd().ToString();            //return 값이 true인지 false인지
            streamReader.Close();
            httpWebResponse.Close();
            if (result == "true")                                           //아이디 비번 맞음
            {
                App.MS.Show();                                              //메인 화면 띄우기
                this.Hide();                                                //로그인창 hide
            }
            else
                System.Windows.MessageBox.Show("잘못된 비밀번호입니다.");
        }

        private void PW_Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                Login_PW();
            }
        }
    }
}
