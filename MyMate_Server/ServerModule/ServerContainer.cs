#pragma warning disable CS1998

using Protocol;
using System.Collections.Concurrent;

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

		public void ServerCreate(int userCode, string title)
		{
			Console.WriteLine(userCode + ": Create new Server");


			// !SQL 서버 생성 후 서버코드 반환
			// !Protocol 서버 생성 결과 전송

			// server.serverCode = ;
			// Console.WriteLine(": Fail to Create new Server");

			UserServer server = new(userCode, 1, "temp");

			serverDict.Add(server.serverCode, server);
			Console.WriteLine(server.serverCode + ": Complite to Create new Server");

			// 유저의 데이터를 넣는다.
			server.EnterUser(userCode);


			// !Protocol Toast userCode에게 생성 완료 메시지 전송 

			// !Protocol Toast userCode에게 생성 실패 메시지 전송

			// Send()
		}

		public void ServerDelete(int serverCode, int userCode)
		{
			// 서버코드의 크리에이터와 같은지 확인한 후 삭제한다.

			// !SQL 서버 삭제 

			// !Protocol Toast userCode에게 삭제 메시지 전송
		}

		public UserServer? GetServer(int serverCode)
		{
			serverDict.TryGetValue(serverCode, out UserServer? server);
			return server;
		}

		// DB에서 서버를 로드함 (Server 최초 실행시)
		private void LoadServer()
		{
			Console.WriteLine(": All Server Load to DB");
			
			// !SQL 서버목록들을 불러온다.
			/*
			foreach(var server in servers )
			{
				UserServer temp_server = new(server.Code,server.Title);
				// !SQL 채널 목록을 불러온다.
				foreach(int channel in channels)
				{
					server.CreateChannel();
				}
				// !SQL 유저 목록을 불러온다.
				foreach(int user in users)
				{
					server.AddUser(user);
				}
				
				// 컨테이너에 서버 추가
				serverDict.Add(server.serverCode, temp_server);

			}
			*/
		}
	}

	public class UserServer
	{
		public string title;
		public int serverCode;
		public int owner;
		public List<int> enterUserList;
		public List<int> channelList;

		// send queue와 세마포 선언
		//private ConcurrentQueue<List<byte>> sendQueue;


		public UserServer(int owner, int serverCode, string title)
		{
			this.serverCode = serverCode;
			this.title = title;
			enterUserList = new List<int>();
			channelList = new List<int>();
			this.owner = owner;
			//sendQueue = new ConcurrentQueue<List<byte>>();
		}

		public bool IsMember(int userCode)
		{
			foreach (int user in enterUserList)
			{
				if (user == userCode)
				{
					return true;
				}
			}
			return false;
		}

		// 데이터를 받아와서 처리함
		async public void Process(int userCode, ReceiveResult target)
		{
			if (!IsMember(userCode))
				return;

			switch(target.Key)
			{
				case DataType.MESSAGE:

					break;

				default:
					break;
			}

			// if(target.serverCode == 0 && target.serverCode != this.serverCode)
			// 서버코드가 0이거나 serverCode가 같지 않다면 잘못온 것이므로 처리하지 않는다.

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

			// Send(target);
		}

		public void ModifyServer(int userCode ,ServerProtocol.Server info)
		{
			// 소유주가 변경을 요청했다면
			if(userCode == owner)
			{
				// !SQL 서버 변경사항 DB 추가

				this.title = info.title;
				this.owner = info.adminCode;
			}

			// !Protocol Server메시지 전송
		}

		public void EnterUser(int userCode)
		{
			// !SQL 서버에 접근한 유저 추가
			// 이미 들어온 적 있다면 코드만 변경
			enterUserList.Add(userCode);

			// !Protocol User메시지 전송
		}

		public void LeaveUser(int userCode)
		{
			// !SQL 서버에 퇴장한 유저 추가

			enterUserList.Remove(userCode);

			// !Protocol User메시지 전송
		}

		public void CreateChannel(int userCode, string title,int channelType)
		{
			int channelCode = 10;
			// Protocol.ChannelType.
			// !SQL 서버에 채널 추가

			// 채널 생성 성공 시 채널코드 반환해야함
			channelList.Add(channelCode);

			Send(userCode, Generater.Generate(new ChannelProtocol.CHNNEL(this.serverCode, channelCode, title, channelType)));
		}

		public void DeleteChannel(int channelCode)
		{
			// !SQL 서버 채널 삭제 (isDelete 속성 fasle로 만들기)

			// !Protocol 채널 삭제 메시지 전송
			// Send()
		}

		// 채널 내용 변경
		public void ModifyChannel(int userCode, ChannelProtocol.CHNNEL info)
		{
			if(userCode == owner)
			{
				// !SQL 채널 변경사항 DB 추가
			}
			// 채널의 생성자가 해당 유저라면
			// if()
			//{
			// !SQL 채널 변경사항 DB 추가
			//}

			// !Protocol 채널 메시지 전송
			// Send()
		}

		public void Message(int userCode, MessageProtocol.MESSAGE msg)
		{
			if (msg.creater != userCode)
				return;

			is


			// !SQL 메시지 저장

			Send(userCode, Generater.Generate(msg));
		}


		// 서버의 모든 사람에게 데이터를 전송한다.
		// 스레드와 비슷하게 작동해야 함
		/*
		private void Send(ReceiveResult target)
		{
			List<byte>? temp = null;

			

			// 만약 데이터 SQL에 저장이 완료되면 해당 데이터를 큐에 삽입
			if (temp != null)
				sendQueue.Enqueue(temp);
		}
		private void Send(List<byte> target)
		{
			// 만약 데이터 SQL에 저장이 완료되면 해당 데이터를 큐에 삽입
			if (target != null)
				sendQueue.Enqueue(target);
		}
		*/
		// 각 유저들에게 데이터를 전송
		async private void Send(int userCode, List<byte> target)
		{


			Console.WriteLine(userCode + " : MessageSending");
			foreach(var user in enterUserList)
			{
				// userContainer를 참조하여 전부 보내줌
				LoginContainer.Instance.Send(user, target);
			}
		}
	}
}
