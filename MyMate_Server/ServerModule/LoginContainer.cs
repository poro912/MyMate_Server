using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerToClient;

namespace ServerSystem
{
	public class LoginContainer
	{
		// 코루틴 구현
		static private LoginContainer? instance;
		static public LoginContainer Instance
		{
			get 
			{ 
				if (null == instance)
				{
					instance = new();
				}
				return instance; 
			}
		}

		public Dictionary<int, UserClient> loginDict;

		private LoginContainer()
		{
			loginDict = new Dictionary<int, UserClient>();
		}

		// 해당 유저가 로그인 상태인지 확인
		// login	: true
		// logout	: false
		public bool IsLogin(int userCode)
		{
			loginDict.TryGetValue(userCode, out UserClient? temp);

			return null != temp;
		}

		// 유저의 클라이언트 저장을 시도한다.
		public void RegistUser(int userCode, UserClient client)
		{
			// 기존에 이미 로그인 상태라면 (로그인 정보가 있다면)
			if(IsLogin(userCode))
			{
				loginDict.TryGetValue(userCode, out UserClient? temp);
				
				// 기존에 사용하던 소켓을 닫아버림
				if (temp != null)
				{
					// !Protocol Toast 다른 클라이언트에서 로그인을 시도 하였습니다.
					temp.Disconnection();
				}
					
				loginDict[userCode] = client;
				// !Protocol Toast 다른 클라이언트에서 해당 계정을 사용중 이였습니다.
			}
			// 로그인 상태가 아니라면
			else
			{
				loginDict.TryAdd(userCode, client);
				client.Start();
			}
		}

		//  등록된 유저를 삭제
		public void EraseUser(int userCode, UserClient target)
		{
			// 현재 메소드 이름 출력
			// Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
			Console.WriteLine("Attempt\t: Delete a User from Login Dictionary");
			loginDict.TryGetValue(userCode, out UserClient? temp);


			// 서로 같은 객체라면 삭제한다.
			if(target == temp)
			{
				loginDict.Remove(userCode);
				Console.WriteLine("Complite\t: 같은객체임을 확인 삭제.\n");
			}
			else
			{
				Console.WriteLine("Fail\t: 서로 다른 객체 삭제 불가\n");
			}
		}

		// 유저 데이터 획득을 시도한다.
		public Client? GetUser(int userCode)
		{
			loginDict.TryGetValue(userCode, out UserClient? value);
			if (value == null)
				return null;
			return value;
		}

		// 유저의 데이터가 있다면 true
		// 유저의 데이터가 없다면 false
		public bool GetUser(int userCode, out UserClient? client)
		{
			loginDict.TryGetValue(userCode, out client);
			if (client == null)
				return false;
			return true;
		}

		// 유저에게 데이터 전송
		// 유저의 SendQueue에 데이터를 넣어준다.
		public void Send(int userCode, List<byte> target)
		{
			if(GetUser(userCode, out UserClient? user))
			{
				if(null != user)
					user.Send(target);
			}
		}
	}
}
