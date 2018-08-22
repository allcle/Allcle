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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using test.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace test
{
    /// <summary>
    /// MainScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainScreen : Window
    {
        static HttpClient client = new HttpClient();
        bool searchButton = true;
        DataTable _timeTable = new DataTable();
        List<Subject> SubjectList = new List<Subject>();
        List<DayOfTheWeek> DayList = new List<DayOfTheWeek>();
        public MainScreen()
        {
            InitializeComponent();

            //_timeTable.Columns.Add(new DataColumn("교시", typeof(string)));
            //_timeTable.Columns.Add(new DataColumn("           월            ", typeof(string)));
            //_timeTable.Columns.Add(new DataColumn("           화            ", typeof(string)));
            //_timeTable.Columns.Add(new DataColumn("           수            ", typeof(string)));
            //_timeTable.Columns.Add(new DataColumn("           목            ", typeof(string)));
            //_timeTable.Columns.Add(new DataColumn("           금            ", typeof(string)));
            //_timeTable.Rows.Add("\n\n1\n\n");
            //_timeTable.Rows.Add("\n\n2\n\n");
            //_timeTable.Rows.Add("\n\n3\n\n");
            //_timeTable.Rows.Add("\n\n4\n\n");
            //_timeTable.Rows.Add("\n\n5\n\n");
            //_timeTable.Rows.Add("\n\n6\n\n");
            //_timeTable.Rows.Add("\n\n7\n\n");
            //_timeTable.Rows.Add("\n\n8\n\n");
            //_timeTable.Rows.Add("\n\n9\n\n");
            //_timeTable.Rows.Add("\n\n10\n\n");
            //_timeTable.Rows.Add("\n\n11\n\n");
            //_timeTable.Rows.Add("\n\n12\n\n");
            DayOfTheWeek a = new DayOfTheWeek("1",null, null, null, null, null, null);
            DayList.Add(a);
            DayOfTheWeek b = new DayOfTheWeek("2", null, null, null,null, null, null);
            DayList.Add(b);
            DayOfTheWeek c = new DayOfTheWeek("3", null, null, null, null, null, null);
            DayList.Add(c);
            DayOfTheWeek d = new DayOfTheWeek("4", null, null, null, null, null, null);
            DayList.Add(d);
            DayOfTheWeek e = new DayOfTheWeek("5", null, null, null, null, null, null);
            DayList.Add(e);
            DayOfTheWeek f = new DayOfTheWeek("6", null, null, null, null, null, null);
            DayList.Add(f);
            DayOfTheWeek g = new DayOfTheWeek("7", null, null, null, null, null, null);
            DayList.Add(g);
            DayOfTheWeek h = new DayOfTheWeek("8", null, null, null, null, null, null);
            DayList.Add(h);
            DayOfTheWeek i = new DayOfTheWeek("9", null, null, null, null, null, null);
            DayList.Add(i);
            DayOfTheWeek j = new DayOfTheWeek("10", null, null, null, null, null, null);
            DayList.Add(j);
            DayOfTheWeek k = new DayOfTheWeek("11", null, null, null, null, null, null);
            DayList.Add(k);
            DayOfTheWeek l = new DayOfTheWeek("12", null, null, null, null, null, null);
            DayList.Add(l);
            TimeTable.ItemsSource = DayList;
            
        }
        private void Search_btn_Click(object sender, RoutedEventArgs e)
        { 
            ShowList();
        }
        private void Search_Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ShowList();
        }
        private void OnOff_btn_Click(object sender, RoutedEventArgs e)
        {
            string urlBase = @"https://allcleapp.azurewebsites.net/api/AllCleSubjects2";
            string url = null;
            if (ListToString(DayList) == null)
                url = urlBase;
            else url = urlBase + "/" + ListToString(DayList);
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            DataListView.ItemsSource = SubjectList;
        }
        private void ShowList()
        {
            string urlBase = @"https://allcleapp.azurewebsites.net/api/AllCleSubjects2";
            string url = null;
            if(ListToString(DayList) == null)
                url = urlBase + "/subject/" + Search_Box.Text;
            else
                url = urlBase + "/" + ListToString(DayList) + "/" + Search_Box.Text;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            DataListView.ItemsSource = SubjectList;
        }
        private void DataListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bool ok = false;
            int index = DataListView.SelectedIndex;
            int _period1=0;
            string _day1=null;
            int _period2 = 0; ;
            string _day2 = null;
            int _period3 = 0; ;
            string _day3 = null;
            int _period4 = 0; ;
            string _day4 = null;
            int _period5 = 0; ;
            string _day5 = null;
            int _period6 = 0; ;
            string _day6 = null;
            int _period7 = 0; ;
            string _day7 = null;
            int _period8 = 0; ;
            string _day8 = null;
            if (DataListView.SelectedItems.Count == 1)
            {
                if (SubjectList[index].Time1 != "")
                {
                    _period1 = Int32.Parse(SubjectList[index].Time1.Substring(1, SubjectList[index].Time1.Length - 1));
                    _day1 = SubjectList[index].Time1.Substring(0, 1);
                    if (_day1 == "월" && DayList[_period1 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day1 == "화" && DayList[_period1 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day1 == "수" && DayList[_period1 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day1 == "목" && DayList[_period1 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day1 == "금" && DayList[_period1 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day1 == "토" && DayList[_period1 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
                if (SubjectList[index].Time2 != "" && ok == true)
                {
                    _period2 = Int32.Parse(SubjectList[index].Time2.Substring(1, SubjectList[index].Time2.Length - 1));
                    _day2 = SubjectList[index].Time2.Substring(0, 1);
                    if (_day2 == "월" && DayList[_period2 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day2 == "화" && DayList[_period2 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day2 == "수" && DayList[_period2 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day2 == "목" && DayList[_period2 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day2 == "금" && DayList[_period2 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day2 == "토" && DayList[_period2 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
                if (SubjectList[index].Time3 != "" && ok == true)
                {
                    _period3 = Int32.Parse(SubjectList[index].Time3.Substring(1, SubjectList[index].Time3.Length - 1));
                    _day3 = SubjectList[index].Time3.Substring(0, 1);
                    if (_day3 == "월" && DayList[_period3 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day3 == "화" && DayList[_period3 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day3 == "수" && DayList[_period3 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day3 == "목" && DayList[_period3 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day3 == "금" && DayList[_period3 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day3 == "토" && DayList[_period3 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
                if (SubjectList[index].Time4 != "" && ok == true)
                {
                    _period4 = Int32.Parse(SubjectList[index].Time4.Substring(1, SubjectList[index].Time4.Length - 1));
                    _day4 = SubjectList[index].Time4.Substring(0, 1);
                    if (_day4 == "월" && DayList[_period4 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day4 == "화" && DayList[_period4 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day4 == "수" && DayList[_period4 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day4 == "목" && DayList[_period4 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day4 == "금" && DayList[_period4 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day4 == "토" && DayList[_period4 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
                if (SubjectList[index].Time5 != "" && ok == true)
                {
                    _period5 = Int32.Parse(SubjectList[index].Time5.Substring(1, SubjectList[index].Time5.Length - 1));
                    _day5 = SubjectList[index].Time5.Substring(0, 1);
                    if (_day5 == "월" && DayList[_period5 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day5 == "화" && DayList[_period5 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day5 == "수" && DayList[_period5 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day5 == "목" && DayList[_period5 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day5 == "금" && DayList[_period5 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day5 == "토" && DayList[_period5 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
                if (SubjectList[index].Time6 != "" && ok == true)
                {
                    _period6 = Int32.Parse(SubjectList[index].Time6.Substring(1, SubjectList[index].Time6.Length - 1));
                    _day6 = SubjectList[index].Time6.Substring(0, 1);
                    if (_day6 == "월" && DayList[_period6 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day6 == "화" && DayList[_period6 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day6 == "수" && DayList[_period6 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day6 == "목" && DayList[_period6 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day6 == "금" && DayList[_period6 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day6 == "토" && DayList[_period6 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
                if (SubjectList[index].Time7 != "" && ok == true)
                {
                    _period7 = Int32.Parse(SubjectList[index].Time7.Substring(1, SubjectList[index].Time7.Length - 1));
                    _day7 = SubjectList[index].Time7.Substring(0, 1);
                    if (_day7 == "월" && DayList[_period7 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day7 == "화" && DayList[_period7 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day7 == "수" && DayList[_period7 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day7 == "목" && DayList[_period7 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day7 == "금" && DayList[_period7 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day7 == "토" && DayList[_period7 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
                if (SubjectList[index].Time8 != "" && ok == true)
                {
                    _period8 = Int32.Parse(SubjectList[index].Time8.Substring(1, SubjectList[index].Time8.Length - 1));
                    _day8 = SubjectList[index].Time8.Substring(0, 1);
                    if (_day8 == "월" && DayList[_period8 - 1].mon == null)
                    {
                        ok = true;
                    }
                    else if (_day8 == "화" && DayList[_period8 - 1].tue == null)
                    {
                        ok = true;
                    }
                    else if (_day8 == "수" && DayList[_period8 - 1].wed == null)
                    {
                        ok = true;
                    }
                    else if (_day8 == "목" && DayList[_period8 - 1].thu == null)
                    {
                        ok = true;
                    }
                    else if (_day8 == "금" && DayList[_period8 - 1].fri == null)
                    {
                        ok = true;
                    }
                    else if (_day8 == "토" && DayList[_period8 - 1].sat == null)
                    {
                        ok = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        ok = false;
                    }
                }
            }
           // SolidColorBrush brush = new SolidColorBrush(Colors.Red);
            if (ok == true)
            {
                if (SubjectList[index].Time1 != null)
                {
                    if (_day1 == "월")
                        DayList[_period1 - 1].mon = SubjectList[index].ClassName;
                    else if (_day1 == "화")
                        DayList[_period1 - 1].tue = SubjectList[index].ClassName;
                    else if (_day1 == "수")
                        DayList[_period1 - 1].wed = SubjectList[index].ClassName;
                    else if (_day1 == "목")
                        DayList[_period1 - 1].thu = SubjectList[index].ClassName;
                    else if (_day1 == "금")
                        DayList[_period1 - 1].fri = SubjectList[index].ClassName;
                    else if (_day1 == "토")
                        DayList[_period1 - 1].sat = SubjectList[index].ClassName;
                }
                if (SubjectList[index].Time2 != null)
                {
                    if (_day2 == "월")
                        DayList[_period2 - 1].mon = SubjectList[index].ClassName;
                    else if (_day2 == "화")
                        DayList[_period2 - 1].tue = SubjectList[index].ClassName;
                    else if (_day2 == "수")
                        DayList[_period2 - 1].wed = SubjectList[index].ClassName;
                    else if (_day2 == "목")
                        DayList[_period2 - 1].thu = SubjectList[index].ClassName;
                    else if (_day2 == "금")
                        DayList[_period2 - 1].fri = SubjectList[index].ClassName;
                    else if (_day2 == "토")
                        DayList[_period2 - 1].sat = SubjectList[index].ClassName;
                }
                if (SubjectList[index].Time3 != null)
                {
                    if (_day3 == "월")
                        DayList[_period3 - 1].mon = SubjectList[index].ClassName;
                    else if (_day3 == "화")
                        DayList[_period3 - 1].tue = SubjectList[index].ClassName;
                    else if (_day3 == "수")
                        DayList[_period3 - 1].wed = SubjectList[index].ClassName;
                    else if (_day3 == "목")
                        DayList[_period3 - 1].thu = SubjectList[index].ClassName;
                    else if (_day3 == "금")
                        DayList[_period3 - 1].fri = SubjectList[index].ClassName;
                    else if (_day3 == "토")
                        DayList[_period3 - 1].sat = SubjectList[index].ClassName;
                }
                if (SubjectList[index].Time4 != null)
                {
                    if (_day4 == "월")
                        DayList[_period4 - 1].mon = SubjectList[index].ClassName;
                    else if (_day4 == "화")
                        DayList[_period4 - 1].tue = SubjectList[index].ClassName;
                    else if (_day4 == "수")
                        DayList[_period4 - 1].wed = SubjectList[index].ClassName;
                    else if (_day4 == "목")
                        DayList[_period4 - 1].thu = SubjectList[index].ClassName;
                    else if (_day4 == "금")
                        DayList[_period4 - 1].fri = SubjectList[index].ClassName;
                    else if (_day4 == "토")
                        DayList[_period4 - 1].sat = SubjectList[index].ClassName;
                }
                if (SubjectList[index].Time5 != null)
                {
                    if (_day5 == "월")
                        DayList[_period5 - 1].mon = SubjectList[index].ClassName;
                    else if (_day5 == "화")
                        DayList[_period5 - 1].tue = SubjectList[index].ClassName;
                    else if (_day5 == "수")
                        DayList[_period5 - 1].wed = SubjectList[index].ClassName;
                    else if (_day5 == "목")
                        DayList[_period5 - 1].thu = SubjectList[index].ClassName;
                    else if (_day5 == "금")
                        DayList[_period5 - 1].fri = SubjectList[index].ClassName;
                    else if (_day5 == "토")
                        DayList[_period5 - 1].sat = SubjectList[index].ClassName;
                }
                if (SubjectList[index].Time6 != null)
                {
                    if (_day6 == "월")
                        DayList[_period6 - 1].mon = SubjectList[index].ClassName;
                    else if (_day6 == "화")
                        DayList[_period6 - 1].tue = SubjectList[index].ClassName;
                    else if (_day6 == "수")
                        DayList[_period6 - 1].wed = SubjectList[index].ClassName;
                    else if (_day6 == "목")
                        DayList[_period6 - 1].thu = SubjectList[index].ClassName;
                    else if (_day6 == "금")
                        DayList[_period6 - 1].fri = SubjectList[index].ClassName;
                    else if (_day6 == "토")
                        DayList[_period6 - 1].sat = SubjectList[index].ClassName;
                }
                if (SubjectList[index].Time7 != null)
                {
                    if (_day7 == "월")
                        DayList[_period7 - 1].mon = SubjectList[index].ClassName;
                    else if (_day7 == "화")
                        DayList[_period7 - 1].tue = SubjectList[index].ClassName;
                    else if (_day7 == "수")
                        DayList[_period7 - 1].wed = SubjectList[index].ClassName;
                    else if (_day7 == "목")
                        DayList[_period7 - 1].thu = SubjectList[index].ClassName;
                    else if (_day7 == "금")
                        DayList[_period7 - 1].fri = SubjectList[index].ClassName;
                    else if (_day7 == "토")
                        DayList[_period7 - 1].sat = SubjectList[index].ClassName;
                }
                if (SubjectList[index].Time8 != null)
                {
                    if (_day8 == "월")
                        DayList[_period8 - 1].mon = SubjectList[index].ClassName;
                    else if (_day8 == "화")
                        DayList[_period8 - 1].tue = SubjectList[index].ClassName;
                    else if (_day8 == "수")
                        DayList[_period8 - 1].wed = SubjectList[index].ClassName;
                    else if (_day8 == "목")
                        DayList[_period8 - 1].thu = SubjectList[index].ClassName;
                    else if (_day8 == "금")
                        DayList[_period8 - 1].fri = SubjectList[index].ClassName;
                    else if (_day8 == "토")
                        DayList[_period8 - 1].sat = SubjectList[index].ClassName;
                }
            }
            TimeTable.ItemsSource = DayList;
            TimeTable.Items.Refresh();
        }
        private string ListToString(List<DayOfTheWeek> _list)
        {
            string result = null;
            for (int i = 1; i < _list.Count+1; i++)
            {
                if (_list[i-1].mon != null)
                {
                    result += "월";
                    result += i.ToString();
                    result += "&";
                }
                if (_list[i-1].tue != null)
                {
                    result += "화";
                    result += i.ToString();
                    result += "&";
                }
                if (_list[i-1].wed != null)
                {
                    result += "수";
                    result += i.ToString();
                    result += "&";
                }
               if (_list[i-1].thu != null)
                {
                    result += "목";
                    result += i.ToString();
                    result += "&";
                }
                if (_list[i-1].fri != null)
                {
                    result += "금";
                    result += i.ToString();
                    result += "&";
                }
                if (_list[i-1].sat != null)
                {
                    result += "토";
                    result += i.ToString();
                    result += "&";
                }
            }
            return result;
        }
    }


}

