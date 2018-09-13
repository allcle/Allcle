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
        List<Subject> ResultSubtject = new List<Subject>();     //결과 과목
        List<Subject> UsersSubjectsList = new List<Subject>(); //유저가 듣는 과목 리스트
        List<UserTimeTable> userTimeTable = new List<UserTimeTable>();    //유저의 시간표
        List<TimeTableClassNumber> timeTableClassNumber = new List<TimeTableClassNumber>(); //시간표의 과목들

        public struct TableSubjects //시간표 한칸의 Data
        {
            public string className;
            public string professor;
            public bool ableToPut;
        }
        TableSubjects[,] TimeTableDB = new TableSubjects[14, 7]; //12교시*일주일 2차원 배열
        string urlBase = @"https://allcleapp.azurewebsites.net/api/AllCleSubjects2"; //기본 url
        string urlTimeTable = @"https://allcleapp.azurewebsites.net/api/UserTimeTable"; //유저의 시간표 리스트를 위한 기본 url
        string urlTimeTableClassNumber = @"https://allcleapp.azurewebsites.net/api/TimeTableClassNumber";       //저장된 시간표의 과목들
        string url = null;  //json으로 쓰일 url
        
        public MainScreen()
        {
            InitializeComponent();
            GetSubjects();
            DataListView_All.ItemsSource = SubjectList;
            InitDB();
        }
        private void Search_btn_Click(object sender, RoutedEventArgs e) //검색 버튼 눌렀을때
        {
            if (FilterOption.timeOption == true)
            {
                if (FilterOption.subjectOption == true)
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOnClassName(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOnProfessor(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOnClassNumber(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOff(TimeInList(UsersSubjectsList),SubjectInList(UsersSubjectsList));
                }
                else
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOnClassName(TimeInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOnProfessor(TimeInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOnClassNumber(TimeInList(UsersSubjectsList), Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOff(TimeInList(UsersSubjectsList));
                }
            }
            else
            {
                if (FilterOption.subjectOption == true)
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOnClassName(SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOnProfessor(SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOnClassNumber(SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOff(SubjectInList(UsersSubjectsList));
                }
                else
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOnClassName(Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOnProfessor(Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOnClassNumber(Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOff();
                }
            }
        }
        private void Search_Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)//엔터키 눌렀을 때
        {
            if (FilterOption.timeOption == true)
            {
                if (FilterOption.subjectOption == true)
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOnClassName(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOnProfessor(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOnClassNumber(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOnSearchOff(TimeInList(UsersSubjectsList), SubjectInList(UsersSubjectsList));
                }
                else
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOnClassName(TimeInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOnProfessor(TimeInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOnClassNumber(TimeInList(UsersSubjectsList), Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOff(TimeInList(UsersSubjectsList));
                }
            }
            else
            {
                if (FilterOption.subjectOption == true)
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOnClassName(SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOnProfessor(SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOnClassNumber(SubjectInList(UsersSubjectsList), Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOnSearchOff(SubjectInList(UsersSubjectsList));
                }
                else
                {
                    if (FilterOption.searchOption == 1)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOnClassName(Search_Box.Text);
                    else if (FilterOption.searchOption == 2)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOnProfessor(Search_Box.Text);
                    else if (FilterOption.searchOption == 3)
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOnClassNumber(Search_Box.Text);
                    else
                        DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOff();
                }
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

        private void GetSubjects()
        {
            url = urlBase;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            SubjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            ResultSubtject = SubjectList;
        }
        private void GetTimeTable()
        {
            url = urlTimeTable + "/" + App.ID;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            userTimeTable = JsonConvert.DeserializeObject<List<UserTimeTable>>(Unicode);            
        }
        private void GetTimeTableClassNumber(string _timeTableName)
        {
            url = urlTimeTableClassNumber + "/" + _timeTableName;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            timeTableClassNumber = JsonConvert.DeserializeObject<List<TimeTableClassNumber>>(Unicode);
        }

        private List<Subject> ShowTimeOnSubjectOnSearchOff(List<string> _time, List<string> _subject)  //남은시간에서만, 담은과목 제외
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOnSubjectOffSearchOff(List<string> _time)                 //남은 시간만에서만
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOffSubjectOnSearchOff(List<string> _subject)                //담은 과목만 제외
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOffSubjectOffSearchOff()                             //전체 모든 과목 보기
        {
            ResultSubtject = SubjectList;
            return ResultSubtject;
        }

        private List<Subject> ShowTimeOnSubjectOnSearchOnClassName(List<string> _time, List<string> _subject, string _search) //남은시간에서만, 담은과목제외, 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.ClassName.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOnSubjectOnSearchOnProfessor(List<string> _time, List<string> _subject, string _search) //남은시간에서만, 담은과목제외, 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.Professor.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOnSubjectOnSearchOnClassNumber(List<string> _time, List<string> _subject, string _search) //남은시간에서만, 담은과목제외, 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.ClassNumber.Contains(_search)).ToList();
            return ResultSubtject;
        }

        private List<Subject> ShowTimeOnSubjectOffSearchOnClassName(List<string> _time, string _search)                 //남은 시간에서만 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.ClassName.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOnSubjectOffSearchOnProfessor(List<string> _time, string _search)                 //남은 시간에서만 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.Professor.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOnSubjectOffSearchOnClassNumber(List<string> _time, string _search)                 //남은 시간에서만 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _time.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.Times.Contains(_time[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.ClassNumber.Contains(_search)).ToList();
            return ResultSubtject;
        }

        private List<Subject> ShowTimeOffSubjectOnSearchOnClassName(List<string> _subject, string _search)              //담은 과목 제외하고 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.ClassName.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOffSubjectOnSearchOnProfessor(List<string> _subject, string _search)              //담은 과목 제외하고 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.Professor.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOffSubjectOnSearchOnClassNumber(List<string> _subject, string _search)              //담은 과목 제외하고 검색
        {
            ResultSubtject = SubjectList;
            for (int i = 0; i < _subject.Count; i++)
            {
                ResultSubtject = ResultSubtject.Where(s => !s.ClassName.Contains(_subject[i])).ToList();
            }
            ResultSubtject = ResultSubtject.Where(s => s.ClassNumber.Contains(_search)).ToList();
            return ResultSubtject;
        }

        private List<Subject> ShowTimeOffSubjectOffSearchOnClassName(string _search)                              //그냥 과목검색
        {
            ResultSubtject = SubjectList;
            ResultSubtject = ResultSubtject.Where(s => s.ClassName.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOffSubjectOffSearchOnProfessor(string _search)                              //그냥 과목검색
        {
            ResultSubtject = SubjectList;
            ResultSubtject = ResultSubtject.Where(s => s.Professor.Contains(_search)).ToList();
            return ResultSubtject;
        }
        private List<Subject> ShowTimeOffSubjectOffSearchOnClassNumber(string _search)                              //그냥 과목검색
        {
            ResultSubtject = SubjectList;
            ResultSubtject = ResultSubtject.Where(s => s.ClassNumber.Contains(_search)).ToList();
            return ResultSubtject;
        }
         
        

        private void DataListView_All_MouseDoubleClick(object sender, MouseButtonEventArgs e) //리스트에 있는 과목을 더블클릭했을때
        {
            bool totalAbleToPut = false;
            int index = DataListView_All.SelectedIndex;
            int period1 = 0; string day1 = null;
            int period2 = 0; string day2 = null;
            int period3 = 0; string day3 = null;
            int period4 = 0; string day4 = null;
            int period5 = 0; string day5 = null;
            int period6 = 0; string day6 = null;
            int period7 = 0; string day7 = null;
            int period8 = 0; string day8 = null;

            string[] daylist = new string[] { day1, day2, day3, day4, day5, day6, day7, day8 };

            int[] _period = new int[] { period1, period2, period3, period4, period5, period6, period7, period8 };

            if (DataListView_All.SelectedItems.Count == 1) //리스트에서 클릭하면
            {
                string[] time = new string[] { ResultSubtject[index].Time1, ResultSubtject[index].Time2, ResultSubtject[index].Time3, ResultSubtject[index].Time4, ResultSubtject[index].Time5, ResultSubtject[index].Time6, ResultSubtject[index].Time7, ResultSubtject[index].Time8 };

                if (time[0] != "")
                {
                    _period[0] = Int32.Parse(time[0].Substring(1, time[0].Length - 1));
                    daylist[0] = time[0].Substring(0, 1);
                    if (daylist[0] == "월" && TimeTableDB[_period[0], 1].ableToPut == true) totalAbleToPut = true;
                    else if (daylist[0] == "화" && TimeTableDB[_period[0], 2].ableToPut == true) totalAbleToPut = true;
                    else if (daylist[0] == "수" && TimeTableDB[_period[0], 3].ableToPut == true) totalAbleToPut = true;
                    else if (daylist[0] == "목" && TimeTableDB[_period[0], 4].ableToPut == true) totalAbleToPut = true;
                    else if (daylist[0] == "금" && TimeTableDB[_period[0], 5].ableToPut == true) totalAbleToPut = true;
                    else if (daylist[0] == "토" && TimeTableDB[_period[0], 6].ableToPut == true) totalAbleToPut = true;
                    else
                    {
                        System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                        totalAbleToPut = false;
                    }
                }

                for (int i=1; i<8; i++)
                {
                    if(time[i] != "" && totalAbleToPut == true)
                    {
                        _period[i] = Int32.Parse(time[i].Substring(1, time[i].Length - 1));
                        daylist[i] = time[i].Substring(0, 1);
                        if (daylist[i] == "월" && TimeTableDB[_period[i], 1].ableToPut == true) totalAbleToPut = true;
                        else if (daylist[i] == "화" && TimeTableDB[_period[i], 2].ableToPut == true) totalAbleToPut = true;
                        else if (daylist[i] == "수" && TimeTableDB[_period[i], 3].ableToPut == true) totalAbleToPut = true;
                        else if (daylist[i] == "목" && TimeTableDB[_period[i], 4].ableToPut == true) totalAbleToPut = true;
                        else if (daylist[i] == "금" && TimeTableDB[_period[i], 5].ableToPut == true) totalAbleToPut = true;
                        else if (daylist[i] == "토" && TimeTableDB[_period[i], 6].ableToPut == true) totalAbleToPut = true;
                        else
                        {
                            System.Windows.MessageBox.Show("이미 그 시간에 과목이 있습니다");
                            totalAbleToPut = false;
                        }
                    }
                }
            }

            if (totalAbleToPut == true) //과목을 넣을 수 있다면
            {
                UsersSubjectsList.Add(new Subject()
                {
                    NO = ResultSubtject[index].NO,
                    Grade = ResultSubtject[index].Grade,
                    ClassNumber = ResultSubtject[index].ClassNumber,
                    ClassName = ResultSubtject[index].ClassName,
                    CreditCourse = ResultSubtject[index].CreditCourse,
                    Professor = ResultSubtject[index].Professor,
                    강의시간 = ResultSubtject[index].강의시간,
                    Time1 = ResultSubtject[index].Time1,
                    Time2 = ResultSubtject[index].Time2,
                    Time3 = ResultSubtject[index].Time3,
                    Time4 = ResultSubtject[index].Time4,
                    Time5 = ResultSubtject[index].Time5,
                    Time6 = ResultSubtject[index].Time6,
                    Time7 = ResultSubtject[index].Time7,
                    Time8 = ResultSubtject[index].Time8,
                }); //과목추가
                RefreshTimeTable();
            }
        }
        private List<string> TimeInList(List<Subject> _UsersSubjectList) //유저가 듣는 시간을 string으로
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _UsersSubjectList.Count; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (_UsersSubjectList[i].Times[j] != "")
                        result.Add(_UsersSubjectList[i].Times[j]);
                }
            }
            return result;
        }
        private List<string> SubjectInList(List<Subject> _UsersSubjectList) //유저가 듣는 과목을 string으로
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _UsersSubjectList.Count; i++)
            {
                    result.Add(_UsersSubjectList[i].ClassName);
            }
            return result;
        }
        private void InitDB() //TimeTable 초기화
        {
            for (int _row = 0; _row < 14; _row++)
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
            int period1 = 0;   string day1 = null;
            int period2 = 0; ; string day2 = null;
            int period3 = 0; ; string day3 = null;
            int period4 = 0; ; string day4 = null;
            int period5 = 0; ; string day5 = null;
            int period6 = 0; ; string day6 = null;
            int period7 = 0; ; string day7 = null;
            int period8 = 0; ; string day8 = null;

            string[] daylist = new string[] { day1, day2, day3, day4, day5, day6, day7, day8};

            int[] period = new int[] {period1, period2, period3, period4, period5, period6, period7, period8};

            TextBlock[,] schedule = new TextBlock[,]
            {
                {mon1, mon2, mon3, mon4, mon5, mon6, mon7, mon8, mon9, mon10, mon11, mon12, mon13},
                {tue1, tue2, tue3, tue4, tue5, tue6, tue7, tue8, tue9, tue10, tue11, tue12, tue13},
                {wed1, wed2, wed3, wed4, wed5, wed6, wed7, wed8, wed9, wed10, wed11, wed12, wed13},
                {thu1, thu2, thu3, thu4, thu5, thu6, thu7, thu8, thu9, thu10, thu11, thu12, thu13},
                {fri1, fri2, fri3, fri4, fri5, fri6, fri7, fri8, fri9, fri10, fri11, fri12, fri13},
                {sat1, sat2, sat3, sat4, sat5, sat6, sat7, sat8, sat9, sat10, sat11, sat12, sat13},
            };

            InitDB();  //초기화
            //이 이하로는 다 하얗게 만들기

            for(int i=0; i<6; i++)
            {
                for(int j=0; j<13; j++)
                {
                    schedule[i, j].Text = string.Empty;
                    schedule[i, j].Background = Brushes.White;
                }
            }
            
            //여기부터는 User의 과목들을 색칠, 2차원 배열에 넣기
            for (int i = 0; i < UsersSubjectsList.Count(); i++)
            {
                string[] time = new string[] { UsersSubjectsList[i].Time1, UsersSubjectsList[i].Time2, UsersSubjectsList[i].Time3, UsersSubjectsList[i].Time4, UsersSubjectsList[i].Time5, UsersSubjectsList[i].Time6, UsersSubjectsList[i].Time7, UsersSubjectsList[i].Time8};
                for(int j=0; j<8; j++)
                {
                    if(time[j] != "")
                    {
                        int _day = 0;
                        int _period = Int32.Parse(time[j].Substring(1, time[j].Length - 1)); //time의 교시 뽑기
                        string s_day = time[j].Substring(0, 1);

                        if (s_day == "월") _day = 0;
                        else if (s_day == "화") _day = 1;
                        else if (s_day == "수") _day = 2;
                        else if (s_day == "목") _day = 3;
                        else if (s_day == "금") _day = 4;
                        else if (s_day == "토") _day = 5;

                        _period -= 1;

                        TimeTableDB[_period + 1, _day + 1].ableToPut = false;
                        TimeTableDB[_period + 1, _day + 1].className = UsersSubjectsList[i].ClassName;
                        TimeTableDB[_period + 1, _day + 1].professor = UsersSubjectsList[i].Professor;
                        Run className = new Run(TimeTableDB[_period + 1, _day + 1].className + "\n");
                        className.FontSize = 12;
                        className.FontWeight = FontWeights.Bold;
                        schedule[_day, _period].Inlines.Add(className);
                        Run professor = new Run(TimeTableDB[_period + 1, _day + 1].professor);
                        professor.FontSize = 10;
                        schedule[_day, _period].Inlines.Add(professor);
                        schedule[_day, _period].Background = (Brush)properties[UsersSubjectsList[i].NO % 141].GetValue(null, null);
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
            GetTimeTable();
            MyGroup_cob.ItemsSource = userTimeTable;
        }

        private void MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            TextBlock[,] _schedule = new TextBlock[,]
            {
                {mon1, mon2, mon3, mon4, mon5, mon6, mon7, mon8, mon9, mon10, mon11, mon12, mon13},
                {tue1, tue2, tue3, tue4, tue5, tue6, tue7, tue8, tue9, tue10, tue11, tue12, tue13},
                {wed1, wed2, wed3, wed4, wed5, wed6, wed7, wed8, wed9, wed10, wed11, wed12, wed13},
                {thu1, thu2, thu3, thu4, thu5, thu6, thu7, thu8, thu9, thu10, thu11, thu12, thu13},
                {fri1, fri2, fri3, fri4, fri5, fri6, fri7, fri8, fri9, fri10, fri11, fri12, fri13},
                {sat1, sat2, sat3, sat4, sat5, sat6, sat7, sat8, sat9, sat10, sat11, sat12, sat13},
            };

            System.Windows.Controls.Button[,] schedule = new System.Windows.Controls.Button[,]
            {
                {mon1_btn, mon2_btn, mon3_btn, mon4_btn, mon5_btn, mon6_btn, mon7_btn, mon8_btn, mon9_btn, mon10_btn, mon11_btn, mon12_btn, mon13_btn},
                {tue1_btn, tue2_btn, tue3_btn, tue4_btn, tue5_btn, tue6_btn, tue7_btn, tue8_btn, tue9_btn, tue10_btn, tue11_btn, tue12_btn, tue13_btn},
                {wed1_btn, wed2_btn, wed3_btn, wed4_btn, wed5_btn, wed6_btn, wed7_btn, wed8_btn, wed9_btn, wed10_btn, wed11_btn, wed12_btn, wed13_btn},
                {thu1_btn, thu2_btn, thu3_btn, thu4_btn, thu5_btn, thu6_btn, thu7_btn, thu8_btn, thu9_btn, thu10_btn, thu11_btn, thu12_btn, thu13_btn},
                {fri1_btn, fri2_btn, fri3_btn, fri4_btn, fri5_btn, fri6_btn, fri7_btn, fri8_btn, fri9_btn, fri10_btn, fri11_btn, fri12_btn, fri13_btn},
                {sat1_btn, sat2_btn, sat3_btn, sat4_btn, sat5_btn, sat6_btn, sat7_btn, sat8_btn, sat9_btn, sat10_btn, sat11_btn, sat12_btn, sat13_btn},
            };
            var panel = sender as TextBlock;

            int week = 0;
            int period = 0;

            for (int i=0; i<6; i++)
            {
                for(int j=0; j<13; j++)
                {
                    if(_schedule[i,j].Name == panel.Name)
                    {
                        week = i + 1;
                        period = j + 1;
                    }
                }
            }
            
            if (TimeTableDB[period, week].ableToPut == false)
                schedule[week - 1, period - 1].Visibility = Visibility.Visible;

        }
        private void Btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button[,] schedule = new System.Windows.Controls.Button[,]
            {
                {mon1_btn, mon2_btn, mon3_btn, mon4_btn, mon5_btn, mon6_btn, mon7_btn, mon8_btn, mon9_btn, mon10_btn, mon11_btn, mon12_btn, mon13_btn},
                {tue1_btn, tue2_btn, tue3_btn, tue4_btn, tue5_btn, tue6_btn, tue7_btn, tue8_btn, tue9_btn, tue10_btn, tue11_btn, tue12_btn, tue13_btn},
                {wed1_btn, wed2_btn, wed3_btn, wed4_btn, wed5_btn, wed6_btn, wed7_btn, wed8_btn, wed9_btn, wed10_btn, wed11_btn, wed12_btn, wed13_btn},
                {thu1_btn, thu2_btn, thu3_btn, thu4_btn, thu5_btn, thu6_btn, thu7_btn, thu8_btn, thu9_btn, thu10_btn, thu11_btn, thu12_btn, thu13_btn},
                {fri1_btn, fri2_btn, fri3_btn, fri4_btn, fri5_btn, fri6_btn, fri7_btn, fri8_btn, fri9_btn, fri10_btn, fri11_btn, fri12_btn, fri13_btn},
                {sat1_btn, sat2_btn, sat3_btn, sat4_btn, sat5_btn, sat6_btn, sat7_btn, sat8_btn, sat9_btn, sat10_btn, sat11_btn, sat12_btn, sat13_btn},
            };
            var panel = sender as System.Windows.Controls.Button;

            int week = 0;
            int period = 0;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (schedule[i, j].Name == panel.Name)
                    {
                        week = i + 1;
                        period = j + 1;
                    }
                }
            }

            if (TimeTableDB[period, week].ableToPut == false)
                schedule[week - 1, period - 1].Visibility = Visibility.Visible;

        }
        private void MouseLeave (object sender, System.Windows.Input.MouseEventArgs e)
        {

            TextBlock[,] _schedule = new TextBlock[,]
            {
                {mon1, mon2, mon3, mon4, mon5, mon6, mon7, mon8, mon9, mon10, mon11, mon12, mon13},
                {tue1, tue2, tue3, tue4, tue5, tue6, tue7, tue8, tue9, tue10, tue11, tue12, tue13},
                {wed1, wed2, wed3, wed4, wed5, wed6, wed7, wed8, wed9, wed10, wed11, wed12, wed13},
                {thu1, thu2, thu3, thu4, thu5, thu6, thu7, thu8, thu9, thu10, thu11, thu12, thu13},
                {fri1, fri2, fri3, fri4, fri5, fri6, fri7, fri8, fri9, fri10, fri11, fri12, fri13},
                {sat1, sat2, sat3, sat4, sat5, sat6, sat7, sat8, sat9, sat10, sat11, sat12, sat13},
            };

            System.Windows.Controls.Button[,] schedule = new System.Windows.Controls.Button[,]
            {
                {mon1_btn, mon2_btn, mon3_btn, mon4_btn, mon5_btn, mon6_btn, mon7_btn, mon8_btn, mon9_btn, mon10_btn, mon11_btn, mon12_btn, mon13_btn},
                {tue1_btn, tue2_btn, tue3_btn, tue4_btn, tue5_btn, tue6_btn, tue7_btn, tue8_btn, tue9_btn, tue10_btn, tue11_btn, tue12_btn, tue13_btn},
                {wed1_btn, wed2_btn, wed3_btn, wed4_btn, wed5_btn, wed6_btn, wed7_btn, wed8_btn, wed9_btn, wed10_btn, wed11_btn, wed12_btn, wed13_btn},
                {thu1_btn, thu2_btn, thu3_btn, thu4_btn, thu5_btn, thu6_btn, thu7_btn, thu8_btn, thu9_btn, thu10_btn, thu11_btn, thu12_btn, thu13_btn},
                {fri1_btn, fri2_btn, fri3_btn, fri4_btn, fri5_btn, fri6_btn, fri7_btn, fri8_btn, fri9_btn, fri10_btn, fri11_btn, fri12_btn, fri13_btn},
                {sat1_btn, sat2_btn, sat3_btn, sat4_btn, sat5_btn, sat6_btn, sat7_btn, sat8_btn, sat9_btn, sat10_btn, sat11_btn, sat12_btn, sat13_btn},
            };
            var panel = sender as TextBlock;

            int week = 0;
            int period = 0;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (_schedule[i, j].Name == panel.Name)
                    {
                        week = i + 1;
                        period = j + 1;
                    }
                }
            }

            schedule[week - 1, period - 1].Visibility = Visibility.Collapsed;

        }
        private void Btn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button[,] schedule = new System.Windows.Controls.Button[,]
            {
                {mon1_btn, mon2_btn, mon3_btn, mon4_btn, mon5_btn, mon6_btn, mon7_btn, mon8_btn, mon9_btn, mon10_btn, mon11_btn, mon12_btn, mon13_btn},
                {tue1_btn, tue2_btn, tue3_btn, tue4_btn, tue5_btn, tue6_btn, tue7_btn, tue8_btn, tue9_btn, tue10_btn, tue11_btn, tue12_btn, tue13_btn},
                {wed1_btn, wed2_btn, wed3_btn, wed4_btn, wed5_btn, wed6_btn, wed7_btn, wed8_btn, wed9_btn, wed10_btn, wed11_btn, wed12_btn, wed13_btn},
                {thu1_btn, thu2_btn, thu3_btn, thu4_btn, thu5_btn, thu6_btn, thu7_btn, thu8_btn, thu9_btn, thu10_btn, thu11_btn, thu12_btn, thu13_btn},
                {fri1_btn, fri2_btn, fri3_btn, fri4_btn, fri5_btn, fri6_btn, fri7_btn, fri8_btn, fri9_btn, fri10_btn, fri11_btn, fri12_btn, fri13_btn},
                {sat1_btn, sat2_btn, sat3_btn, sat4_btn, sat5_btn, sat6_btn, sat7_btn, sat8_btn, sat9_btn, sat10_btn, sat11_btn, sat12_btn, sat13_btn},
            };
            var panel = sender as System.Windows.Controls.Button;

            int week = 0;
            int period = 0;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (schedule[i, j].Name == panel.Name)
                    {
                        week = i + 1;
                        period = j + 1;
                    }
                }
            }

            schedule[week - 1, period - 1].Visibility = Visibility.Collapsed;

        }
        private void btn_Click(object sender, RoutedEventArgs e) //x버튼 한번 클릭
        {
            System.Windows.Controls.Button[,] schedule = new System.Windows.Controls.Button[,]
            {
                {mon1_btn, mon2_btn, mon3_btn, mon4_btn, mon5_btn, mon6_btn, mon7_btn, mon8_btn, mon9_btn, mon10_btn, mon11_btn, mon12_btn, mon13_btn},
                {tue1_btn, tue2_btn, tue3_btn, tue4_btn, tue5_btn, tue6_btn, tue7_btn, tue8_btn, tue9_btn, tue10_btn, tue11_btn, tue12_btn, tue13_btn},
                {wed1_btn, wed2_btn, wed3_btn, wed4_btn, wed5_btn, wed6_btn, wed7_btn, wed8_btn, wed9_btn, wed10_btn, wed11_btn, wed12_btn, wed13_btn},
                {thu1_btn, thu2_btn, thu3_btn, thu4_btn, thu5_btn, thu6_btn, thu7_btn, thu8_btn, thu9_btn, thu10_btn, thu11_btn, thu12_btn, thu13_btn},
                {fri1_btn, fri2_btn, fri3_btn, fri4_btn, fri5_btn, fri6_btn, fri7_btn, fri8_btn, fri9_btn, fri10_btn, fri11_btn, fri12_btn, fri13_btn},
                {sat1_btn, sat2_btn, sat3_btn, sat4_btn, sat5_btn, sat6_btn, sat7_btn, sat8_btn, sat9_btn, sat10_btn, sat11_btn, sat12_btn, sat13_btn},
            };

            string[,] day_time = new string[,]
            {
                {"월1", "월2", "월3", "월4", "월5", "월6", "월7", "월8", "월9", "월10", "월11", "월12", "월13"},
                {"화1", "화2", "화3", "화4", "화5", "화6", "화7", "화8", "화9", "화10", "화11", "화12", "화13"},
                {"수1", "수2", "수3", "수4", "수5", "수6", "수7", "수8", "수9", "수10", "수11", "수12", "수13"},
                {"목1", "목2", "목3", "목4", "목5", "목6", "목7", "목8", "목9", "목10", "목11", "목12", "목13"},
                {"금1", "금2", "금3", "금4", "금5", "금6", "금7", "금8", "금9", "금10", "금11", "금12", "금13"},
                {"토1", "토2", "토3", "토4", "토5", "토6", "토7", "토8", "토9", "토10", "토11", "토12", "토13"}
            };

            var panel = sender as System.Windows.Controls.Button;

            int week = 0;
            int period = 0;

            for(int i=0; i<6; i++)
            {
                for(int j=0; j<13; j++)
                {
                    if(panel.Name == schedule[i, j].Name)
                    {
                        week = i + 1;
                        period = j + 1;
                    }
                }
            }

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(TimeTableDB[period, week].className + "를 삭제하시겠습니까?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            //메세지창 띄우기
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSubjectInTimeTable(day_time[week-1, period-1]);
                RefreshTimeTable(); //새로고침
            }
            schedule[week-1, period-1].Visibility = Visibility.Collapsed;
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
            System.Windows.MessageBox.Show("Value : " + userTimeTable[MyGroup_cob.SelectedIndex]);
        }

        private void MyGroup_cob_Initialized(object sender, EventArgs e)
        {
            //userTimeTable.Add("A");
            //userTimeTable.Add("B");
            //MyGroup_cob.ItemsSource = data;
        }
        
               

        private void Window_Activated(object sender, EventArgs e)
        {
            GetTimeTable();
            TableList.ItemsSource = userTimeTable;
        }

        private void TableList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = TableList.SelectedIndex;
            string tableName = userTimeTable[index].TimeTableName;
            GetTimeTableClassNumber(tableName);
            UsersSubjectsList.Clear();
            for(int i=0;i<timeTableClassNumber.Count;i++)
            {
                UsersSubjectsList.Add(SubjectList.Where(s => s.ClassNumber.Contains(timeTableClassNumber[i].ClassNumber)).ToList().ElementAt(0));                
            }
            RefreshTimeTable();
        }
    }
}