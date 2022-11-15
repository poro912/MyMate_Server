// See https://aka.ms/new-console-template for more information

using ServerToClient;
using ServerCommunicater = ServerToClient.Server;
using ServerSystem;
using System.Data;

Console.WriteLine("MyMate Server Start");

// 서버 생성
ServerCommunicater server = ServerCommunicater.Instance;
server.clientAccept = AcceptProcess.AccpetRun;

// 로그인 컨테이너 생성
// 로그인 이후의 데이터가 저장되는 컨테이너 (Dictionary)
LoginContainer login = LoginContainer.Instance;

// 서버 리스트의 데이터를 저장하는 컨테이너
ServerContainer serverContain = ServerContainer.Instance;

Console.WriteLine("Server ip : " + Default.Network.Address);

// 1분마다 비 로그인 클라이언트가 연결 됐는지 확인하기위한 메소드
//BeforeLoginEvent.ConnectCheckThread();

// 버퍼 대기를 없애기 위한 변수
char k;
while (true)
{
	// 프로세스과부화를 막기위한 + 버퍼 대기를 막기 위한
	//Thread.Sleep(5000);
	k = Console.ReadKey().KeyChar;

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