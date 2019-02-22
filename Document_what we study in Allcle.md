1. Client
  * C# WPF
    + WPF는 window10이 아닌 환경에서도 편하게 사용 가능
    + uwp는 window10에 매우 적합. store에 업로드도 편하다
  * UI 디자인 설계
    + 로그인, 회원가입, 메인, 팝업 등 UI 디자인 설계 한 것 들
  * 프로그램의 사진/아이콘 등 세세한 디자인 및 설정
  * 로그인 구현
    + 암호화 구조
    + smtp 사용한 e-mail 기능 구현
  * UI 디자인 구현
    + 각 UI 구현 기술 기록하자.
    + Reflection을 이용해 객체를 통해 클래스의 정보를 분석하여, 반복되는 메소드 획일화
    + DataGrid 등으로 반복되는 UI와 기능들에 대해 Template 사용
    + binding 사용하여 컨트롤의 source 값에 필요한 데이터만 넣어서 로직 처리
    + ListView control customizing 등 기능 구현. 
    + Definition으로 UI 비율 설계
    + ComboBoxItem, Grid, List, button, Trigger등의 UI 객체 생성
    + Mouseover 등의 UI 메소드 사용
  * JSON을 활용하여 Client와 Server간의 데이터 주고 받기
 
2. Database
  * Microsoft SQL Server. Azure를 활용하여 클라우드 서버에 업로드
  * Relational Database Table 설계. 테이블 n개 정규화 몇차까지?
  * 3-tier 방식의 안전한 구조
  
3. Server
  * Server 코드 구현 기술 기록하자.
  * C# ASP.Core 2.0 Web api
  * Api url 설정
    + http://allcleapp.azurewebsites.net/api/ + (서버 통신 경로)
    + url 암호화 예정
  * Azure 클라우드 서버에 게시하여 로컬이 아닌 어디서든 서버 이용하도록 설정 – paas
  * Rest API, Socket stream
    + GET, POST, PUT
    + http 상태 메시지를 응용해서 결과에 대한 처리 구현
    
4. Tool & Language
  * Language : C#, Python, sql
  * Tool : Visual Studio, Microsoft SQL Server Management Studio 17, Postman, Jupyter Notebook, Azure
  
5. 유통은 어떻게?

6. 데이터 전처리
  * 크롤링 및 파싱
  
7. 깃허브
  * master계정 별도로, 디자인과 2명의 개발자
  * Pull Request, Merge, Branch, Commit, Git, Bash 등에 대한 이해
  * .gitignore
  * 빌어먹을 git conflict 해결 경험 및 git을 이용한 코드 변경 중 발생한 문제점 추적 등의 경험
  * git 블로그 운영 예정

8. 문서작업
  * DB설계, UI설계, 시스템 설계, 개발환경, 기능요구사항, 비기능 요구사항, 시스템 요구사항, 유저 메뉴얼, 인수테스트, 프로젝트 목적 및 개요
  * 깃허브 마크다운 언어를 이용한 문서작업
  * 18.06.25부터 8개월간의 미팅 및 회의 기록 문서화 
