﻿using ServerNetwork;
using ServerNetwork.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSystem
{
	public class LoginContainer
	{
		// 싱글톤 구현
		static private LoginContainer instance;
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

		public Dictionary<int, Client> login_dict;

		private LoginContainer()
		{
			login_dict = new Dictionary<int, Client>();
		}

		// 해당 유저가 로그인 상태인지 확인
		// login	: true
		// logout	: false
		public bool isLogin(int usercode)
		{
			login_dict.TryGetValue(usercode, out Client? temp);

			return null != temp;
		}

		// 유저의 클라이언트 저장을 시도한다.
		public void registUser(int usercode, Client client)
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
			}
		}

		// 유저 데이터 획득을 시도한다.
		public Client? getUser(int usercode)
		{
			login_dict.TryGetValue(usercode, out Client? value);
			return value;
		}

		public void getUser(int usercode, out Client? client)
		{
			login_dict.TryGetValue(usercode, out client);
		}
	}
}
