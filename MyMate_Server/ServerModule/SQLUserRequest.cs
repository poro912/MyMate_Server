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
using queryList = MyMate_DB_Module.QueryString;    
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
			Console.WriteLine(user.UserCode + "\t: 상대 유저에 대한 정보를 전송\t대상\t:" + userCode);
			// !SQL 해당 유저에 대한 프로필 정보를 받아옴
			// SQL 객체 생성
			SQL sql = new();

            UserParm userParm = new();

            DataTable queryResult = new();

			UserProtocol.USER userProfile = new();

			// id가 같은 한사람에대한 정보를 전송
			if("" != id)
			{
				userParm.userCode = userCode;
				userParm.dataFormat = null;

				queryResult = sql.resultConnectDB(userParm, queryList.GetUser);

				userProfile.Set(
					userCode,
                    queryResult.Rows[0]["U_id"].ToString(),
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
				userParm.userCode = 0;	// 모든 유저
                userParm.dataFormat = null;

                queryResult = sql.resultConnectDB(userParm, queryList.GetUser);

                for (int i = 0; i < queryResult.Rows.Count; i++)
				{
					userProfile.Set(
					userCode,
                    queryResult.Rows[i]["U_id"].ToString(),
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

				queryResult = sql.resultConnectDB(userParm, queryList.GetUser);

				userProfile.Set(
					userCode,
                    queryResult.Rows[0]["U_id"].ToString(),
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
			Console.WriteLine(user.UserCode + "\t: 유저 채널 정보를 전송");
			// !SQL userChnnel에 대한 정보를 받아옴
			SQL sql = new();

			UserChannelParm userChannelParm = new();

			ChannelProtocol.CHANNEL channel = new();

			DataTable queryResult = new();

			// 모든 채널 정보 전송
			if (0 == ChannelCode)
			{
                userChannelParm.userCode = userCode;
                userChannelParm.channelCode = 0;

                queryResult = sql.resultConnectDB(userChannelParm, queryList.GetUserChannel);

                for (int i = 0; i < queryResult.Rows.Count; i++)
				{
					channel.Set(
						0,
						Convert.ToInt32(queryResult.Rows[i]["ch_code"]),
						queryResult.Rows[i]["ch_title"].ToString(),
						Convert.ToInt32(queryResult.Rows[i]["state"]));
					user.Send(Generater.Generate(channel));
				}
				return true;
			}
			// 한 채널 정보 전송
			else
			{
                userChannelParm.userCode = userCode;
                userChannelParm.channelCode = ChannelCode;

                queryResult = sql.resultConnectDB(userChannelParm, queryList.GetUserChannel);

				channel.Set(
					0,
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
			Console.WriteLine(user.UserCode + "\t: 유저 캘린더 정보를 전송\t대상 채널\t: " + channel);

			// SQL 객체 생성
			SQL sql = new();

			// userCalendarParm 객체 생성
			UserCalendarParm userCalendarParm = new();

			// query결과 값 할당할 dataTable 객체 생성
			DataTable queryResult = new ();

			// 결과 값을 할당할 calendar 객체 생성
			CalenderProtocol.CALENDER calendar = new();

			// !SQL userCalender에 대한 정보를 받아옴
			// 모든 데이터 전송
			if (channel == 0)
			{
                
				// 값 할당
                userCalendarParm.userCode = userCode;
                userCalendarParm.channelCode = 0;
				userCalendarParm.calendarCode = 0;

				// query 실행
				queryResult = sql.resultConnectDB(userCalendarParm, queryList.GetUserCalendar);


				// 결과 값 할당
				for (int i = 0; i < 10; i++)
				{


					calendar.Set(0,
						Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
						Convert.ToInt32(queryResult.Rows[0]["cal_code"]),
						queryResult.Rows[0]["content"].ToString(),
						userCode,
						new DateTime(Convert.ToInt32(queryResult.Rows[0]["start_time"])),
                        new DateTime(Convert.ToInt32(queryResult.Rows[0]["end_time"])),
						Convert.ToBoolean(queryResult.Rows[0]["is_private"]));
					user.Send(Generater.Generate(calendar));
				}
			}
			else
			{

                // 값 할당
                userCalendarParm.userCode = userCode;
                userCalendarParm.channelCode = channel;
                userCalendarParm.calendarCode = 0;

                // query 실행
                queryResult = sql.resultConnectDB((object)userCalendarParm, queryList.GetUserCalendar);


                // 결과 값 할당
				calendar.Set(0, channel,
                        Convert.ToInt32(queryResult.Rows[0]["cal_code"]),
                        queryResult.Rows[0]["content"].ToString(),
                        userCode,
                        new DateTime(Convert.ToInt32(queryResult.Rows[0]["start_time"])),
                        new DateTime(Convert.ToInt32(queryResult.Rows[0]["end_time"])),
                        Convert.ToBoolean(queryResult.Rows[0]["is_private"]));
                user.Send(Generater.Generate(calendar));
			}
			return true;
		}

		// 유저 프로젝트 정보
		async public static Task<bool> UserProjectRequest(UserClient user, int userCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + "\t: 유저 프로젝트 정보를 전송\t대상 채널\t: " + channel);
			// !SQL userProject에 대한 정보를 받아옴
			SQL sql = new();

			UserChecklistParm userChecklistParm = new();

			DataTable queryResult = new();

			CheckListProtocol.CHECKLIST checklist = new();

			if (channel == 0)
			{
                // 모든 데이터 전송 
                userChecklistParm.userCode = userCode;
                userChecklistParm.channelCode = 0;
				userChecklistParm.checklistCode = 0;

                queryResult = sql.resultConnectDB(userChecklistParm, queryList.GetUserChecklist);

				for (int i = 0; i < queryResult.Rows.Count; i++)
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
                userChecklistParm.checklistCode = 0;

                queryResult = sql.resultConnectDB(userChecklistParm, queryList.GetUserChecklist);

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
			return true;
		}

		// 서버 정보
		async public static Task<bool> ServerRequest(UserClient user, int userCode, int serverCode = 0)
		{
			Console.WriteLine(user.UserCode + "\t: 서버 정보를 전송\t대상서버\t: " + serverCode);
			// !SQL 해당 서버에 대한 정보를 받아옴

			SQL sql = new();

			ServerUserParm serverUserParm = new();

			DataTable queryResult = new();

			Protocol.ServerProtocol.Server server = new();

            ServerParm serverParm = new();
            DataTable tempResult = new();

            if (serverCode == 0)
			{
                // !SQL 접속되어 있는 모든 서버에 대한 정보 전송
                // 접속되어있는 모든 서버에 대한 정보 전송

                serverUserParm.serverCode = 0;
                serverUserParm.userCode = userCode;

                queryResult = sql.resultConnectDB(serverUserParm, queryList.GetServeruser);

				for (int i = 0; i < queryResult.Rows.Count; i++)
				{
					serverParm.serverCode = Convert.ToInt32(queryResult.Rows[i]["s_code"]);
					tempResult = sql.resultConnectDB(serverParm, queryList.GetServer);


                    server.Set(Convert.ToInt32(queryResult.Rows[i]["s_code"]),
                        tempResult.Rows[0]["S_title"].ToString(),
                        Convert.ToInt32(tempResult.Rows[0]["S_admin_code"]),
                        Convert.ToBoolean(tempResult.Rows[0]["is_signle"])
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

                queryResult = sql.resultConnectDB(serverUserParm,queryList.GetServeruser);

				serverParm.serverCode = Convert.ToInt32(queryResult.Rows[0]["s_code"]);
				tempResult = sql.resultConnectDB(serverParm, queryList.GetServer);

                server.Set(Convert.ToInt32(queryResult.Rows[0]["s_code"]),
                       tempResult.Rows[0]["S_title"].ToString(),
                       Convert.ToInt32(tempResult.Rows[0]["S_admin_code"]),
                       Convert.ToBoolean(tempResult.Rows[0]["is_signle"])
                       );
                user.Send(Generater.Generate(server));
			}
            return true;
		}

		async public static Task<bool> ServerChannelRequest(UserClient user, int userCode, int serverCode = 0)
		{
			Console.WriteLine(user.UserCode + "\t: 서버 채널 정보를 전송\t대상서버\t: " + serverCode);

			SQL sql = new();

			ChannelParm channelParm = new();

			DataTable queryResult = new();

			ChannelProtocol.CHANNEL channel = new();

                // SQL 서버 채널 정보 전송
                channelParm.serverCode = serverCode;
                channelParm.channelCode = 0;

                queryResult = sql.resultConnectDB(channelParm, queryList.GetChannel);

                channel.Set(Convert.ToInt32(queryResult.Rows[0]["s_code"]),
                        Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
                        queryResult.Rows[0]["title"].ToString(),
                        Convert.ToInt32(queryResult.Rows[0]["state"])
                        );

                user.Send(Generater.Generate(channel));

            return true;
		}

		async public static Task<bool> ServerCalenderRequest(UserClient user, int userCode, int serverCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + "\t: 서버 캘린더 정보를 전송\t대상서버\t: " + serverCode + "\t대상 채널\t: " + channel);

			// !SQL 서버의 캘린더 정보를 받아옴
			SQL sql = new();

			CalendarParm calendarParm = new();

			DataTable queryResult = new();

			CalenderProtocol.CALENDER calendar = new();

			

			if (channel == 0)
			{
                calendarParm.serverCode = serverCode;
                calendarParm.channelCode = 0;
                calendarParm.calendarCode = 0;

                queryResult = sql.resultConnectDB(calendarParm, queryList.GetCalendar);

                // 해당 서버의 모든 캘린더 데이터 전송
                // !SQL 
                for (int i = 0; i < queryResult.Rows.Count; i++)
				{


					calendar.Set(Convert.ToInt32(queryResult.Rows[i]["s_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["ch_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["cal_code"]),
                        queryResult.Rows[i]["content"].ToString(),
                        Convert.ToInt32(queryResult.Rows[i]["creater"]),
                        new DateTime(Convert.ToInt32(queryResult.Rows[i]["start_time"])),
                        new DateTime(Convert.ToInt32(queryResult.Rows[i]["end_time"])),
                        Convert.ToBoolean(queryResult.Rows[i]["is_private"])
                        );
					user.Send(Generater.Generate(calendar));
				}
			}
			else
			{
                // 해당 서버 한 채널의 코든 캘린더 데이터 전송
                // !SQL
                calendarParm.serverCode = serverCode;
                calendarParm.channelCode = channel;
                calendarParm.calendarCode = 0;

                queryResult = sql.resultConnectDB(calendarParm, queryList.GetCalendar);

                calendar.Set(Convert.ToInt32(queryResult.Rows[0]["s_code"]),
                        Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
                        Convert.ToInt32(queryResult.Rows[0]["cal_code"]),
                        queryResult.Rows[0]["content"].ToString(),
                        Convert.ToInt32(queryResult.Rows[0]["creater"]),
                        new DateTime(Convert.ToInt32(queryResult.Rows[0]["start_time"])),
                        new DateTime(Convert.ToInt32(queryResult.Rows[0]["end_time"])),
                        Convert.ToBoolean(queryResult.Rows[0]["is_private"])
                        );
                    user.Send(Generater.Generate(calendar));

			}
            
            


            return true;
		}

		async public static Task<bool> ServerProjectRequest(UserClient user, int userCode, int serverCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + "\t: 서버 프로젝트 정보를 전송\t대상서버\t: " + serverCode + "\t대상 채널\t: " + channel);
            SQL sql = new();

			ChecklistParm checklistParm = new();

			DataTable queryResult = new();

			CheckListProtocol.CHECKLIST checklist = new();

		

            if (channel == 0)
			{
                checklistParm.serverCode = serverCode;
                checklistParm.channelCode = 0;
                checklistParm.checklistCode = 0;



                queryResult = sql.resultConnectDB(checklistParm, queryList.GetChecklist);

                // 모든 데이터 전송
                for (int i = 0; i < queryResult.Rows.Count; i++)
				{
					checklist.Set(Convert.ToInt32(queryResult.Rows[i]["chk_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["s_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["ch_code"]),
                        new DateTime(Convert.ToInt32(queryResult.Rows[i]["start_time"])),
                        new DateTime(Convert.ToInt32(queryResult.Rows[i]["end_time"])),
                        queryResult.Rows[i]["content"].ToString(),
                        Convert.ToBoolean(queryResult.Rows[i]["checked"]),
                        Convert.ToBoolean(queryResult.Rows[i]["is_private"])
                        );

					// checklist.Set();
					user.Send(Generater.Generate(channel));
				}
			}
			else
			{
                // 대상 채널 정보 전송
                checklistParm.serverCode = serverCode;
                checklistParm.channelCode = channel;
                checklistParm.checklistCode = 0;



                queryResult = sql.resultConnectDB(checklistParm, queryList.GetChecklist);


                checklist.Set(Convert.ToInt32(queryResult.Rows[0]["chk_code"]),
                         Convert.ToInt32(queryResult.Rows[0]["s_code"]),
                         Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
                         new DateTime(Convert.ToInt32(queryResult.Rows[0]["start_time"])),
                         new DateTime(Convert.ToInt32(queryResult.Rows[0]["end_time"])),
                         queryResult.Rows[0]["content"].ToString(),
                         Convert.ToBoolean(queryResult.Rows[0]["checked"]),
                         Convert.ToBoolean(queryResult.Rows[0]["is_private"])
                         );

                user.Send(Generater.Generate(channel));
				
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
			Console.WriteLine(user.UserCode + "\t: 서버 메시지 정보를 전송\t대상서버\t: " + serverCode + "\t대상 채널\t: " + channel);

			// !SQL 서버의 해당 채널의 메시지들을 받아옴
            SQL sql = new();

			MessageParm messageParm = new();

			DataTable queryResult = new();

			MessageProtocol.MESSAGE msg = new();

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
                messageParm.serverCode = serverCode;
                messageParm.channelCode = 0;
                messageParm.messageCode = 0;

                queryResult = sql.resultConnectDB(messageParm, queryList.GetMessage);

                // !SQL 모든 채널 정보를 받아옴
                for (int i = 0; i < queryResult.Rows.Count; i++)
				{


					msg.Set(Convert.ToInt32(queryResult.Rows[i]["msg_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["s_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["ch_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["creater"]),
                        queryResult.Rows[i]["content"].ToString(),
                        new DateTime(Convert.ToInt32(queryResult.Rows[i]["start_time"])),
                        Convert.ToBoolean(queryResult.Rows[i]["is_private"])
                        );
					user.Send(Generater.Generate(msg));
				}
			}
			else
			{
                // !SQL 특정 채널 정보를 받아옴

                messageParm.serverCode = serverCode;
                messageParm.channelCode = channel;
                messageParm.messageCode = 0;

                queryResult = sql.resultConnectDB(messageParm, queryList.GetMessage);

                msg.Set(Convert.ToInt32(queryResult.Rows[0]["msg_code"]),
                        Convert.ToInt32(queryResult.Rows[0]["s_code"]),
                        Convert.ToInt32(queryResult.Rows[0]["ch_code"]),
                        Convert.ToInt32(queryResult.Rows[0]["creater"]),
                        queryResult.Rows[0]["content"].ToString(),
                        new DateTime(Convert.ToInt32(queryResult.Rows[0]["start_time"])),
                        Convert.ToBoolean(queryResult.Rows[0]["is_private"])
                        );
                    user.Send(Generater.Generate(msg));
			}
            return true;
		}

		
		async public static Task<bool> UserFriend(UserClient user, int userCode)
		{
			Console.WriteLine(user.UserCode + "\t: 유저의 친구 정보를 전송\t대상\t: " + userCode);
            // !SQL 해당 유저에 대한 친구 목록을 받아 전송
            SQL sql = new();

			FriendParm friendParm = new();

			DataTable queryResult = new();

			FriendProtocol.FRIEND friend = new();

			

			if(0 == userCode)
			{
                // 모든 친구 데이터를 전송
                // !SQL
                friendParm.userCode = userCode;
                friendParm.friendCode = 0;

                queryResult = sql.resultConnectDB(friendParm, queryList.GetFriend);

                for (int i = 0; i < queryResult.Rows.Count; i++)
				{


					friend.Set(Convert.ToInt32(queryResult.Rows[i]["u_code"]),
                        Convert.ToInt32(queryResult.Rows[i]["fr_code"]),
                        queryResult.Rows[i]["nick"].ToString(),
                        Convert.ToBoolean(queryResult.Rows[i]["fr_hide"]),
                        Convert.ToBoolean(queryResult.Rows[i]["fr_block"])
                        );
					user.Send(Generater.Generate(friend));
				}
			}
			else
			{
                // !SQL
                // 하나의 친구 데이터를 전송
                friendParm.userCode = userCode;
                friendParm.friendCode = null;	// 친구 코드 없음

                queryResult = sql.resultConnectDB(friendParm, queryList.GetFriend);

                friend.Set(Convert.ToInt32(queryResult.Rows[0]["u_code"]),
                        Convert.ToInt32(queryResult.Rows[0]["fr_code"]),
                        queryResult.Rows[0]["nick"].ToString(),
                        Convert.ToBoolean(queryResult.Rows[0]["fr_hide"]),
                        Convert.ToBoolean(queryResult.Rows[0]["fr_block"])
                        );
                user.Send(Generater.Generate(friend));
			}
			return true;
		}
	}
}