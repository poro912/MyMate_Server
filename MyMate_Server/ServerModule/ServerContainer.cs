using Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;

namespace ServerSystem
{
	// 서버측에서 발생한 이벤트
	public delegate void ServerEvent();
	public delegate void SendServer();
	public class ServerContainer
	{
		static private ServerContainer? instance;
		static public ServerContainer Instance
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
		public Dictionary<int, UserServer> serverDict;

		private ServerContainer() 
		{
			serverDict = new Dictionary<int, UserServer>();
			LoadServer();
		}

		// 새로운 서버 하나를 등록
		void Regist(UserServer server)
		{
			Console.WriteLine(": Create new Server");

			// !SQL Server CreateCode

			// !SQL 만들어진 서버 시퀀스를 서버에 저장
			// server.serverCode = ;
			// Console.WriteLine(": Fail to Create new Server");

			serverDict.Add(server.serverCode, server);
			Console.WriteLine(": Complite to Create new Server");

			// 유저의 데이터를 넣는다.
			// server.InvateUser();
			// server.CreateUser();

		}

		// DB에서 서버를 로드함 (Server 최초 실행시)
		private void LoadServer()
		{
			Console.WriteLine(": Server List Load to DB");
			// !SQL 서버목록들을 불러온다.
			/*
			foreach(Server temp in Servercodes )
			{
				serverDict.Add(serverCode, temp);
			}
			*/
		}
	}

	public class UserServer
	{
		public int serverCode;
		public List<int> inviteUserList;
		public List<int> channelList;

		// send queue와 세마포 선언
		private ConcurrentQueue<List<byte>> sendQueue;


		public UserServer(int serverCode)
		{
			this.serverCode = serverCode;
			inviteUserList = new List<int>();
			channelList = new List<int>();
			sendQueue = new ConcurrentQueue<List<byte>>();
		}

		public void InvateUser()
		{
			// !SQL 서버에 접근한 유저 추가
		}

		public void CreateChannel()
		{
			// !SQL 서버에 채널 추가
		}

		// 서버의 모든 사람에게 데이터를 전송한다.
		// 외부에서 서버에게 요청하는 메소드
		// 스레드와 비슷하게 작동해야 함
		// 데이터를 DB에 적용하고 각 유저에게 보낼 수 있도록 가공 저장한다.
		async public void Send(ReceiveResult target)
		{
			List<byte>? temp = null;

			// !SQL 서버에 변경사항 발생
			// 메시지 송신, 체크리스트 변경, 캘린더 변경

			// 메시지
			// 송신

			// 체스리스트
			// 수정		생성전 확인해서 DB에 이미 있는 데이터라면 수정
			// 생성		저장 및 전송
			// 삭제		isDeleted 속성 변경

			// 캘린더
			// 수정		생성전 확인해서 DB에 이미 있는 데이터라면 수정
			// 생성		저장 및 전송
			// 삭제		isDeleted 속성 변경

			if (temp != null)
				sendQueue.Enqueue(temp);
		}

		// 각 유저들에게 데이터를 전송
		async private void Sending()
		{
			while(!sendQueue.IsEmpty)
			{
				sendQueue.TryDequeue(out List<byte>? target);
				if (target == null)
					continue;

				for(int i = 0 ; i < inviteUserList.Count; i++)
				{
					// userContainer를 참조하여 전부 보내줌
					LoginContainer.Instance.Send(inviteUserList[i], target);
				}
			}
		}
	}
}
