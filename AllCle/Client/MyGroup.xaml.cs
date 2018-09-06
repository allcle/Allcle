using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using System.Windows.Shapes;
using Client.Models;

namespace Client
{
    /// <summary>
    /// MyGroup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MyGroup : Window
    {
        ObservableCollection<string> datas = new ObservableCollection<string>(); //User의 myGroup 목록
        public MyGroup()
        {
            InitializeComponent();
            this.datas = MainScreen.data;       //메인에서 데이터 가져오기
            MyGroup_Cob.ItemsSource = this.datas; //combo box의 소스로 이용
        }

        private void MyGroup_Cob_Initialized(object sender, EventArgs e)
        {
            
        }

        private void MyGroup_Cob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
               MessageBox.Show("Value : " + this.datas[MyGroup_Cob.SelectedIndex]); //선택하면 선택한거 보여줌
        }

        private void NewGroup_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(GroupName_Box.Text + "를 추가하시겠습니까?", "Add Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) //yes하면
            {
                this.datas.Add(GroupName_Box.Text);
                //MyGroup_Cob.ItemsSource = this.datas;
                GroupName_Box.Text = "새로운 그룹";
            }
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            //MainScreen.data = this.datas;
            Hide();
        }
    }
}
