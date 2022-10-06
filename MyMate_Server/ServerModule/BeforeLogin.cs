using ServerNetwork.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Protocol;
using Protocol.Protocols;
using Protocal.Protocols;
using System.Net.Sockets;

namespace ServerSystem
{
	// 로그인 이전의 Accept 객체에 대한 확인
	// 로그인 이전의 클라이언트는 지속적으로 연결 중인지 확인해야한다.
	// 큐로 구현하며, 연결되어 있지 않다면 객체를 삭제
	// 연결 되어있다면 큐의 맨 뒤로 이동 
	// 데이터가 들어온다면 이벤트를 발생시켜 스레드를 wakeup 한 후 로그인 성공시에만 LoginContainer로 이동

	public class BeforeLogin : Client
	{
		public Thread thread;

		public int usercode = 0;
		public bool isLogin = false;
		
		public BeforeLogin(TcpClient client) : base(client)
		{
			this.thread = new Thread(Run);

			// 이벤트 등록
			// 수신된 것이 있다면 이벤트가 호출됨
			this.receive.ReceiveEvent += wakeup;

			// 최초 1회 실행함
			this.thread.Start();
		}

		private void Run()
		{
			byte[]? bytes;
			KeyValuePair<byte, object?> result;
			
			// 수신큐가 빌때까지
			while (!receive.IsEmpty())
			{
				// 데이터를 읽어온다
				receive.Pop(out bytes);

				if(bytes == null)
				{
					continue;
				}
				
				result = Converter.Convert(ref bytes);

				if (null == result.Value)
					continue;

				if(DataType.LOGIN == result.Key)
				{
					// 결과를 저장할 배열
					List<byte> send_data = new();
					UserInfoProtocol.User user = new();

					// 로그인 데이터 받아오기
					LoginProtocol.Login login = 
						(LoginProtocol.Login)result.Value;


					// sql 접근 데이터 가져오기
					

					//로그인 성공 시
					//if()
					{
						// 로그인 데이터 저장
						//user.set();
						//usercode = 10;
						//this.isLogin = true;
					}


					// 결과 데이터 전송
					Generater.Generate(ref user, ref send_data);
					send.Data(send_data);

				}
			} 
		}

		public void wakeup(object sender, EventArgs e)
		{
			// 스레드가 실행중이 아니라면
			if (!this.thread.IsAlive)
				this.thread.Start();
		}

		public void Delete()
		{
			// 스레드를 종료시킴
			receive.ReceiveEvent -= this.wakeup;
			thread.Interrupt();
			thread.Join();
		}

	}
}
