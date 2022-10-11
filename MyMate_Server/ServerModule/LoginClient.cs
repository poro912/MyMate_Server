using Protocol.Protocols;
using Protocol;
using ServerNetwork.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSystem
{
	public class LoginClient
	{
		private Thread thread;
		public Client client;

		public LoginClient(Client client)
		{
			this.client = client;

			this.thread = new Thread(Run);

			// 이벤트 등록
			// 수신된 것이 있다면 이벤트가 호출됨
			this.client.receive.ReceiveEvent += wakeup;

			// 최초 1회 실행함
			this.thread.Start();
		}

		// 성윤이가 만든 모듈을 탑재할 부분
		// Event 리스너에 의해 호출될 경우 작동
		private void Run()
		{
			byte[]? bytes;
			KeyValuePair<byte, object?> result;

			// 수신큐가 빌때까지
			while (!this.client.receive.isEmpty())
			{

			}
		}

		public void wakeup(object sender, EventArgs e)
		{
			// 스레드가 실행중이 아니라면
			//if (!this.thread.IsAlive)
				//this.thread.Start();
		}

		public void Delete()
		{
			// 수신 중지
			this.client.receive.Stop();
			this.client.receive.ReceiveEvent -= this.wakeup;

			// 스레드를 종료시킴
			if(thread.IsAlive)
			{
				thread.Interrupt();
				thread.Join();
			}
			
		}

	}
}
