using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerToClient;

using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;
using MyMate_DB_Module;
using System.Data;
using static Protocol.UserProtocol;


/*
				// 제어 타입
				{DataType.LOGIN         , LoginProtocol.Convert},
				{DataType.LOGOUT        , LogoutProtocol.Convert},
				{DataType.ISCONNECT     , isConnectProtocol.Convert},
				//{DataType.FAIL			, FailProtocol.Convert},
				{DataType.REQUEST       , RequestProtocol.Convert},
				{DataType.REQUEST_RECENT_ALL , RequestRecentAllProtocol.Convert},
				{DataType.TOAST         , ToastProtocol.Convert},
				{DataType.SIGNUP        , SignUpProtocol.Convert},

				// 클래스 자료형
				{DataType.USER          , UserInfoProtocol.Convert},
				{DataType.MESSAGE       , MessageProtocol.Convert },
				{DataType.SERVER        , ServerProtocol.Convert},
				{DataType.CHECKLIST     , CheckListProtocol.Convert},
				{DataType.CHNNEL        , ChannelProtocol.Convert},
				{DataType.CALENDER      , CalenderProtocol.Convert},
				{DataType.FRIEND        , FriendProtocol.Convert},
				{DataType.LOGINUSER     , UserInfoProtocol.Convert} 
 */


// 나중에 DB 커넥트 정보를 받아와 실행해야 할지도

// 객체의 정보에 상관 없이 동작하는 프로토콜을 해당 클래스에서 정의해 사용
namespace ServerSystem
{
	delegate void ProtocolMethod(int userCode, ReceiveResult result);

	// 메소드 매핑 테이블
	public static class ProtocolMapping
	{
		// 데이터에따른 실행
		// 서버에서 가져다 쓸 메소드
		private static readonly Dictionary<byte, ProtocolMethod> methodDict = new()
		{
			{DataType.USER			, ProtocolMethods.UserMethod},		// 클라이언트
			{DataType.SERVER        , ProtocolMethods.ServerMethod},	// 서버
			{DataType.CHNNEL        , ProtocolMethods.ChannelMethod},	// 클라이언트, 서버
			{DataType.MESSAGE       , ProtocolMethods.MessageMethod},	// 서버
			{DataType.CHECKLIST     , ProtocolMethods.ChecklistMethod},	// 클라이언트, 서버
			{DataType.CALENDER      , ProtocolMethods.CalendarMethod},	// 클라이언트, 서버
			{DataType.FRIEND        , ProtocolMethods.FriendMethod},   // 클라이언트
			{DataType.INVITE		, ProtocolMethods.InviteMethod }
			//{DataType.DELIVER		,ProtocolMethods.DefaultMethod}
		};

		// 프로토콜들에 대한 메소드를 시작하기 위한 메소드
		public static void RunProtocolMethod(int? userCode, ReceiveResult result)
		{
			// 로그인 안된 사용자가 커멘드 입력을 시도했다면
			if (null == userCode)
				return;

			// 메소드 실행
			methodDict.TryGetValue(result.Key, out ProtocolMethod? protocolMethod);

			// 불러온 명령이 있다면
			if (null != protocolMethod)
				protocolMethod((int)userCode, result);
			// 불러온 명령이 없다면
			else
				ProtocolMethods.DefaultMethod((int)userCode, result);

		}
	}
	// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡMethodsㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
	public static class ProtocolMethods
	{
		public static void UserMethod(int userCode, ReceiveResult result)
		{
			UserProtocol.USER? target = result.Value as UserProtocol.USER;
			Client? user;
			Console.WriteLine(userCode + "\t: Modify User Infomation");

			if (null == target)
				return;
			if (target.userCode != userCode)
				return;

			// !SQL 유저데이터 변경
			SQL sql = new();
			DataTable? queryResult;

			if(false)
			{
				Console.WriteLine(userCode + "\t: Failed to User Information Modify");
			}
			Console.WriteLine(userCode + "\t: Successed to User Information Modify");
			// 바뀐값만 담아 전송
			user = LoginContainer.Instance.GetUser(userCode);
			if(user != null)
				user.Send(Generater.Generate(new LoginUserProtocol.LOGINUSER(userCode)));
		}

		public static void ServerMethod(int userCode, ReceiveResult result)
		{
			// 서버에 대한 생성 정보나 변경 정보를 전달한다.
			ServerProtocol.Server? target = result.Value as ServerProtocol.Server;
			if (null == target)
				return;

			// 생성
			if(target.serverCode == 0)
			{
				// create server 호출
				ServerContainer.Instance.ServerCreate(userCode, target.title, target.isSingle);

				// !Protocol 서버 생성
				return;
			}

			// 서버정보 수정
			UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);

			// 해당하는 서버가 없다면 종료
			if (null == server)
				return;

			server.ModifyServer(userCode, target);

			// 비동기 실행
			//server.Process(result);
		}

