using Protocol;
using Protocol.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSystem
{
	public class BeforeContainer
	{
		// 싱글톤 구현
		static private BeforeContainer instance;
		static public BeforeContainer Instance
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
		private static isConnectProtocol.IsConnect isConnect = new(1);
		private static List<byte> bytes;
		private ConcurrentQueue<BeforeLogin> clients;

		private BeforeContainer()
		{
			clients = new ConcurrentQueue<BeforeLogin>();

			bytes = new List<byte>();
			Generater.Generate(isConnect, ref bytes);
		}

		public void Push(BeforeLogin client)
		{
			clients.Enqueue(client);
		}

		public BeforeLogin? Check()
		{
			clients.TryDequeue(out BeforeLogin? temp);

			// 로그인 대기상태인 클라이언트가 없다면
			if (null == temp)
				return null;

			// 해당 클라이언트가 로그인 상태가 아니라면
			// 아무 데이터를 보내고 보내진다면 연결이 된 것이므로
			// 다시 큐에 넣음
			try
			{
				temp.client.send.Data(bytes);
				clients.Enqueue(temp);
			}
			catch (Exception e)
			{
				// 오류가 발생하였다면 연결이 끊어진 것
				temp.Delete();
				temp = null;
				Console.WriteLine(e);
			}
			// 반환시 데이터가 있다면 로그인 된 것
			// null 이라면 로그인되지 않은 것
			return temp;
		}
	}
}
