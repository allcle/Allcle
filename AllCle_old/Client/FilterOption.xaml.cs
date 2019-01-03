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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// FilterOption.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FilterOption : Window
    {
        public static bool subjectOption;   //시간표에 있는 과목 선택할지말지
        public static bool timeOption;      //시간표에 있는 시간 뺄지말지
        public static int searchOption;     //검색 옵션
        public FilterOption()
        {
            InitializeComponent();
            subjectOption = false;
            timeOption = false;
            searchOption = 1;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (TimeO.IsChecked == true)        //시간 라디오버튼
                timeOption = true;
            else if (TimeX.IsChecked == true)
                timeOption = false;

            if (SubjectO.IsChecked == true)     //과목 라디오버튼
                subjectOption = true;
            else if (SubjectX.IsChecked == true)
                subjectOption = false;

            if (Search1.IsChecked == true)
                searchOption = 1;
            else if (Search2.IsChecked == true)
                searchOption = 2;
            else if (Search3.IsChecked == true)
                searchOption = 3;
            else
                searchOption = 0;

            this.Hide();
            App.MS.RefreshByOption(timeOption, subjectOption);
        }
    }
}
