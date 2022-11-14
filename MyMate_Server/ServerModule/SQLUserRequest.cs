#pragma warning disable CS1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Protocol.ChannelProtocol;
using static Protocol.ToastProtocol;
using MyMate_DB_Module;
using System.Data;
using Convert = System.Convert;
using Protocol;
using System.Globalization;
using static Protocol.CheckListProtocol;

namespace ServerSystem
{
	// 유저가 DB에 데이터를 요청하는 메소드들
	// !SQL userCode에게 start 에서 end시간 사이의 데이터를 전송
	public static class SQLUsesrRequest
	{
		// 유저 정보
		async public static Task<bool> UserRequest(UserClient user, int userCode, string id = "")
		{
			Console.WriteLine(user.UserCode + " : 상대 유저에 대한 정보를 전송\t대상 : " + userCode);
			// !SQL 해당 유저에 대한 프로필 정보를 받아옴
			// SQL 객체 생성
			SQL sql = new();

            UserParm userParm = new UserParm();

            DataTable queryResult = new();

			UserProtocol.USER userProfile = new();

			// id가 같은 한사람에대한 정보를 전송
			if("" != id)
			{
				userParm.userCode = userCode;
				userParm.dataFormat = null;

				queryResult = sql.resultConnectDB((object)userParm, "GetUser");

				userProfile.Set(
					userCode,
					"",
					queryResult.Rows[0]["U_name"].ToString(),
					queryResult.Rows[0]["U_nick"].ToString(),
					queryResult.Rows[0]["U_email"].ToString(),
					queryResult.Rows[0]["U_phone"].ToString(),
					queryResult.Rows[0]["U_content"].ToString(),
					DateTime.Now);
				user.Send(Generater.Generate(userProfile));
			}
			// user에 대해 모든 사람의 정보를 전송
			if(0 == userCode)
			{
				userParm.userCode = userCode;


				for(int i = 0; i < 10; i++)
				{
					userProfile.Set(
					userCode,
					"",
					queryResult.Rows[i]["U_name"].ToString(),
					queryResult.Rows[i]["U_nick"].ToString(),
					queryResult.Rows[i]["U_email"].ToString(),
					queryResult.Rows[i]["U_phone"].ToString(),
					queryResult.Rows[i]["U_content"].ToString(),
					DateTime.Now);
					user.Send(Generater.Generate(userProfile));
				}
				return true;
			}
			// 한사람에대한 정보를 전송
			else
			{
				userParm.userCode = userCode;
				userParm.dataFormat = null;

				queryResult = sql.resultConnectDB((object)userParm, "GetUser");

				userProfile.Set(
					userCode,
					"",
					queryResult.Rows[0]["U_name"].ToString(),
					queryResult.Rows[0]["U_nick"].ToString(),
					queryResult.Rows[0]["U_email"].ToString(),
					queryResult.Rows[0]["U_phone"].ToString(),
					queryResult.Rows[0]["U_content"].ToString(),
					DateTime.Now);
				user.Send(Generater.Generate(userProfile));
				return true;
			}
			return false;
		}

		// 유저 채널 리스트
		async public static Task<bool> UserChannelRequest(UserClient user, int userCode, int ChannelCode = 0)
		{
			Console.WriteLine(user.UserCode + " : 유저 채널 정보를 전송");
			// !SQL userChnnel에 대한 정보를 받아옴
			SQL sql = new();

			UserChannelParm userChannelParm = new UserChannelParm();

			ChannelProtocol.CHANNEL channel = new();

			userChannelParm.userCode = userCode;

			DataTable queryResult = new DataTable();

			// 모든 채널 정보 전송
			if (0 == ChannelCode)
			{
				for (int i = 0; i < 10; i++)
				{
					

					channel.Set(
						Convert.ToInt32(queryResult.Rows[0]["s_code"]),
						Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
						queryResult.Rows[0]["ch_title"].ToString(),
						Convert.ToInt32(queryResult.Rows[0]["state"]));
					user.Send(Generater.Generate(channel));
				}
				return true;
			}
			// 한 채널 정보 전송
			else
			{
				queryResult = sql.resultConnectDB((object)userChannelParm, "GetUserChannel");

				channel.Set(
					Convert.ToInt32(queryResult.Rows[0]["s_code"]),
					Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
					queryResult.Rows[0]["ch_title"].ToString(),
					Convert.ToInt32(queryResult.Rows[0]["state"]));

				user.Send(Generater.Generate(channel));
				return true;
			}

			return false;
		}

