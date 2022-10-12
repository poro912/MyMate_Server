using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Protocol;
using Protocol.Protocols;
using ServerNetwork;
using MyMate_Server.ServerModule;

namespace MyMate_Server
{
    public class RequestThread
    {
        // 스레드를 만들기 위한 변수들
        private Thread progress_thread;
        private bool run = false;

        private Temp temp;

        // 데이터를 받기위한 변수들
        private KeyValuePair<byte, object?> result;
        private byte[]? a_data = new byte[1024];
        private List<byte> l_data = new();

        // 생성자
        private RequestThread(Temp temp)
        {
            this.temp = temp;
            progress_thread = new Thread(() => ProgressRun());

            Start();
            
        }

        // 스레드를 시작시키는 메서드
        public void Start()
        {
            if (!run)
            {
                run = true;
                progress_thread.Start();
            }
            
        }

        // 스레드를 종료시키는 메서드
        public void Stop()
        {
            run = false;
        }

        // 스레드에서 실행 될 메서드
        private void ProgressRun()
        {
            while (run)
            {
                // 변환시 값이 없다면 null 반환
                if (result.Value == null)
                    continue;

                processRequest();
            }
        }

        // 클라이언트의 요청을 처리하는 메서드
        private void processRequest()
        {
            // 클라이언트 전송 큐에서 가져오기
            // 클라이언트 요청 Converter를 사용하여 해석
            // if문으로 역할 처리
            // 프로토콜 사용하여 요청 기능 수행
            // -> 수행 과정에서 SQL 모듈 사용
            // 처리 완료 후 send

            a_data = null;

            // 큐에서 가져오는 부분
            temp.client.receive.Pop(out a_data);

            if (a_data != null)
            {
                // 입력된 데이터를 Key Value 로 변환
                result = Converter.Convert(a_data);

                Console.WriteLine("전송받은 데이터 타입 : " + result.Key);
                Console.WriteLine("전송받은 데이터 : " + result.Value);

                // 데이터가 로그인 데이터 라면
                if (result.Key == Protocol.DataType.LOGIN)
                {
                    processLogin();
                }

                if (result.Key == Protocol.DataType.MESSAGE)
                {
                    processMessage();
                }

                if (result.Key == Protocol.DataType.USER_INFO)
                {
                    processUserInfo();
                }

                // send 부분 생각해보기
                //server.send.Data(l_data);
            }
        }

        // 로그인 요청을 처리하는 메서드
        private void processLogin()
        {
            // 임시 객체 생성
            LoginProtocol.Login login = new();

            // 두 방법 중 편한 것으로 하면 된다.
            login = (LoginProtocol.Login)result.Value;
            //(result.Value as LoginProtocol.Login).Get(out login.id, out login.pw);

            // 결과 출력
            Console.WriteLine("로그인 시도");
            Console.WriteLine("id : " + login.id);
            Console.WriteLine("pw : " + login.pw);

            // SQL 객체 생성 후 처리
            SQL sql = new();
            bool dbResult = sql.noResultConnectDB(login.id, login.pw, sql.Login);
            Console.WriteLine("로그인 결과");
            Console.WriteLine(dbResult ? "로그인 성공" : "로그인 실패");
        }

        // 메시지 요청을 처리하는 메서드
        private void processMessage()
        {
            // 임시 객체 생성
            MessageProtocol.Message message = new();

            // 두 방법 중 편한 것으로 하면 된다.
            message = (MessageProtocol.Message)result.Value;
            //(result.Value as LoginProtocol.Login).Get(out login.id, out login.pw);

            // 결과 출력
            Console.WriteLine("메시지 전송 시도");
            Console.WriteLine("usercode : " + message.usercode);
            Console.WriteLine("servercode : " + message.servercode);
            Console.WriteLine("context : " + message.context);
            Console.WriteLine("date : " + message.date);

            // SQL 객체 생성 후 처리
            // 메시지 아직 미정
        }

        // 회원정보 요청을 처리하는 메서드
        private void processUserInfo()
        {
            // 임시 객체 생성
            UserInfoProtocol.User user = new();

            // 두 방법 중 편한 것으로 하면 된다.
            user = (UserInfoProtocol.User)result.Value;
            //(result.Value as LoginProtocol.Login).Get(out login.id, out login.pw);

            // 결과 출력
            Console.WriteLine("회원정보");
            Console.WriteLine("code : " + user.code);
            Console.WriteLine("id : " + user.id);
            Console.WriteLine("name : " + user.name);
            Console.WriteLine("nick : " + user.nick);
            Console.WriteLine("phone : " + user.phone);

            // SQL 객체 생성 후 처리
            SQL sql = new();
            bool dbResult = sql.Check(user.id, "pwd", user.name, user.nick, user.phone, sql.Signin);
            Console.WriteLine("회원정보 처리 결과");
            Console.WriteLine(dbResult ? "성공" : "실패");
        }
    }

   
}
