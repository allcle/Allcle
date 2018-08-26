# API Url을 어떻게 할 것인가 

## 기본주소
- http://allcleapp.azurewebsites.net/api/AllCleSubjects1 (1학기)  <br>
- http://allcleapp.azurewebsites.net/api/AllCleSubjects2 (2학기)  <br>
## 필요한 기능들
- 로그인
  * Get
  * Post
- Listview
  * Get</br>
 &nbsp;1  전체를 보여주기</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1 남은시간 on 일때 :  /timefilteton/</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1 담은과목 제거 on : /{time}/{subject}</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 2 담은과목 제거 off : /{time}/subjectfilteroff</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 2 남은시간 off 일때 :  /timefiltetoff/</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1 담은과목 제거 on : /timefiltetoff/{subject}</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 2 담은과목 제거 off : /timefiltetoff/subjectfilterff</br>
 &nbsp;2  검색한 결과를 보여주기</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1 남은시간 on 일때 :  /subjectfilteton/</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1 담은과목 제거 on : /{time}/{subject}/{search}</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 2 담은과목 제거 off : /{time}/subjectfilteroff/{search}</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 2 남은시간 off 일때 :  /subjectfiltetoff/</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1 담은과목 제거 on : /timefiltetoff/{subject}/{search}</br>
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 2 담은과목 제거 off : /timefiltetoff/subjectfilterff/{search}</br>
  * Post</br>
