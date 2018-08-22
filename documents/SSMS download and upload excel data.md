* SSMS(SQL Server Management Studio)는 SQL 인프라를 관리하기 위한 통합 환경이다.

* 응용 프로그램에 사용되는 데이터 계층 구성 요소를 배포, 모니터링 및 업그레이드 하고 쿼리 및 스크립트를 작성할 수 있다.

* 로컬 서버 혹은 클라우드 등 어디서나 사용 가능하다.

* 이 문서에서는 **SSMS를 이용하여 azure cloud server에 excel data를 upload하는 방법**을 설명할 것이다.

### SSMS 설치
1.	https://docs.microsoft.com/ko-kr/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017 접속
2.	SQL Server Management Studio 17.8.1 다운로드 클릭
3.	설치 프로그램 실행

### SSMS와 Azure Server 연동
1. Microsoft SQL Server Management Studio 17 프로그램 실행
2. Database를 저장하고 있는 로컬/클라우드 서버의 이름을 “서버 이름”에 입력한다.
3. 로컬의 경우 “windows 인증”, 클라우드의 경우 “SQL server인증”을 선택한다.
    + 클라우드의 경우 서버의 계정 ID/PW를 입력한다.
4. 클라우드의 경우, Azure에 게시한 database에 excel data upload한다. 이에 대한 설명은 아래에 있다.

### Azure Cloud Server와 SSMS를 연동하는 경우, SSMS을 이용하여 Azure Server의 DB에 Excel data를 저장하는 방법
1. SSMS를 실행한다.
2. 좌측의 개체 탐색기에서 “DB이름”에 우클릭한다.
3. 태스크 -> 데이터 가져오기 -> 엑셀 선택 -> sql 10.0 버전 선택 -> 서버 연결
    - 3번째 단계로, "엑셀 선택"할 때, 아래와 같은 Error가 발생할 수 있다. <br>
      **“microsoft.ace.oledb.16.0 공급자는 로컬 컴퓨터에 등록할 수 없습니다.”** <br>
      이는, Excel office와 운영체제 모두 64bit인데, 프로그램에서 32bit를 요구해서 발생한다. <br>
      해결방법은 아래와 같다.
        +	https://www.microsoft.com/ko-kr/download/details.aspx?id=23734  접속, 설치 <br>
          위의 프로그램 설치로도 안되면 다음 블로그 참고
        +	http://www.sysnet.pe.kr/Default.aspx?mode=2&sub=0&pageno=1&wtype=15&wid=1036&detail=1 

### Azure에 연동이 제대로 되었는지 확인
1. Azure 계정에 접속하여, "포탈"에 들어간다.
2. "모든 리소스"에서 SSMS에 연동한 database를 클릭하고, 쿼리 탐색기를 클릭한다.
3. 쿼리탐색기-테이블의 업로드한 “DB이름”을 클릭한 뒤, 서버 계정에 로그인한다.
4. 명령어 입력란에 다음과 같은 sql 명령어을 입력하여 업로드 여부를 확인한다.
    + **select * from “DB이름” where [필드명]=N'검색하고자 하는 것'** <br>
      (검색하고자 하는 키워드는 ‘’으로 감싸주고, N을 붙이는 이유는 ‘’의 내용물이 영어가 아닌 한국어 이기 때문이다.)