		// 유저 캘린더 정보
		async public static Task<bool> UserCalenderRequest(UserClient user, int userCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 유저에 캘린더 정보를 전송\t대상 채널 : " + channel);

			// SQL 객체 생성
			SQL sql = new();

			// userCalendarParm 객체 생성
			UserCalendarParm userCalendarParm = new UserCalendarParm();

			// query결과 값 할당할 dataTable 객체 생성
			DataTable queryResult = new DataTable();

			// 결과 값을 할당할 calendar 객체 생성
			CalenderProtocol.CALENDER calendar = new();

			// !SQL userCalender에 대한 정보를 받아옴
			// 모든 데이터 전송
			if (channel == 0)
			{
                

				// 값 할당
                userCalendarParm.userCode = userCode;
                userCalendarParm.channelCode = null;
				userCalendarParm.calendarCode = null;

				// query 실행
				queryResult = sql.resultConnectDB((object)userCalendarParm, "GetUserCalendar");


				// 결과 값 할당
				for (int i = 0; i < 10; i++)
				{


					calendar.Set(0,
						Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
						Convert.ToInt32(queryResult.Rows[0]["cal_code"]),
						queryResult.Rows[0]["content"].ToString(),
						userCode,
						DateTime.Now.AddDays(-2),
						DateTime.Now.AddDays(2),
						Convert.ToBoolean(queryResult.Rows[0]["is_private"]));
					user.Send(Generater.Generate(calendar));
				}
			}
			else
			{

                // 값 할당
                userCalendarParm.userCode = userCode;
                userCalendarParm.channelCode = channel;
                userCalendarParm.calendarCode = null;

                // query 실행
                queryResult = sql.resultConnectDB((object)userCalendarParm, "GetUserCalendar");


                // 결과 값 할당
				calendar.Set(0, channel,
						Convert.ToInt32(queryResult.Rows[0]["cal_code"]),
						queryResult.Rows[0]["content"].ToString(),
						userCode,
						DateTime.Now.AddDays(-2),
						DateTime.Now.AddDays(2),
						Convert.ToBoolean(queryResult.Rows[0]["is_private"]));
				user.Send(Generater.Generate(calendar));
			}
			return true;
		}

		// 유저 프로젝트 정보
		async public static Task<bool> UserProjectRequest(UserClient user, int userCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 유저에 프로젝트 정보를 전송\t대상 채널 : " + channel);
			// !SQL userProject에 대한 정보를 받아옴
			SQL sql = new();

			UserChecklistParm userChecklistParm = new();

			DataTable queryResult = new DataTable();

			CheckListProtocol.CHECKLIST checklist = new();

			if (channel == 0)
			{
                // 모든 데이터 전송 
                userChecklistParm.userCode = userCode;
                userChecklistParm.channelCode = null;
				userChecklistParm.checklistCode = null;

                queryResult = sql.resultConnectDB((object)userChecklistParm, "GetUserChecklist");

				for (int i = 0; i < 10; i++)
				{


					checklist.Set(
						Convert.ToInt32(queryResult.Rows[0]["chk_code"]),
						0,
						Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
						new DateTime(Convert.ToInt32(queryResult.Rows[0]["start_time"])),
						new DateTime(Convert.ToInt32(queryResult.Rows[0]["end_time"])),
						queryResult.Rows[0]["content"].ToString(),
						Convert.ToBoolean(queryResult.Rows[0]["is_checked"]),
						Convert.ToBoolean(queryResult.Rows[0]["is_private"]));
					user.Send(Generater.Generate(checklist));
				}
			}
			else
			{

                userChecklistParm.userCode = userCode;
                userChecklistParm.channelCode = channel;
                userChecklistParm.checklistCode = null;

                queryResult = sql.resultConnectDB((object)userChecklistParm, "GetUserChecklist");

				for (int i = 0; i < 10; i++)
				{


					checklist.Set(
						Convert.ToInt32(queryResult.Rows[0]["chk_code"]),
						0,
						Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
						new DateTime(Convert.ToInt32(queryResult.Rows[0]["start_time"])),
						new DateTime(Convert.ToInt32(queryResult.Rows[0]["end_time"])),
						queryResult.Rows[0]["content"].ToString(),
						Convert.ToBoolean(queryResult.Rows[0]["is_checked"]),
						Convert.ToBoolean(queryResult.Rows[0]["is_private"]));
					user.Send(Generater.Generate(checklist));
				}
			}
			return true;
		}

