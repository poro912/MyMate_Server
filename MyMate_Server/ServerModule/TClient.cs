using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Protocol;
using Protocol.Protocols;
using ServerNetwork.Module;
// Received Result
using RcdResult = System.Collections.Generic.KeyValuePair<byte, object?>;

namespace ServerSystem
{
	// TClient
	// 임시 클라이언트
	// Receive에서 스레드와 비슷하게 동작하기 때문에 해당 클래스에서는
	// 스레드를 사용하지 않는다.
	public class TClient : Client
	{
		public int? usercode = null;
		public bool isLogin = false;

		private Semaphore semaphore = new(1, 1);

		private List<byte> bytes;
		private RcdResult result;
		public TClient(TcpClient tcpClient) : base(tcpClient)
		{
			// base에서 기본 네트워크 설정 및 Send, Receive를 할당해준다.
			this.receive.ReceiveEvent += new (this.wakeup); //new DataReceived(this.wakeup);

			// 임시 변수 초기화

			// 로그인 이전 이벤트 등록
			BeforeLoginEvent.ConnectCheckEvent += new DelIsConnect(this.ConnectCheck);

			// 수신을 시작한다.
			this.start();
		}

		// 이벤트로 호출 될 클래스
		// public void wakeup()
		public void wakeup(object sender, EventArgs e)
		{
			Console.WriteLine("이벤트로 일어났음");

			// 가장 최근의 통신이 끊어진 상태라면
			if(!this.receive.stream.Socket.Connected)
			{
				this.Delete();
				return;
			}

			// 세마포어를 지금 즉시 받을 수 없다면
			// 이미 실행중인것으로 리턴
			if (!semaphore.WaitOne(10))
				return;

			Console.WriteLine("세마포어 받음");
			// receive 목록에 값이 하나라도 있는경우
			if (!this.receive.isEmpty())
			{
				// 로그인 상태라면 로그인 프로세스로 이동
				if (isLogin)
					AfterLoginProcess();
				// 로그인상태가 아니라면 로그인 이전 프로세스로 이동
				else
					BeforeLoginProcess();
			}

			Console.WriteLine("세마포어 반환");
			// 세마포어 반환
			semaphore.Release();
		}

		// 로그인 이전에 작동할 프로세스
		public void BeforeLoginProcess()
		{
			Console.WriteLine("로그인 이전 프로세스 동작");
			while (!this.receive.isEmpty())
			{
				// 데이터를 읽어옴
				this.receive.Pop(out bytes);
				result = Converter.Convert(bytes);

				// 읽어온 데이터가 없다면 Continue
				if (null == result.Value)
					continue;

				if (DataType.LOGIN == result.Key)
					LoginProcess(result);
			}
		}

		// 로그인 이후에 작동할 프로세스
		public void AfterLoginProcess()
		{
			Console.WriteLine("로그인 이후 프로세스 동작");
			while (!this.receive.isEmpty())
			{
				// 데이터를 읽어옴
				this.receive.Pop(out bytes);
				result = Converter.Convert(bytes);

				// 읽어온 데이터가 없다면 Continue
				if (null == result.Value)
					continue;
			}
		}

		private void LoginProcess(RcdResult result)
		{
			Protocol.Protocols.LoginProtocol.Login? login;
			login = result.Value as Protocol.Protocols.LoginProtocol.Login;
			
			// 형 변환에 실패 했다면 종료
			if (login == null)
				return;

			Console.WriteLine("로그인 시도");
			Console.WriteLine("id : " + login.id);

			// 형 변환에 성공한 경우 DB에서 ID PW 확인
			if(login.id.Equals("admin"))
			{
				Console.WriteLine("로그인 성공");
				// 로그인 컨테이너에 자기 자신 등록
				//LoginContainer.Instance.registUser(this);

				this.isLogin = true;
				this.usercode = 1;
			}
		}

		// 해당 매소드를 사용할 때는 try catch 문장을 꼭 써줘야 한다.
		public void SendCheck(List<byte> bytes)
		{
			try
			{
				this.send.Data(bytes);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				this.Delete();
				throw;
			}
		}

		public void ConnectCheck()
		{
			isConnectProtocol.IsConnect isConnect = new();
			Generater.Generate(isConnect, ref bytes);
			try
			{
				SendCheck(bytes);
				// 가장 최근의 통신이 끊어진 상태라면
				if (!this.send.Stream.Socket.Connected)
					throw new Exception("통신 끊어짐");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				this.Delete();
			}
			
		}

		// 로그아웃 되거나 통신이 강제로 끊긴경우 호출
		public void Delete()
		{
			Console.WriteLine("통신 종료");
			// 로그아웃 상태로 만든다
			this.isLogin = false;

			// 저장된 usercode를 삭제한다.
			this.usercode = null;

			// 이벤트를 해제한다.
			BeforeLoginEvent.ConnectCheckEvent -= this.ConnectCheck;
			this.receive.ReceiveEvent -= this.wakeup;

			// 수신을 중지한다.
			this.receive.Stop();
		}
	}
}
