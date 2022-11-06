using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerToClient;
using ServerToClinet;

using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;


// 나중에 DB 커넥트 정보를 받아와 실행해야 할지도

// 객체의 정보에 상관 없이 동작하는 프로토콜을 해당 클래스에서 정의해 사용
namespace ServerSystem
{
	delegate void ProtocolMethod(int userCode, ReceiveResult result);

	// 메소드 매핑 테이블
	public static class ProtocolMapping
	{
		// 메소드에 따른 데이터 전달
		private static readonly Dictionary<byte, ProtocolMethod> methodDict = new()
		{
			{DataType.MESSAGE, DefaultMethod}

		};

		public static void RunProtocolMethod(int? userCode, ReceiveResult result)
		{
			// 로그인 안된 사용자가 커멘드 입력을 시도했다면
			if (null == userCode)
				return;

			methodDict.TryGetValue(result.Key, out ProtocolMethod? protocolMethod);
			if (null == protocolMethod)
				DefaultMethod((int)userCode, result);
			else
				protocolMethod((int)userCode, result);
		}

		private static void DefaultMethod(int userCode, ReceiveResult result)
		{
			Client? target = LoginContainer.Instance.GetUser(userCode);
			if (null == target)
				return;

			target.Send(Generater.Generate("유효하지 않은 접근입니다."));
			// !Protocol Toast 개발중인 기능이라 전송하기
		}


	}
}
