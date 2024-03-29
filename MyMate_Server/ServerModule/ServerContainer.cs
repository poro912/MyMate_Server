﻿#pragma warning disable CS1998

using Protocol;
using System.Collections.Concurrent;
using MyMate_DB_Module;

using ReceiveResult = System.Collections.Generic.KeyValuePair<byte, object?>;
using ServerToClient;
using Convert = System.Convert;
using System.Data;
using System.Globalization;

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

		public void ServerCreate(int userCode, string title, bool isSingle)
		{
			int serverCode = 0;
			DataTable queryResult;
			SQL sql;
			ServerParm serverParm;
			Console.WriteLine(userCode + "\t: Create new Server");

			// 유저 데이터를 얻을 수 없다면
			Client? user = LoginContainer.Instance.GetUser(userCode);
			if (null == user)
				return;

			// !SQL 서버 생성 후 서버코드 반환
			sql = new();
			serverParm = new ServerParm();

			serverParm.title = title;
			serverParm.adminCode = userCode;
			serverParm.isSingle = Convert.ToInt32(isSingle);

			queryResult = new DataTable();

            queryResult = sql.resultConnectDB((object)serverParm, "AddServer");

			serverCode = Convert.ToInt32(queryResult.Rows[0][0]);
			
			// 실패
			if(serverCode == 0)
			{
				Console.WriteLine(userCode + "\t: Fail to Create new Server");
				user.Send(Generater.Generate(new ToastProtocol.TOAST(0,"서버생성","서버 생성 실패")));
			}

			Console.WriteLine(serverCode + "\t: Success to Create new Server");

			UserServer server = new(userCode, serverCode, "temp");
			// 유저의 데이터를 넣는다.
			server.EnterUser(userCode);

			// 서버 컨테이너에 넣는다.
			serverDict.Add(server.serverCode, server);

			// !Protocol 서버 생성 결과 전송
			user.Send(Generater.Generate(new ServerProtocol.Server(serverCode,title,userCode,isSingle)));

			// !Protocol Toast userCode에게 생성 완료 메시지 전송 
			user.Send(Generater.Generate(new ToastProtocol.TOAST(0, "서버생성", "서버 생성 성공")));
		}

		public void ServerDelete(int serverCode, int userCode)
		{
			// 서버코드의 크리에이터와 같은지 확인한 후 삭제한다.
			Console.WriteLine(serverCode + "\t: Delete Server");
			// !SQL 서버 삭제 
			SQL sql = new();

			ServerParm serverParm = new ServerParm();

			serverParm.serverCode = serverCode;
			serverParm.adminCode = userCode;

			// 서버 is_deleted 속성 true로 변경
			sql.noResultConnectDB((object)serverParm, "RemoveServer");

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
			Console.WriteLine("Attempt\t: Load All Server Information to DB");

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
			Console.WriteLine("Complite\t: Load All Server Information\n");
		}
	}

	public class UserServer
	{
		public string title;
		public int serverCode;
		public int owner;
		public List<int> enterUserList;
		public List<int> channelList;

		private SQL sql;

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
			sql = new();
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


		public void ModifyServer(int userCode ,ServerProtocol.Server info)
		{
			Console.WriteLine(serverCode + " : Server information Modify");
			// 소유주가 변경을 요청했다면
			if(userCode == owner)
			{
				ServerParm serverParm = new ServerParm();

				serverParm.serverCode = info.serverCode;
				serverParm.adminCode = info.adminCode;
				serverParm.title = info.title;

				// 변경 실패시
				if(!sql.noResultConnectDB((object)serverParm, "SetServer"))
				{
					Console.WriteLine(serverCode + " : Fail to Server information Modify");
					return;
				}

				this.title = info.title;
				this.owner = info.adminCode;

				Console.WriteLine(serverCode + " : Success to Server information Modified");

				// 데이터 전송
				this.Send(userCode, Generater.Generate(new ServerProtocol.Server(this.serverCode, this.title, this.owner)));
			}
		}

		public void EnterUser(int userCode)
		{
            // !SQL 서버에 접근한 유저 추가
            SQL sql = new();

            ServerUserParm serverUserParm = new();

			Console.WriteLine(serverCode + " : User Enter the Server\tuser\t: " + userCode);

			serverUserParm.serverCode = this.serverCode;
            serverUserParm.userCode = userCode;

            sql.noResultConnectDB((object)serverUserParm, "AddServeruser");

			// !SQL 추가 목록에 있는지 확인

			
			if(true)
			{
				// 추가된 적 있다면 상태 변경

			}
			else
			{
				// 추가된 적 없다면 새로 추가

			}

			// 이미 들어온 적 있다면 코드만 변경
			enterUserList.Add(userCode);

			// !Protocol User메시지 전송
			Send(userCode, Generater.Generate(new InviteProtocol.Invite(userCode, this.serverCode, "")));
		}

		public void LeaveUser(int userCode)
		{
			// !SQL 서버에 퇴장한 유저 추가
			if (!IsMember(userCode))
				return;

			Console.WriteLine(serverCode + " : User Leave the Server\tuser\t:" + userCode);

			ServerUserParm serverUserParm = new();

			serverUserParm.serverCode = this.serverCode;
			serverUserParm.userCode = userCode;
			serverUserParm.calIsPrivate = null;
			serverUserParm.isDeleted = Convert.ToInt32(true);

			sql.noResultConnectDB((object)serverUserParm,"SetServeruser");

			enterUserList.Remove(userCode);

			// !Protocol User메시지 전송
			Send(userCode, Generater.Generate(new InviteProtocol.Invite(userCode, this.serverCode, "")));
		}

		public void CreateChannel(int userCode, ChannelProtocol.CHANNEL channel)
		{
			Console.WriteLine(serverCode + " : Create new Channel\ttype\t: " + channel.type);
			if (!IsMember(userCode))
				return;

			if (userCode != owner)
				return;

			// int channelCode = 10;
			// Protocol.ChannelType.
			// !SQL 서버에 채널 추가
			sql = new();

			ChannelParm channelParm = new ChannelParm();

			channelParm.serverCode = this.serverCode;
			channelParm.title = channel.title;
			channelParm.state = channel.type;

			DataTable queryResult = new DataTable();

			queryResult = sql.resultConnectDB((object)channelParm, "AddChannel");

			int channelCode = Convert.ToInt32(queryResult.Rows[0][0]);

			// 채널 생성 성공 시 채널코드 반환해야함
			channelList.Add(channelCode);
			channel.channelCode = channelCode;
			Send(userCode, Generater.Generate(channel));
		}

		public void DeleteChannel(int userCode, int channelCode)
		{
			Console.WriteLine(serverCode + " : Delete Channel\tChannel\t: " + channelCode);
			if (userCode != owner)
				return;
			// !SQL 서버 채널 삭제 (isDelete 속성 fasle로 만들기)
			SQL sql = new();

			ChannelParm channelParm = new ChannelParm();

			channelParm.serverCode = this.serverCode;
			channelParm.channelCode = channelCode;
			channelParm.state = null;
			channelParm.title = null;
			channelParm.isDeleted = Convert.ToInt32(true);

			sql.noResultConnectDB((object)channelParm,"SetChannel");


			// !Protocol 채널 삭제 메시지 전송
			// Send()
		}

		// 채널 내용 변경
		public void ModifyChannel(int userCode, ChannelProtocol.CHANNEL info)
		{
			// !SQL 채널 변경사항 DB 추가
			SQL sql = new();
			Console.WriteLine(serverCode + " : Modify or Delete Channel\tChannel\t: " + info.channelCode);
			ChannelParm channelParm = new ChannelParm();

			channelParm.serverCode = this.serverCode;
			channelParm.channelCode = info.channelCode;
			channelParm.state = info.type;
			channelParm.title = info.title;
			channelParm.isDeleted = null;

			sql.noResultConnectDB((object)channelParm, "SetChannel");
			// 채널의 생성자가 해당 유저라면
			if (userCode != owner)
				return;
			// if()
			//{
			// !SQL 채널 변경사항 DB 추가
			//}

			// !Protocol 채널 메시지 전송
			//
			Send(userCode, Generater.Generate(info));
		}

		public void Message(int userCode, MessageProtocol.MESSAGE msg)
		{
			if (msg.creater != userCode)
				return;

			Console.WriteLine(serverCode + " : Message Send\tuser\t: " + msg.creater);

			// !SQL 메시지 저장

			MessageParm messageParm = new MessageParm();

			messageParm.channelCode = msg.channelCode;
			messageParm.content= msg.content;
            messageParm.creater = msg.creater;
            messageParm.isDeleted = null;
            messageParm.isPrivate = Convert.ToInt32(msg.isPrivate);
            messageParm.messageCode = msg.messageCode;
			messageParm.serverCode = msg.serverCode;
			messageParm.startTime = msg.startTime.Ticks;

			sql.noResultConnectDB((object)messageParm, "AddMessage");
          
			Send(userCode, Generater.Generate(msg));
		}
		public void CreateCalendar(int userCode, CalenderProtocol.CALENDER calendar)
		{
			Console.WriteLine(serverCode + " : Create new Calender");
			// !SQL 캘린더 생성
			if (!IsMember(userCode))
				return;

			calendar.calenderCode = 10;

			Send(userCode, Generater.Generate(calendar));
		}
		public void DeleteCalendar(int userCode, DeleteRequestProtocol.DELETE_REQUEST calendar)
		{
			// !SQL 채널 삭제

			Send(userCode, Generater.Generate(calendar));
		}
		public void ModifyCalendar(int userCode, CalenderProtocol.CALENDER calendar)
		{
			Console.WriteLine(serverCode + " : Modify or Delete Calendar\tCalendar\t: " + calendar.channelCode);
			// !SQL 채널 변경
			if (!IsMember(userCode))
				return;


			Send(userCode, Generater.Generate(calendar));
		}
		public void CreateChecklist(int userCode, CheckListProtocol.CHECKLIST check)
		{
			Console.WriteLine(serverCode + " : Create new Checklist");
			// !SQL 체크리스트 생성
			if (!IsMember(userCode))
				return;

			check.checkListCode = 10;

			Send(userCode, Generater.Generate(check));
		}
		public void DeleteChecklist(int userCode, CheckListProtocol.CHECKLIST check)
		{
			// !SQL 채널 삭제
			
			Send(userCode, Generater.Generate(check));
		}
		public void ModifyChecklist(int userCode, CheckListProtocol.CHECKLIST check)
		{
			Console.WriteLine(serverCode + " : Modify or Delete Checklist\tChecklist\t: " + check.checkListCode);
			// !SQL 체크리스트 변경
			if (!IsMember(userCode))
				return;


			Send(userCode, Generater.Generate(check));
		}

		// 각 유저들에게 데이터를 전송
		async private void Send(int userCode, List<byte> target)
		{
			Console.WriteLine(serverCode + "\t: Send Command\tuser : " + userCode);
			foreach(var user in enterUserList)
			{
				// userContainer를 참조하여 전부 보내줌
				LoginContainer.Instance.Send(user, target);
			}
		}
	}
}
