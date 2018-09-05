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
using Client.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Client
{
    /// <summary>
    /// MainScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainScreen : Window
    {
        static HttpClient client = new HttpClient();
        List<Subject> SubjectList = new List<Subject>(); //전체 과목 리스트
        List<UsersSubject> UsersSubjectsList = new List<UsersSubject>(); //유저가 듣는 과목 리스트
        public struct TableSubjects //시간표 한칸의 Data
        {
            public string className;
            public string professor;
            public bool ableToPut;
        }
        TableSubjects[,] TimeTableDB = new TableSubjects[13, 7]; //12교시*일주일 2차원 배열
        string urlBase = @"https://allcleapp.azurewebsites.net/api/AllCleSubjects2"; //기본 url
        string url = null;  //json으로 쓰일 url
        public static ObservableCollection<string> data = new ObservableCollection<string>(); //User의 myGroup 목록        
        public MainScreen()
        {
            InitializeComponent();
            DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOff();
            InitDB();

            //SubjectList.Where(s => !s.Times.Contains("수1"));
        }
        private void Search_btn_Click(object sender, RoutedEventArgs e) //검색 버튼 눌렀을때
        {
            
            if (FilterOption.timeOption == true)
            {
                if (FilterOption.subjectOption == true)
                    DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOn(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList),Search_Box.Text);
                else
                    DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOn(TimeInList(UsersSubjectsList), Search_Box.Text);
            }
            else
            {
                if (FilterOption.subjectOption == true)
                    DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOn(SubjectInList(UsersSubjectsList), Search_Box.Text);
                else
                    DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOn(Search_Box.Text);
            }
            //ShowList();
        }
        private void Search_Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)//엔터키 눌렀을 때
        {

            if (FilterOption.timeOption == true)
            {
                if (FilterOption.subjectOption == true)
                    DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOn(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList), Search_Box.Text);
                else
                    DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOn(TimeInList(UsersSubjectsList), Search_Box.Text);
            }
            else
            {
                if (FilterOption.subjectOption == true)
                    DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOn(SubjectInList(UsersSubjectsList), Search_Box.Text);
                else
                    DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOn(Search_Box.Text);
            }
        }
        private void All_btn_Click(object sender, RoutedEventArgs e)//전체 버튼 클릭시
        {
            Search_Box.Visibility = Visibility.Visible;
            Search_Box.IsEnabled = true;
            Search_btn.Visibility = Visibility.Visible;
            Search_btn.IsEnabled = true;
            MyGroup_cob.Visibility = Visibility.Collapsed;
            MyGroup_cob.IsEnabled = false;
            DataListView_All.Visibility = Visibility.Visible;
            DataListView_All.IsEnabled = true;
            RefreshByOption(FilterOption.timeOption, FilterOption.subjectOption);            
        }
        public void RefreshByOption(bool _timeOption, bool _subjectOption)
        {
            if (_timeOption == true)
            {
                if (_subjectOption == true)
                    DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOff(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList));
                else
                    DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOff(TimeInList(UsersSubjectsList));
            }
            else
            {
                if (_subjectOption == true)
                    DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOff(SubjectInList(UsersSubjectsList));
                else
                    DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOff();
            }
        }

        private List<Subject> ShowTimeOnSubjectOnSearchOff(string _time, string _subject)  //남은시간에서만, 담은과목 제외
        {
            url = urlBase + "/" + _time + "/" + _subject;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;            
        }
        private List<Subject> ShowTimeOnSubjectOffSearchOff(string _time)                 //남은 시간만에서만
        {
            url = urlBase + "/" + _time + "/subjectfilteroff";
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;
        }
        private List<Subject> ShowTimeOffSubjectOnSearchOff(string _subject)                //담은 과목만 제외
        {
            url = urlBase + "/timefilteroff/" + _subject;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;
        }        
        private List<Subject> ShowTimeOffSubjectOffSearchOff()                             //전체 모든 과목 보기
        {
            url = urlBase + "/timefilteroff/subjectfilteroff";
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;
        }

        private List<Subject> ShowTimeOnSubjectOnSearchOn(string _time, string _subject, string _search) //남은시간에서만, 담은과목제외, 검색
        {
            url = urlBase + "/" + _time + "/" + _subject + "/" + _search;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;
        }
        private List<Subject> ShowTimeOnSubjectOffSearchOn(string _time, string _search)                 //남은 시간에서만 검색
        {
            url = urlBase + "/" + _time + "/subjectfilteroff/" + _search;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;
        }
        private List<Subject> ShowTimeOffSubjectOnSearchOn(string _subject, string _search)              //담은 과목 제외하고 검색
        {
            url = urlBase + "/timefilteroff/" + _subject + "/" + _search;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;
        }    
        private List<Subject> ShowTimeOffSubjectOffSearchOn(string _search)                              //그냥 과목검색
        {
            url = urlBase + "/timefilteroff/subjectfilteroff/" + _search;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            return SubjectList;
        }
                
        private void DataListView_All_MouseDoubleClick(object sender, MouseButtonEventArgs e) //리스트에 있는 과목을 더블클릭했을때
        {
            bool totalAbleToPut = false;
            int index = DataListView_All.SelectedIndex;
            int period1 = 0;
            string day1 = null;
            int period2 = 0; ;
            string day2 = null;
            int period3 = 0; ;
            string day3 = null;
            int period4 = 0; ;
            string day4 = null;
            int period5 = 0; ;
            string day5 = null;
            int period6 = 0; ;
            string day6 = null;
            int period7 = 0; ;
            string day7 = null;
            int period8 = 0; ;
            string day8 = null;
            if (DataListView_All.SelectedItems.Count == 1) //리스트에서 클릭하면
            {
                if (SubjectList[index].Time1 != "") //시간1이 존재하면
                {
                    period1 = Int32.Parse(SubjectList[index].Time1.Substring(1, SubjectList[index].Time1.Length - 1)); //교시
                    day1 = SubjectList[index].Time1.Substring(0, 1);  //요일
                    if (day1 == "월" && TimeTableDB[period1, 1].ableToPut == true) //요일에 해당 교시에 과목을 넣을 수있으면
                    {
                        totalAbleToPut = true; //넣을 수 있다
                    }
                    else if (day1 == "화" && TimeTableDB[period1, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day1 == "수" && TimeTableDB[period1, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day1 == "목" && TimeTableDB[period1, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day1 == "금" && TimeTableDB[period1, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day1 == "토" && TimeTableDB[period1, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
                if (SubjectList[index].Time2 != "" && totalAbleToPut == true)
                {
                    period2 = Int32.Parse(SubjectList[index].Time2.Substring(1, SubjectList[index].Time2.Length - 1));
                    day2 = SubjectList[index].Time2.Substring(0, 1);
                    if (day2 == "월" && TimeTableDB[period2, 1].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day2 == "화" && TimeTableDB[period2, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day2 == "수" && TimeTableDB[period2, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day2 == "목" && TimeTableDB[period2, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day2 == "금" && TimeTableDB[period2, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day2 == "토" && TimeTableDB[period2, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
                if (SubjectList[index].Time3 != "" && totalAbleToPut == true)
                {
                    period3 = Int32.Parse(SubjectList[index].Time3.Substring(1, SubjectList[index].Time3.Length - 1));
                    day3 = SubjectList[index].Time3.Substring(0, 1);
                    if (day3 == "월" && TimeTableDB[period3, 1].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day3 == "화" && TimeTableDB[period3, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day3 == "수" && TimeTableDB[period3, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day3 == "목" && TimeTableDB[period3, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day3 == "금" && TimeTableDB[period3, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day3 == "토" && TimeTableDB[period3, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
                if (SubjectList[index].Time4 != "" && totalAbleToPut == true)
                {
                    period4 = Int32.Parse(SubjectList[index].Time4.Substring(1, SubjectList[index].Time4.Length - 1));
                    day4 = SubjectList[index].Time4.Substring(0, 1);
                    if (day4 == "월" && TimeTableDB[period4, 1].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day4 == "화" && TimeTableDB[period4, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day4 == "수" && TimeTableDB[period4, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day4 == "목" && TimeTableDB[period4, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day4 == "금" && TimeTableDB[period4, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day4 == "토" && TimeTableDB[period4, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
                if (SubjectList[index].Time5 != "" && totalAbleToPut == true)
                {
                    period5 = Int32.Parse(SubjectList[index].Time5.Substring(1, SubjectList[index].Time5.Length - 1));
                    day5 = SubjectList[index].Time5.Substring(0, 1);
                    if (day5 == "월" && TimeTableDB[period5, 1].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day5 == "화" && TimeTableDB[period5, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day5 == "수" && TimeTableDB[period5, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day5 == "목" && TimeTableDB[period5, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day5 == "금" && TimeTableDB[period5, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day5 == "토" && TimeTableDB[period5, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
                if (SubjectList[index].Time6 != "" && totalAbleToPut == true)
                {
                    period6 = Int32.Parse(SubjectList[index].Time6.Substring(1, SubjectList[index].Time6.Length - 1));
                    day6 = SubjectList[index].Time6.Substring(0, 1);
                    if (day6 == "월" && TimeTableDB[period6, 1].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day6 == "화" && TimeTableDB[period6, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day6 == "수" && TimeTableDB[period6, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day6 == "목" && TimeTableDB[period6, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day6 == "금" && TimeTableDB[period6, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day6 == "토" && TimeTableDB[period6, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
                if (SubjectList[index].Time7 != "" && totalAbleToPut == true)
                {
                    period7 = Int32.Parse(SubjectList[index].Time7.Substring(1, SubjectList[index].Time7.Length - 1));
                    day7 = SubjectList[index].Time7.Substring(0, 1);
                    if (day7 == "월" && TimeTableDB[period7, 1].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day7 == "화" && TimeTableDB[period7, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day7 == "수" && TimeTableDB[period7, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day7 == "목" && TimeTableDB[period7, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day7 == "금" && TimeTableDB[period7, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day7 == "토" && TimeTableDB[period7, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
                if (SubjectList[index].Time8 != "" && totalAbleToPut == true)
                {
                    period8 = Int32.Parse(SubjectList[index].Time8.Substring(1, SubjectList[index].Time8.Length - 1));
                    day8 = SubjectList[index].Time8.Substring(0, 1);
                    if (day8 == "월" && TimeTableDB[period8, 1].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day8 == "화" && TimeTableDB[period8, 2].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day8 == "수" && TimeTableDB[period8, 3].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day8 == "목" && TimeTableDB[period8, 4].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day8 == "금" && TimeTableDB[period8, 5].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else if (day8 == "토" && TimeTableDB[period8, 6].ableToPut == true)
                    {
                        totalAbleToPut = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }
            }

            if (totalAbleToPut == true) //과목을 넣을 수 있다면
            {
                UsersSubjectsList.Add(new UsersSubject()
                {
                    NO = SubjectList[index].NO,
                    Grade = SubjectList[index].Grade,
                    ClassNumber = SubjectList[index].ClassNumber,
                    ClassName = SubjectList[index].ClassName,
                    CreditCourse = SubjectList[index].CreditCourse,
                    Professor = SubjectList[index].Professor,
                    강의시간 = SubjectList[index].강의시간,
                    Time1 = SubjectList[index].Time1,
                    Time2 = SubjectList[index].Time2,
                    Time3 = SubjectList[index].Time3,
                    Time4 = SubjectList[index].Time4,
                    Time5 = SubjectList[index].Time5,
                    Time6 = SubjectList[index].Time6,
                    Time7 = SubjectList[index].Time7,
                    Time8 = SubjectList[index].Time8,
                    UserName = "User",
                    NumOfTimeTable = 1,
                }); //과목추가
                RefreshTimeTable();
            }
        }
        private string TimeInList(List<UsersSubject> _UsersSubjectList) //유저가 듣는 시간을 string으로
        {
            string result = null;
            for (int i = 0; i < _UsersSubjectList.Count; i++)
            {
                if (_UsersSubjectList[i].Time1 != "")
                {
                    result += _UsersSubjectList[i].Time1;
                    result += "&";
                }
                if (_UsersSubjectList[i].Time2 != "")
                {

                    result += _UsersSubjectList[i].Time2;
                    result += "&";
                }
                if (_UsersSubjectList[i].Time3 != "")
                {

                    result += _UsersSubjectList[i].Time3;
                    result += "&";
                }
                if (_UsersSubjectList[i].Time4 != "")
                {

                    result += _UsersSubjectList[i].Time4;
                    result += "&";
                }
                if (_UsersSubjectList[i].Time5 != "")
                {

                    result += _UsersSubjectList[i].Time5;
                    result += "&";
                }
                if (_UsersSubjectList[i].Time6 != "")
                {

                    result += _UsersSubjectList[i].Time6;
                    result += "&";
                }
                if (_UsersSubjectList[i].Time7 != "")
                {

                    result += _UsersSubjectList[i].Time7;
                    result += "&";
                }
                if (_UsersSubjectList[i].Time8 != "")
                {

                    result += _UsersSubjectList[i].Time8;
                    result += "&";
                }
            }
            return result;
        }
        private string SubjectInList(List<UsersSubject> _UsersSubjectList) //유저가 듣는 과목을 string으로
        {
            string result = null;
            for (int i = 0; i < _UsersSubjectList.Count; i++)
            {
                result += _UsersSubjectList[i].ClassName;
                result += "&";
            }
            return result;
        }
        private void InitDB() //TimeTable 초기화
        {
            for (int _row = 0; _row < 13; _row++)
                for (int _col = 0; _col < 7; _col++)
                {
                    TimeTableDB[_row, _col].className = null;
                    TimeTableDB[_row, _col].professor = null;
                    TimeTableDB[_row, _col].ableToPut = true;
                }
        }
        private void RefreshTimeTable() //시간표 갱신
        {
            Type brushesType = typeof(Brushes);
            PropertyInfo[] properties = brushesType.GetProperties();
            int period1 = 0;
            string day1 = null;
            int period2 = 0; ;
            string day2 = null;
            int period3 = 0; ;
            string day3 = null;
            int period4 = 0; ;
            string day4 = null;
            int period5 = 0; ;
            string day5 = null;
            int period6 = 0; ;
            string day6 = null;
            int period7 = 0; ;
            string day7 = null;
            int period8 = 0; ;
            string day8 = null;
            InitDB();  //초기화
            //이 이하로는 다 하얗게 만들기
            mon1.Text = string.Empty;
            mon1.Background = Brushes.White;
            mon2.Text = string.Empty;
            mon2.Background = Brushes.White;
            mon3.Text = string.Empty;
            mon3.Background = Brushes.White;
            mon4.Text = string.Empty;
            mon4.Background = Brushes.White;
            mon5.Text = string.Empty;
            mon5.Background = Brushes.White;
            mon6.Text = string.Empty;
            mon6.Background = Brushes.White;
            mon7.Text = string.Empty;
            mon7.Background = Brushes.White;
            mon8.Text = string.Empty;
            mon8.Background = Brushes.White;
            mon9.Text = string.Empty;
            mon9.Background = Brushes.White;
            mon10.Text = string.Empty;
            mon10.Background = Brushes.White;
            mon11.Text = string.Empty;
            mon11.Background = Brushes.White;
            mon12.Text = string.Empty;
            mon12.Background = Brushes.White;
            tue1.Text = string.Empty;
            tue1.Background = Brushes.White;
            tue2.Text = string.Empty;
            tue2.Background = Brushes.White;
            tue3.Text = string.Empty;
            tue3.Background = Brushes.White;
            tue4.Text = string.Empty;
            tue4.Background = Brushes.White;
            tue5.Text = string.Empty;
            tue5.Background = Brushes.White;
            tue6.Text = string.Empty;
            tue6.Background = Brushes.White;
            tue7.Text = string.Empty;
            tue7.Background = Brushes.White;
            tue8.Text = string.Empty;
            tue8.Background = Brushes.White;
            tue9.Text = string.Empty;
            tue9.Background = Brushes.White;
            tue10.Text = string.Empty;
            tue10.Background = Brushes.White;
            tue11.Text = string.Empty;
            tue11.Background = Brushes.White;
            tue12.Text = string.Empty;
            tue12.Background = Brushes.White;
            wed1.Text = string.Empty;
            wed1.Background = Brushes.White;
            wed2.Text = string.Empty;
            wed2.Background = Brushes.White;
            wed3.Text = string.Empty;
            wed3.Background = Brushes.White;
            wed4.Text = string.Empty;
            wed4.Background = Brushes.White;
            wed5.Text = string.Empty;
            wed5.Background = Brushes.White;
            wed6.Text = string.Empty;
            wed6.Background = Brushes.White;
            wed7.Text = string.Empty;
            wed7.Background = Brushes.White;
            wed8.Text = string.Empty;
            wed8.Background = Brushes.White;
            wed9.Text = string.Empty;
            wed9.Background = Brushes.White;
            wed10.Text = string.Empty;
            wed10.Background = Brushes.White;
            wed11.Text = string.Empty;
            wed11.Background = Brushes.White;
            wed12.Text = string.Empty;
            wed12.Background = Brushes.White;
            thu1.Text = string.Empty;
            thu1.Background = Brushes.White;
            thu2.Text = string.Empty;
            thu2.Background = Brushes.White;
            thu3.Text = string.Empty;
            thu3.Background = Brushes.White;
            thu4.Text = string.Empty;
            thu4.Background = Brushes.White;
            thu5.Text = string.Empty;
            thu5.Background = Brushes.White;
            thu6.Text = string.Empty;
            thu6.Background = Brushes.White;
            thu7.Text = string.Empty;
            thu7.Background = Brushes.White;
            thu8.Text = string.Empty;
            thu8.Background = Brushes.White;
            thu9.Text = string.Empty;
            thu9.Background = Brushes.White;
            thu10.Text = string.Empty;
            thu10.Background = Brushes.White;
            thu11.Text = string.Empty;
            thu11.Background = Brushes.White;
            thu12.Text = string.Empty;
            thu12.Background = Brushes.White;
            fri1.Text = string.Empty;
            fri1.Background = Brushes.White;
            fri2.Text = string.Empty;
            fri2.Background = Brushes.White;
            fri3.Text = string.Empty;
            fri3.Background = Brushes.White;
            fri4.Text = string.Empty;
            fri4.Background = Brushes.White;
            fri5.Text = string.Empty;
            fri5.Background = Brushes.White;
            fri6.Text = string.Empty;
            fri6.Background = Brushes.White;
            fri7.Text = string.Empty;
            fri7.Background = Brushes.White;
            fri8.Text = string.Empty;
            fri8.Background = Brushes.White;
            fri9.Text = string.Empty;
            fri9.Background = Brushes.White;
            fri10.Text = string.Empty;
            fri10.Background = Brushes.White;
            fri11.Text = string.Empty;
            fri11.Background = Brushes.White;
            fri12.Text = string.Empty;
            fri12.Background = Brushes.White;
            sat1.Text = string.Empty;
            sat1.Background = Brushes.White;
            sat2.Text = string.Empty;
            sat2.Background = Brushes.White;
            sat3.Text = string.Empty;
            sat3.Background = Brushes.White;
            sat4.Text = string.Empty;
            sat4.Background = Brushes.White;
            sat5.Text = string.Empty;
            sat5.Background = Brushes.White;
            sat6.Text = string.Empty;
            sat6.Background = Brushes.White;
            sat7.Text = string.Empty;
            sat7.Background = Brushes.White;
            sat8.Text = string.Empty;
            sat8.Background = Brushes.White;
            sat9.Text = string.Empty;
            sat9.Background = Brushes.White;
            sat10.Text = string.Empty;
            sat10.Background = Brushes.White;
            sat11.Text = string.Empty;
            sat11.Background = Brushes.White;
            sat12.Text = string.Empty;
            sat12.Background = Brushes.White;
            //여기부터는 User의 과목들을 색칠, 2차원 배열에 넣기
            for (int i = 0; i < UsersSubjectsList.Count(); i++)
            {
                if (UsersSubjectsList[i].Time1 != "")
                {
                    period1 = Int32.Parse(UsersSubjectsList[i].Time1.Substring(1, UsersSubjectsList[i].Time1.Length - 1)); //time1의 교시 뽑기
                    day1 = UsersSubjectsList[i].Time1.Substring(0, 1); //time1의 요일 뽑기
                    if (day1 == "월")
                    {
                        if (period1 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day1 == "화")
                    {
                        if (period1 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day1 == "수")
                    {
                        if (period1 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day1 == "목")
                    {
                        if (period1 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day1 == "금")
                    {
                        if (period1 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day1 == "토")
                    {
                        if (period1 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period1 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                }
                if (UsersSubjectsList[i].Time2 != "")
                {
                    period2 = Int32.Parse(UsersSubjectsList[i].Time2.Substring(1, UsersSubjectsList[i].Time2.Length - 1));
                    day2 = UsersSubjectsList[i].Time2.Substring(0, 1);
                    if (day2 == "월")
                    {
                        if (period2 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day2 == "화")
                    {
                        if (period2 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day2 == "수")
                    {
                        if (period2 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day2 == "목")
                    {
                        if (period2 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day2 == "금")
                    {
                        if (period2 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day2 == "토")
                    {
                        if (period2 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period2 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                }
                if (UsersSubjectsList[i].Time3 != "")
                {
                    period3 = Int32.Parse(UsersSubjectsList[i].Time3.Substring(1, UsersSubjectsList[i].Time3.Length - 1));
                    day3 = UsersSubjectsList[i].Time3.Substring(0, 1);
                    if (day3 == "월")
                    {
                        if (period3 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day3 == "화")
                    {
                        if (period3 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day3 == "수")
                    {
                        if (period3 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day3 == "목")
                    {
                        if (period3 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day3 == "금")
                    {
                        if (period3 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day3 == "토")
                    {
                        if (period3 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period3 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }

                }
                if (UsersSubjectsList[i].Time4 != "")
                {
                    period4 = Int32.Parse(UsersSubjectsList[i].Time4.Substring(1, UsersSubjectsList[i].Time4.Length - 1));
                    day4 = UsersSubjectsList[i].Time4.Substring(0, 1);
                    if (day4 == "월")
                    {
                        if (period4 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day4 == "화")
                    {
                        if (period4 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day4 == "수")
                    {
                        if (period4 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day4 == "목")
                    {
                        if (period4 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day4 == "금")
                    {
                        if (period4 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day4 == "토")
                    {
                        if (period4 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period4 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                }
                if (UsersSubjectsList[i].Time5 != "")
                {
                    period5 = Int32.Parse(UsersSubjectsList[i].Time5.Substring(1, UsersSubjectsList[i].Time5.Length - 1));
                    day5 = UsersSubjectsList[i].Time5.Substring(0, 1);
                    if (day5 == "월")
                    {
                        if (period5 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day5 == "화")
                    {
                        if (period5 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day5 == "수")
                    {
                        if (period5 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day5 == "목")
                    {
                        if (period5 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day5 == "금")
                    {
                        if (period5 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day5 == "토")
                    {
                        if (period5 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period5 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                }
                if (UsersSubjectsList[i].Time6 != "")
                {
                    period6 = Int32.Parse(UsersSubjectsList[i].Time6.Substring(1, UsersSubjectsList[i].Time6.Length - 1));
                    day6 = UsersSubjectsList[i].Time6.Substring(0, 1);
                    if (day6 == "월")
                    {
                        if (period6 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day6 == "화")
                    {
                        if (period6 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day6 == "수")
                    {
                        if (period6 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day6 == "목")
                    {
                        if (period6 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day6 == "금")
                    {
                        if (period6 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day6 == "토")
                    {
                        if (period6 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period6 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                }
                if (UsersSubjectsList[i].Time7 != "")
                {
                    period7 = Int32.Parse(UsersSubjectsList[i].Time7.Substring(1, UsersSubjectsList[i].Time7.Length - 1));
                    day7 = UsersSubjectsList[i].Time7.Substring(0, 1);
                    if (day7 == "월")
                    {
                        if (period7 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day7 == "화")
                    {
                        if (period7 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day7 == "수")
                    {
                        if (period7 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day7 == "목")
                    {
                        if (period7 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day7 == "금")
                    {
                        if (period7 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day7 == "토")
                    {
                        if (period7 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period7 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                }
                if (UsersSubjectsList[i].Time8 != "")
                {
                    period8 = Int32.Parse(UsersSubjectsList[i].Time8.Substring(1, UsersSubjectsList[i].Time8.Length - 1));
                    day8 = UsersSubjectsList[i].Time8.Substring(0, 1);
                    if (day8 == "월")
                    {
                        if (period8 == 1)
                        {
                            TimeTableDB[1, 1].ableToPut = false;
                            TimeTableDB[1, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 1].professor);
                            professor.FontSize = 10;
                            mon1.Inlines.Add(professor);
                            mon1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 2)
                        {
                            TimeTableDB[2, 1].ableToPut = false;
                            TimeTableDB[2, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 1].professor);
                            professor.FontSize = 10;
                            mon2.Inlines.Add(professor);
                            mon2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 3)
                        {
                            TimeTableDB[3, 1].ableToPut = false;
                            TimeTableDB[3, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 1].professor);
                            professor.FontSize = 10;
                            mon3.Inlines.Add(professor);
                            mon3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 4)
                        {
                            TimeTableDB[4, 1].ableToPut = false;
                            TimeTableDB[4, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 1].professor);
                            professor.FontSize = 10;
                            mon4.Inlines.Add(professor);
                            mon4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 5)
                        {
                            TimeTableDB[5, 1].ableToPut = false;
                            TimeTableDB[5, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 1].professor);
                            professor.FontSize = 10;
                            mon5.Inlines.Add(professor);
                            mon5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 6)
                        {
                            TimeTableDB[6, 1].ableToPut = false;
                            TimeTableDB[6, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 1].professor);
                            professor.FontSize = 10;
                            mon6.Inlines.Add(professor);
                            mon6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 7)
                        {
                            TimeTableDB[7, 1].ableToPut = false;
                            TimeTableDB[7, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 1].professor);
                            professor.FontSize = 10;
                            mon7.Inlines.Add(professor);
                            mon7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 8)
                        {
                            TimeTableDB[8, 1].ableToPut = false;
                            TimeTableDB[8, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 1].professor);
                            professor.FontSize = 10;
                            mon8.Inlines.Add(professor);
                            mon8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 9)
                        {
                            TimeTableDB[9, 1].ableToPut = false;
                            TimeTableDB[9, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 1].professor);
                            professor.FontSize = 10;
                            mon9.Inlines.Add(professor);
                            mon9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 10)
                        {
                            TimeTableDB[10, 1].ableToPut = false;
                            TimeTableDB[10, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 1].professor);
                            professor.FontSize = 10;
                            mon10.Inlines.Add(professor);
                            mon10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 11)
                        {
                            TimeTableDB[11, 1].ableToPut = false;
                            TimeTableDB[11, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 1].professor);
                            professor.FontSize = 10;
                            mon11.Inlines.Add(professor);
                            mon11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 12)
                        {
                            TimeTableDB[12, 1].ableToPut = false;
                            TimeTableDB[12, 1].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 1].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 1].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            mon12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 1].professor);
                            professor.FontSize = 10;
                            mon12.Inlines.Add(professor);
                            mon12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day8 == "화")
                    {
                        if (period8 == 1)
                        {
                            TimeTableDB[1, 2].ableToPut = false;
                            TimeTableDB[1, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 2].professor);
                            professor.FontSize = 10;
                            tue1.Inlines.Add(professor);
                            tue1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 2)
                        {
                            TimeTableDB[2, 2].ableToPut = false;
                            TimeTableDB[2, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 2].professor);
                            professor.FontSize = 10;
                            tue2.Inlines.Add(professor);
                            tue2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 3)
                        {
                            TimeTableDB[3, 2].ableToPut = false;
                            TimeTableDB[3, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 2].professor);
                            professor.FontSize = 10;
                            tue3.Inlines.Add(professor);
                            tue3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 4)
                        {
                            TimeTableDB[4, 2].ableToPut = false;
                            TimeTableDB[4, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 2].professor);
                            professor.FontSize = 10;
                            tue4.Inlines.Add(professor);
                            tue4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 5)
                        {
                            TimeTableDB[5, 2].ableToPut = false;
                            TimeTableDB[5, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 2].professor);
                            professor.FontSize = 10;
                            tue5.Inlines.Add(professor);
                            tue5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 6)
                        {
                            TimeTableDB[6, 2].ableToPut = false;
                            TimeTableDB[6, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 2].professor);
                            professor.FontSize = 10;
                            tue6.Inlines.Add(professor);
                            tue6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 7)
                        {
                            TimeTableDB[7, 2].ableToPut = false;
                            TimeTableDB[7, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 2].professor);
                            professor.FontSize = 10;
                            tue7.Inlines.Add(professor);
                            tue7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 8)
                        {
                            TimeTableDB[8, 2].ableToPut = false;
                            TimeTableDB[8, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 2].professor);
                            professor.FontSize = 10;
                            tue8.Inlines.Add(professor);
                            tue8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 9)
                        {
                            TimeTableDB[9, 2].ableToPut = false;
                            TimeTableDB[9, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 2].professor);
                            professor.FontSize = 10;
                            tue9.Inlines.Add(professor);
                            tue9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 10)
                        {
                            TimeTableDB[10, 2].ableToPut = false;
                            TimeTableDB[10, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 2].professor);
                            professor.FontSize = 10;
                            tue10.Inlines.Add(professor);
                            tue10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 11)
                        {
                            TimeTableDB[11, 2].ableToPut = false;
                            TimeTableDB[11, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 2].professor);
                            professor.FontSize = 10;
                            tue11.Inlines.Add(professor);
                            tue11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 12)
                        {
                            TimeTableDB[12, 2].ableToPut = false;
                            TimeTableDB[12, 2].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 2].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 2].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            tue12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 2].professor);
                            professor.FontSize = 10;
                            tue12.Inlines.Add(professor);
                            tue12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day8 == "수")
                    {
                        if (period8 == 1)
                        {
                            TimeTableDB[1, 3].ableToPut = false;
                            TimeTableDB[1, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 3].professor);
                            professor.FontSize = 10;
                            wed1.Inlines.Add(professor);
                            wed1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 2)
                        {
                            TimeTableDB[2, 3].ableToPut = false;
                            TimeTableDB[2, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 3].professor);
                            professor.FontSize = 10;
                            wed2.Inlines.Add(professor);
                            wed2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 3)
                        {
                            TimeTableDB[3, 3].ableToPut = false;
                            TimeTableDB[3, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 3].professor);
                            professor.FontSize = 10;
                            wed3.Inlines.Add(professor);
                            wed3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 4)
                        {
                            TimeTableDB[4, 3].ableToPut = false;
                            TimeTableDB[4, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 3].professor);
                            professor.FontSize = 10;
                            wed4.Inlines.Add(professor);
                            wed4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 5)
                        {
                            TimeTableDB[5, 3].ableToPut = false;
                            TimeTableDB[5, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 3].professor);
                            professor.FontSize = 10;
                            wed5.Inlines.Add(professor);
                            wed5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 6)
                        {
                            TimeTableDB[6, 3].ableToPut = false;
                            TimeTableDB[6, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 3].professor);
                            professor.FontSize = 10;
                            wed6.Inlines.Add(professor);
                            wed6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 7)
                        {
                            TimeTableDB[7, 3].ableToPut = false;
                            TimeTableDB[7, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 3].professor);
                            professor.FontSize = 10;
                            wed7.Inlines.Add(professor);
                            wed7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 8)
                        {
                            TimeTableDB[8, 3].ableToPut = false;
                            TimeTableDB[8, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 3].professor);
                            professor.FontSize = 10;
                            wed8.Inlines.Add(professor);
                            wed8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 9)
                        {
                            TimeTableDB[9, 3].ableToPut = false;
                            TimeTableDB[9, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 3].professor);
                            professor.FontSize = 10;
                            wed9.Inlines.Add(professor);
                            wed9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 10)
                        {
                            TimeTableDB[10, 3].ableToPut = false;
                            TimeTableDB[10, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 3].professor);
                            professor.FontSize = 10;
                            wed10.Inlines.Add(professor);
                            wed10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 11)
                        {
                            TimeTableDB[11, 3].ableToPut = false;
                            TimeTableDB[11, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 3].professor);
                            professor.FontSize = 10;
                            wed11.Inlines.Add(professor);
                            wed11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 12)
                        {
                            TimeTableDB[12, 3].ableToPut = false;
                            TimeTableDB[12, 3].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 3].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 3].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            wed12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 3].professor);
                            professor.FontSize = 10;
                            wed12.Inlines.Add(professor);
                            wed12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day8 == "목")
                    {
                        if (period8 == 1)
                        {
                            TimeTableDB[1, 4].ableToPut = false;
                            TimeTableDB[1, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 4].professor);
                            professor.FontSize = 10;
                            thu1.Inlines.Add(professor);
                            thu1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 2)
                        {
                            TimeTableDB[2, 4].ableToPut = false;
                            TimeTableDB[2, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 4].professor);
                            professor.FontSize = 10;
                            thu2.Inlines.Add(professor);
                            thu2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 3)
                        {
                            TimeTableDB[3, 4].ableToPut = false;
                            TimeTableDB[3, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 4].professor);
                            professor.FontSize = 10;
                            thu3.Inlines.Add(professor);
                            thu3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 4)
                        {
                            TimeTableDB[4, 4].ableToPut = false;
                            TimeTableDB[4, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 4].professor);
                            professor.FontSize = 10;
                            thu4.Inlines.Add(professor);
                            thu4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 5)
                        {
                            TimeTableDB[5, 4].ableToPut = false;
                            TimeTableDB[5, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 4].professor);
                            professor.FontSize = 10;
                            thu5.Inlines.Add(professor);
                            thu5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 6)
                        {
                            TimeTableDB[6, 4].ableToPut = false;
                            TimeTableDB[6, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 4].professor);
                            professor.FontSize = 10;
                            thu6.Inlines.Add(professor);
                            thu6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 7)
                        {
                            TimeTableDB[7, 4].ableToPut = false;
                            TimeTableDB[7, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 4].professor);
                            professor.FontSize = 10;
                            thu7.Inlines.Add(professor);
                            thu7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 8)
                        {
                            TimeTableDB[8, 4].ableToPut = false;
                            TimeTableDB[8, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 4].professor);
                            professor.FontSize = 10;
                            thu8.Inlines.Add(professor);
                            thu8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 9)
                        {
                            TimeTableDB[9, 4].ableToPut = false;
                            TimeTableDB[9, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 4].professor);
                            professor.FontSize = 10;
                            thu9.Inlines.Add(professor);
                            thu9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 10)
                        {
                            TimeTableDB[10, 4].ableToPut = false;
                            TimeTableDB[10, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 4].professor);
                            professor.FontSize = 10;
                            thu10.Inlines.Add(professor);
                            thu10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 11)
                        {
                            TimeTableDB[11, 4].ableToPut = false;
                            TimeTableDB[11, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 4].professor);
                            professor.FontSize = 10;
                            thu11.Inlines.Add(professor);
                            thu11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 12)
                        {
                            TimeTableDB[12, 4].ableToPut = false;
                            TimeTableDB[12, 4].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 4].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 4].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            thu12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 4].professor);
                            professor.FontSize = 10;
                            thu12.Inlines.Add(professor);
                            thu12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day8 == "금")
                    {
                        if (period8 == 1)
                        {
                            TimeTableDB[1, 5].ableToPut = false;
                            TimeTableDB[1, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 5].professor);
                            professor.FontSize = 10;
                            fri1.Inlines.Add(professor);
                            fri1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 2)
                        {
                            TimeTableDB[2, 5].ableToPut = false;
                            TimeTableDB[2, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 5].professor);
                            professor.FontSize = 10;
                            fri2.Inlines.Add(professor);
                            fri2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 3)
                        {
                            TimeTableDB[3, 5].ableToPut = false;
                            TimeTableDB[3, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 5].professor);
                            professor.FontSize = 10;
                            fri3.Inlines.Add(professor);
                            fri3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 4)
                        {
                            TimeTableDB[4, 5].ableToPut = false;
                            TimeTableDB[4, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 5].professor);
                            professor.FontSize = 10;
                            fri4.Inlines.Add(professor);
                            fri4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 5)
                        {
                            TimeTableDB[5, 5].ableToPut = false;
                            TimeTableDB[5, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 5].professor);
                            professor.FontSize = 10;
                            fri5.Inlines.Add(professor);
                            fri5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 6)
                        {
                            TimeTableDB[6, 5].ableToPut = false;
                            TimeTableDB[6, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 5].professor);
                            professor.FontSize = 10;
                            fri6.Inlines.Add(professor);
                            fri6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 7)
                        {
                            TimeTableDB[7, 5].ableToPut = false;
                            TimeTableDB[7, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 5].professor);
                            professor.FontSize = 10;
                            fri7.Inlines.Add(professor);
                            fri7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 8)
                        {
                            TimeTableDB[8, 5].ableToPut = false;
                            TimeTableDB[8, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 5].professor);
                            professor.FontSize = 10;
                            fri8.Inlines.Add(professor);
                            fri8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 9)
                        {
                            TimeTableDB[9, 5].ableToPut = false;
                            TimeTableDB[9, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 5].professor);
                            professor.FontSize = 10;
                            fri9.Inlines.Add(professor);
                            fri9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 10)
                        {
                            TimeTableDB[10, 5].ableToPut = false;
                            TimeTableDB[10, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 5].professor);
                            professor.FontSize = 10;
                            fri10.Inlines.Add(professor);
                            fri10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 11)
                        {
                            TimeTableDB[11, 5].ableToPut = false;
                            TimeTableDB[11, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 5].professor);
                            professor.FontSize = 10;
                            fri11.Inlines.Add(professor);
                            fri11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 12)
                        {
                            TimeTableDB[12, 5].ableToPut = false;
                            TimeTableDB[12, 5].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 5].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 5].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            fri12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 5].professor);
                            professor.FontSize = 10;
                            fri12.Inlines.Add(professor);
                            fri12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                    else if (day8 == "토")
                    {
                        if (period8 == 1)
                        {
                            TimeTableDB[1, 6].ableToPut = false;
                            TimeTableDB[1, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[1, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[1, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat1.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[1, 6].professor);
                            professor.FontSize = 10;
                            sat1.Inlines.Add(professor);
                            sat1.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 2)
                        {
                            TimeTableDB[2, 6].ableToPut = false;
                            TimeTableDB[2, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[2, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[2, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat2.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[2, 6].professor);
                            professor.FontSize = 10;
                            sat2.Inlines.Add(professor);
                            sat2.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 3)
                        {
                            TimeTableDB[3, 6].ableToPut = false;
                            TimeTableDB[3, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[3, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[3, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat3.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[3, 6].professor);
                            professor.FontSize = 10;
                            sat3.Inlines.Add(professor);
                            sat3.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 4)
                        {
                            TimeTableDB[4, 6].ableToPut = false;
                            TimeTableDB[4, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[4, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[4, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat4.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[4, 6].professor);
                            professor.FontSize = 10;
                            sat4.Inlines.Add(professor);
                            sat4.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 5)
                        {
                            TimeTableDB[5, 6].ableToPut = false;
                            TimeTableDB[5, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[5, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[5, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat5.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[5, 6].professor);
                            professor.FontSize = 10;
                            sat5.Inlines.Add(professor);
                            sat5.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 6)
                        {
                            TimeTableDB[6, 6].ableToPut = false;
                            TimeTableDB[6, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[6, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[6, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat6.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[6, 6].professor);
                            professor.FontSize = 10;
                            sat6.Inlines.Add(professor);
                            sat6.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 7)
                        {
                            TimeTableDB[7, 6].ableToPut = false;
                            TimeTableDB[7, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[7, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[7, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat7.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[7, 6].professor);
                            professor.FontSize = 10;
                            sat7.Inlines.Add(professor);
                            sat7.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 8)
                        {
                            TimeTableDB[8, 6].ableToPut = false;
                            TimeTableDB[8, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[8, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[8, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat8.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[8, 6].professor);
                            professor.FontSize = 10;
                            sat8.Inlines.Add(professor);
                            sat8.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 9)
                        {
                            TimeTableDB[9, 6].ableToPut = false;
                            TimeTableDB[9, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[9, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[9, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat9.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[9, 6].professor);
                            professor.FontSize = 10;
                            sat9.Inlines.Add(professor);
                            sat9.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 10)
                        {
                            TimeTableDB[10, 6].ableToPut = false;
                            TimeTableDB[10, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[10, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[10, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat10.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[10, 6].professor);
                            professor.FontSize = 10;
                            sat10.Inlines.Add(professor);
                            sat10.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 11)
                        {
                            TimeTableDB[11, 6].ableToPut = false;
                            TimeTableDB[11, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[11, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[11, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat11.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[11, 6].professor);
                            professor.FontSize = 10;
                            sat11.Inlines.Add(professor);
                            sat11.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                        else if (period8 == 12)
                        {
                            TimeTableDB[12, 6].ableToPut = false;
                            TimeTableDB[12, 6].className = UsersSubjectsList[i].ClassName;
                            TimeTableDB[12, 6].professor = UsersSubjectsList[i].Professor;
                            Run className = new Run(TimeTableDB[12, 6].className + "\n");
                            className.FontSize = 12;
                            className.FontWeight = FontWeights.Bold;
                            sat12.Inlines.Add(className);
                            Run professor = new Run(TimeTableDB[12, 6].professor);
                            professor.FontSize = 10;
                            sat12.Inlines.Add(professor);
                            sat12.Background = (Brush)properties[UsersSubjectsList[i].NO%141].GetValue(null, null);
                        }
                    }
                }
            } //User의 과목들을 색칠, 2차원 배열에 넣기
        }
        private void DeleteSubjectInTimeTable(string _dayAndPeriod)//매개변수 시간에 있는 과목 삭제
        {
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time1 == _dayAndPeriod));
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time2 == _dayAndPeriod));
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time3 == _dayAndPeriod));
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time4 == _dayAndPeriod));
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time5 == _dayAndPeriod));
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time6 == _dayAndPeriod));
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time7 == _dayAndPeriod));
            UsersSubjectsList.Remove(UsersSubjectsList.Find(x => x.Time8 == _dayAndPeriod));
        }
        private void Option_btn_Click(object sender, RoutedEventArgs e) //onoff버튼 클릭하면
        {
            FilterOption FO = new FilterOption();
            FO.Show();
        }
        private void MyGroup_btn_Click(object sender, RoutedEventArgs e)//MyGroup을 누르면 검색창 없어지고, combobox만 뜸 
        {
            Search_Box.Visibility = Visibility.Collapsed;
            Search_Box.IsEnabled = false;
            Search_btn.Visibility = Visibility.Collapsed;
            Search_btn.IsEnabled = false;
            MyGroup_cob.Visibility = Visibility.Visible;
            MyGroup_cob.IsEnabled = true;
            MyGroup_cob.ItemsSource = data;
        }

        //이 이하로는 mon1만 대표로 주석을 달겠음

        private void mon1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) //mon1칸에 마우스가 들어갔을 때
        {
            if (TimeTableDB[1, 1].ableToPut == false) //안에 과목이 있으면 (과목이 없으면 x버튼이 보일 필요가 없어서)
                mon1_btn.Visibility = Visibility.Visible; //x버튼 보이게 만들기
        }
        private void mon1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) //mon1칸에서 마우스가 떠났을 때
        {
            mon1_btn.Visibility = Visibility.Collapsed; //x버튼 안보이게 만들기
        }
        private void mon1_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) //mon1의 x버튼에 마우스가 들어갔을 때
        {
            if (TimeTableDB[1, 1].ableToPut == false) //안에 과목이 있으면 (과목이 없으면 x버튼이 보일 필요가 없어서)
                mon1_btn.Visibility = Visibility.Visible; //x버튼 보이게 
        }
        private void mon1_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e) //x버튼 더블 클릭
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            //삭제하겠냐고 묻는 메세지창
            if (messageBoxResult == MessageBoxResult.Yes) //yes하면
            {
                DeleteSubjectInTimeTable("월1"); //찾아서 지우고               
                RefreshTimeTable(); //새로고침
            }
            mon1_btn.Visibility = Visibility.Collapsed;
        }
        private void mon1_btn_Click(object sender, RoutedEventArgs e) //x버튼 한번 클릭
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            //메세지창 띄우기
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월1");
                RefreshTimeTable(); //새로고침
            }
            mon1_btn.Visibility = Visibility.Collapsed;
        }

        private void mon2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 1].ableToPut == false)
                mon2_btn.Visibility = Visibility.Visible;
        }
        private void mon2_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon2_btn.Visibility = Visibility.Collapsed;
        }
        private void mon2_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 1].ableToPut == false)
                mon2_btn.Visibility = Visibility.Visible;
        }
        private void mon2_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월1");
                RefreshTimeTable();
            }
            mon2_btn.Visibility = Visibility.Collapsed;
        }
        private void mon2_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월2");
                RefreshTimeTable();
            }
            mon2_btn.Visibility = Visibility.Collapsed;
        }

        private void mon3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 1].ableToPut == false)
                mon3_btn.Visibility = Visibility.Visible;
        }
        private void mon3_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon3_btn.Visibility = Visibility.Collapsed;
        }
        private void mon3_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 1].ableToPut == false)
                mon3_btn.Visibility = Visibility.Visible;
        }
        private void mon3_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월3");
                RefreshTimeTable();
            }
            mon3_btn.Visibility = Visibility.Collapsed;
        }
        private void mon3_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월3");
                RefreshTimeTable();
            }
            mon3_btn.Visibility = Visibility.Collapsed;
        }

        private void mon4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 1].ableToPut == false)
                mon4_btn.Visibility = Visibility.Visible;
        }
        private void mon4_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon4_btn.Visibility = Visibility.Collapsed;
        }
        private void mon4_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 1].ableToPut == false)
                mon4_btn.Visibility = Visibility.Visible;
        }
        private void mon4_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월4");
                RefreshTimeTable();
            }
            mon4_btn.Visibility = Visibility.Collapsed;
        }
        private void mon4_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월4");
                RefreshTimeTable();
            }
            mon4_btn.Visibility = Visibility.Collapsed;
        }

        private void mon5_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 1].ableToPut == false)
                mon5_btn.Visibility = Visibility.Visible;
        }
        private void mon5_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon5_btn.Visibility = Visibility.Collapsed;
        }
        private void mon5_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 1].ableToPut == false)
                mon5_btn.Visibility = Visibility.Visible;
        }
        private void mon5_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월5");
                RefreshTimeTable();
            }
            mon5_btn.Visibility = Visibility.Collapsed;
        }
        private void mon5_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월5");
                RefreshTimeTable();
            }
            mon5_btn.Visibility = Visibility.Collapsed;
        }

        private void mon6_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 1].ableToPut == false)
                mon6_btn.Visibility = Visibility.Visible;
        }
        private void mon6_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon6_btn.Visibility = Visibility.Collapsed;
        }
        private void mon6_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 1].ableToPut == false)
                mon6_btn.Visibility = Visibility.Visible;
        }
        private void mon6_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월6");
                RefreshTimeTable();
            }
            mon6_btn.Visibility = Visibility.Collapsed;
        }
        private void mon6_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월6");
                RefreshTimeTable();
            }
            mon6_btn.Visibility = Visibility.Collapsed;
        }

        private void mon7_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 1].ableToPut == false)
                mon7_btn.Visibility = Visibility.Visible;
        }
        private void mon7_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon7_btn.Visibility = Visibility.Collapsed;
        }
        private void mon7_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 1].ableToPut == false)
                mon7_btn.Visibility = Visibility.Visible;
        }
        private void mon7_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월7");
                RefreshTimeTable();
            }
            mon7_btn.Visibility = Visibility.Collapsed;
        }
        private void mon7_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월7");
                RefreshTimeTable();
            }
            mon7_btn.Visibility = Visibility.Collapsed;
        }

        private void mon8_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 1].ableToPut == false)
                mon8_btn.Visibility = Visibility.Visible;
        }
        private void mon8_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon8_btn.Visibility = Visibility.Collapsed;
        }
        private void mon8_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 1].ableToPut == false)
                mon8_btn.Visibility = Visibility.Visible;
        }
        private void mon8_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월8");
                RefreshTimeTable();
            }
            mon8_btn.Visibility = Visibility.Collapsed;
        }
        private void mon8_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월8");
                RefreshTimeTable();
            }
            mon8_btn.Visibility = Visibility.Collapsed;
        }

        private void mon9_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 1].ableToPut == false)
                mon9_btn.Visibility = Visibility.Visible;
        }
        private void mon9_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon9_btn.Visibility = Visibility.Collapsed;
        }
        private void mon9_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 1].ableToPut == false)
                mon9_btn.Visibility = Visibility.Visible;
        }
        private void mon9_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월9");
                RefreshTimeTable();
            }
            mon9_btn.Visibility = Visibility.Collapsed;
        }
        private void mon9_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월9");
                RefreshTimeTable();
            }
            mon9_btn.Visibility = Visibility.Collapsed;
        }

        private void mon10_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 1].ableToPut == false)
                mon10_btn.Visibility = Visibility.Visible;
        }
        private void mon10_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon10_btn.Visibility = Visibility.Collapsed;
        }
        private void mon10_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 1].ableToPut == false)
                mon10_btn.Visibility = Visibility.Visible;
        }
        private void mon10_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월10");
                RefreshTimeTable();
            }
            mon10_btn.Visibility = Visibility.Collapsed;
        }
        private void mon10_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월10");
                RefreshTimeTable();
            }
            mon10_btn.Visibility = Visibility.Collapsed;
        }

        private void mon11_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 1].ableToPut == false)
                mon11_btn.Visibility = Visibility.Visible;
        }
        private void mon11_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon11_btn.Visibility = Visibility.Collapsed;
        }
        private void mon11_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 1].ableToPut == false)
                mon11_btn.Visibility = Visibility.Visible;
        }
        private void mon11_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월11");
                RefreshTimeTable();
            }
            mon11_btn.Visibility = Visibility.Collapsed;
        }
        private void mon11_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월11");
                RefreshTimeTable();
            }
            mon11_btn.Visibility = Visibility.Collapsed;
        }

        private void mon12_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 1].ableToPut == false)
                mon12_btn.Visibility = Visibility.Visible;
        }
        private void mon12_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mon12_btn.Visibility = Visibility.Collapsed;
        }
        private void mon12_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 1].ableToPut == false)
                mon12_btn.Visibility = Visibility.Visible;
        }
        private void mon12_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월12");
                RefreshTimeTable();
            }
            mon12_btn.Visibility = Visibility.Collapsed;
        }
        private void mon12_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 1].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("월12");
                RefreshTimeTable();
            }
            mon12_btn.Visibility = Visibility.Collapsed;
        }

        private void tue1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 2].ableToPut == false)
                tue1_btn.Visibility = Visibility.Visible;
        }
        private void tue1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue1_btn.Visibility = Visibility.Collapsed;
        }
        private void tue1_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 2].ableToPut == false)
                tue1_btn.Visibility = Visibility.Visible;
        }
        private void tue1_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화1");
                RefreshTimeTable();
            }
            tue1_btn.Visibility = Visibility.Collapsed;
        }
        private void tue1_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화1");
                RefreshTimeTable();
            }
            tue1_btn.Visibility = Visibility.Collapsed;
        }

        private void tue2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 2].ableToPut == false)
                tue2_btn.Visibility = Visibility.Visible;
        }
        private void tue2_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue2_btn.Visibility = Visibility.Collapsed;
        }
        private void tue2_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 2].ableToPut == false)
                tue2_btn.Visibility = Visibility.Visible;
        }
        private void tue2_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화2");
                RefreshTimeTable();
            }
            tue2_btn.Visibility = Visibility.Collapsed;
        }
        private void tue2_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화2");
                RefreshTimeTable();
            }
            tue2_btn.Visibility = Visibility.Collapsed;
        }

        private void tue3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 2].ableToPut == false)
                tue3_btn.Visibility = Visibility.Visible;
        }
        private void tue3_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue3_btn.Visibility = Visibility.Collapsed;
        }
        private void tue3_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 2].ableToPut == false)
                tue3_btn.Visibility = Visibility.Visible;
        }
        private void tue3_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화3");
                RefreshTimeTable();
            }
            tue3_btn.Visibility = Visibility.Collapsed;
        }
        private void tue3_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화3");
                RefreshTimeTable();
            }
            tue3_btn.Visibility = Visibility.Collapsed;
        }

        private void tue4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 2].ableToPut == false)
                tue4_btn.Visibility = Visibility.Visible;
        }
        private void tue4_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue4_btn.Visibility = Visibility.Collapsed;
        }
        private void tue4_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 2].ableToPut == false)
                tue4_btn.Visibility = Visibility.Visible;
        }
        private void tue4_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화4");
                RefreshTimeTable();
            }
            tue4_btn.Visibility = Visibility.Collapsed;
        }
        private void tue4_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화4");
                RefreshTimeTable();
            }
            tue4_btn.Visibility = Visibility.Collapsed;
        }

        private void tue5_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 2].ableToPut == false)
                tue5_btn.Visibility = Visibility.Visible;
        }
        private void tue5_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue5_btn.Visibility = Visibility.Collapsed;
        }
        private void tue5_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 2].ableToPut == false)
                tue5_btn.Visibility = Visibility.Visible;
        }
        private void tue5_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화5");
                RefreshTimeTable();
            }
            tue5_btn.Visibility = Visibility.Collapsed;
        }
        private void tue5_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화5");
                RefreshTimeTable();
            }
            tue5_btn.Visibility = Visibility.Collapsed;
        }

        private void tue6_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 2].ableToPut == false)
                tue6_btn.Visibility = Visibility.Visible;
        }
        private void tue6_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue6_btn.Visibility = Visibility.Collapsed;
        }
        private void tue6_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 2].ableToPut == false)
                tue6_btn.Visibility = Visibility.Visible;
        }
        private void tue6_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화6");
                RefreshTimeTable();
            }
            tue6_btn.Visibility = Visibility.Collapsed;
        }
        private void tue6_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화6");
                RefreshTimeTable();
            }
            tue6_btn.Visibility = Visibility.Collapsed;
        }

        private void tue7_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 2].ableToPut == false)
                tue7_btn.Visibility = Visibility.Visible;
        }
        private void tue7_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue7_btn.Visibility = Visibility.Collapsed;
        }
        private void tue7_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 2].ableToPut == false)
                tue7_btn.Visibility = Visibility.Visible;
        }
        private void tue7_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화7");
                RefreshTimeTable();
            }
            tue7_btn.Visibility = Visibility.Collapsed;
        }
        private void tue7_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화7");
                RefreshTimeTable();
            }
            tue7_btn.Visibility = Visibility.Collapsed;
        }

        private void tue8_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 2].ableToPut == false)
                tue8_btn.Visibility = Visibility.Visible;
        }
        private void tue8_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue8_btn.Visibility = Visibility.Collapsed;
        }
        private void tue8_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 2].ableToPut == false)
                tue8_btn.Visibility = Visibility.Visible;
        }
        private void tue8_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화8");
                RefreshTimeTable();
            }
            tue8_btn.Visibility = Visibility.Collapsed;
        }
        private void tue8_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화8");
                RefreshTimeTable();
            }
            tue8_btn.Visibility = Visibility.Collapsed;
        }

        private void tue9_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 2].ableToPut == false)
                tue9_btn.Visibility = Visibility.Visible;
        }
        private void tue9_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue9_btn.Visibility = Visibility.Collapsed;
        }
        private void tue9_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 2].ableToPut == false)
                tue9_btn.Visibility = Visibility.Visible;
        }
        private void tue9_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화9");
                RefreshTimeTable();
            }
            tue9_btn.Visibility = Visibility.Collapsed;
        }
        private void tue9_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화9");
                RefreshTimeTable();
            }
            tue9_btn.Visibility = Visibility.Collapsed;
        }

        private void tue10_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 2].ableToPut == false)
                tue10_btn.Visibility = Visibility.Visible;
        }
        private void tue10_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue10_btn.Visibility = Visibility.Collapsed;
        }
        private void tue10_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 2].ableToPut == false)
                tue10_btn.Visibility = Visibility.Visible;
        }
        private void tue10_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화10");
                RefreshTimeTable();
            }
            tue10_btn.Visibility = Visibility.Collapsed;
        }
        private void tue10_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화10");
                RefreshTimeTable();
            }
            tue10_btn.Visibility = Visibility.Collapsed;
        }

        private void tue11_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 2].ableToPut == false)
                tue11_btn.Visibility = Visibility.Visible;
        }
        private void tue11_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue11_btn.Visibility = Visibility.Collapsed;
        }
        private void tue11_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 2].ableToPut == false)
                tue11_btn.Visibility = Visibility.Visible;
        }
        private void tue11_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화11");
                RefreshTimeTable();
            }
            tue11_btn.Visibility = Visibility.Collapsed;
        }
        private void tue11_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화11");
                RefreshTimeTable();
            }
            tue11_btn.Visibility = Visibility.Collapsed;
        }

        private void tue12_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 2].ableToPut == false)
                tue12_btn.Visibility = Visibility.Visible;
        }
        private void tue12_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tue12_btn.Visibility = Visibility.Collapsed;
        }
        private void tue12_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 2].ableToPut == false)
                tue12_btn.Visibility = Visibility.Visible;
        }
        private void tue12_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화12");
                RefreshTimeTable();
            }
            tue12_btn.Visibility = Visibility.Collapsed;
        }
        private void tue12_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 2].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("화12");
                RefreshTimeTable();
            }
            tue12_btn.Visibility = Visibility.Collapsed;
        }

        private void wed1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 3].ableToPut == false)
                wed1_btn.Visibility = Visibility.Visible;
        }
        private void wed1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed1_btn.Visibility = Visibility.Collapsed;
        }
        private void wed1_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 3].ableToPut == false)
                wed1_btn.Visibility = Visibility.Visible;
        }
        private void wed1_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수1");
                RefreshTimeTable();
            }
            wed1_btn.Visibility = Visibility.Collapsed;
        }
        private void wed1_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수1");
                RefreshTimeTable();
            }
            wed1_btn.Visibility = Visibility.Collapsed;
        }

        private void wed2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 3].ableToPut == false)
                wed2_btn.Visibility = Visibility.Visible;
        }
        private void wed2_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed2_btn.Visibility = Visibility.Collapsed;
        }
        private void wed2_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 3].ableToPut == false)
                wed2_btn.Visibility = Visibility.Visible;
        }
        private void wed2_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수2");
                RefreshTimeTable();
            }
            wed2_btn.Visibility = Visibility.Collapsed;
        }
        private void wed2_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수2");
                RefreshTimeTable();
            }
            wed2_btn.Visibility = Visibility.Collapsed;
        }

        private void wed3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 3].ableToPut == false)
                wed3_btn.Visibility = Visibility.Visible;
        }
        private void wed3_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed3_btn.Visibility = Visibility.Collapsed;
        }
        private void wed3_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 3].ableToPut == false)
                wed3_btn.Visibility = Visibility.Visible;
        }
        private void wed3_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수3");
                RefreshTimeTable();
            }
            wed3_btn.Visibility = Visibility.Collapsed;
        }
        private void wed3_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수3");
                RefreshTimeTable();
            }
            wed3_btn.Visibility = Visibility.Collapsed;
        }

        private void wed4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 3].ableToPut == false)
                wed4_btn.Visibility = Visibility.Visible;
        }
        private void wed4_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed4_btn.Visibility = Visibility.Collapsed;
        }
        private void wed4_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 3].ableToPut == false)
                wed4_btn.Visibility = Visibility.Visible;
        }
        private void wed4_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수4");
                RefreshTimeTable();
            }
            wed4_btn.Visibility = Visibility.Collapsed;
        }
        private void wed4_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수4");
                RefreshTimeTable();
            }
            wed4_btn.Visibility = Visibility.Collapsed;
        }

        private void wed5_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 3].ableToPut == false)
                wed5_btn.Visibility = Visibility.Visible;
        }
        private void wed5_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed5_btn.Visibility = Visibility.Collapsed;
        }
        private void wed5_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 3].ableToPut == false)
                wed5_btn.Visibility = Visibility.Visible;
        }
        private void wed5_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수5");
                RefreshTimeTable();
            }
            wed5_btn.Visibility = Visibility.Collapsed;
        }
        private void wed5_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수5");
                RefreshTimeTable();
            }
            wed5_btn.Visibility = Visibility.Collapsed;
        }

        private void wed6_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 3].ableToPut == false)
                wed6_btn.Visibility = Visibility.Visible;
        }
        private void wed6_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed6_btn.Visibility = Visibility.Collapsed;
        }
        private void wed6_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 3].ableToPut == false)
                wed6_btn.Visibility = Visibility.Visible;
        }
        private void wed6_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수6");
                RefreshTimeTable();
            }
            wed6_btn.Visibility = Visibility.Collapsed;
        }
        private void wed6_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수6");
                RefreshTimeTable();
            }
            wed6_btn.Visibility = Visibility.Collapsed;
        }

        private void wed7_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 3].ableToPut == false)
                wed7_btn.Visibility = Visibility.Visible;
        }
        private void wed7_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed7_btn.Visibility = Visibility.Collapsed;
        }
        private void wed7_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 3].ableToPut == false)
                wed7_btn.Visibility = Visibility.Visible;
        }
        private void wed7_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수7");
                RefreshTimeTable();
            }
            wed7_btn.Visibility = Visibility.Collapsed;
        }
        private void wed7_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수7");
                RefreshTimeTable();
            }
            wed7_btn.Visibility = Visibility.Collapsed;
        }

        private void wed8_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 3].ableToPut == false)
                wed8_btn.Visibility = Visibility.Visible;
        }
        private void wed8_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed8_btn.Visibility = Visibility.Collapsed;
        }
        private void wed8_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 3].ableToPut == false)
                wed8_btn.Visibility = Visibility.Visible;
        }
        private void wed8_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수8");
                RefreshTimeTable();
            }
            wed8_btn.Visibility = Visibility.Collapsed;
        }
        private void wed8_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수8");
                RefreshTimeTable();
            }
            wed8_btn.Visibility = Visibility.Collapsed;
        }

        private void wed9_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 3].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void wed9_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed9_btn.Visibility = Visibility.Collapsed;
        }
        private void wed9_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 3].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void wed9_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수9");
                RefreshTimeTable();
            }
            wed9_btn.Visibility = Visibility.Collapsed;
        }
        private void wed9_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수9");
                RefreshTimeTable();
            }
            wed9_btn.Visibility = Visibility.Collapsed;
        }

        private void wed10_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 3].ableToPut == false)
                wed10_btn.Visibility = Visibility.Visible;
        }
        private void wed10_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed10_btn.Visibility = Visibility.Collapsed;
        }
        private void wed10_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 3].ableToPut == false)
                wed10_btn.Visibility = Visibility.Visible;
        }
        private void wed10_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수10");
                RefreshTimeTable();
            }
            wed10_btn.Visibility = Visibility.Collapsed;
        }
        private void wed10_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수10");
                RefreshTimeTable();
            }
            wed10_btn.Visibility = Visibility.Collapsed;
        }

        private void wed11_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 3].ableToPut == false)
                wed11_btn.Visibility = Visibility.Visible;
        }
        private void wed11_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed11_btn.Visibility = Visibility.Collapsed;
        }
        private void wed11_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 3].ableToPut == false)
                wed11_btn.Visibility = Visibility.Visible;
        }
        private void wed11_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수11");
                RefreshTimeTable();
            }
            wed11_btn.Visibility = Visibility.Collapsed;
        }
        private void wed11_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수11");
                RefreshTimeTable();
            }
            wed11_btn.Visibility = Visibility.Collapsed;
        }

        private void wed12_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 3].ableToPut == false)
                wed12_btn.Visibility = Visibility.Visible;
        }
        private void wed12_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed12_btn.Visibility = Visibility.Collapsed;
        }
        private void wed12_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 3].ableToPut == false)
                wed12_btn.Visibility = Visibility.Visible;
        }
        private void wed12_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수12");
                RefreshTimeTable();
            }
            wed12_btn.Visibility = Visibility.Collapsed;
        }
        private void wed12_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 3].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("수12");
                RefreshTimeTable();
            }
            wed12_btn.Visibility = Visibility.Collapsed;
        }

        private void thu1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 4].ableToPut == false)
                thu1_btn.Visibility = Visibility.Visible;
        }
        private void thu1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu1_btn.Visibility = Visibility.Collapsed;
        }
        private void thu1_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 4].ableToPut == false)
                thu1_btn.Visibility = Visibility.Visible;
        }
        private void thu1_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목1");
                RefreshTimeTable();
            }
            thu1_btn.Visibility = Visibility.Collapsed;
        }
        private void thu1_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목1");
                RefreshTimeTable();
            }
            thu1_btn.Visibility = Visibility.Collapsed;
        }

        private void thu2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 4].ableToPut == false)
                thu2_btn.Visibility = Visibility.Visible;
        }
        private void thu2_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu2_btn.Visibility = Visibility.Collapsed;
        }
        private void thu2_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 4].ableToPut == false)
                thu2_btn.Visibility = Visibility.Visible;
        }
        private void thu2_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목2");
                RefreshTimeTable();
            }
            thu2_btn.Visibility = Visibility.Collapsed;
        }
        private void thu2_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목2");
                RefreshTimeTable();
            }
            thu2_btn.Visibility = Visibility.Collapsed;
        }

        private void thu3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 4].ableToPut == false)
                thu3_btn.Visibility = Visibility.Visible;
        }
        private void thu3_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu3_btn.Visibility = Visibility.Collapsed;
        }
        private void thu3_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 4].ableToPut == false)
                thu3_btn.Visibility = Visibility.Visible;
        }
        private void thu3_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목3");
                RefreshTimeTable();
            }
            thu3_btn.Visibility = Visibility.Collapsed;
        }
        private void thu3_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목3");
                RefreshTimeTable();
            }
            thu3_btn.Visibility = Visibility.Collapsed;
        }

        private void thu4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 4].ableToPut == false)
                thu4_btn.Visibility = Visibility.Visible;
        }
        private void thu4_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu4_btn.Visibility = Visibility.Collapsed;
        }
        private void thu4_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 4].ableToPut == false)
                thu4_btn.Visibility = Visibility.Visible;
        }
        private void thu4_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목4");
                RefreshTimeTable();
            }
            thu4_btn.Visibility = Visibility.Collapsed;
        }
        private void thu4_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목4");
                RefreshTimeTable();
            }
            thu4_btn.Visibility = Visibility.Collapsed;
        }

        private void thu5_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 4].ableToPut == false)
                thu5_btn.Visibility = Visibility.Visible;
        }
        private void thu5_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu5_btn.Visibility = Visibility.Collapsed;
        }
        private void thu5_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 4].ableToPut == false)
                thu5_btn.Visibility = Visibility.Visible;
        }
        private void thu5_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목5");
                RefreshTimeTable();
            }
            thu5_btn.Visibility = Visibility.Collapsed;
        }
        private void thu5_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목5");
                RefreshTimeTable();
            }
            thu5_btn.Visibility = Visibility.Collapsed;
        }

        private void thu6_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 4].ableToPut == false)
                thu6_btn.Visibility = Visibility.Visible;
        }
        private void thu6_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu6_btn.Visibility = Visibility.Collapsed;
        }
        private void thu6_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 4].ableToPut == false)
                thu6_btn.Visibility = Visibility.Visible;
        }
        private void thu6_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목6");
                RefreshTimeTable();
            }
            thu6_btn.Visibility = Visibility.Collapsed;
        }
        private void thu6_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목6");
                RefreshTimeTable();
            }
            thu6_btn.Visibility = Visibility.Collapsed;
        }

        private void thu7_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 4].ableToPut == false)
                thu7_btn.Visibility = Visibility.Visible;
        }
        private void thu7_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu7_btn.Visibility = Visibility.Collapsed;
        }
        private void thu7_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 4].ableToPut == false)
                thu7_btn.Visibility = Visibility.Visible;
        }
        private void thu7_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목7");
                RefreshTimeTable();
            }
            thu7_btn.Visibility = Visibility.Collapsed;
        }
        private void thu7_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목7");
                RefreshTimeTable();
            }
            thu7_btn.Visibility = Visibility.Collapsed;
        }

        private void thu8_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 4].ableToPut == false)
                thu8_btn.Visibility = Visibility.Visible;
        }
        private void thu8_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu8_btn.Visibility = Visibility.Collapsed;
        }
        private void thu8_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 4].ableToPut == false)
                thu8_btn.Visibility = Visibility.Visible;
        }
        private void thu8_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목8");
                RefreshTimeTable();
            }
            thu8_btn.Visibility = Visibility.Collapsed;
        }
        private void thu8_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목8");
                RefreshTimeTable();
            }
            thu8_btn.Visibility = Visibility.Collapsed;
        }

        private void thu9_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 4].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void thu9_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed9_btn.Visibility = Visibility.Collapsed;
        }
        private void thu9_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 4].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void thu9_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목9");
                RefreshTimeTable();
            }
            thu9_btn.Visibility = Visibility.Collapsed;
        }
        private void thu9_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목9");
                RefreshTimeTable();
            }
            thu9_btn.Visibility = Visibility.Collapsed;
        }

        private void thu10_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 4].ableToPut == false)
                thu10_btn.Visibility = Visibility.Visible;
        }
        private void thu10_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu10_btn.Visibility = Visibility.Collapsed;
        }
        private void thu10_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 4].ableToPut == false)
                thu10_btn.Visibility = Visibility.Visible;
        }
        private void thu10_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목10");
                RefreshTimeTable();
            }
            thu10_btn.Visibility = Visibility.Collapsed;
        }
        private void thu10_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목10");
                RefreshTimeTable();
            }
            thu10_btn.Visibility = Visibility.Collapsed;
        }

        private void thu11_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 4].ableToPut == false)
                thu11_btn.Visibility = Visibility.Visible;
        }
        private void thu11_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu11_btn.Visibility = Visibility.Collapsed;
        }
        private void thu11_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 4].ableToPut == false)
                thu11_btn.Visibility = Visibility.Visible;
        }
        private void thu11_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목11");
                RefreshTimeTable();
            }
            thu11_btn.Visibility = Visibility.Collapsed;
        }
        private void thu11_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목11");
                RefreshTimeTable();
            }
            thu11_btn.Visibility = Visibility.Collapsed;
        }

        private void thu12_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 4].ableToPut == false)
                thu12_btn.Visibility = Visibility.Visible;
        }
        private void thu12_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            thu12_btn.Visibility = Visibility.Collapsed;
        }
        private void thu12_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 4].ableToPut == false)
                thu12_btn.Visibility = Visibility.Visible;
        }
        private void thu12_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목12");
                RefreshTimeTable();
            }
            thu12_btn.Visibility = Visibility.Collapsed;
        }
        private void thu12_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 4].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("목12");
                RefreshTimeTable();
            }
            thu12_btn.Visibility = Visibility.Collapsed;
        }

        private void fri1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 5].ableToPut == false)
                fri1_btn.Visibility = Visibility.Visible;
        }
        private void fri1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri1_btn.Visibility = Visibility.Collapsed;
        }
        private void fri1_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 5].ableToPut == false)
                fri1_btn.Visibility = Visibility.Visible;
        }
        private void fri1_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금1");
                RefreshTimeTable();
            }
            fri1_btn.Visibility = Visibility.Collapsed;
        }
        private void fri1_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금1");
                RefreshTimeTable();
            }
            fri1_btn.Visibility = Visibility.Collapsed;
        }

        private void fri2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 5].ableToPut == false)
                fri2_btn.Visibility = Visibility.Visible;
        }
        private void fri2_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri2_btn.Visibility = Visibility.Collapsed;
        }
        private void fri2_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 5].ableToPut == false)
                fri2_btn.Visibility = Visibility.Visible;
        }
        private void fri2_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금2");
                RefreshTimeTable();
            }
            fri2_btn.Visibility = Visibility.Collapsed;
        }
        private void fri2_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금2");
                RefreshTimeTable();
            }
            fri2_btn.Visibility = Visibility.Collapsed;
        }

        private void fri3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 5].ableToPut == false)
                fri3_btn.Visibility = Visibility.Visible;
        }
        private void fri3_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri3_btn.Visibility = Visibility.Collapsed;
        }
        private void fri3_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 5].ableToPut == false)
                fri3_btn.Visibility = Visibility.Visible;
        }
        private void fri3_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금3");
                RefreshTimeTable();
            }
            fri3_btn.Visibility = Visibility.Collapsed;
        }
        private void fri3_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금3");
                RefreshTimeTable();
            }
            fri3_btn.Visibility = Visibility.Collapsed;
        }

        private void fri4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 5].ableToPut == false)
                fri4_btn.Visibility = Visibility.Visible;
        }
        private void fri4_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri4_btn.Visibility = Visibility.Collapsed;
        }
        private void fri4_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 5].ableToPut == false)
                fri4_btn.Visibility = Visibility.Visible;
        }
        private void fri4_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금4");
                RefreshTimeTable();
            }
            fri4_btn.Visibility = Visibility.Collapsed;
        }
        private void fri4_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금4");
                RefreshTimeTable();
            }
            fri4_btn.Visibility = Visibility.Collapsed;
        }

        private void fri5_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 5].ableToPut == false)
                fri5_btn.Visibility = Visibility.Visible;
        }
        private void fri5_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri5_btn.Visibility = Visibility.Collapsed;
        }
        private void fri5_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 5].ableToPut == false)
                fri5_btn.Visibility = Visibility.Visible;
        }
        private void fri5_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금5");
                RefreshTimeTable();
            }
            fri5_btn.Visibility = Visibility.Collapsed;
        }
        private void fri5_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금5");
                RefreshTimeTable();
            }
            fri5_btn.Visibility = Visibility.Collapsed;
        }

        private void fri6_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 5].ableToPut == false)
                fri6_btn.Visibility = Visibility.Visible;
        }
        private void fri6_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri6_btn.Visibility = Visibility.Collapsed;
        }
        private void fri6_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 5].ableToPut == false)
                fri6_btn.Visibility = Visibility.Visible;
        }
        private void fri6_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금6");
                RefreshTimeTable();
            }
            fri6_btn.Visibility = Visibility.Collapsed;
        }
        private void fri6_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금6");
                RefreshTimeTable();
            }
            fri6_btn.Visibility = Visibility.Collapsed;
        }

        private void fri7_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 5].ableToPut == false)
                fri7_btn.Visibility = Visibility.Visible;
        }
        private void fri7_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri7_btn.Visibility = Visibility.Collapsed;
        }
        private void fri7_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 5].ableToPut == false)
                fri7_btn.Visibility = Visibility.Visible;
        }
        private void fri7_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금7");
                RefreshTimeTable();
            }
            fri7_btn.Visibility = Visibility.Collapsed;
        }
        private void fri7_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금7");
                RefreshTimeTable();
            }
            fri7_btn.Visibility = Visibility.Collapsed;
        }

        private void fri8_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 5].ableToPut == false)
                fri8_btn.Visibility = Visibility.Visible;
        }
        private void fri8_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri8_btn.Visibility = Visibility.Collapsed;
        }
        private void fri8_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 5].ableToPut == false)
                fri8_btn.Visibility = Visibility.Visible;
        }
        private void fri8_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금8");
                RefreshTimeTable();
            }
            fri8_btn.Visibility = Visibility.Collapsed;
        }
        private void fri8_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금8");
                RefreshTimeTable();
            }
            fri8_btn.Visibility = Visibility.Collapsed;
        }

        private void fri9_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 5].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void fri9_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed9_btn.Visibility = Visibility.Collapsed;
        }
        private void fri9_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 5].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void fri9_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금9");
                RefreshTimeTable();
            }
            fri9_btn.Visibility = Visibility.Collapsed;
        }
        private void fri9_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금9");
                RefreshTimeTable();
            }
            fri9_btn.Visibility = Visibility.Collapsed;
        }

        private void fri10_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 5].ableToPut == false)
                fri10_btn.Visibility = Visibility.Visible;
        }
        private void fri10_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri10_btn.Visibility = Visibility.Collapsed;
        }
        private void fri10_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 5].ableToPut == false)
                fri10_btn.Visibility = Visibility.Visible;
        }
        private void fri10_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금10");
                RefreshTimeTable();
            }
            fri10_btn.Visibility = Visibility.Collapsed;
        }
        private void fri10_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금10");
                RefreshTimeTable();
            }
            fri10_btn.Visibility = Visibility.Collapsed;
        }

        private void fri11_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 5].ableToPut == false)
                fri11_btn.Visibility = Visibility.Visible;
        }
        private void fri11_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri11_btn.Visibility = Visibility.Collapsed;
        }
        private void fri11_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 5].ableToPut == false)
                fri11_btn.Visibility = Visibility.Visible;
        }
        private void fri11_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금11");
                RefreshTimeTable();
            }
            fri11_btn.Visibility = Visibility.Collapsed;
        }
        private void fri11_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금11");
                RefreshTimeTable();
            }
            fri11_btn.Visibility = Visibility.Collapsed;
        }

        private void fri12_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 5].ableToPut == false)
                fri12_btn.Visibility = Visibility.Visible;
        }
        private void fri12_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fri12_btn.Visibility = Visibility.Collapsed;
        }
        private void fri12_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 5].ableToPut == false)
                fri12_btn.Visibility = Visibility.Visible;
        }
        private void fri12_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금12");
                RefreshTimeTable();
            }
            fri12_btn.Visibility = Visibility.Collapsed;
        }
        private void fri12_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 5].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("금12");
                RefreshTimeTable();
            }
            fri12_btn.Visibility = Visibility.Collapsed;
        }

        private void sat1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 6].ableToPut == false)
                sat1_btn.Visibility = Visibility.Visible;
        }
        private void sat1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat1_btn.Visibility = Visibility.Collapsed;
        }
        private void sat1_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[1, 6].ableToPut == false)
                sat1_btn.Visibility = Visibility.Visible;
        }
        private void sat1_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토1");
                RefreshTimeTable();
            }
            sat1_btn.Visibility = Visibility.Collapsed;
        }
        private void sat1_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[1, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토2");
                RefreshTimeTable();
            }
            sat2_btn.Visibility = Visibility.Collapsed;
        }


        private void sat2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 6].ableToPut == false)
                sat2_btn.Visibility = Visibility.Visible;
        }
        private void sat2_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat2_btn.Visibility = Visibility.Collapsed;
        }
        private void sat2_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[2, 6].ableToPut == false)
                sat2_btn.Visibility = Visibility.Visible;
        }
        private void sat2_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토2");
                RefreshTimeTable();
            }
            sat2_btn.Visibility = Visibility.Collapsed;
        }
        private void sat2_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[2, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토2");
                RefreshTimeTable();
            }
            sat2_btn.Visibility = Visibility.Collapsed;
        }

        private void sat3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 6].ableToPut == false)
                sat3_btn.Visibility = Visibility.Visible;
        }
        private void sat3_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat3_btn.Visibility = Visibility.Collapsed;
        }
        private void sat3_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[3, 6].ableToPut == false)
                sat3_btn.Visibility = Visibility.Visible;
        }
        private void sat3_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토3");
                RefreshTimeTable();
            }
            sat3_btn.Visibility = Visibility.Collapsed;
        }
        private void sat3_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[3, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토3");
                RefreshTimeTable();
            }
            sat3_btn.Visibility = Visibility.Collapsed;
        }

        private void sat4_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 6].ableToPut == false)
                sat4_btn.Visibility = Visibility.Visible;
        }
        private void sat4_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat4_btn.Visibility = Visibility.Collapsed;
        }
        private void sat4_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[4, 6].ableToPut == false)
                sat4_btn.Visibility = Visibility.Visible;
        }
        private void sat4_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토4");
                RefreshTimeTable();
            }
            sat4_btn.Visibility = Visibility.Collapsed;
        }
        private void sat4_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[4, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토4");
                RefreshTimeTable();
            }
            sat4_btn.Visibility = Visibility.Collapsed;
        }

        private void sat5_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 6].ableToPut == false)
                sat5_btn.Visibility = Visibility.Visible;
        }
        private void sat5_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat5_btn.Visibility = Visibility.Collapsed;
        }
        private void sat5_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[5, 6].ableToPut == false)
                sat5_btn.Visibility = Visibility.Visible;
        }
        private void sat5_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토5");
                RefreshTimeTable();
            }
            sat5_btn.Visibility = Visibility.Collapsed;
        }
        private void sat5_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[5, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토5");
                RefreshTimeTable();
            }
            sat5_btn.Visibility = Visibility.Collapsed;
        }

        private void sat6_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 6].ableToPut == false)
                sat6_btn.Visibility = Visibility.Visible;
        }
        private void sat6_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat6_btn.Visibility = Visibility.Collapsed;
        }
        private void sat6_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[6, 6].ableToPut == false)
                sat6_btn.Visibility = Visibility.Visible;
        }
        private void sat6_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토6");
                RefreshTimeTable();
            }
            sat6_btn.Visibility = Visibility.Collapsed;
        }
        private void sat6_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[6, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토6");
                RefreshTimeTable();
            }
            sat6_btn.Visibility = Visibility.Collapsed;
        }

        private void sat7_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 6].ableToPut == false)
                sat7_btn.Visibility = Visibility.Visible;
        }
        private void sat7_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat7_btn.Visibility = Visibility.Collapsed;
        }
        private void sat7_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[7, 6].ableToPut == false)
                sat7_btn.Visibility = Visibility.Visible;
        }
        private void sat7_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토7");
                RefreshTimeTable();
            }
            sat7_btn.Visibility = Visibility.Collapsed;
        }
        private void sat7_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[7, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토7");
                RefreshTimeTable();
            }
            sat7_btn.Visibility = Visibility.Collapsed;
        }

        private void sat8_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 6].ableToPut == false)
                sat8_btn.Visibility = Visibility.Visible;
        }
        private void sat8_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat8_btn.Visibility = Visibility.Collapsed;
        }
        private void sat8_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[8, 6].ableToPut == false)
                sat8_btn.Visibility = Visibility.Visible;
        }
        private void sat8_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토8");
                RefreshTimeTable();
            }
            sat8_btn.Visibility = Visibility.Collapsed;
        }
        private void sat8_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[8, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토8");
                RefreshTimeTable();
            }
            sat8_btn.Visibility = Visibility.Collapsed;
        }

        private void sat9_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 6].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void sat9_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            wed9_btn.Visibility = Visibility.Collapsed;
        }
        private void sat9_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[9, 6].ableToPut == false)
                wed9_btn.Visibility = Visibility.Visible;
        }
        private void sat9_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토9");
                RefreshTimeTable();
            }
            sat9_btn.Visibility = Visibility.Collapsed;
        }
        private void sat9_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[9, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토9");
                RefreshTimeTable();
            }
            sat9_btn.Visibility = Visibility.Collapsed;
        }

        private void sat10_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 6].ableToPut == false)
                sat10_btn.Visibility = Visibility.Visible;
        }
        private void sat10_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat10_btn.Visibility = Visibility.Collapsed;
        }
        private void sat10_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[10, 6].ableToPut == false)
                sat10_btn.Visibility = Visibility.Visible;
        }
        private void sat10_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토10");
                RefreshTimeTable();
            }
            sat10_btn.Visibility = Visibility.Collapsed;
        }
        private void sat10_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[10, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토10");
                RefreshTimeTable();
            }
            sat10_btn.Visibility = Visibility.Collapsed;
        }

        private void sat11_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 6].ableToPut == false)
                sat11_btn.Visibility = Visibility.Visible;
        }
        private void sat11_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat11_btn.Visibility = Visibility.Collapsed;
        }
        private void sat11_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[11, 6].ableToPut == false)
                sat11_btn.Visibility = Visibility.Visible;
        }
        private void sat11_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토11");
                RefreshTimeTable();
            }
            sat11_btn.Visibility = Visibility.Collapsed;
        }
        private void sat11_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[11, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토11");
                RefreshTimeTable();
            }
            sat11_btn.Visibility = Visibility.Collapsed;
        }

        private void sat12_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 6].ableToPut == false)
                sat12_btn.Visibility = Visibility.Visible;
        }
        private void sat12_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            sat12_btn.Visibility = Visibility.Collapsed;
        }
        private void sat12_btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (TimeTableDB[12, 6].ableToPut == false)
                sat12_btn.Visibility = Visibility.Visible;
        }
        private void sat12_btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토12");
                RefreshTimeTable();
            }
            sat12_btn.Visibility = Visibility.Collapsed;
        }
        private void sat12_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[12, 6].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable("토12");
                RefreshTimeTable();
            }
            sat12_btn.Visibility = Visibility.Collapsed;
        }

        private void DataListView_All_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //System.Windows.MessageBox.Show(sender.ToString());

            //DataListView_All.SelectedItem = (sender as Border).DataContext;
            //if (!DataListView_All.IsFocused)
            //    DataListView_All.Focus();
           // mon1.Background = Brushes.Red;
        }

        private void DataListView_All_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
           // mon1.Background = Brushes.White;
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            App.MG.Show();
        }

        private void MyGroup_cob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.MessageBox.Show("Value : " + data[MyGroup_cob.SelectedIndex]);
        }

        private void MyGroup_cob_Initialized(object sender, EventArgs e)
        {
            data.Add("A");
            data.Add("B");
            //MyGroup_cob.ItemsSource = data;
        }
    }


}

