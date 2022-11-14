#pragma warning disable CS1998		// null 역참조
#pragma warning disable CS4014		// async

using System.Data;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using MyMate_DB_Module;
using Protocol;

using ServerToClient;
using Convert = System.Convert;
using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;

namespace ServerSystem
{
	public class UserClient : Client
	{
		// 접속한 유저의 Code를 저장
		private int? userCode;
		public int UserCode
		{
			get { 
				if (userCode == null)
					return 0;
				else
					return userCode.Value;
			}
		}

		// 로그인 상태인지 확인
		private bool isLogin;
		// 객체에 접근하는 객체의 수를 제한(Send)
		private readonly Semaphore semaphore = new(1, 1);
		// Receive 에서 바이너리 데이터를 임시 저장하기 위한 리스트
		private List<byte>? bytes = new();
		// 결과값을 임시로 저장하는 객체 변수
		private ReceiveResult result;
		// SQL에 접근하기 위한 객체
		private SQL sql;
		private DataTable queryResult;

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

			// SQL 객체 생성
			SQL sql = new();
			queryResult = new DataTable();
		}

		// Receive로 일어나기 위한 코드
		private void WakeUp()
		{
			Console.WriteLine(userCode + "\t: Receive event is occurred.");

			// 연결 상태가 아니라면
			if (!tcpClient.Connected)
			{
				// 연결 종료
				Disconnection();
				return;
			}
			// 소켓이 연결 상태가 아니라면
			if (this.Stream != null)
			{
				if (this.Stream.Socket.Connected == false)
				{
					Disconnection();
					return;
				}
			}
			// 현재 Receive가 실행중이 아니라면
			if(!this.receiveRun)
			{
				Disconnection();
			}

			// 세마포어 획득을 시도
			if (!semaphore.WaitOne(10))
			{
				return;
			}

			Console.WriteLine(userCode + "\t: Semaphore is assigned.");

			// Receive 큐가 빌때까지 반복
			while (!IsEmpty())
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
			Console.WriteLine(userCode + "\t: Semaphore is returned.\n");
			semaphore.Release();
		}

		// 로그인 이전의 처리 (로그인, 회원가입)
		private void BeforeLogin()
		{

			// 수신받은 정보를 받아온다.
			result = Receive();

			switch (result.Key)
			{
				case DataType.LOGIN:
					// 로그인 시도
					if (Login(result))
					{
						// 로그인 성공시
						Console.WriteLine(userCode + "\t: Login Successed");
						// 필요한 기본 데이터 전부 전송
						BaseDataSend();
						return;
					}
					else
					{
						Console.WriteLine(userCode + "\t: Login Failed");
						Send(Generater.Generate(new ToastProtocol.TOAST(0, "Login", "Login Failed")));
					}
					break;
				case DataType.SIGNUP:
					// 회원가입 시도
					if (SignUp(result))
					{
						Console.WriteLine(userCode + "\t: SignUp Successed");
						Send(Generater.Generate(new ToastProtocol.TOAST(0, "SignUp", "SignUp Successed")));
					}
					else
					{
						Console.WriteLine(userCode + "\t: SignUp Failed");
						Send(Generater.Generate(new ToastProtocol.TOAST(0, "SignUp", "SignUp Failed")));
					}
					break;
				default:
					Console.WriteLine(userCode + "\t: 현재 상태에서는 사용할 수 없는 명령");
					break;
			}
		}

		// 로그인 이후의 처리 (로그아웃, 데이터 요청, 기반데이터 전송)
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

