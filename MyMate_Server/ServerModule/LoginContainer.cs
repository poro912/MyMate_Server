﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerToClient;
using ServerToClinet;

namespace ServerSystem
{
	public class LoginContainer
	{
		// 싱글톤 구현
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

		public Dictionary<int, UserClient> login_dict;

		private LoginContainer()
		{
			login_dict = new Dictionary<int, UserClient>();
		}

		// 해당 유저가 로그인 상태인지 확인
		// login	: true
		// logout	: false
		public bool isLogin(int usercode)
		{
			login_dict.TryGetValue(usercode, out UserClient? temp);

			return null != temp;
		}

		// 유저의 클라이언트 저장을 시도한다.
		public void registUser(int usercode, UserClient client)
		{
			// 로그인 상태라면 (로그인 정보가 있다면)
			if(isLogin(usercode))
			{
				login_dict[usercode] = client;
			}
			// 로그인 상태가 아니라면
			else
			{
				login_dict.TryAdd(usercode, client);
				client.Start();
			}
		}

		// 유저 데이터 획득을 시도한다.
		public Client? getUser(int usercode)
		{
			login_dict.TryGetValue(usercode, out UserClient? value);
			if (value == null)
				return null;
			return value;
		}

		public void getUser(int usercode, out UserClient? client)
		{
			login_dict.TryGetValue(usercode, out client);
		}
	}
}