		public static void ChannelMethod(int userCode, ReceiveResult result)
		{
			Client? user;
			ChannelProtocol.CHANNEL? target = result.Value as ChannelProtocol.CHANNEL;
			if (null == target)
				return;
			
			// 채널 생성
			if (0 == target.channelCode)
			{
				// 개인 채널 생성
				if (0 == target.serverCode)
				{
					//ChannelType.Calender;
					// !SQL 개인 채널 생성

					target.channelCode = 10;

					user = LoginContainer.Instance.GetUser(userCode);
					if (user != null)
						user.Send(Generater.Generate(target));
				}
				// 서버 채널 생성
				else
				{
					UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
					if (null == server)
						return;
					server.CreateChannel(userCode, target);
				}
			}
			// 변경
			else
			{
				// 개인 채널 변경
				if (0 == target.serverCode)
				{
					// !SQL 개인 채널 변경

					user = LoginContainer.Instance.GetUser(userCode);
					if (user != null)
						user.Send(Generater.Generate(target));
				}
				// 서버 채널 변경
				else
				{
					UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
					if (null == server)
						return;
					server.ModifyChannel(userCode, target);
				}
			}
		}

		public static void MessageMethod(int userCode, ReceiveResult result)
		{
			// 메시지를 result에 등록된 서버에 전달한다.
			MessageProtocol.MESSAGE? target = result.Value as MessageProtocol.MESSAGE;

			if (null == target)
				return;

			// 서버를 불러온다.
			UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);

			// 서버가 없다면 종료
			if (null == server)
				return;

			// 비동기 실행
			server.Message(userCode, target);
		}

		public static void CalendarMethod(int userCode, ReceiveResult result)
		{
			Client? user;
			CalenderProtocol.CALENDER? target = result.Value as CalenderProtocol.CALENDER;
			if (null == target)
				return;

			// 캘린더(일정) 생성
			if(0 == target.calenderCode)
			{
				// 개인 일정 생성
				if (0 == target.serverCode)
				{
					// !SQL 개인 일정 생성

					target.calenderCode = 10;

					user = LoginContainer.Instance.GetUser(userCode);
					if (user != null)
						user.Send(Generater.Generate(target));
				}
				// 서버 일정 생성
				else
				{
					UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
					if (null == server)
						return;
					server.CreateCalendar(userCode, target);
				}
			}
			// 변경
			else
			{
				// 개인 일정 변경
				if (0 == target.serverCode)
				{
					// !SQL 개인 일정 변경

					user = LoginContainer.Instance.GetUser(userCode);
					if (user != null)
						user.Send(Generater.Generate(target));
				}
				// 서버 일정 변경
				else
				{
					UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
					if (null == server)
						return;
					server.ModifyCalendar(userCode, target);
				}
			}
		}

		public static void ChecklistMethod(int userCode, ReceiveResult result)
		{
			Client? user;
			CheckListProtocol.CHECKLIST? target = result.Value as CheckListProtocol.CHECKLIST;
			if (null == target)
				return;

			// 체크리스트 (체크리스트) 생성
			if (0 == target.checkListCode)
			{
				// 개인 체크리스트 생성
				if (0 == target.serverCode)
				{
					// !SQL 개인 체크리스트 생성

					target.checkListCode = 10;

					user = LoginContainer.Instance.GetUser(userCode);
					if (user != null)
						user.Send(Generater.Generate(target));
				}
				// 서버 체크리스트 생성
				else
				{
					UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
					if (null == server)
						return;
					server.CreateChecklist(userCode, target);
				}
			}
			// 변경
			else
			{
				// 개인 체크리스트 변경
				if (0 == target.serverCode)
				{
					// !SQL 개인 체크리스트 변경

					user = LoginContainer.Instance.GetUser(userCode);
					if (user != null)
						user.Send(Generater.Generate(target));
				}
				// 서버 체크리스트 변경
				else
				{
					UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
					if (null == server)
						return;
					server.ModifyChecklist(userCode, target);
				}
			}
		}

		public static void FriendMethod(int userCode, ReceiveResult result)
		{
			FriendProtocol.FRIEND? target = result.Value as FriendProtocol.FRIEND;
			Client? user;
			Console.WriteLine(userCode + "\t: Modify User Infomation");

			if (null == target)
				return;
			if (target.userCode != userCode)
				return;

			SQL sql = new();
			DataTable? queryResult;

			// !SQL 친구 데이터가 있는지 확인

			// 데이터가 있다면
			if(true)
			{
				// !SQL 친구 데이터 변경
			}
			else
			{
				// !SQL 친구 데이터 추가
			}


			// 바뀐값만 담아 전송
			user = LoginContainer.Instance.GetUser(userCode);
			if (user != null)
				user.Send(Generater.Generate(target));
		}

		public static void InviteMethod(int userCode, ReceiveResult result)
		{
			InviteProtocol.Invite? target = result.Value as InviteProtocol.Invite;
			Client? user;
			Console.WriteLine(userCode + "\t: Modify User Infomation");

			if (null == target)
				return;
			if (target.userCode != userCode)
				return;

			// 서버정보 수정
			UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);

			// 해당하는 서버가 없다면 종료
			if (null == server)
				return;

			server.EnterUser(userCode);
		}

		public static void DefaultMethod(int userCode, ReceiveResult result)
		{
			Client? target = LoginContainer.Instance.GetUser(userCode);
			if (null == target)
				return;

			target.Send(Generater.Generate("유효하지 않은 접근입니다."));

			// !Protocol Toast 개발중인 기능이라 전송하기
		}
	}
}
