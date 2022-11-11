#pragma warning disable CS1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Protocol.ChannelProtocol;
using static Protocol.ToastProtocol;

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
			}
			else
			{

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
			if(channel == 0)
			{
				// 모든 데이터 전송
			}
			else
			{

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
			}
			else
			{

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