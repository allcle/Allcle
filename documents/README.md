## 논의 필요 항목

* 전공/학번 별로 다른 카테고리를 전공에 맞춰서 출력해줄 것 인지, 에브리타임처럼 전체 출력해줄 것 인지
  => 그냥 다 출력(에타와 다르게 하는것을 희망)
    
* 시간표 UI는 어떤 도구를 사용할 것 인지 (Grid, listView 등)
  => grid가 최적인듯

* 마감 시간 및 학기 중 목표 정도 생각해보기. 
  => 1월까지 개발 완료를 목표 <br/>
  9월까지 UI를 마무리(UI를 마무리하고 backend를 진행하는게 더 좋을꺼 같다고 생각) <br/>
  10월까지 로그인 서비스를 마무리(Naver API를 이용한 로그인 서비스) <br/>
  11,12월은 추가적 개발 <br/>

<code>
  
    ### Server
    1. Visual Studio에서 ASP.Core 2.0 Web api 프로젝트를 만든다.
    2. ASP.Core 2.0 Web api 프로젝트는 Azure 서버에 업로드 된다. 프로젝트의 URL이 정해진다.
    3. ASP.Core 2.0 Web api 프로젝트를 이용해서, Azure에 업로드 되어 있는 Database에 접근한다.
    4. 즉, User는 Azure에 있는 ASP.Core 2.0 Web api에 접근하여, 또 다른 Cloud Server에 있는 Database에 접근할 수 있다.
  
</code>