				switch (result.Key)
				{
					case DataType.LOGOUT:
						Logout(result);
						break;
					case DataType.REQUEST:
						RequestDataSend(result);
						break;
					case DataType.REQUEST_RECENT_ALL:
						RecentDataSend();
						break;
					//case DataType.DELETEREQUEST:
					default:
						// 프로토콜을 매핑해서 실행
						ProtocolMapping.RunProtocolMethod(userCode, result);
						break;
				}
			}
		}

		// 오버라이딩
		// 메시지 전송
		public new void Send(List<byte> bytes)
		{
			try
			{
				base.Send(bytes);
				// 연결이 끊겼다면
				if (!tcpClient.Connected)
				{
					Console.WriteLine(userCode + "\t: User Client is Disconnected.");
					throw new Exception();
				}
			}
			catch (Exception e)
			{
				// 접속이 끊어지거나 네트워크 오류 발생 시
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
			isConnectProtocol.ISCONNECT isConnect = new(1);
			Generater.Generate(isConnect, ref dummy);

			// 데이터를 전송 한 후 상대방이 받으면 연결 유지
			try
			{
				// 연결이 끊겼다면
				if (!tcpClient.Connected)
				{
					Console.WriteLine(userCode + "\t: User Client is Disconnected.");
					throw new Exception();
				}
				base.Send(dummy);
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
			Console.WriteLine(userCode + "\t: User Client is Disconnected.");
			// 로그인 이전 이벤트 끊기
			BeforeLoginEvent.ConnectCheckEvent -= CheckConnection;
			// Receive 이벤트를 끊는다.
			ReceiveEvent -= WakeUp;
			// Recevie를 종료한다.
			StopReceive();
			
			if(isLogin)
			{
				Console.WriteLine(userCode + "\t: Delete a User");
			}

			// 로그인 코드를 초기화
			isLogin = false;
			userCode = null;
		}

		// 최근 3일내의 정보를 가져와 보낸다.
		private void RecentDataSend()
		{
			Console.WriteLine(userCode + ": Recent data all send");
			//DateSendByTime(null,new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 3));

			// Message
			// 


		}

		// 최초 구동시 필요한 데이터를 뽑아내 전부 전달
		async private void BaseDataSend()
		{
			Console.WriteLine( userCode + " : BaseData Sending");
			// !Protocol Toast 기반데이터 전송 시작

			// 유저 채널 리스트 전송
			await SQLUsesrRequest.UserChnnalRequest(this, UserCode);

			// 서버리스트 전송
			await SQLUsesrRequest.ServerRequest(this, UserCode);

			// 유저 체크리스트 전송
			SQLUsesrRequest.UserProjectRequest(this, UserCode);

			// 유저 캘린더 전송
			SQLUsesrRequest.UserCalenderRequest(this, UserCode);

			// 친구리스트 전송
			SQLUsesrRequest.UserFriend(this, UserCode);

			// 서버채널 정보 전송
			await SQLUsesrRequest.ServerChannelRequest(this, UserCode);
			
		}

		// 요청한 데이터 전송
		async private void RequestDataSend( ReceiveResult result )
		{
			RequestProtocol.REQUEST? request = result.Value as RequestProtocol.REQUEST;

			if (request == null)
				return;

			switch(request.dataType)
			{
				case DataType.USER:
					SQLUsesrRequest.UserRequest(this, request.userCode);
					break;
				case DataType.SERVER:
					SQLUsesrRequest.ServerRequest(this, request.userCode, request.serverCode);
					break;
				case DataType.CHNNEL:
					SQLUsesrRequest.ServerChannelRequest(this, request.userCode, request.serverCode);
					break;
				case DataType.MESSAGE:
					SQLUsesrRequest.ServerMessageRequest(this, request.userCode, request.serverCode, request.channelCode, request.startTime, request.endTime);
					break;
				case DataType.CALENDER:
					if (request.serverCode == 0)
						SQLUsesrRequest.ServerCalenderRequest(this, request.userCode, request.serverCode, request.channelCode);
					else
						SQLUsesrRequest.UserCalenderRequest(this, request.userCode, request.channelCode);
					break;
				case DataType.CHECKLIST:
					if (request.serverCode == 0)
						SQLUsesrRequest.ServerProjectRequest(this, request.userCode, request.serverCode, request.channelCode);
					else
						SQLUsesrRequest.UserProjectRequest(this, request.userCode, request.channelCode);
					break;
				case DataType.FRIEND:
					SQLUsesrRequest.UserFriend(this, request.userCode);
					break;
				default:
					// !Protocol Toast 지원하지 않는 Request 입니다.
					break;
			}
		}

		// 로그인 메소드
		private bool Login(ReceiveResult result)
		{
			var login = result.Value as LoginProtocol.LOGIN;

			if (login == null)
			{
				Console.WriteLine("no Data");
				return false;
			}

			Console.WriteLine(userCode + ": Logging in...");
			Console.WriteLine(userCode + ": ID \"" + login.id + "\" has been attempted.");

			// UserParm 객체 생성
			UserParm userParm = new UserParm() { id = login.id, pwd = login.pw };

			// UserParm 값 할당
			// userParm.id = login.id;
			// userParm.pwd = login.pw;

			// 로그인 결과 받아올 DataTable 객체 생성
			//DataTable queryResult = new DataTable();

			// 로그인 query 실행
			// queryResult = sql.resultConnectDB((object)userParm,"Login");

			queryResult = sql.resultConnectDB(userParm, "Login");

			Console.WriteLine("DB Result\t: " + queryResult.Rows[0][0]);

			// 로그인 성공 시 작동할 부분
			if (queryResult.Rows[0][0] is true)
			{
				// 프로토콜 객체 생성
				LoginUserProtocol.LOGINUSER loginUser = new();

				// 로그인 이전 이벤트를 끊는다.
				BeforeLoginEvent.ConnectCheckEvent -= CheckConnection;

				// 유저의 데이터를 전송 가능한 형태로 집어 넣는다.
				//LoginUserProtocol.LOGINUSER user = new LoginUserProtocol.LOGINUSER();

				// 유저 코드 가져오는 query 실행 및 유저 코드 할당
				queryResult = sql.resultConnectDB((object)userParm, "GetUserCode");

				// 로그인 정보 삽입
				isLogin = true;
				this.userCode = Convert.ToInt32(queryResult.Rows[0]["U_code"]);

				// 로그인 컨테이너 등록
				LoginContainer.Instance.RegistUser((int)userCode, this);

				// 유저 정보 가져기 위해서 UserParm 값 할당
				userParm.dataFormat = null;

				// 유저 정보 가져오는 query 실행
				queryResult = sql.resultConnectDB((object)userParm, "GetUser");

				// 보낼 정보 세팅
				loginUser.Set(
					(int)this.userCode,
					"id",
					queryResult.Rows[0]["U_name"].ToString(),
					queryResult.Rows[0]["U_email"].ToString(),
					queryResult.Rows[0]["U_email"].ToString(),
					queryResult.Rows[0]["U_phone"].ToString(), 
					"context",
					DateTime.Now);

				// 유저에게 정보 전송
				Console.WriteLine(userCode + " Login Data Sending");
				Send(Generater.Generate(loginUser));
				return true;
			}

			return false;
		}

		// 로그아웃 메소드
		// 로그인 상태의 클라이언트를 로그인 이전 상태로 돌린다.
		private void Logout(ReceiveResult result)
		{
			Console.WriteLine("로그아웃 시도");
			var logout = result.Value as LogoutProtocol.LOGOUT;

			if (logout == null)
			{
				return;
			}

			if (logout.usercode != this.userCode)
			{
				return;
			}

			// 컨테이너에 존재하는지 확인
			var loginContainer = LoginContainer.Instance;
			if (loginContainer.IsLogin(logout.usercode))
			{
				// 컨테이너에서 삭제
				if (null != userCode)
					LoginContainer.Instance.EraseUser((int)userCode, this);
			}

			Console.WriteLine("로그아웃 시작");

			// 메시지 전송
			Send(Generater.Generate("You have been logged out."));

			// 객체 초기화
			isLogin = false;
			userCode = null;

			// 로그인 이전 상태로 돌리기
			BeforeLoginEvent.ConnectCheckEvent += CheckConnection;

			// !Protocol Success 프로토콜 전송

			// !Protocol Toast 메시지 "로그아웃이 되었습니다."
			// Toast 메시지 전송
		}

		// 회원가입 메소드
		private bool SignUp(ReceiveResult result)
		{
			Console.WriteLine("회원가입 시도");
			SignUpProtocol.SiginUp? siginUp = result.Value as SignUpProtocol.SiginUp;

			if (null == siginUp)
				return false;
			ToastProtocol.TOAST toast = new(0, "회원가입 시도");
			Send(Generater.Generate(toast));

			// SQL 객체 생성
			SQL sql = new();

			// UserParm 객체 생성
			UserParm userParm = new UserParm();

			// UserParm 객체 값 할당
			userParm.id = siginUp.id;
			userParm.pwd = siginUp.password;
			userParm.name = siginUp.name;
			userParm.nick = siginUp.nickname;
			userParm.email = siginUp.email;
			userParm.phone = siginUp.phone;

			if (!sql.checkValue((object)userParm))
			{
				// 유효성 검사 실패
				return false;
			}

			// !SQL 회원가입 
			if (sql.noResultConnectDB((object)userParm, "Signin")) // (SQL.SIGNUP())
			{
				// 성공시 
				// !Protocol Toast 회원가입 성공
				toast.content = "회원가입 시도";
				Send(Generater.Generate(toast));
				return true;
			}

			return false;
		}
	}
}