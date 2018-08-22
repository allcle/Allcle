## 이 문서에서는 Asp.net core web app program을 azure 계정에 게시하는 방법과
## 이 때 DB와 server를 함께 연동하는 과정 설명하겠다.

* asp.net core web application 작업은 visual studio에서 했다.

* visual studio로 작업을 하고, azure에 바로 게시할 수 있으며, DB/Server와 연동되어 있는 Asp.net core web application을 만들 수 있다.

### Visual Studio의 웹 어플리케이션과 Azure 서버의 연동 방법
1. ASP.NET 환경 설치
    * Visual studio에서 “도구-도구 및 기능 가져오기”를 클릭한다.
    * 웹 및 클라우드 – ASP.NET 및 웹 개발을 설치한다.

2. Asp.net core web application 프로젝트 게시 방법
    * 파일-새로 만들기-프로젝트-웹-ASP.NET Core 웹 응용 프로그램 선택
    * 우측의 솔루션 탐색기에서 ASP.NET Core Web App 이름에 우클릭한다.
    * 6번째의 지구본 아이콘의 게시를 클릭한다.
    * “게시 대상 선택”화면 : Azure App service 새로 만들기 선택, 게시
    * “App service 만들기” 화면 :
        - 앱 이름: 게시할 어플리케이션의 이름 설정
        -	리소스 그룹: Azure에는 어플리케이션 뿐 아니라 서버, 디비 등 여러가지 서비스를 게시할 수 있다. <br>
                      그러한 서비스 중, 공통된 프로젝트를 모으는 그룹이라고 생각하면 된다
        -	우측의 “추가 Azure 서비스 탐색”에서 “SQL 데이터베이스 만들기”를 클릭한다.
        -	SQL 데이터베이스 구성: 
            *	새로 만들 DataBase의 이름을 설정한다.
            *	DataBase를 저장할 SQL Server를 선택한다. <br>
               새로 만들기를 통해 지금의 DB를 위한 서버를 새로 설정할 수 있다.
            *	SQL Server의 계정 정보를 입력하여 접속 허용한다.
            *	연결 문자열 이름 기본은 “DefaultConnection”이다. <br>
               이 후 사용하므로, 변경하는 경우 꼭 기록한다. (Azure에서 확인 가능하긴 하다.)
        -	우측 하단에 게시할 서비스 목록을 보여준다. 게시를 누른다.
        -	한 번 더 게시를 눌러야하는 화면이 나온다. 똑같이 게시를 눌러준다.
    * Azure 계정 접속, 포탈에 들어간 뒤, "모든 리소스"에서 게시 여부를 확인한다.