		// 서버 정보
		async public static Task<bool> ServerRequest(UserClient user, int userCode, int serverCode = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버에 대한 정보를 전송\t대상서버 : " + serverCode);
			// !SQL 해당 서버에 대한 정보를 받아옴

			SQL sql = new();

			ServerUserParm serverUserParm = new();

			DataTable queryResult = new();

			Protocol.ServerProtocol.Server server = new();

			if (serverCode == 0)
			{
				// !SQL 접속되어 있는 모든 서버에 대한 정보 전송
                // 접속되어있는 모든 서버에 대한 정보 전송

                // 리스트 사용할 경우 필요 없어짐

                serverUserParm.serverCode = null;
                serverUserParm.userCode = userCode;

                queryResult = sql.resultConnectDB((object)serverUserParm, "GetServeruser");
				for (int i = 0; i < 10; i++)
				{


					server.Set(0, "", 0, false
						);
					user.Send(Generater.Generate(server));
				}

				// 리스트에 결과값 할당
			}
            else
			{
				// !SQL 하나의 서버에 대해 접근
                serverUserParm.serverCode = serverCode;
                serverUserParm.userCode = userCode;

                queryResult = sql.resultConnectDB((object)serverUserParm, "GetServeruser");

				server.Set(0, "", 0, false
						);
				user.Send(Generater.Generate(server));
			}
            return true;
		}

		async public static Task<bool> ServerChannelRequest(UserClient user, int userCode, int serverCode = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버 채널 정보를 전송\t대상서버 : " + serverCode);

			SQL sql = new();

			ChannelParm channelParm = new ChannelParm();

			DataTable queryResult = new DataTable();

			ChannelProtocol.CHANNEL channel = new ();

			if (serverCode == 0)
			{
                // 접속되어있는 모든 서버의 모든 채널에 대한 정보 전송

				// server_user 테이블에서 서버 번호 가져온 후 채널 정보 가져오기 또는 조인테이블 작성
                
				// SQL 서버 채널 정보 전송
                channelParm.serverCode = serverCode;
                channelParm.channelCode = null;

                queryResult = sql.resultConnectDB((object)channelParm, "GetChannel");

				for (int i = 0; i < 10; i++)
				{
					user.Send(Generater.Generate(channel));
				}
			}
			else
			{
				// SQL 서버 채널 정보 전송
				for (int i = 0; i < 10; i++)
				{
					user.Send(Generater.Generate(channel));
				}
			}

            return true;
		}

		async public static Task<bool> ServerCalenderRequest(UserClient user, int userCode, int serverCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버 캘린더 정보를 전송\t대상서버 : " + serverCode + "\t대상 채널 : " + channel);

			// !SQL 서버의 캘린더 정보를 받아옴
			SQL sql = new();

			CalendarParm calendarParm = new CalendarParm();

			DataTable queryResult = new DataTable();

			CalenderProtocol.CALENDER calendar = new ();

			calendarParm.serverCode = serverCode;
			calendarParm.channelCode = channel;
			calendarParm.calendarCode = null;

			queryResult = sql.resultConnectDB((object)calendarParm, "GetCalendar");

			if (channel == 0)
			{
				// 해당 서버의 모든 캘린더 데이터 전송
				// !SQL 
				for (int i = 0; i < 10; i++)
				{


					calendar.Set(0, 0, 0, "", 0, DateTime.Now, DateTime.Now, false
						);
					user.Send(Generater.Generate(calendar));
				}
			}
			else
			{
				// 해당 서버 한 채널의 코든 캘린더 데이터 전송
				// !SQL
				for (int i = 0; i < 10; i++)
				{


					calendar.Set(0, 0, 0, "", 0, DateTime.Now, DateTime.Now, false
						);
					user.Send(Generater.Generate(calendar));
				}
			}
            
            


            return true;
		}

