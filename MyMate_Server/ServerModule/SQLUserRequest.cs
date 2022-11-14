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

namespace ServerSystem
{
	// 유저가 DB에 데이터를 요청하는 메소드들
	// !SQL userCode에게 start 에서 end시간 사이의 데이터를 전송
	public static class SQLUsesrRequest
	{
		// 유저 정보
		async public static Task<bool> UserRequest(UserClient user, int userCode)
		{
			Console.WriteLine(user.UserCode + " : 유저에 대한 정보를 전송\t대상 : " + userCode);
			// !SQL 해당 유저에 대한 프로필 정보를 받아옴
			// SQL 객체 생성
			SQL sql = new();

            UserParm userParm = new UserParm();

            DataTable queryResult = new();

			Protocol.UserProtocol.USER userProfile = new();

			userParm.userCode = userCode;
			userParm.dataFormat = null;

			queryResult = sql.resultConnectDB((object)userParm, "GetUser");

			userProfile.name = queryResult.Rows[0]["U_name"].ToString();
			userProfile.nickname = queryResult.Rows[0]["U_nick"].ToString();
			userProfile.phone = queryResult.Rows[0]["U_phone"].ToString();
			userProfile.email = queryResult.Rows[0]["U_email"].ToString();
			userProfile.content = queryResult.Rows[0]["U_content"].ToString();

			/*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
			return true;
		}

		// 유저 채널 리스트
		async public static Task<bool> UserChnnalRequest(UserClient user, int userCode)
		{
			Console.WriteLine(user.UserCode + " : 유저 채널 정보를 전송");
			// !SQL userChnnel에 대한 정보를 받아옴
			SQL sql = new();

			UserChannelParm userChannelParm = new UserChannelParm();

			userChannelParm.userCode = userCode;

			DataTable queryResult = new DataTable();

			queryResult = sql.resultConnectDB((object)userChannelParm, "GetUserChannel");

			Protocol.ChannelProtocol.CHNNEL channel = new();

			channel.serverCode = Convert.ToInt32(queryResult.Rows[0]["s_code"]);
            channel.channelCode = Convert.ToInt32(queryResult.Rows[0]["ch_code"]);
            channel.title = queryResult.Rows[0]["ch_title"].ToString();
            channel.state = Convert.ToInt32(queryResult.Rows[0]["state"]);

            /*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
            return true;

		}

		// 유저 캘린더 정보
		async public static Task<bool> UserCalenderRequest(UserClient user, int userCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 유저에 캘린더 정보를 전송\t대상 채널 : " + channel);
			// !SQL userCalender에 대한 정보를 받아옴
            if (channel == 0)
			{
                // 모든 데이터 전송

				// SQL 객체 생성
                SQL sql = new();

				// userCalendarParm 객체 생성
                UserCalendarParm userCalendarParm = new UserCalendarParm();

				// query결과 값 할당할 dataTable 객체 생성
                DataTable queryResult = new DataTable();

				// 결과 값을 할당할 calendar 객체 생성
                Protocol.CalenderProtocol.CALENDER calendar = new();

				// 값 할당
                userCalendarParm.userCode = userCode;
                userCalendarParm.channelCode = null;
				userCalendarParm.calendarCode = null;

				// query 실행
				queryResult = sql.resultConnectDB((object)userCalendarParm, "GetUserCalendar");


				// 결과 값 할당
                calendar.channelCode = Convert.ToInt32(queryResult.Rows[0]["ch_code"]);
                calendar.calenderCode = Convert.ToInt32(queryResult.Rows[0]["cal_code"]);
				calendar.content = queryResult.Rows[0]["content"].ToString();
                //calendar.startTime.Ticks = Convert.ToInt32(queryResult.Rows[0]["start_time"]);
                //calendar.endTime.Ticks = Convert.ToInt32(queryResult.Rows[0]["end_time"]);
                calendar.isPrivate = Convert.ToBoolean(queryResult.Rows[0]["is_private"]);

            }
			else
			{
                // SQL 객체 생성
                SQL sql = new();

                // userCalendarParm 객체 생성
                UserCalendarParm userCalendarParm = new UserCalendarParm();

                // query결과 값 할당할 dataTable 객체 생성
                DataTable queryResult = new DataTable();

                // 결과 값을 할당할 calendar 객체 생성
                Protocol.CalenderProtocol.CALENDER calendar = new();

                // 값 할당
                userCalendarParm.userCode = userCode;
                userCalendarParm.channelCode = channel;
                userCalendarParm.calendarCode = null;

                // query 실행
                queryResult = sql.resultConnectDB((object)userCalendarParm, "GetUserCalendar");


                // 결과 값 할당
                calendar.calenderCode = Convert.ToInt32(queryResult.Rows[0]["cal_code"]);
                calendar.content = queryResult.Rows[0]["content"].ToString();
                //calendar.startTime.Ticks = Convert.ToInt32(queryResult.Rows[0]["start_time"]);
                //calendar.endTime.Ticks = Convert.ToInt32(queryResult.Rows[0]["end_time"]);
                calendar.isPrivate = Convert.ToBoolean(queryResult.Rows[0]["is_private"]);
            }

			/*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
			return true;
		}

		// 유저 프로젝트 정보
		async public static Task<bool> UserProjectRequest(UserClient user, int userCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 유저에 프로젝트 정보를 전송\t대상 채널 : " + channel);
            // !SQL userProject에 대한 정보를 받아옴

            if (channel == 0)
			{
                // 모든 데이터 전송 
                SQL sql = new();

                UserChecklistParm userChecklistParm = new();

                DataTable queryResult = new DataTable();

                Protocol.CheckListProtocol.CHECKLIST checklist = new();

                userChecklistParm.userCode = userCode;
                userChecklistParm.channelCode = null;
				userChecklistParm.checklistCode = null;

                queryResult = sql.resultConnectDB((object)userChecklistParm, "GetUserChecklist");

                checklist.channelCode = Convert.ToInt32(queryResult.Rows[0]["ch_code"]);
                checklist.checkListCode = Convert.ToInt32(queryResult.Rows[0]["chk_code"]);
                checklist.content = queryResult.Rows[0]["content"].ToString();
                //checklist.startDate.Ticks = Convert.ToInt32(queryResult.Rows[0]["start_time"]);
                //checklist.endDate.Ticks = Convert.ToInt32(queryResult.Rows[0]["end_time"]);
                checklist.isChecked = Convert.ToBoolean(queryResult.Rows[0]["is_checked"]);
                checklist.isPrivate = Convert.ToBoolean(queryResult.Rows[0]["is_private"]);
            }
			else
			{
                SQL sql = new();

                UserChecklistParm userChecklistParm = new();

                DataTable queryResult = new DataTable();

                Protocol.CheckListProtocol.CHECKLIST checklist = new();

                userChecklistParm.userCode = userCode;
                userChecklistParm.channelCode = channel;
                userChecklistParm.checklistCode = null;

                queryResult = sql.resultConnectDB((object)userChecklistParm, "GetUserChecklist");

                checklist.checkListCode = Convert.ToInt32(queryResult.Rows[0]["chk_code"]);
                checklist.content = queryResult.Rows[0]["content"].ToString();
                //checklist.startDate.Ticks = Convert.ToInt32(queryResult.Rows[0]["start_time"]);
                //checklist.endDate.Ticks = Convert.ToInt32(queryResult.Rows[0]["end_time"]);
                checklist.isChecked = Convert.ToBoolean(queryResult.Rows[0]["is_checked"]);
                checklist.isPrivate = Convert.ToBoolean(queryResult.Rows[0]["is_private"]);
            }

			/*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
			return true;
		}

		// 서버 정보
		async public static Task<bool> ServerRequest(UserClient user, int userCode, int serverCode = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버에 대한 정보를 전송\t대상서버 : " + serverCode);
			// !SQL 해당 서버에 대한 정보를 받아옴
			


			if (serverCode == 0)
			{
                // 접속되어있는 모든 서버에 대한 정보 전송

                SQL sql = new();

                ServerUserParm serverUserParm = new();

                DataTable queryResult = new();

                // Protocol.ServerProtocol.Server serverResult = new();
                // 리스트 사용할 경우 필요 없어짐

                serverUserParm.serverCode = null;
                serverUserParm.userCode = userCode;

                queryResult = sql.resultConnectDB((object)serverUserParm, "GetServeruser");

                // 리스트에 결과값 할당
            }
            else
			{
                SQL sql = new();

                ServerUserParm serverUserParm = new();

                DataTable queryResult = new();

                // Protocol.ServerProtocol.Server serverResult = new();
                // 리스트 사용할 경우 필요 없어짐

                serverUserParm.serverCode = serverCode;
                serverUserParm.userCode = userCode;

                queryResult = sql.resultConnectDB((object)serverUserParm, "GetServeruser");

                // 리스트에 결과값 할당
            }
            /*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
            return true;
		}

		async public static Task<bool> ServerChannelRequest(UserClient user, int userCode, int serverCode = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버 채널 정보를 전송\t대상서버 : " + serverCode);

			if (serverCode == 0)
			{
                // 접속되어있는 모든 서버의 모든 채널 정보에 대한 정보 전송

				// server_user 테이블에서 서버 번호 가져온 후 채널 정보 가져오기 또는 조인테이블 작성
                SQL sql = new();

                ChannelParm channelParm = new ChannelParm();

                channelParm.serverCode = serverCode;
                channelParm.channelCode = null;

                DataTable queryResult = new DataTable();

                queryResult = sql.resultConnectDB((object)channelParm, "GetChannel");

            }
			else
			{
            }
            // !SQL 서버의 채널리스트를 받아옴


            /*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
            return true;
		}
		async public static Task<bool> ServerCalenderRequest(UserClient user, int userCode, int serverCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버 캘린더 정보를 전송\t대상서버 : " + serverCode + "\t대상 채널 : " + channel);
			if (channel == 0)
			{
				// 모든 데이터 전송
			}
			else
			{

			}
            // !SQL 서버의 캘린더 정보를 받아옴
            SQL sql = new();

			CalendarParm calendarParm = new CalendarParm();

            calendarParm.serverCode = serverCode;
            calendarParm.channelCode = channel;
			calendarParm.calendarCode = null;

            DataTable queryResult = new DataTable();

            queryResult = sql.resultConnectDB((object)calendarParm, "GetCalendar");


            /*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
            return true;
		}

		async public static Task<bool> ServerProjectRequest(UserClient user, int userCode, int serverCode, int channel = 0)
		{
			Console.WriteLine(user.UserCode + " : 서버 프로젝트 정보를 전송\t대상서버 : " + serverCode + "\t대상 채널 : " + channel);
            SQL sql = new();

			ChecklistParm checklistParm = new();

            checklistParm.serverCode = serverCode;
            checklistParm.channelCode = channel;
            checklistParm.checklistCode = null;

			DataTable queryResult = new();

            queryResult = sql.resultConnectDB((object)checklistParm, "GetChecklist");

            if (channel == 0)
			{
				// 모든 데이터 전송
			}
			else
			{

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
				// 서버의 모든 채팅방에 대한 메시지를 전송
			}
			else
			{

			}

            // !SQL 서버의 해당 채널의 메시지들을 받아옴
            SQL sql = new();

			MessageParm messageParm = new MessageParm();

            messageParm.serverCode = serverCode;
            messageParm.channelCode = channel;
			messageParm.messageCode = null;

            DataTable queryResult = new DataTable();

            queryResult = sql.resultConnectDB((object)messageParm, "GetMessage");

            /*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
            return true;
		}

		
		async public static Task<bool> UserFriend(UserClient user, int userCode)
		{
			Console.WriteLine(user.UserCode + " : 유저의 친구 정보를 전송\t대상 : " + userCode);
            // !SQL 해당 유저에 대한 친구 목록을 받아 전송
            SQL sql = new();

			FriendParm friendParm = new FriendParm();

			friendParm.userCode = userCode;
			friendParm.friendCode = null;

            DataTable queryResult = new DataTable();

            queryResult = sql.resultConnectDB((object)friendParm, "GetFriend");

            /*
			foreach(var i in data)
			{
				user.Send(i);
			}
			*/
            return true;
		}
	}
}