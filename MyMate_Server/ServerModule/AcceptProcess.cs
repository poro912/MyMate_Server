using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Protocol;
using ServerToClient;
using ServerSystem;

namespace ServerSystem
{
	// 클라이언트가 처음 엑셉트 했을 때 동작할 메소드
	public class AcceptProcess
	{
		static public void AccpetRun(Client client)
		{
			Console.WriteLine("Attempt\t: 클라이언트 접근");
            if (client.socket != null)
                Console.WriteLine("포트 번호 : " + client.socket.RemoteEndPoint + "\n");

            //BeforeLogin beforeclient = new(client);

            // 큐에 삽입
            //BeforeContainer.Instance.Push(beforeclient);
            new UserClient(client.tcpClient);
		}
	}
}
