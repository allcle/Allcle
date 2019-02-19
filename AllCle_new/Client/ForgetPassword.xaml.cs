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
using Client.Models;
using Newtonsoft.Json;
using System.Net.Mail;


namespace Client
{
    /// <summary>
    /// ForgetPassword.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ForgetPassword : Window
    {
        public ForgetPassword()
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

        private void Login_Encrypt(String NewpostData)
        {
            String callUrl = "http://allcleapp.azurewebsites.net/api/Users";

            HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(callUrl);// 인코딩 UTF-8
            byte[] sendData2 = UTF8Encoding.UTF8.GetBytes(NewpostData);
            httpWebRequest2.ContentType = "application/json; charset=UTF-8";
            httpWebRequest2.Method = "PUT";

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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = ID_box.Text + "@";
            if (email_cbx.Text == "직접입력")            
                email = email + emailWrite_tbx;            
            else
                email = email + email_tbk.Text;
            MailMessage mail = new MailMessage("allcle@naver.com", email);
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.naver.com";
            client.Credentials = new System.Net.NetworkCredential("allcle", "1q2w3e4r!");
            mail.Subject = "올클에서 회원님의 임시비밀번호를 드립니다.";



            string tempPW = null;
            Random rand = new Random();
            int len = rand.Next(6, 10);
            for (int i = 0; i < len; i++)
            {
                tempPW += (char)rand.Next(48, 57);  // 랜덤으로 대문자 암호화키 생성
            }
            string setkey = null;
            len = rand.Next(6, 10);
            for (int i = 0; i < len; i++)
            {
                setkey += (char)rand.Next(65, 122);  // 랜덤으로 대문자 암호화키 생성
            }
            string NewEncryptedPW = Encrypt(tempPW, setkey);
            mail.Body = "회원님의 임시 비밀번호는 " + tempPW + "입니다.";
            try
            {
                client.Send(mail);
                System.Windows.MessageBox.Show("Cool!");
                String NewpostData = "{ \"Password\" : \"" + NewEncryptedPW + "\", \"EncryptKey\" : \"" + setkey + "\", \"Id\" : \"" + email + "\", \"YearOfEntry\" : \"0\", \"College\" : \"0\", \"Major\" : \"0\"}";
                Login_Encrypt(NewpostData);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Fail");
            }
            this.Hide();
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