		async public static Task<bool> ServerProjectRequest(UserClient user, int userCode, int serverCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버 프로젝트 정보를 전송\t대상서버 : " + serverCode + "\t대상 채널 : " + channel);
            SQL sql = new();

			ChecklistParm checklistParm = new();

			DataTable queryResult = new();

			CheckListProtocol.CHECKLIST checklist = new();

			checklistParm.serverCode = serverCode;
            checklistParm.channelCode = channel;
            checklistParm.checklistCode = null;

			

            queryResult = sql.resultConnectDB((object)checklistParm, "GetChecklist");

            if (channel == 0)
			{
				// 모든 데이터 전송
				for (int i = 0; i < 10; i++)
				{


					// checklist.Set();
					user.Send(Generater.Generate(channel));
				}
			}
			else
			{
				// 대상 채널 정보 전송
				for (int i = 0; i < 10; i++)
				{


					// checklist.Set();
					user.Send(Generater.Generate(channel));
				}
			}
			// !SQL 서버의 프로젝트 정보를 받아옴
			/*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
			return true;
		}

		async public static Task<bool> ServerMessageRequest(UserClient user, int userCode, int serverCode, int channel = 0, DateTime? start = null, DateTime? end = null)
		{
			Console.WriteLine(user.UserCode + " : 서버 메시지 정보를 전송\t대상서버 : " + serverCode + "\t대상 채널 : " + channel);

			// !SQL 서버의 해당 채널의 메시지들을 받아옴
            SQL sql = new();

			MessageParm messageParm = new MessageParm();

			DataTable queryResult = new DataTable();

			MessageProtocol.MESSAGE msg = new();

			messageParm.serverCode = serverCode;
            messageParm.channelCode = channel;
			messageParm.messageCode = null;

            

			queryResult = sql.resultConnectDB((object)messageParm, "GetMessage");
			if (start == null)
			{
				start = new(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day - 3);
			}
			if (end == null)
			{
				end = DateTime.Today;
			}

			if(channel == 0)
			{
				// !SQL 모든 채널 정보를 받아옴
				for (int i = 0; i < 10; i++)
				{


					msg.Set(0,0,0,0,"",DateTime.Now,false
						);
					user.Send(Generater.Generate(msg));
				}
			}
			else
			{
				// !SQL 특정 채널 정보를 받아옴
				for (int i = 0; i < 10; i++)
				{


					msg.Set(0, 0, 0, 0, "", DateTime.Now, false
						);
					user.Send(Generater.Generate(msg));
				}
			}
            return true;
		}

		
		async public static Task<bool> UserFriend(UserClient user, int userCode)
		{
			Console.WriteLine(user.UserCode + " : 유저의 친구 정보를 전송\t대상 : " + userCode);
            // !SQL 해당 유저에 대한 친구 목록을 받아 전송
            SQL sql = new();

			FriendParm friendParm = new FriendParm();

			DataTable queryResult = new DataTable();

			FriendProtocol.FRIEND friend = new();

			friendParm.userCode = userCode;
			friendParm.friendCode = null;

            queryResult = sql.resultConnectDB((object)friendParm, "GetFriend");

			if(0 == userCode)
			{
				// 모든 친구 데이터를 전송
				// !SQL

				for (int i = 0; i < 10; i++)
				{


					friend.Set(0,0,"",false,false
						);
					user.Send(Generater.Generate(friend));
				}
			}
			else
			{
				// !SQL
				// 하나의 친구 데이터를 전송

				friend.Set(0, 0, "", false, false
						);
				user.Send(Generater.Generate(friend));
			}
			return true;
		}
	}
}