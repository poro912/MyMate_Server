using System.Data;
using System.Net.Sockets;
using Protocol;

using ServerToClient;
using ServerToClinet;

using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;


namespace ServerSystem
{

	public class UserClient :
		Client
	{
		// 접속한 유저의 Code를 저장
		private int? userCode;
		// 로그인 상태인지 확인
		private bool isLogin;
		// 객체에 접근하는 객체의 수를 제한(Send)
		private readonly Semaphore semaphore = new(1, 1);
		// Receive 에서 바이너리 데이터를 임시 저장하기 위한 리스트
		private List<byte>? bytes = new();
		// 결과값을 임시로 저장하는 객체 변수
		private ReceiveResult result;

		public UserClient(TcpClient tcpClient) :
			base(tcpClient)
		{
			// 수신 완료 시 이벤트를 등록
			ReceiveEvent += WakeUp;

			// 통신 시작 메소드
			Start();

			// 최초연결시는 로그인 이전이므로 로그인 이전의 이벤트에 연결
			// 로그인 이전이라면 지속적으로 데이터를 보내 연결상태가 맞는지 확인한다.
			BeforeLoginEvent.ConnectCheckEvent += CheckConnection;
		}

		// Receive로 일어나기 위한 코드
		private void WakeUp()
		{
			Console.WriteLine(userCode + ": Receive event is occurred.");

			// 연결 상태가 아니라면
			if (!tcpClient.Connected)
			{
				// 연결 종료
				Disconnection();
				return;
			}

			// 세마포어 획득을 시도
			if (!semaphore.WaitOne(10))
			{
				return;
			}

			Console.WriteLine(userCode + ": Semaphore is assigned.");

			// Receive 큐가 비어있는지 확인
			if (!IsEmpty())
			{
				// 로그인 상태라면
				if (isLogin)
				{
					// 로그인 처리로 이동
					AfterLogin();
				}
				else
				{
					// 로그인 이전 처리로 이동
					BeforeLogin();
				}
			}

			// 세마포어 반환
			Console.WriteLine(userCode + ": Semaphore is returned.");
			semaphore.Release();
		}

		// 로그인 이전의 처리
		private void BeforeLogin()
		{
			// 큐가 빌때까지 반복
			while (!IsEmpty())
			{
				// 수신받은 정보를 받아온다.
				result = Receive();

				// 결과값이 null 이라면 다음 데이터를 읽어온다.
				if (result.Value == null)
				{
					continue;
				}
				
				switch (result.Key)
				{
					case DataType.LOGIN:
						// 로그인 시도
						if(Login(result))
						{
							// 로그인 성공시
							return;
						}
						break;
				}
			}
		}

		// 로그인 시도 메소드
		private bool Login(ReceiveResult result)
		{
			var login = result.Value as LoginProtocol.Login;

			if (login == null)
			{
				return false;
			}
			
			Console.WriteLine(userCode + ": Logging in...");
			Console.WriteLine(userCode + ": ID \"" + login.id + "\" has been attempted.");

			// !SQL 로그인 시도

			// 로그인 성공 시 작동할 부분
			if (true)
			{
				// 로그인 컨테이너 정보를 가져옴
				var container = LoginContainer.Instance;

				// 로그인 이전 이벤트를 끊는다.
				BeforeLoginEvent.ConnectCheckEvent -= CheckConnection;

				// 유저의 데이터를 전송 가능한 형태로 집어 넣는다.
				UserInfoProtocol.User user = new((int)10, "admin", "poro", "poro", "010-8355-3460");

				// 로그인 정보 삽입
				userCode = 10;
				isLogin = true;

				// 로그인 컨테이너에 유저 정보 등록
				container.RegistUser((int)userCode, this);

				// 유저에게 정보 전송
				SendData(Generater.Generate(user));
				Console.WriteLine(userCode + ": Login succeeded.");

				// 클라이언트 프로그램 실행에 필요한 데이터를 전부 쏴준다.
				FirstDataSend();

				return true;
			}

			/* SQL 코드로 로그인을 시도하는 부분
			SQL sql = new();

			if (sql.noResultConnectDB(login.id, login.pw, sql.Login))
			{
				DataTable dataTable = new();

				dataTable = sql.resultConnectDB(login.id, sql.GetUserinfo);

				if (dataTable == null)
				{
					bytes = new();
					Generater.Generate("ID does not exist.", ref bytes);
					SendData(bytes);
				}
				else
				{
					isLogin = true;
					userCode = (int)dataTable.Rows[0]["U_code"];

					var container = LoginContainer.Instance;
					
					container.registUser((int)userCode, this);

					var id = dataTable.Rows[0]["U_id"].ToString();
					var name = dataTable.Rows[0]["U_name"].ToString();
					var nickname = dataTable.Rows[0]["U_Nickname"].ToString();
					var phone = dataTable.Rows[0]["U_phone"].ToString();

					UserInfoProtocol.User user = new((int)userCode, id, name, nickname, phone);
					bytes = new();
					Generater.Generate(user, ref bytes);
					SendData(bytes);

					BeforeLoginEvent.ConnectCheckEvent -= CheckConnection;
					
					Console.WriteLine(userCode + ": Login succeeded.");
				}
			}
			*/
		}

		async private void FirstDataSend()
		{
			// 정보를 가져와 Send큐에 곧바로 밀어 넣는다.
			// 유저 채널 리스트 전송

			// 체크리스트 전송
			
			// 캘린더 전송

			// 친구리스트 전송

			// 서버리스트 전송
			// 서버 정보채널 정보는 이후에 유저가 요청할 때 받는다.

		}

		// 로그인 이후의 처리
		private void AfterLogin()
		{
			while (!IsEmpty())
			{
				// 데이터를 받아 실행
				result = Receive();

				if (result.Value == null)
				{
					continue;
				}

				// 프로토콜을 매핑해서 실행
				ProtocolMapping.RunProtocolMethod(userCode, result);

				switch (result.Key)
				{
					case DataType.LOGOUT:
						Logout(result);
						break;
				}
			}
		}

		// 로그아웃 메소드
		// 로그인 상태의 클라이언트를 로그인 이전 상태로 돌린다.
		private void Logout(ReceiveResult result)
		{
			Console.WriteLine("로그아웃 시도");
			var logout = result.Value as LogoutProtocol.Logout;

			if (logout == null)
			{
				return;
			}

			var loginContainer = LoginContainer.Instance;

			if (loginContainer.IsLogin(logout.usercode))
			{
				Console.WriteLine("로그아웃 시작");
				// 로그인 컨테이너에서 유저 삭제 함수 추가 예정

				// 메시지 전송
				SendData(Generater.Generate("You have been logged out."));

				// 컨테이너에서 삭제
				if(null != userCode)
					LoginContainer.Instance.EraseUser((int)userCode, this);

				// 객체 초기화
				isLogin = false;
				userCode = null;

				// 로그인 이전 상태로 돌리기
				BeforeLoginEvent.ConnectCheckEvent += CheckConnection;

				// !Protocol Success 프로토콜 전송

				// !Protocol Toast 메시지 로그아웃이 완료되었습니다.
				// Toast 메시지 전송
			}
		}

		// 메시지 전송
		private void SendData(List<byte> bytes)
		{
			try
			{
				Send(bytes);
				// 연결이 끊겼다면
				if (!tcpClient.Connected)
				{
					Console.WriteLine(userCode + ": User Client is Disconnected.");
					throw new Exception();
				}
			}
			catch (Exception e)
			{
				// 컨테이너에서 삭제
				if (null != userCode)
					LoginContainer.Instance.EraseUser((int)userCode, this);

				// 연결이 안된 상태로 소켓을 닫는다.
				Console.WriteLine(e);

				Disconnection();
				throw;
			}
		}

		// 해당 클라이언트가 연결되어있는 상태인지 확인
		private void CheckConnection()
		{
			// 더미데이터 생성
			List<byte> dummy = new();
			isConnectProtocol.IsConnect isConnect = new(1);
			Generater.Generate(isConnect, ref dummy);

			// 데이터를 전송 한 후 상대방이 받으면 연결 유지
			try
			{
				SendData(dummy);
				// 연결이 끊겼다면
				if (!tcpClient.Connected)
				{
					Console.WriteLine(userCode + ": User Client is Disconnected.");
					throw new Exception();
				}
			}
			catch (Exception e)
			{
				// 컨테이너에서 삭제
				if (null != userCode)
					LoginContainer.Instance.EraseUser((int)userCode, this);

				// 연결이 안된 상태로 소켓을 닫는다.
				Console.WriteLine(e);

				Disconnection();
				throw;
			}
		}

		// 소켓을 닫기위한 메소드
		public void Disconnection()
		{
			Console.WriteLine(userCode + ": User Client is Disconnected.");

			// 로그인 이전 이벤트 끊기
			BeforeLoginEvent.ConnectCheckEvent -= CheckConnection;
			// Receive 이벤트를 끊는다.
			ReceiveEvent -= WakeUp;
			// Recevie를 종료한다.
			StopReceive();
			
			// 로그인 코드를 초기화
			isLogin = false;
			userCode = null;
		}
	}
}