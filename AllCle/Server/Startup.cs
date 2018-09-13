using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Server.Models;   // Models라는 디렉토리 using
                        // Models는 Server를 통해서 DataBase에서 어떤식으로 데이터를 가지고 올지에 대한 코드가 저장되어 있는 디렉토리.
                        // DataBase에서 Data를 수신하는 방법을 크게 4가지로 구분(18.08.15 기준, 변동 예정)하였다.

// Subject.cs는 DataBase가 구분되어 있는 쿼리를 public 변수로 저장하고 있는 class를 갖고 있다. 쿼리 별로 데이터를 가져올 때 사용할 class 객체이다.
// SubjectRepository.cs는 데이터를 수신하는 4가지 방법에 대한 구현 코드가 있는 class이다.
// ISubjectRepository.cs는 4가지 방법을 호출할 때 사용할 코드가 구현되어 있는 class이다.

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)    // 기본 framework 코드
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }    // 기본 framework 코드

        // This method gets called by the runtime. Use this method to add services to the container.
        // Configure method보다 먼저 호출된다. (선택사항)
        // Add 확장 메서드를 통해 실질적인 기능 설정이 가능하다.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();                          // 기본 framework 코드

            // 아래의 코드 2줄(46, 49)은 DI 설정. 
            // AllcleApiController.cs에서 DI를 생성자를 통해 받는다.
            // SubjectRepository.cs에서 데이터 가져오고 DB 접근하는 등 기능 구현

            // [DNN][!] Configuration 개체 주입:
            //    IConfiguration 또는 IConfigurationRoot에 Configuration 개체 전달 (의존성 주입)
            //    appsettings.json 파일의 데이터베이스 연결 문자열을
            //    리파지터리 클래스에서 사용할 수 있도록 설정
            services.AddSingleton<IConfiguration>(Configuration);

            //[Tech] 기술 목록
            services.AddTransient<ISubjectRepository1, SubjectRepository1>();   //1학기 과목들 검색
            services.AddTransient<ISubjectRepository2, SubjectRepository2>();   //2학기 과목들 검색
            services.AddTransient<IUserRepository, UserRepository>();           //유저의 로그인, 회원가입
            services.AddTransient<IUserTimeTableRepository, UserTimeTalbeRepository>();   //유저 - 타임테이블 
            services.AddTransient<ITimeTableClassNumberRepository, TimeTableClassNumberRepository>();   //타임테이블 - 과목
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // 기본 framework 코드
        // Configure method : HTTP 요청에 응답하는 방식을 지정.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }   
    }
}
