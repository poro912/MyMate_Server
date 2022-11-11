// See https://aka.ms/new-console-template for more information

using ServerToClient;
using ServerCommunicater = ServerToClient.Server;
using ServerSystem;
using System.Data;

// 서버 생성
ServerCommunicater server = ServerCommunicater.Instance;
server.clientAccept = AcceptProcess.AccpetRun;

// 로그인 이전의 데이터가 저장되는 컨테이너 (큐)
//BeforeContainer before = BeforeContainer.Instance;
//BeforeLogin? beforeClient;

// 로그인 이후의 데이터가 저장되는 컨테이너 (사전)
LoginContainer login = LoginContainer.Instance;

// 서버 리스트의 데이터를 저장하는 컨테이너

while (true)
{
	// 프로세스과부화를 막기위한 sleep
	Thread.Sleep(5000);

	//BeforeLoginEvent.ConnectCheck();
	
	
	//beforeClient=before.Check();

	// 로그인 된 클라이언트가 있다면
	//if(null!=beforeClient)
	//{
		// beforeLogin 종료
	//	beforeClient.Delete();
	//	login.registUser(beforeClient.usercode, new(beforeClient.client));
	//}
}