using System;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using Client.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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
        string urlBase = @"https://allcleapp.azurewebsites.net/api/Users"; //기본 url
        string url = null;  //json으로 쓰일 url
        string Major = "";

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


        private void Connect(String url, String NewpostData, String Method)     //post or put을 결정
        {
            // POST : Save_Schedule_Click, TimeAdd_btn_Click
            // PUT: TableEdit_txtbox_KeyDown
            try
            {
                HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url);// 인코딩 UTF-8
                byte[] sendData2 = UTF8Encoding.UTF8.GetBytes(NewpostData);
                httpWebRequest2.ContentType = "application/json; charset=UTF-8";
                httpWebRequest2.Method = Method;
                httpWebRequest2.ContentLength = sendData2.Length;
                Stream requestStream2 = httpWebRequest2.GetRequestStream();
                requestStream2.Write(sendData2, 0, sendData2.Length);
                requestStream2.Close();
                HttpWebResponse httpWebResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();
                StreamReader streamReader2 = new StreamReader(httpWebResponse2.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                streamReader2.ReadToEnd();
                streamReader2.Close();
                httpWebResponse2.Close();
            }
            catch
            {
                System.Windows.MessageBox.Show("다시 시도해주세요");
            }
        }

        public void PutUser(string idbox, string password, string yearofentry, string college, string major)
        {
            string setkey = null;
            Random rand = new Random();
            int len = rand.Next(6, 10);
            for (int i = 0; i < len; i++)
            {
                setkey += (char)rand.Next(65, 122);  // 랜덤으로 대문자 암호화키 생성
            }
            string NewEncryptedPW = Encrypt(password, setkey);
            string NewpostData = "{ \"Password\" : \"" + NewEncryptedPW + "\", \"EncryptKey\" : \"" + setkey + "\", \"Id\" : \"" + idbox + "\", \"YearOfEntry\" : \"" + yearofentry + "\", \"College\" : \"" + college + "\", \"Major\" : \"" + major + "\"}";
            url = urlBase;
            Connect(url, NewpostData, "PUT");
            
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
            YearOfEntry_cbx.Text = App.MS.YearOfEntry;
            College_cbx.Text =  App.MS.College;
            College_cbx_DropDownClosed(College_cbx, null);
            if (College_cbx.Text == "일반대학")
                Major_normal.Text = App.MS.Major;
            else if (College_cbx.Text == "공과대학")
                Major_engineer.Text = App.MS.Major;
            else if (College_cbx.Text == "건축대학")
                Major_architecture.Text = App.MS.Major;
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
            wrongCurPW_tbx.Visibility = Visibility.Collapsed;
            wrongNewPW_tbx.Visibility = Visibility.Collapsed;
            wrongConPW_tbx.Visibility = Visibility.Collapsed;
            Regex regexPW = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{2,})$");     //비밀번호 정규식
            Boolean foundMatch = regexPW.IsMatch(newPW_pwx.Password);
            try
            {
                url = urlBase + "/" + App.ID;
                var json = new WebClient().DownloadData(url);
                string Unicode = Encoding.UTF8.GetString(json);
                String postData = "";

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

                Unicode = streamReader.ReadToEnd().ToString();
                User result = JsonConvert.DeserializeObject<User>(Unicode); //유저 정보 post로 가져온거
                streamReader.Close();
                httpWebResponse.Close();
                string encryptedPW = Encrypt(curPW_pwx.Password, result.EncryptKey);   //비밀번호 암호화하기
                if (encryptedPW != result.Password)      //기존꺼랑 비교
                {
                    wrongCurPW_tbx.Visibility = Visibility.Visible;
                    return;
                }
                if (newPW_pwx.Password.Length < 8 || !foundMatch)   //비밀번호 형식
                {
                    wrongNewPW_tbx.Visibility = Visibility.Visible;
                    return;
                }

                if (newPW_pwx.Password != conPW_pwx.Password)       //비밀번호 두개가 맞는지
                {
                    wrongConPW_tbx.Visibility = Visibility.Visible;
                    return;
                }
                if (College_cbx.Text == "일반대학")
                    Major = Major_normal.Text;
                else if (College_cbx.Text == "공과대학")
                    Major = Major_engineer.Text;
                else if (College_cbx.Text == "건축대학")
                    Major = Major_architecture.Text;
                PutUser(App.ID, newPW_pwx.Password, YearOfEntry_cbx.Text, College_cbx.Text, Major);
                System.Windows.MessageBox.Show("성공적으로 변경되었습니다.");
                this.Hide();
            }
            catch
            {
                System.Windows.MessageBox.Show("다시 시도해주세요");
                return;
            }

        }
    }
}
