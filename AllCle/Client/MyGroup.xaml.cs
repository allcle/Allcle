using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace Client
{
    /// <summary>
    /// MyGroup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MyGroup : Window
    {
        List<UserTimeTable> userTimeTable = new List<UserTimeTable>(); //User의 myGroup 목록
        string urlTimeTalbe = @"https://allcleapp.azurewebsites.net/api/UserTimeTable"; //combobox를 위한 기본 url
        string url = null;
        private void GetTimeTable()
        {
            url = urlTimeTalbe + "/" + App.ID;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            userTimeTable = JsonConvert.DeserializeObject<List<UserTimeTable>>(Unicode);
        }

        public MyGroup()
        {
            InitializeComponent(); 
        }

        private void MyGroup_Cob_Initialized(object sender, EventArgs e)
        {
            
        }

        private void MyGroup_Cob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
               MessageBox.Show("Value : " + userTimeTable[MyGroup_Cob.SelectedIndex]); //선택하면 선택한거 보여줌
        }

        private void NewGroup_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(GroupName_Box.Text + "를 추가하시겠습니까?", "Add Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) //yes하면
            {
                //is.userTimeTable.Add(GroupName_Box.Text);
                //MyGroup_Cob.ItemsSource = this.datas;
                GroupName_Box.Text = "새로운 그룹";
            }
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            //MainScreen.data = this.datas;
            Hide();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            //GetTimeTable();                         //Data 가져오기
            //MyGroup_Cob.ItemsSource = userTimeTable; //combo box의 소스로 이용
        }
    }
}
