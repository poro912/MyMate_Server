using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerToClient;

using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;


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
			{DataType.SERVER        , ProtocolMethods.PassToServer},	// 서버
			{DataType.CHNNEL        , ProtocolMethods.DefaultMethod},	// 클라이언트, 서버
			{DataType.MESSAGE       , ProtocolMethods.MessageMethod},	// 서버
			{DataType.CHECKLIST     , ProtocolMethods.DefaultMethod},	// 클라이언트, 서버
			{DataType.CALENDER      , ProtocolMethods.DefaultMethod},	// 클라이언트, 서버
			{DataType.FRIEND        , ProtocolMethods.DefaultMethod},   // 클라이언트
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

	public static class ProtocolMethods
	{
		// 서버로 데이터를 넘겨준다.
		public static void PassToServer(int userCode, ReceiveResult result)
		{
			MessageProtocol.MESSAGE? target = result.Value as MessageProtocol.MESSAGE;
			if (null == target)
				return;
			//target.serverCode;
			UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
			if (null == server)
				return;
			// 비동기 실행
			server.Process(result);
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
				ServerContainer.Instance.ServerCreate(userCode, target.title);

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

		public static void MessageMethod(int userCode, ReceiveResult result)
		{
			// 메시지를 result에 등록된 서버에 전달한다.
			MessageProtocol.MESSAGE? target = result.Value as MessageProtocol.MESSAGE;
			if (null == target)
				return;
			//target.serverCode;
			UserServer? server = ServerContainer.Instance.GetServer(target.serverCode);
			if (null == server)
				return;
			// 비동기 실행
			server.Process(result);
		}

		

		public static void DefaultMethod(int userCode, ReceiveResult result)
		{
			Client? target = LoginContainer.Instance.GetUser(userCode);
			if (null == target)
				return;

			target.Send(Generater.Generate("유효하지 않은 접근입니다."));

			// !Protocol Toast 개발중인 기능이라 전송하기
		}

		/*
		private static void Method(int userCode, ReceiveResult result)
		{
			MessageProtocol.MESSAGE? msg =  result.Value as MessageProtocol.MESSAGE;
			if (null == msg)
				return;
		}

		private static void Method(int userCode, ReceiveResult result)
		{
			MessageProtocol.MESSAGE? msg =  result.Value as MessageProtocol.MESSAGE;
			if (null == msg)
				return;
		}

		private static void Method(int userCode, ReceiveResult result)
		{
			MessageProtocol.MESSAGE? msg =  result.Value as MessageProtocol.MESSAGE;
			if (null == msg)
				return;
		}

		private static void Method(int userCode, ReceiveResult result)
		{
			MessageProtocol.MESSAGE? msg =  result.Value as MessageProtocol.MESSAGE;
			if (null == msg)
				return;
		}

		private static void Method(int userCode, ReceiveResult result)
		{
			MessageProtocol.MESSAGE? msg =  result.Value as MessageProtocol.MESSAGE;
			if (null == msg)
				return;
		}

		private static void Method(int userCode, ReceiveResult result)
		{
			MessageProtocol.MESSAGE? msg =  result.Value as MessageProtocol.MESSAGE;
			if (null == msg)
				return;
		}
		*/
	}
}
