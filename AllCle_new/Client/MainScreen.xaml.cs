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
using System.IO;
using System.Windows.Media.Animation;
using System.Threading;

namespace Client
{
    /// <summary>
    /// MainScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainScreen : Window
    {
        static HttpClient client = new HttpClient();
        List<Subject> subjectList = new List<Subject>(); //전체 과목 리스트
        List<Subject> resultSubtject = new List<Subject>();     //결과 과목
        List<Subject> usersSubjectsList = new List<Subject>(); //유저가 듣는 과목 리스트
        List<UserTimeTable> userTimeTable = new List<UserTimeTable>();    //유저의 시간표
        List<string> timeTableClassNumber = new List<string>(); //시간표의 과목들
        List<UserMyGroup> userMyGroup = new List<UserMyGroup>();    //유저의 myGroup
        List<string> myGroupClassNumber = new List<string>();   //하나의 myGroup안에 과목들
    

        List<ABEEK> aBEEKs = new List<ABEEK>();             //아빅 전체
        List<ABEEK> resultABEEKs = new List<ABEEK>();       //아빅에서 원하는거만 선택
        List<EngNormal> engNormals = new List<EngNormal>(); //공대 일반교양 전체
        List<EngNormal> resultEngNormals = new List<EngNormal>(); //공대 일반교양 선택
        List<Major> majors = new List<Major>(); //전공 전체
        List<Major> resultMajors = new List<Major>(); //전공에서 원하는거 선택

        string[] br = { "#FFCFCFD6", "#FFB1D3D2", "#FF8BC6C1", "#FF2FA5B8", "#FFF4AFA1", "#FFF7B990", "#FF86C26D", "#FF6DE182", "#FFEDE8AD" };
        /*색 리스트*/
        User user = new User();
        bool tabActive;             //tab bar active or not
        bool myGroup_btn;           //마이그룹 활성화
        string tableName = "";      //현재 테이블 이름
        int userTimeTalbeNO = 0;    //유저 타임테이블
        int groupNum = 0;

        public struct TableSubjects //시간표 한칸의 Data
        {
            public string className;
            public string professor;
            public bool ableToPut;
        }
        TableSubjects[,] TimeTableDB = new TableSubjects[14, 7]; //12교시*일주일 2차원 배열
        string urlBase = @"https://allcleapp.azurewebsites.net/api/AllCleSubjects1"; //기본 url
        string urlUserTimeTable = @"https://allcleapp.azurewebsites.net/api/UserTimeTable"; //유저의 시간표 리스트를 위한 기본 url
        string urlTimeTableClassNumber = @"https://allcleapp.azurewebsites.net/api/TimeTableClassNumber";       //저장된 시간표의 과목들
        string urlUserMyGroup = @"https://allcleapp.azurewebsites.net/api/UserMyGroup";
        string urlMyGroupClassNumber = @"https://allcleapp.azurewebsites.net/api/MyGroupClassNumber";       //저장된 MyGroup의 과목들
        string urlABEEK = @"https://allcleapp.azurewebsites.net/api/ABEEK";
        string urlEngNormal = @"https://allcleapp.azurewebsites.net/api/EngNormal";
        string urlMajor = @"https://allcleapp.azurewebsites.net/api/Major";
        string url = null;  //json으로 쓰일 url

        public MainScreen()
        {
            InitializeComponent();
            GetSubjects();
            GetMajor();
            DataListView_All.ItemsSource = subjectList;
            InitDB();
            tabActive = false;
            myGroup_btn = false;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            GetUserTimeTable();
            TableList.ItemsSource = userTimeTable;
            SetUserMyGroup();
            UserId.Text = App.ID;
            if (!App.guest)
                InitUserInfo();
            else if (App.guest)
                GuestLogIn();

        }


        private void InitUserInfo()
        {
            url = @"https://allcleapp.azurewebsites.net/api/Users/" + App.ID;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);// 인코딩 UTF-8
            byte[] sendData = new byte[0];
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = sendData.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Close();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            user = JsonConvert.DeserializeObject<User>(streamReader.ReadToEnd());
            streamReader.Close();
            httpWebResponse.Close();

            UserAdmissionYear.Text = "학번 : " + user.YearOfEntry;
            UserMajor.Text = user.Major;

            GetUserTimeTable();
            if (userTimeTable.Count() != 0)
            {
                //TableEdit_txtbox.Text = userTimeTable[0].TimeTableName;
                //GetTimeTableClassNumber(userTimeTable[0].NO);
                GetTimeTableClassNumber(userTimeTable[0].TimeTableName);
                RefreshTimeTable();
            }
            normal.Visibility = Visibility.Collapsed;
            engineer.Visibility = Visibility.Collapsed;
            architecture.Visibility = Visibility.Collapsed;
            if (user.College == "공과대학")
            {
                engineer.Visibility = Visibility.Visible;
                GetEngineerABEEK();
                GetEngineerNormal();
            }
            else if (user.College == "일반대학")
            {
                normal.Visibility = Visibility.Visible;

            }
            else
            {
                architecture.Visibility = Visibility.Visible;

            }

        }
        private void GuestLogIn()
        {
            UserId.Text = "Guest";
            UserAdmissionYear.Text = "Guest";
            UserMajor.Text = "Guest";
        }
        private List<Subject> SearchByWord(List<Subject> subjects)//검색함수
        {
            List<Subject> tempSubjects = new List<Subject>();
            tempSubjects = subjects;
            string _word = Search_Box.Text;
            if (_word == "" || _word == "교과명, 학수번호, 교수이름으로 검색하기")
                return tempSubjects;
            else
                tempSubjects = tempSubjects.Where(s => s.ClassName.Contains(_word) || s.Professor.Contains(_word) || s.ClassNumber.Contains(_word)).ToList();
            return tempSubjects;
        }
        private List<Subject> SearchByLunch(List<Subject> subjects)//점심시간
        {
            List<Subject> tempSubjects = new List<Subject>();
            tempSubjects = subjects;
            string from = LunchFrom.Text;
            string to = LunchTo.Text;
            int fromToInt = 0;
            int toToInt = 0;

            if (from == "오전 9시")
                fromToInt = 1;
            else if (from == "오전 10시")
                fromToInt = 2;
            else if (from == "오전 11시")
                fromToInt = 3;
            else if (from == "오전 12시")
                fromToInt = 4;
            else if (from == "오후 1시")
                fromToInt = 5;
            else if (from == "오후 2시")
                fromToInt = 6;
            else if (from == "오후 3시")
                fromToInt = 7;
            else if (from == "오후 4시")
                fromToInt = 8;
            else if (from == "오후 5시")
                fromToInt = 9;
            else if (from == "오후 6시")
                fromToInt = 10;
            else if (from == "오후 7시")
                fromToInt = 11;
            else if (from == "오후 8시")
                fromToInt = 12;
            else
                fromToInt = -1;

            if (to == "오전 9시")
                toToInt = 1;
            else if (to == "오전 10시")
                toToInt = 2;
            else if (to == "오전 11시")
                toToInt = 3;
            else if (to == "오전 12시")
                toToInt = 4;
            else if (to == "오후 1시")
                toToInt = 5;
            else if (to == "오후 2시")
                toToInt = 6;
            else if (to == "오후 3시")
                toToInt = 7;
            else if (to == "오후 4시")
                toToInt = 8;
            else if (to == "오후 5시")
                toToInt = 9;
            else if (to == "오후 6시")
                toToInt = 10;
            else if (to == "오후 7시")
                toToInt = 11;
            else if (to == "오후 8시")
                toToInt = 12;

            if (fromToInt == -1)
                return tempSubjects;
            else
            {
                Console.WriteLine(fromToInt + " ~ " + toToInt);
                if (toToInt <= fromToInt)
                {
                    System.Windows.MessageBox.Show("숫자를 잘못설정하였습니다");
                    return tempSubjects;
                }
                else
                {
                    for (int i = 0; i < toToInt - fromToInt; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            tempSubjects = tempSubjects.Where(s => !s.Times[j].Equals("월" + (fromToInt + i).ToString())).ToList();
                            tempSubjects = tempSubjects.Where(s => !s.Times[j].Equals("화" + (fromToInt + i).ToString())).ToList();
                            tempSubjects = tempSubjects.Where(s => !s.Times[j].Equals("수" + (fromToInt + i).ToString())).ToList();
                            tempSubjects = tempSubjects.Where(s => !s.Times[j].Equals("목" + (fromToInt + i).ToString())).ToList();
                            tempSubjects = tempSubjects.Where(s => !s.Times[j].Equals("금" + (fromToInt + i).ToString())).ToList();
                            tempSubjects = tempSubjects.Where(s => !s.Times[j].Equals("토" + (fromToInt + i).ToString())).ToList();
                        }
                    }
                }
                return tempSubjects;
            }
        }
        private List<Subject> SearchByDay(List<Subject> subjects)//공강만들기
        {
            List<Subject> tempSubjects = new List<Subject>();
            tempSubjects = subjects;
            if (rdo_mon.IsChecked == true)
            {
                for (int j = 0; j < 8; j++)
                    for (int i = 1; i <= 13; i++)                    
                        tempSubjects = tempSubjects.Where(s => !s.Times[j].Contains("월" + i.ToString())).ToList();                    
            }
            if (rdo_tue.IsChecked == true)
            {
                for (int j = 0; j < 8; j++)
                    for (int i = 1; i <= 13; i++)
                        tempSubjects = tempSubjects.Where(s => !s.Times[j].Contains("화" + i.ToString())).ToList();
            }
            if (rdo_wed.IsChecked == true)
            {
                for (int j = 0; j < 8; j++)
                    for (int i = 1; i <= 13; i++)
                        tempSubjects = tempSubjects.Where(s => !s.Times[j].Contains("수" + i.ToString())).ToList();
            }
            if (rdo_thu.IsChecked == true)
            {
                for (int j = 0; j < 8; j++)
                    for (int i = 1; i <= 13; i++)
                        tempSubjects = tempSubjects.Where(s => !s.Times[j].Contains("목" + i.ToString())).ToList();
            }
            if (rdo_fri.IsChecked == true)
            {
                for (int j = 0; j < 8; j++)
                    for (int i = 1; i <= 13; i++)
                        tempSubjects = tempSubjects.Where(s => !s.Times[j].Contains("금" + i.ToString())).ToList();
            }
            if (rdo_sat.IsChecked == true)
            {
                for (int j = 0; j < 8; j++)
                    for (int i = 1; i <= 13; i++)
                        tempSubjects = tempSubjects.Where(s => !s.Times[j].Contains("토" + i.ToString())).ToList();
            }
           
            return tempSubjects;

        }
        private List<Subject> SearchByEmpty(List<Subject> subjects)//빈시간만 적용
        {
            
            List<Subject> tempSubjects = new List<Subject>();
            tempSubjects = subjects;
            if (TimeOp_cbx.Text == "전체 시간")
                return tempSubjects;
            List<string> tempStrings = new List<string>();
            tempStrings = TimeInList(usersSubjectsList);
            for (int i = 0; i < tempStrings.Count; i++)
            {
                Console.WriteLine(tempStrings[i] + "  ");
                for (int j = 0; j < 8; j++)
                    tempSubjects = tempSubjects.Where(s => !s.Times[j].Contains(tempStrings[i])).ToList();
            }
            return tempSubjects;
        }
        private void SearchEngNormal_All()  //공대 교양 전체 찾기
        {
            resultABEEKs = aBEEKs;
            List<Subject> tempSubjects = new List<Subject>();
            for (int i = 0; i < resultABEEKs.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultABEEKs[i].ClassNumber)).ToList());
            }
            resultEngNormals = engNormals;
            for (int i = 0; i < resultEngNormals.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultEngNormals[i].ClassNumber)).ToList());
            }
           // tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            //tempSubjects = SearchByWord(tempSubjects);//이런곳에 검색함수 넣으면 됨 tempsubject
            DataListView_All.ItemsSource = tempSubjects;
        }
        private void SearchABEEKAll()       //공대 아빅 전체 찾기
        {
            resultABEEKs = aBEEKs;
            List<Subject> tempSubjects = new List<Subject>();
            for (int i = 0; i < resultABEEKs.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultABEEKs[i].ClassNumber)).ToList());
            }
            tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            tempSubjects = SearchByWord(tempSubjects);//이런곳에 검색함수 넣으면 됨 tempsubject
            tempSubjects = SearchByDay(tempSubjects);   //공강만들기
            tempSubjects = SearchByEmpty(tempSubjects); //빈시간만 적용
            resultSubtject = tempSubjects;
            DataListView_All.ItemsSource = tempSubjects;
        }
        private void SearchABEEKType(string _type)  //공대 아빅에서 MSC과학같은거 찾기
        {
            resultABEEKs = aBEEKs.Where(s => s.ABEEKType.Equals(_type)).ToList();
            List<Subject> tempSubjects = new List<Subject>();
            for (int i = 0; i < resultABEEKs.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultABEEKs[i].ClassNumber)).ToList());
            }
            tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            tempSubjects = SearchByWord(tempSubjects);//이런곳에 검색함수 넣으면 됨 tempsubject
            tempSubjects = SearchByDay(tempSubjects);   //공강만들기
            tempSubjects = SearchByEmpty(tempSubjects); //빈시간만 적용
            resultSubtject = tempSubjects;
            DataListView_All.ItemsSource = tempSubjects;
        }
        private void SearchABEEKNormal(string _normal)  //공대 아빅 드래곤볼 분야 찾기
        {
            resultABEEKs = aBEEKs.Where(s => s.ABEEKNormal.Equals(_normal)).ToList();
            List<Subject> tempSubjects = new List<Subject>();
            for (int i = 0; i < resultABEEKs.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultABEEKs[i].ClassNumber)).ToList());
            }
            tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            tempSubjects = SearchByWord(tempSubjects);//이런곳에 검색함수 넣으면 됨 tempsubject
            tempSubjects = SearchByDay(tempSubjects);   //공강만들기
            tempSubjects = SearchByEmpty(tempSubjects); //빈시간만 적용
            resultSubtject = tempSubjects;
            DataListView_All.ItemsSource = tempSubjects;
        }
        private void SearchEngNormalAll()
        {
            resultEngNormals = engNormals;
            List<Subject> tempSubjects = new List<Subject>();
            for (int i = 0; i < resultEngNormals.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultEngNormals[i].ClassNumber)).ToList());
            }
            tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            tempSubjects = SearchByWord(tempSubjects);//이런곳에 검색함수 넣으면 됨 tempsubject
            tempSubjects = SearchByDay(tempSubjects);   //공강만들기
            tempSubjects = SearchByEmpty(tempSubjects); //빈시간만 적용
            resultSubtject = tempSubjects;
            DataListView_All.ItemsSource = tempSubjects;
        }
        private void SearchEngNormal(string _normal)    //공대 일반교양에서 분야
        {
            resultEngNormals = engNormals.Where(s => s.ABEEKNormal.Equals(_normal)).ToList();
            List<Subject> tempSubjects = new List<Subject>();
            for (int i = 0; i < resultEngNormals.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultEngNormals[i].ClassNumber)).ToList());
            }
            tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            tempSubjects = SearchByWord(tempSubjects);//이런곳에 검색함수 넣으면 됨 tempsubject
            tempSubjects = SearchByDay(tempSubjects);   //공강만들기
            tempSubjects = SearchByEmpty(tempSubjects); //빈시간만 적용
            resultSubtject = tempSubjects;
            DataListView_All.ItemsSource = tempSubjects;
        }
        

        //여기 일반대, 건축대 자리

        private void SearchMajor(string _major)
        {
            resultMajors = majors.Where(s => s.ByMajor.Equals(_major)).ToList();
            List<Subject> tempSubjects = new List<Subject>();
            for (int i = 0; i < resultMajors.Count; i++)
            {
                tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultMajors[i].ClassNumber)).ToList());
            }
            tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            tempSubjects = SearchByWord(tempSubjects);//이런곳에 검색함수 넣으면 됨 tempsubject
            tempSubjects = SearchByDay(tempSubjects);   //공강만들기
            tempSubjects = SearchByEmpty(tempSubjects); //빈시간만 적용
            resultSubtject = tempSubjects;
            DataListView_All.ItemsSource = tempSubjects;
        }
        private void SearchMajor(List<string> _major)
        {
            List<Subject> tempSubjects = new List<Subject>();
            for (int j = 0; j < _major.Count; j++)
            {
                resultMajors = majors.Where(s => s.ByMajor.Equals(_major[0])).ToList();
                for (int i = 0; i < resultMajors.Count; i++)
                {
                    tempSubjects.AddRange(subjectList.Where(s => s.ClassNumber.Contains(resultMajors[i].ClassNumber)).ToList());
                }
            }
            tempSubjects = SearchByLunch(tempSubjects); //점심시간 제외
            tempSubjects = SearchByWord(tempSubjects);  //이런곳에 검색함수 넣으면 됨 tempsubject
            tempSubjects = SearchByDay(tempSubjects);   //공강만들기
            tempSubjects = SearchByEmpty(tempSubjects); //빈시간만 적용
            resultSubtject = tempSubjects;
            DataListView_All.ItemsSource = tempSubjects;
        }
        private void SearchMajor()
        {
            if (major.Text == "공과대학")
            {
                if (major_engineer.Text == "전공공통")
                {
                    List<string> tempSubject = new List<string>() { "공대교학과", "기초과학과(서울)", "전공공통" };
                    SearchMajor(tempSubject);
                }
                else if (major_engineer.Text == "건설도시공학부")
                {
                    List<string> tempSubject = new List<string>() { "건설·도시공학부", "도시공학전공", "토목공학전공" };
                    SearchMajor(tempSubject);
                }
                else if (major_engineer.Text == "전자전기공학부")
                    SearchMajor("전자·전기공학부");
                else if (major_engineer.Text == "정보컴퓨터공학부")
                {
                    List<string> tempSubject = new List<string>() { "컴퓨터공학전공", "산업공학전공" };
                    SearchMajor(tempSubject);
                }
                else if (major_engineer.Text == "신소재화공시스템공학부")
                {
                    List<string> tempSubject = new List<string>() { "신소재·화공시스템공학부", "화학공학전공", "신소재공학전공" };
                    SearchMajor(tempSubject);
                }
                else if (major_engineer.Text == "기계시스템디자인공학과")
                    SearchMajor("기계ㆍ시스템디자인공학과");
            }
            else if (major.Text == "경영대학")
            {
                if (major_business.Text == "경영학부")
                {
                    List<string> tempSubject = new List<string>() { "경영학부", "경영학전공" };
                    SearchMajor(tempSubject);
                }
            }
            else if (major.Text == "문과대학")
            {
                if (major_language.Text == "영어영문학과")
                    SearchMajor("영어영문학과");
                else if (major_language.Text == "독어독문학과")
                    SearchMajor("독어독문학과");
                if (major_language.Text == "불어불문학과")
                    SearchMajor("불어불문학과");
                else if (major_language.Text == "국어국문학과")
                    SearchMajor("국어국문학과");
            }
            else if (major.Text == "사범대학")
            {
                if (major_language.Text == "교직과(서울)")
                    SearchMajor("교직과(서울)");
                else if (major_language.Text == "수학교육과")
                    SearchMajor("수학교육과");
                if (major_language.Text == "국어교육과")
                    SearchMajor("국어교육과");
                else if (major_language.Text == "영어교육과")
                    SearchMajor("영어교육과");
                if (major_language.Text == "역사교육과")
                    SearchMajor("역사교육과");
                else if (major_language.Text == "교육학과")
                    SearchMajor("교육학과");
            }
            else if (major.Text == "미술대학")
            {
                if (major_language.Text == "전공공통")
                    SearchMajor("전공공통");
                else if (major_language.Text == "수학교육과")
                    SearchMajor("수학교육과");
                else if (major_language.Text == "동양학과")
                    SearchMajor("동양학과");
                else if (major_language.Text == "회화과")
                    SearchMajor("회화과");
                else if (major_language.Text == "판화과")
                    SearchMajor("판화과");
                else if (major_language.Text == "조소과")
                    SearchMajor("조소과");
                else if (major_language.Text == "목조형가구학과")
                    SearchMajor("목조형가구학과");
                else if (major_language.Text == "예술학과")
                    SearchMajor("예술학과");
                else if (major_language.Text == "금속디자인학과")
                    SearchMajor("금속디자인학과");
                else if (major_language.Text == "도유유리과")
                    SearchMajor("도유유리과");
                else if (major_language.Text == "섬유미술패션디자인학과")
                    SearchMajor("섬유미술패션디자인학과");
                else if (major_language.Text == "디자인학부")
                    SearchMajor("디자인학부");
            }
            else if (major.Text == "건축대학")
            {
                if (major_architecture.Text == "건축학부")
                    SearchMajor("건축학부");
            }
            else if (major.Text == "법과대학")
            {
                if (major_law.Text == "전공공통")
                    SearchMajor("법과대 교학과");
                if (major_law.Text == "법학부")
                {
                    List<string> tempSubject = new List<string>() { "법학부", "사법(비즈니스·정보법)전공", "공법(공공서비스법)전공" };
                    SearchMajor(tempSubject);
                }
            }
            else if (major.Text == "경제학부")
                if (major_economics.Text == "경제학부")
                {
                    List<string> tempSubject = new List<string>() { "경제학전공", "경제학부" };
                    SearchMajor(tempSubject);
                }
        }

        private void SearchEngineer()   //공대 search 함수
        {
            if (course.Text == "교양")
            {
                if(engineer.Text == "전체")
                {
                    SearchEngNormal_All();
                }
                else if(engineer.Text == "ABEEK 교양")
                {
                    if(engineer_ABEEK.Text == "전체")
                        SearchABEEKAll();
                    else if (engineer_ABEEK.Text == "기초교양")
                    {
                        SearchABEEKType("교양필수");
                    }
                    else if (engineer_ABEEK.Text == "일반교양")
                    {
                        if (engineer_type_normal.Text == "언어와논리")
                            SearchABEEKType("언어와논리");
                        else if (engineer_type_normal.Text == "사회와경제")
                            SearchABEEKType("사회와경제");
                        else if (engineer_type_normal.Text == "역사와문화")
                            SearchABEEKType("역사와문화");
                        else if (engineer_type_normal.Text == "예술과철학")
                            SearchABEEKType("예술과철학");
                        else if (engineer_type_normal.Text == "제2외국어")
                            SearchABEEKType("제2외국어");
                        else
                            System.Windows.MessageBox.Show("교양->ABEEK->일반교양->에러");
                    }
                    else if (engineer_ABEEK.Text == "핵심교양")
                    {
                        if (engineer_type_mainpoint.Text == "경영과법률")
                            SearchABEEKType("경영과법률");
                        else if (engineer_type_mainpoint.Text == "공학의이해")
                            SearchABEEKType("공학의이해");
                        else if (engineer_type_mainpoint.Text == "의사소통")
                            SearchABEEKType("의사소통");
                        else
                            System.Windows.MessageBox.Show("교양->ABEEK->핵심교양->에러");
                    }
                    else if (engineer_ABEEK.Text == "MSC수학")
                        SearchABEEKNormal("MSC수학");
                    else if (engineer_ABEEK.Text == "MSC과학")
                        SearchABEEKNormal("MSC과학");
                    else if (engineer_ABEEK.Text == "MSC전산")
                        SearchABEEKNormal("MSC전산");
                    else
                    {
                        System.Windows.MessageBox.Show("교양->ABEEK->에러");
                    }
                }
                else if (engineer.Text == "일반교양")
                {
                    if (engineer_normal.Text == "전체")
                        SearchEngNormalAll();
                    else if (engineer_normal.Text == "인문계열")
                        SearchEngNormal("인문계열");
                    else if (engineer_normal.Text == "사회계열")
                        SearchEngNormal("사회계열");
                    else if (engineer_normal.Text == "자연계열")
                        SearchEngNormal("자연계열");
                    else if (engineer_normal.Text == "예체능계열")
                        SearchEngNormal("예체능계열");
                    else if (engineer_normal.Text == "제2외국어계열")
                        SearchEngNormal("제2외국어계열");
                    else if (engineer_normal.Text == "교직계열")
                        SearchEngNormal("교직계열");
                }
                else
                {
                    System.Windows.MessageBox.Show("교양->에러");
                }
            }
            else if (course.Text == "전공")
            {
                SearchMajor();
            }
            else
                System.Windows.MessageBox.Show("전공 vs 교양에서 오류 ㄷㄷ");

            /*if (TimeOp_cbx.Text == "전체 시간")
            {
                DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOff();
            }
            else if (TimeOp_cbx.Text == "빈 시간만 적용")
            {
                DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOff(TimeInList(UsersSubjectsList));
            }
            else
            {
                System.Windows.MessageBox.Show("hi");
            }*/


        }
        private void SearchNormal()     //일반대 search 함수
        {   //수정요망
            if (course.Text == "공통교양")
            {
                if (engineer.Text == "전체")
                {
                    SearchEngNormal_All();
                }
                else if (engineer.Text == "ABEEK 교양")
                {
                    if (engineer_ABEEK.Text == "전체")
                        SearchABEEKAll();
                    else if (engineer_ABEEK.Text == "기초교양")
                    {
                        SearchABEEKType("교양필수");
                    }
                    else if (engineer_ABEEK.Text == "일반교양")
                    {
                        if (engineer_type_normal.Text == "언어와논리")
                            SearchABEEKType("언어와논리");
                        else if (engineer_type_normal.Text == "사회와경제")
                            SearchABEEKType("사회와경제");
                        else if (engineer_type_normal.Text == "역사와문화")
                            SearchABEEKType("역사와문화");
                        else if (engineer_type_normal.Text == "예술과철학")
                            SearchABEEKType("예술과철학");
                        else if (engineer_type_normal.Text == "제2외국어")
                            SearchABEEKType("제2외국어");
                        else
                            System.Windows.MessageBox.Show("교양->ABEEK->일반교양->에러");
                    }
                    else if (engineer_ABEEK.Text == "핵심교양")
                    {
                        if (engineer_type_mainpoint.Text == "경영과법률")
                            SearchABEEKType("경영과법률");
                        else if (engineer_type_mainpoint.Text == "공학의이해")
                            SearchABEEKType("공학의이해");
                        else if (engineer_type_mainpoint.Text == "의사소통")
                            SearchABEEKType("의사소통");
                        else
                            System.Windows.MessageBox.Show("교양->ABEEK->핵심교양->에러");
                    }
                    else if (engineer_ABEEK.Text == "MSC수학")
                        SearchABEEKNormal("MSC수학");
                    else if (engineer_ABEEK.Text == "MSC과학")
                        SearchABEEKNormal("MSC과학");
                    else if (engineer_ABEEK.Text == "MSC전산")
                        SearchABEEKNormal("MSC전산");
                    else
                    {
                        System.Windows.MessageBox.Show("교양->ABEEK->에러");
                    }
                }
                else if (engineer.Text == "일반교양")
                {
                    if (engineer_normal.Text == "전체")
                        SearchEngNormalAll();
                    else if (engineer_normal.Text == "인문계열")
                        SearchEngNormal("인문계열");
                    else if (engineer_normal.Text == "사회계열")
                        SearchEngNormal("사회계열");
                    else if (engineer_normal.Text == "자연계열")
                        SearchEngNormal("자연계열");
                    else if (engineer_normal.Text == "예체능계열")
                        SearchEngNormal("예체능계열");
                    else if (engineer_normal.Text == "제2외국어계열")
                        SearchEngNormal("제2외국어계열");
                    else if (engineer_normal.Text == "교직계열")
                        SearchEngNormal("교직계열");
                }
                else
                {
                    System.Windows.MessageBox.Show("교양->에러");
                }
            }
            else if (course.Text == "전공")
            {
                SearchMajor();
            }
            else
                System.Windows.MessageBox.Show("전공 vs 교양에서 오류 ㄷㄷ");

            /*if (TimeOp_cbx.Text == "전체 시간")
            {
                DataListView_All.ItemsSource = ShowTimeOffSubjectOffSearchOff();
            }
            else if (TimeOp_cbx.Text == "빈 시간만 적용")
            {
                DataListView_All.ItemsSource = ShowTimeOnSubjectOffSearchOff(TimeInList(UsersSubjectsList));
            }
            else
            {
                System.Windows.MessageBox.Show("hi");
            }*/


        }
        private void SearchArchitecture()
        {//수정요망

        }
        private void GetSubjects()
        {
            url = urlBase;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            subjectList = JsonConvert.DeserializeObject<List<Subject>>(Unicode);
            resultSubtject = subjectList;
        }
        private void GetUserTimeTable()
        {
            url = urlUserTimeTable + "/" + App.ID;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            userTimeTable = JsonConvert.DeserializeObject<List<UserTimeTable>>(Unicode);
        }
        private void GetTimeTableClassNumber(String TimeTableName)
        {
            url = urlTimeTableClassNumber + "/" + App.ID + "/TimeTableName/" + TimeTableName;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            timeTableClassNumber = JsonConvert.DeserializeObject<List<string>>(Unicode);
        }
        private void GetUserMyGroup()
        {
            url = urlUserMyGroup + "/" + App.ID;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            userMyGroup = JsonConvert.DeserializeObject<List<UserMyGroup>>(Unicode);
        }
        private void GetmyGroupClassNumber(string MyGroupName)
        {
            url = urlMyGroupClassNumber + "/" + App.ID + "/MyGroupName/" + MyGroupName;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            myGroupClassNumber = JsonConvert.DeserializeObject<List<string>>(Unicode);
        }

        private void GetEngineerABEEK()
        {
            url = urlABEEK;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            aBEEKs = JsonConvert.DeserializeObject<List<ABEEK>>(Unicode);
        }
        private void GetEngineerNormal()
        {
            url = urlEngNormal;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            engNormals = JsonConvert.DeserializeObject<List<EngNormal>>(Unicode);

        }
        private void GetMajor()
        {
            url = urlMajor;
            var json = new WebClient().DownloadData(url);
            string Unicode = Encoding.UTF8.GetString(json);
            majors = JsonConvert.DeserializeObject<List<Major>>(Unicode);

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
        
        // 시간표 이미지 저장
        private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            Size visualSize = new Size(visual.ActualWidth, visual.ActualHeight);
            visual.Measure(visualSize);
            visual.Arrange(new Rect(visualSize));
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        private void SetUserMyGroup()   //MyGroup 불러오고, 화면에 세팅
        {
            TextBlock[] myGroupTbk = new TextBlock[] { Group1Name_tbk, Group2Name_tbk, Group3Name_tbk, Group4Name_tbk, Group5Name_tbk, Group6Name_tbk, Group7Name_tbk, Group8Name_tbk, Group9Name_tbk,  };
            System.Windows.Controls.ListView[] myGroupLst = new System.Windows.Controls.ListView[] { Group1_lst, Group2_lst, Group3_lst, Group4_lst, Group5_lst, Group6_lst, Group7_lst, Group8_lst, Group9_lst };
            Grid[] myGroupGrd = new Grid[] { Group1_grd, Group2_grd, Group3_grd, Group4_grd, Group5_grd, Group6_grd, Group7_grd, Group8_grd, Group9_grd };
            GetUserMyGroup();
            for (int i = 0; i < Math.Min(userMyGroup.Count,9); i++)
            {
                myGroupGrd[i].Visibility = Visibility;
                myGroupTbk[i].Text = userMyGroup[i].MyGroupName;
                GetmyGroupClassNumber(userMyGroup[i].MyGroupName);
                List<Subject> subjectFromMyGroup = new List<Subject>();            //MyGroup에서 가져온 과목들
                for (int j = 0; j < myGroupClassNumber.Count; j++)
                {
                    subjectFromMyGroup.AddRange(subjectList.Where(s => s.ClassNumber.Equals(myGroupClassNumber[j])).ToList());
                }
                myGroupLst[i].ItemsSource = subjectFromMyGroup;
            }

        }

        private void RefreshTimeTable() //시간표 갱신
        {
            Type brushesType = typeof(Brushes);
            PropertyInfo[] properties = brushesType.GetProperties();
            int period1 = 0; string day1 = null;
            int period2 = 0; ; string day2 = null;
            int period3 = 0; ; string day3 = null;
            int period4 = 0; ; string day4 = null;
            int period5 = 0; ; string day5 = null;
            int period6 = 0; ; string day6 = null;
            int period7 = 0; ; string day7 = null;
            int period8 = 0; ; string day8 = null;

            string[] daylist = new string[] { day1, day2, day3, day4, day5, day6, day7, day8 };

            int[] period = new int[] { period1, period2, period3, period4, period5, period6, period7, period8 };

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

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    schedule[i, j].Text = string.Empty;
                    schedule[i, j].Background = Brushes.White;
                }
            }

            //여기부터는 User의 과목들을 색칠, 2차원 배열에 넣기
            for (int i = 0; i < usersSubjectsList.Count(); i++)
            {
                string[] time = new string[] { usersSubjectsList[i].Time1, usersSubjectsList[i].Time2, usersSubjectsList[i].Time3, usersSubjectsList[i].Time4, usersSubjectsList[i].Time5, usersSubjectsList[i].Time6, usersSubjectsList[i].Time7, usersSubjectsList[i].Time8 };
                for (int j = 0; j < 8; j++)
                {
                    if (time[j] != "")
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
                        TimeTableDB[_period + 1, _day + 1].className = usersSubjectsList[i].ClassName;
                        TimeTableDB[_period + 1, _day + 1].professor = usersSubjectsList[i].Professor;
                        Run className = new Run(TimeTableDB[_period + 1, _day + 1].className + "\n");
                        className.FontSize = 12;
                        className.FontWeight = FontWeights.Bold;
                        schedule[_day, _period].Inlines.Add(className);
                        Run professor = new Run(TimeTableDB[_period + 1, _day + 1].professor);
                        professor.FontSize = 10;
                        schedule[_day, _period].Inlines.Add(professor);
                        
                        BrushConverter bc = new BrushConverter();
                        schedule[_day, _period].Background = (Brush)bc.ConvertFrom(br[i]);
                    }
                }

            } //User의 과목들을 색칠, 2차원 배열에 넣기
        }

        private void DeleteSubjectInTimeTable(string _dayAndPeriod)//매개변수 시간에 있는 과목 삭제
        {
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time1 == _dayAndPeriod));
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time2 == _dayAndPeriod));
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time3 == _dayAndPeriod));
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time4 == _dayAndPeriod));
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time5 == _dayAndPeriod));
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time6 == _dayAndPeriod));
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time7 == _dayAndPeriod));
            usersSubjectsList.Remove(usersSubjectsList.Find(x => x.Time8 == _dayAndPeriod));
        }  
        
        private void AddSubjectToTimeTable()
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
                string[] time = new string[] { resultSubtject[index].Time1, resultSubtject[index].Time2, resultSubtject[index].Time3, resultSubtject[index].Time4, resultSubtject[index].Time5, resultSubtject[index].Time6, resultSubtject[index].Time7, resultSubtject[index].Time8 };

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

                for (int i = 1; i < 8; i++)
                {
                    if (time[i] != "" && totalAbleToPut == true)
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
                usersSubjectsList.Add(new Subject()
                {
                    NO = resultSubtject[index].NO,
                    Grade = resultSubtject[index].Grade,
                    ClassNumber = resultSubtject[index].ClassNumber,
                    ClassName = resultSubtject[index].ClassName,
                    CreditCourse = resultSubtject[index].CreditCourse,
                    Professor = resultSubtject[index].Professor,
                    강의시간 = resultSubtject[index].강의시간,
                    Time1 = resultSubtject[index].Time1,
                    Time2 = resultSubtject[index].Time2,
                    Time3 = resultSubtject[index].Time3,
                    Time4 = resultSubtject[index].Time4,
                    Time5 = resultSubtject[index].Time5,
                    Time6 = resultSubtject[index].Time6,
                    Time7 = resultSubtject[index].Time7,
                    Time8 = resultSubtject[index].Time8,
                }); //과목추가
                RefreshTimeTable();
            }
        }



        private void Search_btn_Click(object sender, RoutedEventArgs e) //검색 버튼 눌렀을때
        {
            if (user.College == "공과대학")
                SearchEngineer();
            else if (user.College == "일반대학")
            {

            }
            else if (user.College == "건축대학")
            {

            }
            else
                System.Windows.MessageBox.Show("유저 정보 error in search_btn_clic");
        }

        private void DataListView_All_MouseDoubleClick(object sender, MouseButtonEventArgs e) //리스트에 있는 과목을 더블클릭했을때
        {
            if (myGroup_btn)
            {

                return;
            }


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
                string[] time = new string[] { resultSubtject[index].Time1, resultSubtject[index].Time2, resultSubtject[index].Time3, resultSubtject[index].Time4, resultSubtject[index].Time5, resultSubtject[index].Time6, resultSubtject[index].Time7, resultSubtject[index].Time8 };

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

                for (int i = 1; i < 8; i++)
                {
                    if (time[i] != "" && totalAbleToPut == true)
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
                usersSubjectsList.Add(new Subject()
                {
                    NO = resultSubtject[index].NO,
                    Grade = resultSubtject[index].Grade,
                    ClassNumber = resultSubtject[index].ClassNumber,
                    ClassName = resultSubtject[index].ClassName,
                    CreditCourse = resultSubtject[index].CreditCourse,
                    Professor = resultSubtject[index].Professor,
                    강의시간 = resultSubtject[index].강의시간,
                    Time1 = resultSubtject[index].Time1,
                    Time2 = resultSubtject[index].Time2,
                    Time3 = resultSubtject[index].Time3,
                    Time4 = resultSubtject[index].Time4,
                    Time5 = resultSubtject[index].Time5,
                    Time6 = resultSubtject[index].Time6,
                    Time7 = resultSubtject[index].Time7,
                    Time8 = resultSubtject[index].Time8,
                }); //과목추가
                RefreshTimeTable();
            }
        }

        private void Search_Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)//엔터키 눌렀을 때
        {
            if (e.Key == Key.Enter)
            {
                if (user.College == "공과대학")
                    SearchEngineer();
                else if (user.College == "일반대학")
                {

                }
                else if (user.College == "건축대학")
                {

                }
                else
                    System.Windows.MessageBox.Show("유저 정보 error in search_btn_clic");
            }
        }

        private void Logout_btn_Click(object sender, RoutedEventArgs e)
        {
            App.MW.Show();
            App.first = false;
            TableEdit_txtbox.Text = "시간표1";
            this.Visibility = Visibility.Collapsed;
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

        private void MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
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

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (panel.Name == schedule[i, j].Name)
                    {
                        week = i + 1;
                        period = j + 1;
                    }
                }
            }

            DeleteSubject DS = new DeleteSubject(TimeTableDB[period, week].className);
            bool? diagResult = DS.ShowDialog();
            if (diagResult == true)
            {
                DeleteSubjectInTimeTable(day_time[week - 1, period - 1]);
                RefreshTimeTable(); //새로고침
            }
            schedule[week - 1, period - 1].Visibility = Visibility.Collapsed; 
        }

        private void TableList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = TableList.SelectedIndex;
            tableName = userTimeTable[index].TimeTableName;
