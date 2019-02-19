using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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
        public string callUrl = "http://allcleapp.azurewebsites.net/api/Users";
        public string Encrypt(string strToEncrypt, string strKey)
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
            return Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                TransformFinalBlock(byteBuff, 0, byteBuff.Length)); ;

        }
        public bool IdExists(string idbox)
        {
            string url = callUrl + "/" + idbox;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            if (Unicode == "true")
                return true;
            else
                return false;
        }
        public void PostUser(string idbox, string password, string yearofentry, string college, string major)
        {
            // 첫 회원가입 시, Encrypt key 랜덤 생성. 로그인 마다 바뀔 예정
            string setkey = null;
            Random rand = new Random();
            int len = rand.Next(6, 10);
            for (int i = 0; i < len; i++)
            {
                setkey += (char)rand.Next(65, 122);  // 랜덤으로 대문자 암호화키 생성
            }
            string encrypted = Encrypt(password, setkey);
            string url = callUrl + "/" + idbox;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            String postData = "{ \"Id\" : \"" + idbox + "\", \"Password\" : \"" + encrypted + "\", \"EncryptKey\" : \"" + setkey + "\", \"YearOfEntry\" : \"" + yearofentry + "\", \"College\" : \"" + college + "\", \"Major\" : \"" + major + "\"}";
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


        private void Save_btn_Click(object sender, RoutedEventArgs e)       //수정
        {
            ID_concern.Visibility = Visibility.Hidden;      // 처음엔 일단 숨기기
            PW_corcenrn.Visibility = Visibility.Hidden;     //처음엔 일단 숨기기
            PW_corcenrn2.Visibility = Visibility.Hidden;    // 처음엔 일단 숨기기
            Regex regexPW = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{2,})$");     //비밀번호 정규식
            string id = null;                                                           //id가 id + 이메일이라서 필요
            Boolean foundMatch = regexPW.IsMatch(PW_Box.Password);
            if (PW_Box.Password != PWCon_Box.Password)                     //비밀번호 strcmp
                PW_corcenrn2.Visibility = Visibility.Visible;       //다르다고 경고

            if (IdExists(ID_box.Text))                                          //이미 아이디가 존재하거나 이메일 형식이 아니다
            {
                ID_concern.Visibility = Visibility.Visible;
                ID_box.Text = "";
                ID_box.Focus();
            }
            else if (PW_Box.Password.Length < 8 || !foundMatch)
            {
                PW_corcenrn.Visibility = Visibility.Visible;
            }
            else if (PW_Box.Password == PWCon_Box.Password)
            {
                string major = "";
                if (College_cbx.Text == "일반대학")
                {
                    major = Major_normal.Text;
                }
                else if (College_cbx.Text == "공과대학")
                {
                    major = Major_engineer.Text;
                }
                else if (College_cbx.Text == "건축대학")
                {
                    major = Major_architecture.Text;
                }
                if (email_cbx.Text == "직접입력")
                    id = ID_box.Text + "@" + emailWrite_tbx.Text;
                else
                    id = ID_box.Text + "@" + email_tbk.Text;
                PostUser(id, PW_Box.Password, YearOfEntry_cbx.Text, College_cbx.Text, major);
                System.Windows.MessageBox.Show("회원가입이 완료되었습니다");
                this.Close();
            }
            else
            {
                PWCon_Box.Focus();
            }
        }

        private void ID_box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ID_box.Text == "아이디")
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
                ID_box.Text = "아이디";
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

        private void College_cbx_DropDownClosed(object sender, EventArgs e)
        {
            Major_normal.Visibility = Visibility.Collapsed;
            Major_engineer.Visibility = Visibility.Collapsed;
            Major_architecture.Visibility = Visibility.Collapsed;

            if (College_cbx.Text == "일반대학")
            {
                Major_normal.Visibility = Visibility.Visible;
            }
            else if (College_cbx.Text == "공과대학")
            {
                Major_engineer.Visibility = Visibility.Visible;
            }
            else if (College_cbx.Text == "건축대학")
            {
                Major_architecture.Visibility = Visibility.Visible;
            }
            else
                System.Windows.MessageBox.Show("3개대학 이외 다른게 존재. error");
        }

        private void email_cbx_DropDownClosed(object sender, EventArgs e)
        {
            email_tbk.Visibility = Visibility.Collapsed;
            emailWrite_tbx.Visibility = Visibility.Collapsed;
            if (email_cbx.Text == "직접입력")
            {
                emailWrite_tbx.Visibility = Visibility.Visible;
            }
            else
            {
                email_tbk.Visibility = Visibility.Visible;
                email_tbk.Text = email_cbx.Text;
            }

        }
    }
}