//            userTimeTalbeNO = userTimeTable[index].NO;
            TableEdit_txtbox.Text = tableName;
            //GetTimeTableClassNumber(userTimeTalbeNO);
            GetTimeTableClassNumber(userTimeTable[index].TimeTableName);
            usersSubjectsList.Clear();
            for (int i = 0; i < timeTableClassNumber.Count; i++)
            {
                usersSubjectsList.Add(subjectList.Where(s => s.ClassNumber.Contains(timeTableClassNumber[i])).ToList().ElementAt(0));
            }
            RefreshTimeTable();
        }

        private void tab_Click(object sender, RoutedEventArgs e)
        {
            MyGroup_grd.Visibility = Visibility.Collapsed;
            TimeTable_grd.Visibility = Visibility.Visible;
            if (tabActive)
            {
                MainView_grd.SetValue(Grid.ColumnSpanProperty, 2);
                list_grid.Visibility = Visibility.Collapsed;
                tabActive = false;
            }
            else
            {
                MainView_grd.SetValue(Grid.ColumnSpanProperty, 1);
                list_grid.Visibility = Visibility.Visible;
                tabActive = true;
            }

        }

        private void normal_DropDownClosed(object sender, EventArgs e)
        {
            engineer_type_all.Visibility = Visibility.Visible;
            engineer_type_normal.Visibility = Visibility.Collapsed;
            engineer_type_mainpoint.Visibility = Visibility.Collapsed;
            engineer_type_all.Visibility = Visibility.Visible;
            major_grade.Visibility = Visibility.Collapsed;
            if (user.College == "일반대학")
            {
                all_all.Visibility = Visibility.Collapsed;
                normal_common.Visibility = Visibility.Collapsed;
                normal_normal.Visibility = Visibility.Collapsed;
                if (normal.Text == "전체")
                {
                    all_all.Visibility = Visibility.Visible;
                }
                else if (normal.Text == "공통교양")
                {
                    normal_common.Visibility = Visibility.Visible;
                }
                else if (normal.Text == "일반교양")
                {
                    normal_normal.Visibility = Visibility.Visible;
                }
                else
                    System.Windows.MessageBox.Show("normal combobox에 다른 4번째 값이 존재");
            }
            else
                System.Windows.MessageBox.Show("일반대학이 아닌데, normal combobox에 접근");

        }

        private void engineer_DropDownClosed(object sender, EventArgs e)
        {
            engineer_type_all.Visibility = Visibility.Visible;
            engineer_type_normal.Visibility = Visibility.Collapsed;
            engineer_type_mainpoint.Visibility = Visibility.Collapsed;
            if (user.College == "공과대학")
            {
                all_all.Visibility = Visibility.Collapsed;
                engineer_ABEEK.Visibility = Visibility.Collapsed;
                engineer_normal.Visibility = Visibility.Collapsed;
                if (engineer.Text == "전체")
                {
                    all_all.Visibility = Visibility.Visible;
                }
                else if (engineer.Text == "ABEEK 교양")
                {
                    engineer_ABEEK.Visibility = Visibility.Visible;
                }
                else if (engineer.Text == "일반교양")
                {
                    engineer_normal.Visibility = Visibility.Visible;
                }
                else
                    System.Windows.MessageBox.Show("engineer combobox에 다른 4번째 값이 존재");
            }
            else
                System.Windows.MessageBox.Show("공과대학이 아닌데, engineer combobox에 접근");
        }

        private void course_DropDownClosed(object sender, EventArgs e)
        {
            normal.Visibility = Visibility.Collapsed;
            engineer.Visibility = Visibility.Collapsed;
            architecture.Visibility = Visibility.Collapsed;
            all_all.Visibility = Visibility.Collapsed;
            normal_normal.Visibility = Visibility.Collapsed;
            normal_common.Visibility = Visibility.Collapsed;
            engineer_ABEEK.Visibility = Visibility.Collapsed;
            engineer_normal.Visibility = Visibility.Collapsed;
            major.Visibility = Visibility.Collapsed;
            major_engineer.Visibility = Visibility.Collapsed;
            major_business.Visibility = Visibility.Collapsed;
            major_language.Visibility = Visibility.Collapsed;
            major_teach.Visibility = Visibility.Collapsed;
            major_art.Visibility = Visibility.Collapsed;
            major_architecture.Visibility = Visibility.Collapsed;
            major_law.Visibility = Visibility.Collapsed;
            major_economics.Visibility = Visibility.Collapsed;
            engineer_type_all.Visibility = Visibility.Visible;
            engineer_type_normal.Visibility = Visibility.Collapsed;
            engineer_type_mainpoint.Visibility = Visibility.Collapsed;

            if (course.Text == "전체")
            {
                all_all.Visibility = Visibility.Visible;
                engineer_type_all.Visibility = Visibility.Visible;
            }
            else if (course.Text == "교양")
            {
                if (user.College == "일반대학")
                {
                    normal.Visibility = Visibility.Visible;
                    all_all.Visibility = Visibility.Visible;
                }
                else if (user.College == "공과대학")
                {
                    engineer.Visibility = Visibility.Visible;
                    all_all.Visibility = Visibility.Visible;
                }
                else if (user.College == "건축대학")
                {
                    architecture.Visibility = Visibility.Visible;
                    all_all.Visibility = Visibility.Visible;
                }
            }
            else if (course.Text == "전공")
            {
                major.Visibility = Visibility.Visible;
                major_engineer.Visibility = Visibility.Visible;
            }
            else
            {
                System.Windows.MessageBox.Show("모든과목, 교양, 전공 이외에 다른 combobox가 존재");
            }
        }

        private void major_DropDownClosed(object sender, EventArgs e)
        {
            major_engineer.Visibility = Visibility.Collapsed;
            major_business.Visibility = Visibility.Collapsed;
            major_language.Visibility = Visibility.Collapsed;
            major_teach.Visibility = Visibility.Collapsed;
            major_art.Visibility = Visibility.Collapsed;
            major_architecture.Visibility = Visibility.Collapsed;
            major_law.Visibility = Visibility.Collapsed;
            major_economics.Visibility = Visibility.Collapsed;
            engineer_type_normal.Visibility = Visibility.Collapsed;
            engineer_type_mainpoint.Visibility = Visibility.Collapsed;
            engineer_type_all.Visibility = Visibility.Collapsed;
            major_grade.Visibility = Visibility.Visible;
            if (major.Text == "공과대학")
            {
                major_engineer.Visibility = Visibility.Visible;
            }
            else if (major.Text == "경영대학")
            {
                major_business.Visibility = Visibility.Visible;
            }
            else if (major.Text == "문과대학")
            {
                major_language.Visibility = Visibility.Visible;
            }
            else if (major.Text == "사범대학")
            {
                major_teach.Visibility = Visibility.Visible;
            }
            else if (major.Text == "미술대학")
            {
                major_art.Visibility = Visibility.Visible;
            }
            else if (major.Text == "건축대학")
            {
                major_architecture.Visibility = Visibility.Visible;
            }
            else if (major.Text == "법과대학")
            {
                major_law.Visibility = Visibility.Visible;
            }
            else if (major.Text == "경제학부")
            {
                major_economics.Visibility = Visibility.Visible;
            }
            else
                System.Windows.MessageBox.Show("전공에 다른 무언가가 존재. error");
        }

        private void Search_Box_GotFocus(object sender, RoutedEventArgs e)
        {
            Search_Box.Foreground = Brushes.Black;
            if (Search_Box.Text == "교과명, 학수번호, 교수이름으로 검색하기")
                Search_Box.Text = "";
        }

        private void Search_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Search_Box.Text == "")
            {
                Search_Box.Foreground = new SolidColorBrush(Color.FromArgb(255, 211, 211, 211));
                Search_Box.Text = "교과명, 학수번호, 교수이름으로 검색하기";
            }
        }

        private void TableEdit_txtbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (userTimeTable.Count() != 0)
                {
                    url = urlUserTimeTable + "/" + App.ID;
                    String NewpostData = "{ \"ID\" : \"" + App.ID + "\", \"TimeTableName\" : \"" + TableEdit_txtbox.Text + "\", \"NO\" : \"" + userTimeTalbeNO + "\"}";
                    connect(url, NewpostData, "PUT");

                    GetUserTimeTable();
                    TableList.ItemsSource = userTimeTable;
                }
            }
        }

        private void TimeAdd_btn_Click(object sender, RoutedEventArgs e)
        {

            url = urlUserTimeTable;
            String NewpostData = "{ \"ID\" : \"" + App.ID + "\", \"NO\" : " + "5" + ", \"TimeTableName\" : \"시간표1\"}";
            connect(url, NewpostData, "POST");

            GetUserTimeTable();
            TableList.ItemsSource = userTimeTable;
            TableEdit_txtbox.Text = "시간표1";
        }

        private void Save_Schedule_Click(object sender, RoutedEventArgs e)
        {
            string check_url = "";
            string insert_url = "";
            string update_url = "";
            check_url = urlUserTimeTable + "/" + App.ID + "/TimeTableName/" + TableEdit_txtbox.Text;
            var json = new WebClient().DownloadData(check_url);
            string Unicode = Encoding.UTF8.GetString(json);
            if (Unicode == "[]")
            {
                // 해당 데이터가 존재하지 않는다! 새로 저장하기
                insert_url = urlUserTimeTable;
                String now = DateTime.Now.ToString("MMddHHmmss");
                //String NewpostData = "{ \"ID\" : '" + App.ID + "', \"NO\" : \"" + now + "\", \"TimeTableName\" : \"" + TableEdit_txtbox.Text + "\"}";
                String NewpostData1 = "{ \"ID\" : '" + App.ID + "', \"TimeTableName\" : \"" + TableEdit_txtbox.Text + "\", \"SaveTime\" : \"" + now + "\", \"EditTime\" : \"" + now + "\"}";
                connect(insert_url, NewpostData1, "POST");
            }
            else
            {
                // 해당 데이터가 이미 존재한다! edittime만 갱신하기
                String EditTime = DateTime.Now.ToString("MMddHHmmss");
                //update_url = urlUserTimeTable + "/" + App.ID + "/update_edittime/" + TableEdit_txtbox.Text + "/EditTime/" + EditTime;
                update_url = "http://allcleapp.azurewebsites.net/api/UserTimeTable";
                String NewpostData2 = "{ \"EditTime\" : '" + EditTime + "', \"ID\" : \"" + App.ID + "\", \"TimeTableName\" : \"" + TableEdit_txtbox.Text + "\"}";
                connect(update_url, NewpostData2, "PUT"); // 여기서 에러 발생!
            }

            GetUserTimeTable();
            TableList.ItemsSource = userTimeTable;

            // 리스트 위에 있는 과목들도 저장해야됨
            // 얘네는 TimeTableClass DB에 저장해야됨
            Save_Schedule_Subject_Click();
        }

        private void Save_Schedule_Subject_Click()
        {
            // 시간표 이름에 맞춰서 UI에 등록된 과목들 순차적으로 저장
            TextBlock[,] _schedule = new TextBlock[,]
            {
                {mon1, mon2, mon3, mon4, mon5, mon6, mon7, mon8, mon9, mon10, mon11, mon12, mon13},
                {tue1, tue2, tue3, tue4, tue5, tue6, tue7, tue8, tue9, tue10, tue11, tue12, tue13},
                {wed1, wed2, wed3, wed4, wed5, wed6, wed7, wed8, wed9, wed10, wed11, wed12, wed13},
                {thu1, thu2, thu3, thu4, thu5, thu6, thu7, thu8, thu9, thu10, thu11, thu12, thu13},
                {fri1, fri2, fri3, fri4, fri5, fri6, fri7, fri8, fri9, fri10, fri11, fri12, fri13},
                {sat1, sat2, sat3, sat4, sat5, sat6, sat7, sat8, sat9, sat10, sat11, sat12, sat13},
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
            for (int i = 0; i < 6; i++)
            {
                for (int j=0; j<13; j++)
                {
                    if (_schedule[i, j].Text != "")
                    {
                        // 강의시간, 강좌명, 강사 나오니까 이걸로 ClassNumber select
                        String[] str = _schedule[i, j].Text.Split('\n');
                        String classname = str[0];
                        String temp_classTeach = str[1];
                        String classTeach = "";
                        String classtime = day_time[i, j];

                        String[] parsing_classTeach = temp_classTeach.Split(' ');
                        for(int k = 0; k<parsing_classTeach.Length; k++)
                        {
                            classTeach = classTeach + "_" + parsing_classTeach[k];
                        }

                        string ClassNumber_url = urlBase + "/" + classname + "/" + classTeach + "/" + classtime;
                        var json = new WebClient().DownloadData(ClassNumber_url);
                        string temp_classnumber = Encoding.UTF8.GetString(json);
                        string[] temp = temp_classnumber.Split('"');
                        string classnumber = temp[7];

                        // TimeTableClassNumber Table에 해당 ClassNumber 있는지 확인
                        string CheckClassNumber = urlTimeTableClassNumber + "/" + App.ID + "/" + TableEdit_txtbox.Text + "/" + classnumber;
                        var json2 = new WebClient().DownloadData(CheckClassNumber);
                        string Unicode = Encoding.UTF8.GetString(json2);
                        if (Unicode == "true")
                            // 이미 해당 classnumber를 저장한 경우
                            continue;
                        else
                        {
                            // 해당 classnumber가 없는 경우 삽입.
                            // 여기서 에러 발생. 현재 저장만 하면 된다..
                            string insert_ClassNumber_url = urlTimeTableClassNumber;
                            String ClassNumber_postData = "{ \"ID\" : \"" + App.ID + "\", \"TimeTableClassNumber\" : \"" + TableEdit_txtbox.Text + "\", \"ClassNumber\" : \"" + classnumber + "\"}";
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(insert_ClassNumber_url);// 인코딩 UTF-8
                            byte[] sendData = UTF8Encoding.UTF8.GetBytes(ClassNumber_postData);
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
                    }
                }
            }


            // 강의시간, 강좌명, 강사 이용해서 학수번호 출력
            // 얻은 학수번호를 밑의 ClassNumber 변수에 저장

            //String url = urlTimeTableClassNumber;
            //String NewpostData1 = "{ \"ID\" : '" + App.ID + "', \"TimeTableName\" : \"" + TableEdit_txtbox.Text + "\", \"ClassNumber\" : \"" + ClassNumber + "\"}";
            //connect(url, NewpostData1, "POST");
        }

        private void connect(String url, String NewpostData, String Method)
        {
            // POST : Save_Schedule_Click, TimeAdd_btn_Click
            // PUT: TableEdit_txtbox_KeyDown
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

        private void MyGroup_btn_Click(object sender, RoutedEventArgs e)
        {
            list_grid.Visibility = Visibility.Visible;
            MainView_grd.SetValue(Grid.ColumnSpanProperty, 1);
            if (myGroup_btn)
            {
                MyGroup_grd.Visibility = Visibility.Collapsed;
                TimeTable_grd.Visibility = Visibility.Visible;
                myGroup_btn = false;
                tabActive = true;
            }
            else
            {
                MyGroup_grd.Visibility = Visibility.Visible;
                TimeTable_grd.Visibility = Visibility.Collapsed;
                myGroup_btn = true;
                tabActive = false;
            }

        }

        private void Save_Image_Click(object sender, RoutedEventArgs e)
        {
            SaveToPng(TimeTable, "image.png");
            RenderTargetBitmap bitmap = new RenderTargetBitmap(684, 353, 96, 96, PixelFormats.Pbgra32);
        }

        private void engineer_ABEEK_DropDownClosed(object sender, EventArgs e)
        {
            engineer_type_all.Visibility = Visibility.Collapsed;
            engineer_type_normal.Visibility = Visibility.Collapsed;
            engineer_type_mainpoint.Visibility = Visibility.Collapsed;
            if (engineer_ABEEK.Text == "일반교양")
            {
                engineer_type_normal.Visibility = Visibility.Visible;
            }
            else if(engineer_ABEEK.Text == "핵심교양")
            {
                engineer_type_mainpoint.Visibility = Visibility.Visible;
            }
            else
            {
                engineer_type_all.Visibility = Visibility.Visible;
            }
        }

        private void Group_grd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid[] grids = new Grid[] { Group1_grd, Group2_grd, Group3_grd, Group4_grd, Group5_grd, Group6_grd, Group7_grd, Group8_grd, Group9_grd };
            var panel = sender as Grid;
            for (int i = 0; i < 9; i++) 
            {
                grids[i].Background = Brushes.Gray;
                if (grids[i].Name == panel.Name)
                {
                    grids[i].Background = Brushes.Yellow;
                    groupNum = i + 1;
                }
            }
            
        }

        // 시간표 정보(이름, 수강 과목 등) 수정 시 On update cascade해놨으니까 UserTimeTable만 업데이트하고, TimeTableClassNumber는 바로 insert하면 된다.
    }
}